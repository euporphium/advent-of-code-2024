using AdventOfCode.Cli;
using AdventOfCode.Cli.Solvers;
using AdventOfCode.Cli.Util;
using Cocona;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.AddCommand("solve", async (IEnumerable<ISolver> solvers, SimpleReader reader, int day, string part, string input, int year = 2024) =>
{
    Console.WriteLine($"Solving for Day {day} Part {part}");

    var (lines, _) = await Performance.LogTimeAsync("Read input file", 
        async () => await reader.GetInputLinesAsync(year, day, input));
    
    var solver = solvers.GetSolver(year, day);

    var (result, _) = Performance.LogTime("Solve", 
        () => part == "b" ? solver.SolvePartB(lines) : solver.SolvePartA(lines));

    Console.WriteLine($"Answer: {result}");

    return Task.CompletedTask;
});

app.AddCommand("report", async (IEnumerable<ISolver> solvers, SimpleReader reader, int? day, int year = 2024) =>
{
    var firstReportDay = day ?? 1;
    var lastReportDay = day ?? reader.GetLatestDay(year);

    solvers = solvers.ToList();
    var reports = new List<DayReport>();
    for (var currentDay = firstReportDay; currentDay <= lastReportDay; currentDay++)
    {
        Console.WriteLine($"Generating report for Day {currentDay}");

        var (lines, _) = await Performance.LogTimeAsync("Read input file",
            async () => await reader.GetInputLinesAsync(year, currentDay, "actual.txt"));
            
        var solver = solvers.GetSolver(year, currentDay);

        var (resultA, elapsedMillisecondsA) = Performance.LogTime("Solve Part A", () => solver.SolvePartA(lines));
        reports.Add(new DayReport(currentDay, "a", resultA, elapsedMillisecondsA));

        var (resultB, elapsedMillisecondsB) = Performance.LogTime("Solve Part B", () => solver.SolvePartB(lines));
        reports.Add(new DayReport(currentDay, "b", resultB, elapsedMillisecondsB));
    }

    Console.Clear();
    ReportWriter.WriteReport(reports);
    
    return Task.CompletedTask;
});

app.Run();