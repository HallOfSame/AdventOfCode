using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Helpers.Interfaces;
using Helpers.Structure;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPuzzleStructure(this IServiceCollection services)
        {
            services.AddSingleton<IPuzzleContainer, PuzzleContainer>();

            return services;
        }

        public static IServiceCollection AddPuzzlesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var singlePuzzles = assembly.GetTypes()
                .Where(t => typeof(ISingleExecutionPuzzle).IsAssignableFrom(t) &&
                            t is { IsInterface: false, IsAbstract: false })
                .ToList();
            var stepPuzzles = assembly.GetTypes()
                .Where(t => typeof(IStepExecutionPuzzle).IsAssignableFrom(t) &&
                            t is { IsInterface: false, IsAbstract: false })
                .ToList();

            singlePuzzles.ForEach(x => RegisterPuzzle(typeof(ISingleExecutionPuzzle), x));
            stepPuzzles.ForEach(x => RegisterPuzzle(typeof(IStepExecutionPuzzle), x));

            return services;

            void RegisterPuzzle(Type puzzleType, Type concreteType)
            {
                services.AddSingleton(puzzleType, concreteType);
                services.AddSingleton(typeof(IPuzzle), concreteType);
            }
        }
    }
}
