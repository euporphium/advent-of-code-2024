namespace AdventOfCode.Cli.Solvers._2024;

public class Day6Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var patrol = new GuardPatrol(input);
        var count = patrol.CountVisitedTiles();
        return count.ToString();
    }

    public string SolvePartB(string[] input)
    {
        var patrol = new GuardPatrol(input);
        var count = patrol.CountPossibleLoopPositions();
        return count.ToString();
    }
}

public class GuardPatrol
{
    private readonly char[,] _grid;
    private readonly HashSet<(int row, int col)> _visited = new();
    private (int row, int col) _position;

    private enum Direction
    {
        North,
        East,
        South,
        West
    }

    private Direction _facing;

    public GuardPatrol(string[] input)
    {
        _grid = new char[input.Length, input[0].Length];

        // Initialize grid and find starting position
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = 0; j < input[i].Length; j++)
            {
                _grid[i, j] = input[i][j];
                if (IsGuard(input[i][j]))
                {
                    _position = (i, j);
                    _facing = GetInitialDirection(input[i][j]);
                }
            }
        }
    }

    public int CountVisitedTiles()
    {
        _visited.Add(_position);

        while (true)
        {
            var nextPos = GetNextPosition();
            if (!IsValidPosition(nextPos))
                break;

            if (_grid[nextPos.row, nextPos.col] == '#')
            {
                // Hit wall, turn right
                _facing = (Direction)(((int)_facing + 1) % 4);
                continue;
            }

            // Move to next position
            _position = nextPos;
            _visited.Add(_position);
        }

        return _visited.Count;
    }

    public int CountPossibleLoopPositions()
    {
        var loopCount = 0;
        var currentPos = _position;
        var currentDir = _facing;

        while (true)
        {
            var nextPos = GetNextPosition(currentPos, currentDir);
            if (!IsValidPosition(nextPos))
                break;

            switch (_grid[nextPos.row, nextPos.col])
            {
                case '#':
                    currentDir = (Direction)(((int)currentDir + 1) % 4);
                    continue;
                // Test for loop with obstacle at this position
                case '.':
                {
                    var originalState = (currentPos, currentDir);
                    if (CreatesLoopFromState(nextPos, originalState))
                        loopCount++;
                    break;
                }
            }

            // Continue normal path
            currentPos = nextPos;
        }

        return loopCount;
    }

    private bool CreatesLoopFromState((int row, int col) obstaclePos,
        ((int row, int col) pos, Direction dir) startState)
    {
        var visited = new HashSet<((int row, int col) pos, Direction dir)> { startState };
        var pos = startState.pos;
        var dir = (Direction)(((int)startState.dir + 1) % 4); // First turn at obstacle

        while (true)
        {
            var nextPos = GetNextPosition(pos, dir);
            if (!IsValidPosition(nextPos))
                return false;

            if (_grid[nextPos.row, nextPos.col] == '#' || nextPos == obstaclePos)
            {
                var state = (pos, dir);
                if (!visited.Add(state))
                    return true;

                dir = (Direction)(((int)dir + 1) % 4);
                continue;
            }

            pos = nextPos;
        }
    }

    private bool IsValidPosition((int row, int col) pos) =>
        pos.row >= 0 && pos.row < _grid.GetLength(0) &&
        pos.col >= 0 && pos.col < _grid.GetLength(1);


    private (int row, int col) GetNextPosition() => GetNextPosition(_position, _facing);

    private static bool IsGuard(char c) => c is '^' or '>' or 'v' or '<';

    private static Direction GetInitialDirection(char c) => c switch
    {
        '^' => Direction.North,
        '>' => Direction.East,
        'v' => Direction.South,
        '<' => Direction.West,
        _ => throw new ArgumentException($"Invalid guard character: {c}")
    };

    private static (int row, int col) GetNextPosition((int row, int col) pos, Direction dir)
    {
        return dir switch
        {
            Direction.North => (pos.row - 1, pos.col),
            Direction.East => (pos.row, pos.col + 1),
            Direction.South => (pos.row + 1, pos.col),
            Direction.West => (pos.row, pos.col - 1),
            _ => throw new ArgumentException($"Invalid direction: {dir}")
        };
    }
}