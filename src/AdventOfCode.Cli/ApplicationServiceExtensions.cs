using AdventOfCode.Cli.Solvers;
using AdventOfCode.Cli.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Cli;

internal static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptions.AdventOfCode));

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