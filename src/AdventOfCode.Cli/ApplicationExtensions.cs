using AdventOfCode.Cli.Solvers;

internal static class ApplicationExtensions
{
    internal static ISolver GetSolver(this IEnumerable<ISolver> solvers, int year, int day)
    {
        var solver = solvers
            .Where(s => s.GetType().Namespace?.Split('.').Last() == $"_{year}")
            .FirstOrDefault(s => s.GetType().Name == $"Day{day}Solver");

        if (solver == null)
        {
            throw new InvalidOperationException($"Solver for day {day} year {year} not found");
        }
        
        return solver;
    }
}