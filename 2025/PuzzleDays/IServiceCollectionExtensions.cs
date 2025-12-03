using Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace PuzzleDays;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPuzzles(this IServiceCollection services)
    {
        services.AddSingleton<IPuzzle, Day01>();

        return services;
    }
}