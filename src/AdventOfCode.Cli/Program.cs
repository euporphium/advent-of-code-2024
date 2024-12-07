using System.Diagnostics;
using AdventOfCode.Cli;
using AdventOfCode.Cli.IO;
using AdventOfCode.Cli.Solvers;
using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

var builder = CoconaApp.CreateBuilder();

builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection(ApplicationOptions.AdventOfCode));

builder.Services.AddApplicationServices();

var app = builder.Build();

app.AddCommand("solve",
    async (IEnumerable<ISolver> solvers, SimpleReader reader, int day, string part, string input) =>
    {
        Console.WriteLine($"Solving for Day {day} Part {part}");

        var (lines, _) = await Performance.LogTimeAsync("Read input file",
            async () => await reader.GetInputLinesAsync(day, input));
        var solver = solvers.FirstOrDefault(s => s.GetType().Name == $"Day{day}Solver");
        Debug.Assert(solver != null, $"Solver for day {day} not found");

        var (result, _) = Performance.LogTime("Solve",
            () => part == "b" ? solver.SolvePartB(lines) : solver.SolvePartA(lines));

        Console.WriteLine($"Answer: {result}");

        return Task.CompletedTask;
    });

app.AddCommand("report", async (IEnumerable<ISolver> solvers, SimpleReader reader, int? day) =>
{
    var firstReportDay = day ?? 1;
    var lastReportDay = day ?? reader.GetLatestDay();

    solvers = solvers.ToList();
    var reports = new List<DayReport>();
    for (var i = firstReportDay; i <= lastReportDay; i++)
    {
        Console.WriteLine($"Generating report for Day {i}");

        var (lines, _) = await Performance.LogTimeAsync("Read input file",
            async () => await reader.GetInputLinesAsync(i, "actual.txt"));
        var solver = solvers.FirstOrDefault(s => s.GetType().Name == $"Day{i}Solver");
        Debug.Assert(solver != null, $"Solver for day {i} not found");

        var (resultA, elapsedMillisecondsA) = Performance.LogTime("Solve Part A", () => solver.SolvePartA(lines));
        reports.Add(new DayReport(i, "a", resultA, elapsedMillisecondsA));

        var (resultB, elapsedMillisecondsB) = Performance.LogTime("Solve Part B", () => solver.SolvePartB(lines));
        reports.Add(new DayReport(i, "b", resultB, elapsedMillisecondsB));
    }

    Console.Clear();
    ReportWriter.WriteReport(reports);
});

app.Run();

internal record DayReport(int Day, string Part, string Answer, long ElapsedMilliseconds);

internal static class ReportWriter
{
    public static void WriteReport(IEnumerable<DayReport> reports)
    {
        var table = new Table
        {
            Border = TableBorder.Rounded
        };

        table.AddColumn("Day");
        table.AddColumn("Part A | Answer");
        table.AddColumn("Part A | Elapsed Time (ms)");
        table.AddColumn("Part B | Answer");
        table.AddColumn("Part B | Elapsed Time (ms)");

        var groupedReports = reports
            .GroupBy(r => r.Day)
            .OrderBy(g => g.Key);

        foreach (var dayGroup in groupedReports)
        {
            var partA = dayGroup.First(r => r.Part == "a");
            var partB = dayGroup.First(r => r.Part == "b");

            table.AddRow(dayGroup.Key.ToString(), partA.Answer, partA.ElapsedMilliseconds.ToString(), partB.Answer,
                partB.ElapsedMilliseconds.ToString());
        }

        AnsiConsole.Write(table);
    }
}