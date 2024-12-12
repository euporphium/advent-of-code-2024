namespace AdventOfCode.Cli.Solvers._2024;

public class Day2Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        // The levels are either all increasing or all decreasing.
        // Any two adjacent levels differ by at least one and at most three.

        var safeCount = input.Select(line => line.Split(' ').Select(int.Parse).ToList())
            .Count(IsSafe);

        return safeCount.ToString();
    }

    public string SolvePartB(string[] input)
    {
        var safeCount = input.Select(line => line.Split(' ').Select(int.Parse).ToList())
            .Count(levels => IsSafeWithDampener(levels));

        return safeCount.ToString();
    }

    private static bool IsSafe(List<int> levels)
    {
        bool? increasing = null;
        for (var i = 1; i < levels.Count; i++)
        {
            var diff = levels[i] - levels[i - 1];
            
            var absDiff = Math.Abs(diff);
            if (absDiff is < 1 or > 3) return false;

            if (diff > 0)
            {
                increasing ??= true;
                if (!increasing.Value) return false;
            }
            else
            {
                increasing ??= false;
                if (increasing.Value) return false;
            }
        }

        return true;
    }

    private static bool IsSafeWithDampener(List<int> levels, int index = 0)
    {
        // if removing a single level from an unsafe report would make it safe, the report instead counts as safe
        if (index >= levels.Count) return false;
        
        var copy = new List<int>(levels);
        copy.RemoveAt(index);

        return IsSafe(copy) || IsSafeWithDampener(levels, index + 1);
    }
}