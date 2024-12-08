namespace AdventOfCode.Cli.Solvers;

public class Day7Solver : ISolver
{
    private readonly record struct Operation(Func<long, long, long> Apply);

    private static readonly Dictionary<string, Operation> Operations = new()
    {
        ["+"] = new Operation((a, b) => a + b),
        ["*"] = new Operation((a, b) => a * b),
        ["||"] = new Operation((a, b) => long.Parse($"{a}{b}"))
    };
    
    // Cache of operator combinations based on length needed and operation count allowed
    private static readonly Dictionary<(int length, int opCount), Operation[][]> CombinationCache = new();

    public string SolvePartA(string[] input) =>
        Solve(input, [Operations["+"], Operations["*"]]);

    public string SolvePartB(string[] input) =>
        Solve(input, [Operations["+"], Operations["*"], Operations["||"]]);

    private static string Solve(string[] input, Operation[] allowedOperations)
    {
        return input.Where(line => EvaluatesToTargetValue(line, allowedOperations))
            .Sum(line => long.Parse(line.Split(':')[0]))
            .ToString();
    }

    private static bool EvaluatesToTargetValue(string line, Operation[] allowedOperations)
    {
        var (testValue, operands) = ParseInput(line);
        if (operands.Count == 1) return operands[0] == testValue;

        return GenerateOperatorCombinations(allowedOperations, operands.Count - 1)
            .Any(operators => EvaluateExpression(operands, operators, testValue));
    }

    private static (long targetValue, List<long> operands) ParseInput(string line)
    {
        var parts = line.Split(':');
        return (
            long.Parse(parts[0]),
            parts[1].Trim().Split(' ').Select(long.Parse).ToList()
        );
    }

    private static Operation[][] GenerateOperatorCombinations(Operation[] options, int length)
    {
        var key = (length, options.Length);
        if (CombinationCache.TryGetValue(key, out var cached)) 
            return cached;
        
        var total = (int)Math.Pow(options.Length, length);
        var combinations = new Operation[total][];

        for (var i = 0; i < total; i++)
        {
            var combination = new Operation[length];
            var temp = i;
            for (var j = 0; j < length; j++)
            {
                combination[j] = options[temp % options.Length];
                temp /= options.Length;
            }

            combinations[i] = combination;
        }
        
        CombinationCache[key] = combinations;

        return combinations;
    }

    private static bool EvaluateExpression(List<long> operands, Operation[] operators, long targetValue)
    {
        var value = operands[0];

        for (var i = 1; i < operands.Count; i++)
        {
            if (value > targetValue) return false;

            value = operators[i - 1].Apply(value, operands[i]);
        }

        return value == targetValue;
    }
}