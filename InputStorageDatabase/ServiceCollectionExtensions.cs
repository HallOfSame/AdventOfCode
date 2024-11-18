using Microsoft.Extensions.DependencyInjection;

namespace InputStorageDatabase;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<AdventOfCodeContext>();

        return serviceCollection;
    }
}