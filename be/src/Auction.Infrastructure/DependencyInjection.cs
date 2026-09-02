using Auction.Application.Items.CreateItem;
using Auction.Infrastructure.Messaging;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
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
        services.AddSingleton<IItemEventPublisher, ServiceBusItemEventPublisher>();
    }
}
