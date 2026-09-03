using Auction.Application.Items.CreateItem;
using Auction.Application.Users;
using Auction.Domain.Users;
using Auction.Infrastructure;
using Auction.Infrastructure.Auth;
using Auction.WebApi.Endpoints;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateItemCommand).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

var googleClientId = builder.Configuration["Authentication:Google:ClientId"]
    ?? throw new InvalidOperationException("Missing configuration: Authentication:Google:ClientId");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = GoogleTokenValidation.Issuer;
        options.Audience = googleClientId;
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var (id, email, name, avatarUrl) = GoogleTokenValidation.ExtractUserProfile(context.Principal!);
                await userRepository.UpsertAsync(new User(id, email, name, avatarUrl), context.HttpContext.RequestAborted);
            },
        };
    });
builder.Services.AddAuthorization();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services
        .AddOpenTelemetry()
        .UseAzureMonitor();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(builder
            .Configuration
                .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapItemsEndpoints();
app.MapMeEndpoints();

app.Run();
