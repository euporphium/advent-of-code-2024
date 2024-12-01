using AdventOfCode.Cli.IO;
using AdventOfCode.Cli.Solvers;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Cli;

public static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<SimpleReader>();
        services.AddScopedSolvers();
    }

    // This method will find all types that implement ISolver and add them to the service collection
    private static void AddScopedSolvers(this IServiceCollection services)
    {
        var solverTypes = typeof(ISolver).Assembly
            .GetTypes()
            .Where(t => t is { IsInterface: false, IsAbstract: false } && typeof(ISolver).IsAssignableFrom(t));
        
        foreach (var solverType in solverTypes)
        {
            services.AddScoped(typeof(ISolver), solverType);
        }
    }
}