namespace AdventOfCode.Cli.Solvers._2024;

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
            var col = line.IndexOf(':');
            _answers[idx] = Convert.ToUInt64(line[..col]);
            _input[idx] = line[(col + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToUInt64(s.Trim()))
                .ToArray();
        }
    }

    public string SolvePartA(string[] input)
    {
        Init(input);
        return CalculateSum([Div, Sub]).ToString();
    }

    public string SolvePartB(string[] input)
    {
        Init(input);
        return CalculateSum([Split, Div, Sub]).ToString();
    }

    private ulong CalculateSum(Operation[] operations)
    {
        var chunkSize = (int)Math.Ceiling((double)_input.Length / NumberOfTasks);
        var tasks = Enumerable.Range(0, NumberOfTasks)
            .Select(i => CalculateSumAsync(operations, i * chunkSize, chunkSize))
            .ToArray();
        Task.WaitAll(tasks);
        return tasks.Aggregate(0UL, (acc, t) => acc += t.Result);
    }

    private Task<ulong> CalculateSumAsync(Operation[] operations, int start, int count)
        => Task.Run(() => CalculateSum(operations, start, count));

    private ulong CalculateSum(Operation[] operations, int start, int count)
    {
        var end = Math.Min(start + count, _input.Length);
        var sum = 0UL;
        for (int i = start; i < end; i++)
            if (BackFind(_input[i], operations, _answers[i]))
                sum += _answers[i];
        return sum;
    }

    private static bool BackFind(ulong[] arr, Operation[] operations, ulong result)
    {
        var stack = new Stack<(int index, ulong acc)>();
        stack.Push((arr.Length - 1, result));
        while (stack.Count > 0)
        {
            var (index, acc) = stack.Pop();
            if (index == 0)
            {
                if (acc == arr[0])
                    return true;
                continue;
            }
            for (int i = 0; i < operations.Length; i++)
                if (operations[i](acc, arr[index], out var value))
                    stack.Push((index - 1, value));
        }
        return false;
    }

    private static bool Div(ulong a, ulong b, out ulong result)
    {
        result = a / b;
        return a % b == 0;
    }

    private static bool Sub(ulong a, ulong b, out ulong result)
    {
        result = a - b;
        return result > 0;
    }

    private static bool Split(ulong a, ulong b, out ulong result)
    {
        var div = (ulong)Math.Pow(10, (ulong)Math.Log10(b) + 1UL);
        result = a / div;
        return a % div == b;
    }
}