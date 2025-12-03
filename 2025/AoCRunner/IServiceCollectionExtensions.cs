using AoCRunner.Services;
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
            services.AddSingleton<CalendarViewModel>();
            services.AddSingleton<NavigationService>();

            return services;
        }
    }
}
