namespace AdventOfCode.Cli.Solvers;

// Included as an alternative solution for its clever use of backwards solving (division/subtraction)
// and parallel processing optimizations.
// All credit to GrigoryanArtem => https://github.com/GrigoryanArtem/advent-of-code-2024/blob/master/Puzzles.Runner/2024/Day7.cs

public class Day7Solver_GrigoryanArtem : ISolver
{
    private const int NumberOfTasks = 128;

    private delegate bool Operation(ulong a, ulong b, out ulong result);

    private ulong[] _answers = [];
    private ulong[][] _input = [];

    private void Init(string[] lines)
    {
        _answers = new ulong[lines.Length];
        _input = new ulong[lines.Length][];

        foreach (var (line, idx) in lines.Select((value, i) => (value, i)))
        {
            var parts = line.Split(':');
            _answers[idx] = ulong.Parse(parts[0].Trim());
            _input[idx] = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => ulong.Parse(s.Trim()))
                .ToArray();
        }
    }

    public string SolvePartA(string[] input)
    {
        Init(input);
        return CalculateSum([Mul, Add]).ToString();
    }

    public string SolvePartB(string[] input)
    {
        Init(input);
        return CalculateSum([Mul, Add, Concat]).ToString();
    }

    private ulong CalculateSum(Operation[] operations)
    {
        var chunkSize = (int)Math.Ceiling((double)_input.Length / NumberOfTasks);
        var tasks = Enumerable.Range(0, NumberOfTasks)
            .Select(i => CalculateSumAsync(operations, i * chunkSize, chunkSize))
            .ToArray();
        Task.WaitAll(tasks);
        return tasks.Aggregate(0UL, (acc, t) => acc + t.Result);
    }

    private Task<ulong> CalculateSumAsync(Operation[] operations, int start, int count)
        => Task.Run(() => CalculateSum(operations, start, count));

    private ulong CalculateSum(Operation[] operations, int start, int count)
    {
        var end = Math.Min(start + count, _input.Length);
        var sum = 0UL;
        for (int i = start; i < end; i++)
            if (ForwardFind(_input[i], operations, _answers[i]))
                sum += _answers[i];
        return sum;
    }

    private static bool ForwardFind(ulong[] arr, Operation[] operations, ulong target)
    {
        if (arr.Length == 1) return arr[0] == target;

        var stack = new Stack<(int index, ulong value)>();
        stack.Push((1, arr[0]));

        while (stack.Count > 0)
        {
            var (index, currentValue) = stack.Pop();

            if (index == arr.Length)
            {
                if (currentValue == target)
                    return true;
                continue;
            }

            foreach (var op in operations)
            {
                if (op(currentValue, arr[index], out var result))
                {
                    if (result <= target) // Optimization: don't continue if we've exceeded target
                        stack.Push((index + 1, result));
                }
            }
        }

        return false;
    }

    private static bool Add(ulong a, ulong b, out ulong result)
    {
        result = a + b;
        return true;
    }

    private static bool Mul(ulong a, ulong b, out ulong result)
    {
        result = a * b;
        return true;
    }

    private static bool Concat(ulong a, ulong b, out ulong result)
    {
        // Calculate how many digits b has
        var digits = (ulong)Math.Floor(Math.Log10(b) + 1);
        var multiplier = (ulong)Math.Pow(10, digits);
        result = a * multiplier + b;
        return true;
    }
}