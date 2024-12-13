namespace AdventOfCode.Cli.Solvers._2024;

public class Day12Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var regions = FindRegions(input);
        return regions.Sum(r => (long)r.Area * r.Perimeter).ToString();
    }

    public string SolvePartB(string[] input)
    {
        var regions = FindRegions(input);
        return regions.Sum(r => (long)r.Area * r.Corners).ToString();
    }

    private record struct Region(int Area, int Perimeter, int Corners);

    private List<Region> FindRegions(string[] input)
    {
        var regions = new List<Region>();
        var grid = input.Select(line => line.ToCharArray()).ToArray();
        var visited = new HashSet<(int y, int x)>();

        // Iterate through all positions in the grid
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (visited.Contains((y, x))) continue;

                var area = 0;
                var perimeter = 0;
                var spaces = new Dictionary<(int y, int x), (int y, int x, int edges)>();
                var inRegionPositions = new Queue<(int y, int x)>();
                inRegionPositions.Enqueue((y, x));
                var regionChar = grid[y][x];

                while (inRegionPositions.Count > 0)
                {
                    var current = inRegionPositions.Dequeue();
                    if (!visited.Add(current)) continue;

                    var adjacent = new[]
                    {
                        (current.y - 1, current.x), // up
                        (current.y + 1, current.x), // down
                        (current.y, current.x - 1), // left
                        (current.y, current.x + 1)  // right
                    }.Where(pos => IsInGrid(pos, grid));

                    var inRegionAdjacent = 0;
                    foreach (var pos in adjacent)
                    {
                        if (grid[pos.Item1][pos.Item2] == regionChar)
                        {
                            inRegionAdjacent++;
                            if (!visited.Contains(pos))
                                inRegionPositions.Enqueue(pos);
                        }
                    }

                    var edges = 4 - inRegionAdjacent;
                    perimeter += edges;
                    area++;
                    spaces[current] = (current.y, current.x, edges);
                }

                if (area > 0)  // Only add regions that have area
                {
                    var corners = CountCorners(spaces, grid.Length, grid[0].Length);
                    regions.Add(new Region(area, perimeter, corners));
                }
            }
        }

        return regions;
    }

    private int CountCorners(Dictionary<(int y, int x), (int y, int x, int edges)> spaces, int maxY, int maxX)
    {
        int corners = 0;
        foreach (var space in spaces.Values)
        {
            bool HasSpace(int y, int x) => spaces.ContainsKey((y, x));
            bool InBounds(int y, int x) => y >= 0 && y < maxY && x >= 0 && x < maxX;

            var (y, x, edges) = space;
            bool hasUp = HasSpace(y - 1, x);
            bool hasDown = HasSpace(y + 1, x);
            bool hasLeft = HasSpace(y, x - 1);
            bool hasRight = HasSpace(y, x + 1);
            bool hasUpperRight = HasSpace(y - 1, x + 1);
            bool hasLowerRight = HasSpace(y + 1, x + 1);
            bool hasLowerLeft = HasSpace(y + 1, x - 1);
            bool hasUpperLeft = HasSpace(y - 1, x - 1);

            bool inBoundsUpperRight = InBounds(y - 1, x + 1);
            bool inBoundsLowerRight = InBounds(y + 1, x + 1);
            bool inBoundsLowerLeft = InBounds(y + 1, x - 1);
            bool inBoundsUpperLeft = InBounds(y - 1, x - 1);

            if (edges == 4)
                corners += 4;
            else if (edges == 3)
                corners += 2;
            else if (edges == 2)
            {
                if (hasUp && hasRight)
                    corners += hasUpperRight ? 1 : 2;
                else if (hasRight && hasDown)
                    corners += hasLowerRight ? 1 : 2;
                else if (hasDown && hasLeft)
                    corners += hasLowerLeft ? 1 : 2;
                else if (hasLeft && hasUp)
                    corners += hasUpperLeft ? 1 : 2;
            }
            else if (edges == 1)
            {
                if (hasLeft && hasDown && hasRight && !hasUp)
                {
                    if (!hasLowerRight && inBoundsLowerRight) corners++;
                    if (!hasLowerLeft && inBoundsLowerLeft) corners++;
                }
                else if (hasLeft && hasDown && hasUp && !hasRight)
                {
                    if (!hasUpperLeft && inBoundsUpperLeft) corners++;
                    if (!hasLowerLeft && inBoundsLowerLeft) corners++;
                }
                else if (hasLeft && hasUp && hasRight && !hasDown)
                {
                    if (!hasUpperRight && inBoundsUpperRight) corners++;
                    if (!hasUpperLeft && inBoundsUpperLeft) corners++;
                }
                else if (hasUp && hasDown && hasRight && !hasLeft)
                {
                    if (!hasLowerRight && inBoundsLowerRight) corners++;
                    if (!hasUpperRight && inBoundsUpperRight) corners++;
                }
            }
            else if (edges == 0)
            {
                if (!hasLowerRight && inBoundsLowerRight) corners++;
                if (!hasLowerLeft && inBoundsLowerLeft) corners++;
                if (!hasUpperRight && inBoundsUpperRight) corners++;
                if (!hasUpperLeft && inBoundsUpperLeft) corners++;
            }
        }
        return corners;
    }

    private bool IsInGrid((int y, int x) pos, char[][] grid) =>
        pos.y >= 0 && pos.y < grid.Length && pos.x >= 0 && pos.x < grid[pos.y].Length;
}