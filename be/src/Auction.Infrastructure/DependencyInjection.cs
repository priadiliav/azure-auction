using Auction.Application.Items;
using Auction.Application.Items.CreateItem;
using Auction.Infrastructure.Messaging;
using Auction.Infrastructure.Persistence;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auction.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusNamespace = configuration["ServiceBus:FullyQualifiedNamespace"]
            ?? throw new InvalidOperationException("Missing configuration: ServiceBus:FullyQualifiedNamespace");

        services.AddSingleton(new ServiceBusClient(serviceBusNamespace, new DefaultAzureCredential()));
        services.AddSingleton<IItemPublisher, ServiceBusItemPublisher>();

        var connectionString = configuration.GetConnectionString("AuctionDb")
            ?? throw new InvalidOperationException("Missing configuration: ConnectionStrings:AuctionDb");

        services.AddDbContext<AuctionDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IItemRepository, ItemRepository>();
    }
}
