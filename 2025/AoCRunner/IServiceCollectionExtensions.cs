using AoCRunner.ViewModels;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable InconsistentNaming

namespace AoCRunner
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();

            return services;
        }
    }
}
