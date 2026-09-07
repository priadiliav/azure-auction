using Auction.Application.Items;
using Auction.Application.Items.CreateItem;
using Auction.Application.Users;
using Auction.Infrastructure.Ai;
using Auction.Infrastructure.Messaging;
using Auction.Infrastructure.Persistence;
using Auction.Infrastructure.Storage;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
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
        services.AddSingleton<IStatePublisher, ServiceBusStatePublisher>();
        services.AddSingleton<IBidPublisher, ServiceBusBidPublisher>();
        services.AddSingleton<IInteractionPublisher, ServiceBusInteractionPublisher>();

        var blobStorageAccountName = configuration["BlobStorage:AccountName"]
            ?? throw new InvalidOperationException("Missing configuration: BlobStorage:AccountName");

        services.AddSingleton(new BlobServiceClient(
            new Uri($"https://{blobStorageAccountName}.blob.core.windows.net"),
            new DefaultAzureCredential()));
        services.AddSingleton<IBlobSasService, BlobSasService>();

        var openAiEndpoint = configuration["AzureOpenAI:Endpoint"]
            ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Endpoint");
        var embeddingDeploymentName = configuration["AzureOpenAI:EmbeddingDeploymentName"]
            ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:EmbeddingDeploymentName");

        var openAiClient = new AzureOpenAIClient(new Uri(openAiEndpoint), new DefaultAzureCredential());
        services.AddSingleton<IEmbeddingService>(
            new AzureOpenAiEmbeddingService(openAiClient.GetEmbeddingClient(embeddingDeploymentName)));

        var connectionString = configuration.GetConnectionString("AuctionDb")
            ?? throw new InvalidOperationException("Missing configuration: ConnectionStrings:AuctionDb");

        services.AddDbContext<AuctionDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IBidPlacementService, BidPlacementService>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IInteractionRepository, InteractionRepository>();
    }
}
