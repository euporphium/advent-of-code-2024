namespace AdventOfCode.Cli.Solvers._2024;

public class Day5Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var (_, reversedOrderingRules, updates) = ParseInput(input);

        var sum = updates.Where(pageNumbers => IsOrdered(pageNumbers, reversedOrderingRules))
            .Sum(pageNumbers => pageNumbers[(pageNumbers.Count - 1) / 2]);

        return sum.ToString();
    }

    public string SolvePartB(string[] input)
    {
        var (orderingRules, reversedOrderingRules, updates) = ParseInput(input);
        
        var sum = 0;
        foreach (var pageNumbers in updates.Where(pageNumbers => !IsOrdered(pageNumbers, reversedOrderingRules)))
        {
            pageNumbers.Sort((x, y) =>
            {
                if (orderingRules.TryGetValue(x, out var ruleX))
                {
                    if (ruleX.Contains(y))
                    {
                        return -1; // x is less than y
                    }
                }
                if (orderingRules.TryGetValue(y, out var ruleY))
                {
                    if (ruleY.Contains(x))
                    {
                        return 1; // x is greater than y
                    }
                }
                
                return 0; // "x equals y" (ignore)
            });
            
            sum += pageNumbers[(pageNumbers.Count - 1) / 2];
        }

        return sum.ToString();
    }

    private static bool IsOrdered(List<int> pageNumbers, Dictionary<int, List<int>> reversedOrderingRules)
    {
        var disallowedPageNumbers = new HashSet<int>();
        foreach (var pageNumber in pageNumbers)
        {
            if (disallowedPageNumbers.Contains(pageNumber))
            {
                return false;
            }

            if (reversedOrderingRules.TryGetValue(pageNumber, out var rule))
            {
                disallowedPageNumbers.UnionWith(rule);
            }
        }

        return true;
    }

    private static InputData ParseInput(string[] input)
    {
        Dictionary<int, List<int>> orderingRules = new();
        Dictionary<int, List<int>> reversedOrderingRules = new();
        List<List<int>> updates = [];

        var instructions = true;
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                instructions = false;
                continue;
            }

            if (instructions)
            {
                var ruleNums = line.Split('|').Select(int.Parse).ToList();
                if (orderingRules.TryGetValue(ruleNums[0], out var incList))
                {
                    incList.Add(ruleNums[1]);
                }
                else
                {
                    orderingRules[ruleNums[0]] = [ruleNums[1]];
                }

                if (reversedOrderingRules.TryGetValue(ruleNums[1], out var decList))
                {
                    decList.Add(ruleNums[0]);
                }
                else
                {
                    reversedOrderingRules[ruleNums[1]] = [ruleNums[0]];
                }
            }
            else
            {
                updates.Add(line.Split(',').Select(int.Parse).ToList());
            }
        }

        return new InputData(orderingRules, reversedOrderingRules, updates);
    }

    private record InputData(
        Dictionary<int, List<int>> OrderingRules,
        Dictionary<int, List<int>> ReversedOrderingRules,
        List<List<int>> Updates);
}