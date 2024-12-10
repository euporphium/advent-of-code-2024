namespace AdventOfCode.Cli.Solvers;

public class Day10Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var map = new TopographicMap(input);
        return map.SumTrailheadScores().ToString();
    }

    public string SolvePartB(string[] input)
    {
        var map = new TopographicMap(input);
        return map.SumTrailheadRatings().ToString();

    }
}

public class TopographicMap
{
    private readonly int _rows;
    private readonly int _cols;
    private readonly int[,] _grid;
    private readonly HashSet<(int row, int col)> _trailheads = [];

    public TopographicMap(string[] input)
    {
        _rows = input.Length;
        _cols = input[0].Length;

        _grid = new int[_rows, _cols];

        for (var i = 0; i < _rows; i++)
        {
            for (var j = 0; j < _cols; j++)
            {
                _grid[i, j] = input[i][j] == '.' ? -1 : int.Parse(input[i][j].ToString());

                if (_grid[i, j] == 0)
                {
                    _trailheads.Add((i, j));
                }
            }
        }
    }

    public int SumTrailheadScores()
    {
        var sum = 0;
        foreach (var (row, col) in _trailheads)
        {
            var trailheadScore = AscendFromPoint(row, col);
            sum += trailheadScore;
        }

        return sum;
    }

    public int SumTrailheadRatings()
    {
        var sum = 0;
        foreach (var (row, col) in _trailheads)
        {
            var trailheadScore = AscendFromPoint(row, col, false); // allow duplicate summits
            sum += trailheadScore;
        }

        return sum;
    }

    private int AscendFromPoint(int row, int col, bool requireUnique = true, HashSet<(int row, int col)>? summits = null)
    {
        summits ??= [];

        var value = _grid[row, col];
        
        if (value == 9)
        {
            // found a summit
            return summits.Add((row, col)) ? 1 : requireUnique ? 0 : 1;
        }
        
        var paths = 0;
        var neighbors = GetNeighbors(row, col);
        foreach (var (nRow, nCol) in neighbors)
        {
            var nValue = _grid[nRow, nCol];

            if (nValue == value + 1)
            {
                paths += AscendFromPoint(nRow, nCol, requireUnique, summits); // keep ascending
            }
        }

        return paths;
    }

    private HashSet<(int row, int col)> GetNeighbors(int row, int col)
    {
        var neighbors = new HashSet<(int row, int col)>();

        if (row > 0)
        {
            neighbors.Add((row - 1, col));
        }

        if (row < _rows - 1)
        {
            neighbors.Add((row + 1, col));
        }

        if (col > 0)
        {
            neighbors.Add((row, col - 1));
        }

        if (col < _cols - 1)
        {
            neighbors.Add((row, col + 1));
        }

        return neighbors;
    }
}