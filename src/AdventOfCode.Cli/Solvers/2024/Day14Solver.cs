using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Cli.Solvers._2024;

public partial class Day14Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        const int width = 101;
        const int height = 103;
        const int ttp = 1000; // time to pee

        var robots = new List<Robot>();

        foreach (var line in input)
        {
            var regex = RobotRegex();
            var match = regex.Match(line);
            if (!match.Success || match.Groups.Count != 5) // Groups[0] is full match
            {
                throw new FormatException($"Input '{line}' does not match expected format");
            }

            var x = int.Parse(match.Groups[1].Value);
            var y = int.Parse(match.Groups[2].Value);
            var dx = int.Parse(match.Groups[3].Value);
            var dy = int.Parse(match.Groups[4].Value);
            var robot = new Robot(x, y, dx, dy);
            robots.Add(robot);
        }

        // Console.WriteLine("Initial positions:");
        // PrintGrid(robots, width, height);

        var logs = new List<string>();
        for (var i = 0; i < ttp; i++)
        {
            foreach (var robot in robots)
            {
                robot.X = ((robot.X + robot.Dx) % width + width) % width;
                robot.Y = ((robot.Y + robot.Dy) % height + height) % height;
            }

            logs.Add($"\nAfter step {i + 1}:");
            logs.Add(GetGridLog(robots, width, height));

            
            // if (i % 100 == 0)
            // {
            //     var logFile = Path.Join("/Users/daniel/Development/source/AdventOfCode/data/outputs/2024/14",
            //         $"grid_{i}.txt");
            //     File.WriteAllText(logFile, string.Join('\n', logs));
            // }
            // PrintGrid(robots, width, height);
        }

        var space = new int[height, width];
        foreach (var robot in robots)
        {
            space[robot.Y, robot.X]++;
        }

        var q1 = 0;
        var q2 = 0;
        var q3 = 0;
        var q4 = 0;
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                if (i == height / 2 || j == width / 2)
                {
                    continue;
                }

                switch (i)
                {
                    case < height / 2 when j < width / 2:
                        q1 += space[i, j];
                        break;
                    case < height / 2 when j > width / 2:
                        q2 += space[i, j];
                        break;
                    case > height / 2 when j < width / 2:
                        q3 += space[i, j];
                        break;
                    case > height / 2 when j > width / 2:
                        q4 += space[i, j];
                        break;
                }
            }
        }

        return (q1 * q2 * q3 * q4).ToString();
    }

    public string SolvePartB(string[] input)
    {
        const int width = 101;
        const int height = 103;
        const double maxStdDev = 25.0;

        var robots = new List<Robot>();
        foreach (var line in input)
        {
            var regex = RobotRegex();
            var match = regex.Match(line);
            if (!match.Success || match.Groups.Count != 5) // Groups[0] is full match
            {
                throw new FormatException($"Input '{line}' does not match expected format");
            }

            var x = int.Parse(match.Groups[1].Value);
            var y = int.Parse(match.Groups[2].Value);
            var dx = int.Parse(match.Groups[3].Value);
            var dy = int.Parse(match.Groups[4].Value);
            var robot = new Robot(x, y, dx, dy);
            robots.Add(robot);
        }

        int steps = 0;
        while (true)
        {
            // Calculate standard deviation for X and Y coordinates
            double stdDevX = CalculateStdDev(robots.Select(r => r.X));
            double stdDevY = CalculateStdDev(robots.Select(r => r.Y));

            // If both are below our threshold, this might be our tree!
            if (stdDevX < maxStdDev && stdDevY < maxStdDev)
            {
                Console.WriteLine($"\nStep {steps}:");
                // uncomment to print the grid and manually confirm the tree (reporting ((correctly)) assumes first found is the correct one)
                // PrintGrid(robots, width, height);
                return steps.ToString();
            }

            // Move robots
            foreach (var robot in robots)
            {
                robot.X = ((robot.X + robot.Dx) % width + width) % width;
                robot.Y = ((robot.Y + robot.Dy) % height + height) % height;
            }
            steps++;
        }
    }

    private static void PrintGrid(List<Robot> robots, int width, int height)
    {
        var space = new int[height, width];
        foreach (var robot in robots)
        {
            space[robot.Y, robot.X]++;
        }

        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                Console.Write(space[i, j] > 0 ? space[i, j] : ".");
            }

            Console.WriteLine();
        }
    }
    
    private static double CalculateStdDev(IEnumerable<int> values)
    {
        var list = values.ToList();
        double avg = list.Average();
        double sumOfSquaresOfDifferences = list.Select(val => (val - avg) * (val - avg)).Sum();
        return Math.Sqrt(sumOfSquaresOfDifferences / list.Count);
    }

    private static string GetGridLog(List<Robot> robots, int width, int height)
    {
        var sb = new StringBuilder();
        var space = new int[height, width];
        foreach (var robot in robots)
        {
            space[robot.Y, robot.X]++;
        }

        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                sb.Append(space[i, j] > 0 ? space[i, j] : ".");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    [GeneratedRegex(@"^p=(\d+),(\d+) v=(-?\d+),(-?\d+)$")]
    private static partial Regex RobotRegex();
}

internal class Robot(int x, int y, int dx, int dy)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Dx { get; set; } = dx;
    public int Dy { get; set; } = dy;
}