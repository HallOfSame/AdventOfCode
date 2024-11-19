using Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace PuzzleDays;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Add2024Puzzles(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        services.AddPuzzlesFromAssembly(assembly);

        return services;
    }
}