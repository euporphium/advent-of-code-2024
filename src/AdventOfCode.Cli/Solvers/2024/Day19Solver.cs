namespace AdventOfCode.Cli.Solvers._2024;

public class Day19Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var availableTowels = input[0].Split(", ");
        var designs = input.Skip(2);

        return designs.Count(design => CheckDesign(design, availableTowels)).ToString();
    }

    public string SolvePartB(string[] input)
    {
        var availableTowels = input[0].Split(", ");
        var designs = input.Skip(2);

        return designs.Sum(design =>
        {
            var cache = new Dictionary<int, long>();
            return CountDesigns(design, 0, availableTowels, cache);
        }).ToString();
    }

    private static bool CheckDesign(string design, string[] availableTowels)
    {
        return string.IsNullOrEmpty(design) || availableTowels
            .Where(design.StartsWith)
            .Any(towel => CheckDesign(design[towel.Length..], availableTowels));
    }

    private static long CountDesigns(string design, int startIndex, string[] availableTowels,
        Dictionary<int, long> cache)
    {
        // If we've reached the end, we found a valid combination
        if (startIndex == design.Length)
            return 1;

        // If we've already calculated this subproblem, return cached result
        if (cache.TryGetValue(startIndex, out var cachedResult))
            return cachedResult;

        var remainingDesign = design[startIndex..];

        // Try each towel pattern that could fit at the current position
        var count = availableTowels.Where(towel => remainingDesign.StartsWith(towel)).Sum(towel =>
            CountDesigns(design, startIndex + towel.Length, availableTowels, cache));

        // Cache the result before returning
        cache[startIndex] = count;
        return count;
    }
}