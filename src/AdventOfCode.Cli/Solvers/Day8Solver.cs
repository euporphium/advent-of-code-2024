namespace AdventOfCode.Cli.Solvers;

public class Day8Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var grid = new EasterBunnyAntennaGrid(input);

        return grid.CalculateAntinodes().ToString();
    }

    public string SolvePartB(string[] input)
    {
        var grid = new EasterBunnyAntennaGrid(input);

        return grid.CalculateAntinodesWithHarmonics().ToString();
    }
}

internal record Coordinate(int Row, int Col);

public class EasterBunnyAntennaGrid : Grid
{
    private readonly Dictionary<int, HashSet<Coordinate>> _frequencyAntennaMap = new();

    public EasterBunnyAntennaGrid(string[] lines) : base(lines) { FindAntennas(); }

    private void FindAntennas()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Cols; j++)
            {
                var value = _grid[i][j];
                if (value == '.') continue;

                if (_frequencyAntennaMap.TryGetValue(value, out var coordinates))
                {
                    coordinates.Add(new Coordinate(i, j));
                }
                else
                {
                    _frequencyAntennaMap[value] = [new Coordinate(i, j)];
                }
            }
        }
    }

    public int CalculateAntinodes()
    {
        var antinodeCount = 0;
        foreach (var (_, coordinates) in _frequencyAntennaMap)
        {
            var coordinateList = coordinates.ToList();
            // of these coordinates, get all combinations of 2
            var combinations = new List<(Coordinate, Coordinate)>();
            for (var i = 0; i < coordinateList.Count; i++)
            {
                for (var j = i + 1; j < coordinateList.Count; j++)
                {
                    var coord1 = coordinateList[i];
                    var coord2 = coordinateList[j];
                    combinations.Add((coord1, coord2));
                }
            }

            foreach (var (coord1, coord2) in combinations)
            {
                var rowDiff = coord2.Row - coord1.Row;
                var colDiff = coord2.Col - coord1.Col;

                EvaluatePossibleAntinode(coord2.Row + rowDiff, coord2.Col + colDiff);
                EvaluatePossibleAntinode(coord1.Row - rowDiff, coord1.Col - colDiff);
            }

            continue;

            void EvaluatePossibleAntinode(int row, int col)
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols) return;
                if (_grid[row][col] == '#') return;
                _grid[row][col] = '#';
                antinodeCount++;
            }
        }

        // Console.WriteLine(this);
        return antinodeCount;
    }
    
    public int CalculateAntinodesWithHarmonics()
    {
        var antinodeCount = 0;
        foreach (var (_, coordinates) in _frequencyAntennaMap)
        {
            var coordinateList = coordinates.ToList();
            // of these coordinates, get all combinations of 2
            var combinations = new List<(Coordinate, Coordinate)>();
            for (var i = 0; i < coordinateList.Count; i++)
            {
                for (var j = i + 1; j < coordinateList.Count; j++)
                {
                    var coord1 = coordinateList[i];
                    var coord2 = coordinateList[j];
                    combinations.Add((coord1, coord2));
                }
            }

            foreach (var (coord1, coord2) in combinations)
            {
                var rowDiff = coord2.Row - coord1.Row;
                var colDiff = coord2.Col - coord1.Col;

                var positive = coord2;
                while (IsInBounds(positive.Row, positive.Col))
                {
                    EvaluatePossibleAntinode(positive.Row, positive.Col);
                    positive = new Coordinate(positive.Row + rowDiff, positive.Col + colDiff);
                }

                var negative = coord1;
                while (IsInBounds(negative.Row, negative.Col))
                {
                    EvaluatePossibleAntinode(negative.Row, negative.Col);
                    negative = new Coordinate(negative.Row - rowDiff, negative.Col - colDiff);
                }
            }

            continue;

            void EvaluatePossibleAntinode(int row, int col)
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols) return;
                if (_grid[row][col] == '#') return;
                _grid[row][col] = '#';
                antinodeCount++;
            }
        }

        // Console.WriteLine(this);
        return antinodeCount;
    }
}

public class Grid
{
    protected readonly char[][] _grid;

    public Grid(string[] lines)
    {
        Rows = lines.Length;
        Cols = lines[0].Length;

        _grid = new char[Rows][];
        for (var i = 0; i < Rows; i++)
        {
            _grid[i] = lines[i].ToCharArray();
        }
    }

    public int Rows { get; }
    public int Cols { get; }

    public bool IsInBounds(int row, int col) =>
        row >= 0 && row < Rows && col >= 0 && col < Cols;

    public override string ToString() =>
        string.Join("\n", _grid.Select(row => new string(row)));
}