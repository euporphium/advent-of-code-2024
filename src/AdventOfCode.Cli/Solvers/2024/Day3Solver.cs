using System.Text.RegularExpressions;

namespace AdventOfCode.Cli.Solvers._2024;

public partial class Day3Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var sum = (from line in input
            from Match match in MulRegex().Matches(line)
            let x = int.Parse(match.Groups[1].Value)
            let y = int.Parse(match.Groups[2].Value)
            select x * y).Sum();

        return sum.ToString();
    }

    public string SolvePartB(string[] input)
    {
        const string enable = "do()";
        const string disable = "don't()";
        
        var sum = 0;
        var enabled = true;
        foreach (var line in input)
        {
            var matches = LogicRegex().Matches(line);

            foreach (Match match in matches)
            {
                var matchValue = match.ToString();
                switch (matchValue)
                {
                    case enable:
                        enabled = true;
                        break;
                    case disable:
                        enabled = false;
                        break;
                    default:
                    {
                        if (enabled)
                        {
                            var x = int.Parse(match.Groups[1].Value);
                            var y = int.Parse(match.Groups[2].Value);
                            sum += x * y;
                        }

                        break;
                    }
                }
            }
        }

        return sum.ToString();
    }

    [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
    private static partial Regex MulRegex();

    [GeneratedRegex(@"mul\((\d+),(\d+)\)|do\(\)|don't\(\)")]
    private static partial Regex LogicRegex();
}