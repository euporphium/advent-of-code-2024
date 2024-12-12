using Microsoft.Extensions.Options;

namespace AdventOfCode.Cli.Util;

internal class SimpleReader(IOptions<ApplicationOptions> options)
{
    public async Task<string[]> GetInputLinesAsync(int year, int day, string input)
    {
        var path = Path.Join(options.Value.InputDataDirectory, year.ToString(), day.ToString(), input);
        return await File.ReadAllLinesAsync(path);
    }

    public IEnumerable<int> GetYears()
    {
        var directoryInfo = new DirectoryInfo(options.Value.InputDataDirectory);
        return directoryInfo.GetDirectories().Select(d => int.Parse(d.Name));
    }

    public int GetLatestDay(int year)
    {
        var yearDirectory = Path.Join(options.Value.InputDataDirectory, year.ToString());
        var directoryInfo = new DirectoryInfo(yearDirectory);
        return directoryInfo.GetDirectories().Select(d => int.Parse(d.Name)).Max();
    }
}