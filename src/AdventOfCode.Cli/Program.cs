using System.Diagnostics;
using AdventOfCode.Cli;
using AdventOfCode.Cli.IO;
using AdventOfCode.Cli.Solvers;
using Cocona;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection(ApplicationOptions.AdventOfCode));

builder.Services.AddApplicationServices();

var app = builder.Build();

app.AddCommand("solve",
    async (IEnumerable<ISolver> solvers, SimpleReader reader, int day, string part, string input) =>
    {
        Console.WriteLine($"Solving for Day {day} Part {part}");

        var lines = await Performance.LogTimeAsync("Read input file",
            async () => await reader.GetInputLinesAsync(day, input));
        var solver = solvers.FirstOrDefault(s => s.GetType().Name == $"Day{day}Solver");
        Debug.Assert(solver != null, $"Solver for day {day} not found");

        var result = Performance.LogTime("Solve",
            () => part == "b" ? solver.SolvePartB(lines) : solver.SolvePartA(lines));
        
        Console.WriteLine($"Answer: {result}");

        return Task.CompletedTask;
    });

app.Run();