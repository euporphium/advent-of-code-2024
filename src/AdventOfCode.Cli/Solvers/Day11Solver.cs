using System.Numerics;
using AdventOfCode.Cli.Solvers;

public class Day11Solver : ISolver
{
    private readonly Dictionary<(string number, int stepsLeft), long> _cache = new();
    
    public string SolvePartA(string[] input) => SolveForBlinks(input, 25);
    public string SolvePartB(string[] input) => SolveForBlinks(input, 75);

    private string SolveForBlinks(string[] input, int blinkCount)
    {
        var stones = input[0].Split(' ');
        var totalStones = stones.Sum(stone => CountDescendants(stone, blinkCount));

        return totalStones.ToString();
    }
    
    private long CountDescendants(string number, int stepsLeft)
    {
        if (stepsLeft == 0) return 1;
        
        var key = (number, stepsLeft);
        if (_cache.TryGetValue(key, out var cached))
            return cached;
            
        long result;
        if (number == "0")
        {
            result = CountDescendants("1", stepsLeft - 1);
        }
        else if (number.Length % 2 == 0)
        {
            var mid = number.Length / 2;
            var left = number[..mid];
            var right = number[mid..].TrimStart('0');
            if (string.IsNullOrEmpty(right)) right = "0";
            
            result = CountDescendants(left, stepsLeft - 1) + 
                     CountDescendants(right, stepsLeft - 1);
        }
        else
        {
            var newValue = (BigInteger.Parse(number) * 2024).ToString();
            result = CountDescendants(newValue, stepsLeft - 1);
        }
        
        _cache[key] = result;
        return result;
    }
}