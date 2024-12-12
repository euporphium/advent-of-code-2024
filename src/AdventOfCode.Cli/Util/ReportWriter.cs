using Spectre.Console;

namespace AdventOfCode.Cli.Util;

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

internal record DayReport(int Day, string Part, string Answer, long ElapsedMilliseconds);