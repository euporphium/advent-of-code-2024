namespace AdventOfCode.Cli.Solvers._2024;

public class Day18Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var memorySpace = new MemorySpace(71, input);
        memorySpace.SimulateFallingBytes(1024);
        return memorySpace.FindShortestPathLength().ToString();
    }

    public string SolvePartB(string[] input)
    {
        var memorySpace = new MemorySpace(71, input);
        return memorySpace.SimulateFallingBytesUntilBlocked().ToString();
    }
}

internal class MemorySpace
{
    private readonly int _rows;
    private readonly int _cols;
    private readonly bool[,] _grid;
    private readonly List<(int Row, int Col)> _incomingBytePositions = [];
    private readonly (int Row, int Col) _start;
    private readonly (int Row, int Col) _end;

    public MemorySpace(int gridSize, string[] input)
    {
        _rows = gridSize;
        _cols = gridSize;
        _grid = new bool[_rows, _cols];

        _start = (0, 0);
        _end = (_rows - 1, _cols - 1);

        foreach (var line in input)
        {
            var coords = line.Split(',').Select(int.Parse).ToArray();
            _incomingBytePositions.Add((coords[0], coords[1]));
        }
    }

    public void SimulateFallingBytes(int count)
    {
        if (count > _incomingBytePositions.Count)
        {
            throw new Exception("Not enough incoming bytes to simulate");
        }

        for (var i = 0; i < count; i++)
        {
            _grid[_incomingBytePositions[i].Row, _incomingBytePositions[i].Col] = true;
        }
    }

    public (int Row, int Col) SimulateFallingBytesUntilBlocked()
    {
        for (var i = 0; i < _incomingBytePositions.Count; i++)
        {
            _grid[_incomingBytePositions[i].Row, _incomingBytePositions[i].Col] = true;
            if (FindShortestPathLength() == int.MaxValue)
            {
                return _incomingBytePositions[i];
            }
        }
        
        throw new Exception("No blocking byte found");
    }

    public int FindShortestPathLength()
    {
        int[] dx = [-1, 0, 1, 0];
        int[] dy = [0, 1, 0, -1];

        var queue = new Queue<(int row, int col)>();
        var visited = new bool[_rows, _cols];

        queue.Enqueue(_start);
        visited[_start.Row, _start.Col] = true;
        var distance = new int[_rows, _cols];

        // Initialize distances to "infinity"
        for (var i = 0; i < _rows; i++)
        for (var j = 0; j < _cols; j++)
            distance[i, j] = int.MaxValue;

        distance[_start.Row, _start.Col] = 0;

        while (queue.Count > 0)
        {
            var (currentRow, currentCol) = queue.Dequeue();
            var currentDistance = distance[currentRow, currentCol];

            // Check all 4 adjacent cells
            for (var i = 0; i < 4; i++)
            {
                var newRow = currentRow + dx[i];
                var newCol = currentCol + dy[i];

                // Check if the new position is valid and not visited
                if (IsValid(newRow, newCol) &&
                    !visited[newRow, newCol] &&
                    !IsBlocked(newRow, newCol))
                {
                    queue.Enqueue((newRow, newCol));
                    visited[newRow, newCol] = true;
                    distance[newRow, newCol] = currentDistance + 1;
                }
            }
        }

        return distance[_end.Row, _end.Col];
    }

    private bool IsValid(int row, int col) { return row >= 0 && row < _rows && col >= 0 && col < _cols; }
    private bool IsBlocked(int row, int col) => _grid[row, col];
}