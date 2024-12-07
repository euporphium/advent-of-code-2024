using Microsoft.Extensions.Options;

namespace AdventOfCode.Cli.IO;

internal class SimpleReader(IOptions<ApplicationOptions> options)
{
    public async Task<string[]> GetInputLinesAsync(int day, string input)
    {
        var path = Path.Join(options.Value.InputDataDirectory, day.ToString(), input);
        return await File.ReadAllLinesAsync(path);
    }
    
    public int GetLatestDay()
    {
        var directory = new DirectoryInfo(options.Value.InputDataDirectory);
        return directory.GetDirectories().Select(d => int.Parse(d.Name)).Max();
    }
}