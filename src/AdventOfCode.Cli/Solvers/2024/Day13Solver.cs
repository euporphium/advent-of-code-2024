using System.Text.RegularExpressions;

namespace AdventOfCode.Cli.Solvers._2024;

internal record Button(long X, long Y, int Size);

public partial class Day13Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var totalCost = 0;

        for (var i = 0; i < input.Length; i += 4)
        {
            var aVals = ParseInput(input[i]);
            var bVals = ParseInput(input[i + 1]);
            var prize = ParseInput(input[i + 2]);

            var a = new Button(aVals.X, aVals.Y, 3);
            var b = new Button(bVals.X, bVals.Y, 1);

            var lowestCost = int.MaxValue;
            
            // Use linear algebra to solve for button presses
            // We need to solve the system of equations:
            // aCount * a.X + bCount * b.X = prize.X
            // aCount * a.Y + bCount * b.Y = prize.Y
            
            // Using Cramer's rule to solve for bCount:
            // bCount = (prize.Y * a.X - prize.X * a.Y) / (b.Y * a.X - b.X * a.Y)
            var denominator = b.Y * a.X - b.X * a.Y;
            if (denominator != 0)
            {
                var bCount = (prize.Y * a.X - prize.X * a.Y) / (decimal)denominator;
                
                // If bCount is a positive integer, calculate aCount
                if (bCount >= 0 && bCount == Math.Floor(bCount))
                {
                    var aCount = (prize.X - (long)bCount * b.X) / (decimal)a.X;
                    
                    // Check if aCount is a positive integer and the solution is valid
                    if (aCount >= 0 && aCount == Math.Floor(aCount))
                    {
                        var cost = ((long)aCount * a.Size) + ((long)bCount * b.Size);
                        if (cost < lowestCost)
                        {
                            lowestCost = (int)cost;
                        }
                    }
                }
            }

            if (lowestCost != int.MaxValue)
            {
                totalCost += lowestCost;
            }
        }

        return totalCost.ToString();
    }

    // This solution uses substitution to solve for the number of button presses
    public string SolvePartB(string[] input)
    {
        var totalCost = 0L;
        const long padding = 10000000000000;
   
        for (var i = 0; i < input.Length; i += 4)
        {
            var aVals = ParseInput(input[i]);
            var bVals = ParseInput(input[i + 1]);
            var prize = ParseInput(input[i + 2]);
       
            var a = new Button(aVals.X, aVals.Y, 3);
            var b = new Button(bVals.X, bVals.Y, 1);

            // Add padding to prize coordinates
            var paddedPrizeX = prize.X + padding;
            var paddedPrizeY = prize.Y + padding;

            // Solve for button presses using substitution
            var bPresses = (paddedPrizeY * a.X - paddedPrizeX * a.Y) / (b.Y * a.X - b.X * a.Y);
            var aPresses = (paddedPrizeX - bPresses * b.X) / a.X;
       
            // Verify solution is valid and non-negative
            if (aPresses >= 0 && bPresses >= 0 && 
                bPresses * b.X + aPresses * a.X == paddedPrizeX && 
                bPresses * b.Y + aPresses * a.Y == paddedPrizeY)
            {
                totalCost += aPresses * a.Size + bPresses * b.Size;
            }
        }
   
        return totalCost.ToString();
    }

    private static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    private static (long X, long Y) ParseInput(string line)
    {
        var regex = MyRegex();
        var match = regex.Match(line);
        if (!match.Success || match.Groups.Count != 3) // Groups[0] is full match
        {
            throw new FormatException($"Input '{line}' does not match expected format");
        }

        if (!long.TryParse(match.Groups[1].Value, out var x) ||
            !long.TryParse(match.Groups[2].Value, out var y))
        {
            throw new FormatException($"Failed to parse coordinate values from '{line}'");
        }

        return (x, y);
    }

    [GeneratedRegex(@"^.*:\sX[+=](\d+),\sY[+=](\d+)$")]
    private static partial Regex MyRegex();
}