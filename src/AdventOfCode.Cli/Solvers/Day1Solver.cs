namespace AdventOfCode.Cli.Solvers;

public class Day1Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        // Pair up the smallest number in the left list with the smallest number in the right list,
        // then the second-smallest left number with the second-smallest right number, and so on.
        // Within each pair, figure out how far apart the two numbers are; you'll need to add up all of those distances.

        List<int> left = [];
        List<int> right = [];
        
        foreach (var line in input)
        {
            var each = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(each[0]));
            right.Add(int.Parse(each[1]));
        }

        left.Sort();
        right.Sort();

        var distance = left.Select((t, i) => Math.Abs(t - right[i])).Sum();
        return distance.ToString();
    }

    public string SolvePartB(string[] input)
    {
        //Calculate a total similarity score by adding up each number in the left list
        //after multiplying it by the number of times that number appears in the right list.
        
        Dictionary<int, int> leftCounts = new();
        Dictionary<int, int> rightCounts = new();
        
        foreach (var line in input)
        {
            var each = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            var left = int.Parse(each[0]);
            var right = int.Parse(each[1]);
            
            if (!leftCounts.TryAdd(left, 1))
            {
                leftCounts[left]++;
            }

            if (!rightCounts.TryAdd(right, 1))
            {
                rightCounts[right]++;
            }
        }

        var score = rightCounts.Sum(kvp => kvp.Key * kvp.Value * leftCounts.GetValueOrDefault(kvp.Key, 0));
        return score.ToString();
    }
}