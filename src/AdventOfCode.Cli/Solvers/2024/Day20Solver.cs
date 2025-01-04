namespace AdventOfCode.Cli.Solvers._2024;

public class Day20Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var racetrack = new Racetrack(input);
        return racetrack.CountCheatPaths(100, 2).ToString();
    }

    public string SolvePartB(string[] input)
    {
        var racetrack = new Racetrack(input);
        return racetrack.CountCheatPaths(100, 20).ToString();
    }
}

internal class Racetrack
{
    private readonly int _rows;
    private readonly int _cols;
    private readonly char[,] _grid;
    private readonly int[,] _legalDistances;

    public Racetrack(string[] input)
    {
        _rows = input.Length;
        _cols = input[0].Length;

        _grid = new char[_rows, _cols];

        for (var i = 0; i < _rows; i++)
        {
            for (var j = 0; j < _cols; j++)
            {
                var value = input[i][j];
                switch (value)
                {
                    case 'S':
                        Start = (i, j);
                        _grid[i, j] = '.';
                        break;
                    case 'E':
                        Finish = (i, j);
                        _grid[i, j] = '.';
                        break;
                    default:
                        _grid[i, j] = value;
                        break;
                }

                _grid[i, j] = input[i][j];
            }
        }

        _legalDistances = new int[_rows, _cols];
    }

    public (int Row, int Col) Start { get; }
    public (int Row, int Col) Finish { get; }

    public int CountCheatPaths(int minTimeSaved, int allowedCheatTime)
    {
        CalculateLegalDistances();

        var sum = 0;
        for (var i = 0; i < _rows; i++)
        for (var j = 0; j < _cols; j++)
        {
            var current = _legalDistances[i, j];
            if (current == int.MaxValue) continue;

            var currentPos = (i, j);
            var neighbors = GetNeighborsWithinDistance(currentPos, allowedCheatTime);
            sum += neighbors.Count(neighbor =>
            {
                var cheatCost = ManhattanDistance(currentPos, neighbor);
                return current - _legalDistances[neighbor.row, neighbor.col] - cheatCost >= minTimeSaved;
            });
        }

        return sum;
    }

    private void CalculateLegalDistances()
    {
        for (var i = 0; i < _rows; i++)
        for (var j = 0; j < _cols; j++)
            _legalDistances[i, j] = int.MaxValue;

        var visited = new bool[_rows, _cols];
        var queue = new Queue<(int Row, int Col)>();

        queue.Enqueue(Finish);
        visited[Finish.Row, Finish.Col] = true;
        _legalDistances[Finish.Row, Finish.Col] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var neighbor in GetNeighbors(current))
            {
                if (visited[neighbor.row, neighbor.col] || _grid[neighbor.row, neighbor.col] == '#') continue;
                visited[neighbor.row, neighbor.col] = true;
                _legalDistances[neighbor.row, neighbor.col] = _legalDistances[current.Row, current.Col] + 1;
                queue.Enqueue(neighbor);
            }
        }
    }

    private IEnumerable<(int row, int col)> GetNeighbors((int row, int col) position)
    {
        var (row, col) = position;
        if (row > 0) yield return (row - 1, col);
        if (row < _rows - 1) yield return (row + 1, col);
        if (col > 0) yield return (row, col - 1);
        if (col < _cols - 1) yield return (row, col + 1);
    }

    private IEnumerable<(int row, int col)> GetNeighborsWithinDistance((int row, int col) position, int distance)
    {
        var (centerRow, centerCol) = position;

        // For each possible row offset
        for (var rowOffset = -distance; rowOffset <= distance; rowOffset++)
        {
            var newRow = centerRow + rowOffset;
            if (newRow < 0 || newRow >= _rows) continue;

            // Remaining distance we can move horizontally after moving vertically
            var remainingDist = distance - Math.Abs(rowOffset);

            // For each possible column offset within remaining distance
            for (var colOffset = -remainingDist; colOffset <= remainingDist; colOffset++)
            {
                var newCol = centerCol + colOffset;
                if (newCol < 0 || newCol >= _cols) continue;

                // Skip the center position itself
                if (rowOffset == 0 && colOffset == 0) continue;

                // Skip worse positions
                if (_legalDistances[newRow, newCol] > _legalDistances[centerRow, centerCol]) continue;

                yield return (newRow, newCol);
            }
        }
    }

    private static int ManhattanDistance((int row, int col) a, (int row, int col) b)
    {
        return Math.Abs(a.row - b.row) + Math.Abs(a.col - b.col);
    }
}