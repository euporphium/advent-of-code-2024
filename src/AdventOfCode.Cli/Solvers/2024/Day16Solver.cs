namespace AdventOfCode.Cli.Solvers._2024;

public class Day16Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var grid = new Day16Grid(input);
        return grid.AStar().ToString();
    }

    public string SolvePartB(string[] input)
    {
        var grid = new Day16Grid(input);
        var optimalTiles = grid.FindOptimalPathTiles();
        return optimalTiles.Count.ToString();
    }
}

internal partial class Day16Grid
{
    private readonly int _rows;
    private readonly int _cols;
    private readonly char[,] _grid;
    private (int Row, int Col) _start;
    private (int Row, int Col) _end;

    public Day16Grid(string[] input)
    {
        _rows = input.Length;
        _cols = input[0].Length;

        _grid = new char[_rows, _cols];
        for (var i = 0; i < _rows; i++)
        {
            for (var j = 0; j < _cols; j++)
            {
                _grid[i, j] = input[i][j];
                switch (_grid[i, j])
                {
                    case 'S':
                        _start = (i, j);
                        break;
                    case 'E':
                        _end = (i, j);
                        break;
                }
            }
        }
    }

    // public int AStar()
    // {
    //     var initialState = new State(_start.Row, _start.Col, Direction.East);
    //
    //     var openSet = new PriorityQueue<State, int>();
    //     openSet.Enqueue(initialState, 0);
    //
    //     var gScores = new Dictionary<State, int> { [initialState] = 0 };
    //
    //     while (openSet.Count > 0)
    //     {
    //         var current = openSet.Dequeue();
    //         var currentPos = (current.Row, current.Col);
    //
    //         // Check if we've reached the end
    //         if (currentPos == _end)
    //         {
    //             return gScores[current];
    //         }
    //
    //         // Try moving forward
    //         var (nextRow, nextCol) = current.GetNextPosition();
    //         if (IsValid(nextRow, nextCol, _rows, _cols) && _grid[nextRow, nextCol] != '#')
    //         {
    //             var nextState = new State(nextRow, nextCol, current.Dir);
    //             var tentativeG = gScores[current] + 1; // Cost of moving forward
    //
    //             if (!gScores.TryGetValue(nextState, out var value) || tentativeG < value)
    //             {
    //                 value = tentativeG;
    //                 gScores[nextState] = value;
    //                 var hScore = ManhattanDistance((nextRow, nextCol), _end) +
    //                              GetMinimumRotations(nextState.Dir, (nextRow, nextCol), _end) * 1000;
    //                 openSet.Enqueue(nextState, tentativeG + hScore);
    //             }
    //         }
    //
    //         // Try rotating left and right
    //         foreach (var nextDir in new[] { current.TurnLeft(), current.TurnRight() })
    //         {
    //             var nextState = current with { Dir = nextDir };
    //             var tentativeG = gScores[current] + 1000; // Cost of rotation
    //
    //             if (!gScores.TryGetValue(nextState, out var value) || tentativeG < value)
    //             {
    //                 value = tentativeG;
    //                 gScores[nextState] = value;
    //                 var hScore = ManhattanDistance(currentPos, _end) +
    //                              GetMinimumRotations(nextDir, currentPos, _end) * 1000;
    //                 openSet.Enqueue(nextState, tentativeG + hScore);
    //             }
    //         }
    //     }
    //
    //     return int.MaxValue;
    // }

    private static int ManhattanDistance((int Row, int Col) a, (int Row, int Col) b)
        => Math.Abs(b.Row - a.Row) + Math.Abs(b.Col - a.Col);

    private static int GetMinimumRotations(Direction currentDir, (int Row, int Col) current, (int Row, int Col) target)
    {
        var dx = target.Col - current.Col;
        var dy = target.Row - current.Row;

        // If we need to move both horizontally and vertically, we need at least one rotation
        if (dx != 0 && dy != 0) return 1;

        // If we're not moving directly towards the target, we need at least one rotation
        return currentDir switch
        {
            Direction.East when dx <= 0 => 1,
            Direction.West when dx >= 0 => 1,
            Direction.South when dy <= 0 => 1,
            Direction.North when dy >= 0 => 1,
            _ => 0
        };
    }

    private static bool IsValid(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;
}

internal partial class Day16Grid
{
    private readonly Dictionary<State, int> _bestScores = new();
    private readonly Dictionary<State, HashSet<State>> _cameFrom = new();

    public HashSet<(int Row, int Col)> FindOptimalPathTiles()
    {
        var optimalTiles = new HashSet<(int Row, int Col)> { _start, _end }; // Always include start and end
        var minScore = AStar();

        // Find all end states with optimal score
        var optimalEndStates = _bestScores
            .Where(kvp => kvp.Key.Row == _end.Row && kvp.Key.Col == _end.Col && kvp.Value == minScore)
            .Select(kvp => kvp.Key);

        foreach (var endState in optimalEndStates)
        {
            ReconstructAllPaths(endState, optimalTiles, new HashSet<State>());
        }

        return optimalTiles;
    }

    private void ReconstructAllPaths(State current, HashSet<(int Row, int Col)> optimalTiles,
        HashSet<State> visitedStates)
    {
        if (!visitedStates.Add(current))
            return;

        optimalTiles.Add((current.Row, current.Col));

        if (!_cameFrom.TryGetValue(current, out var value))
            return;

        foreach (var prev in value)
        {
            ReconstructAllPaths(prev, optimalTiles, visitedStates);
        }
    }

    public int AStar()
    {
        _cameFrom.Clear();
        _bestScores.Clear();
        var initialState = new State(_start.Row, _start.Col, Direction.East);

        var openSet = new PriorityQueue<State, int>();
        openSet.Enqueue(initialState, 0);

        _bestScores[initialState] = 0;
        var minScore = int.MaxValue;

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            var currentScore = _bestScores[current];

            // If we've reached the end, update minimum score
            if ((current.Row, current.Col) == _end)
            {
                minScore = Math.Min(minScore, currentScore);
                continue;
            }

            // If current path is already worse than best found, skip it
            if (minScore != int.MaxValue && currentScore > minScore)
                continue;

            // Try moving forward
            var (nextRow, nextCol) = current.GetNextPosition();
            if (IsValid(nextRow, nextCol, _rows, _cols) && _grid[nextRow, nextCol] != '#')
            {
                var nextState = new State(nextRow, nextCol, current.Dir);
                var tentativeG = currentScore + 1;

                ProcessNextState(current, nextState, tentativeG, openSet);
            }

            // Try rotating left and right
            foreach (var nextDir in new[] { current.TurnLeft(), current.TurnRight() })
            {
                var nextState = current with { Dir = nextDir };
                var tentativeG = currentScore + 1000;

                ProcessNextState(current, nextState, tentativeG, openSet);
            }
        }

        return minScore;
    }

    private void ProcessNextState(State current, State nextState, int tentativeG, PriorityQueue<State, int> openSet)
    {
        if (!_bestScores.TryGetValue(nextState, out var previousScore))
        {
            // New state, always add it
            _cameFrom[nextState] = [current];
            _bestScores[nextState] = tentativeG;
            var hScore = ManhattanDistance((nextState.Row, nextState.Col), _end) +
                         GetMinimumRotations(nextState.Dir, (nextState.Row, nextState.Col), _end) * 1000;
            openSet.Enqueue(nextState, tentativeG + hScore);
        }
        else if (tentativeG <= previousScore)
        {
            // Equal or better path to existing state
            if (tentativeG < previousScore)
            {
                // Better path, clear previous paths
                _cameFrom[nextState] = [current];
            }
            else
            {
                // Equal path, add to existing paths
                if (!_cameFrom.ContainsKey(nextState))
                    _cameFrom[nextState] = [];
                _cameFrom[nextState].Add(current);
            }

            _bestScores[nextState] = tentativeG;
            var hScore = ManhattanDistance((nextState.Row, nextState.Col), _end) +
                         GetMinimumRotations(nextState.Dir, (nextState.Row, nextState.Col), _end) * 1000;
            openSet.Enqueue(nextState, tentativeG + hScore);
        }
    }
}

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

public readonly record struct State(int Row, int Col, Direction Dir)
{
    public (int Row, int Col) GetNextPosition()
    {
        return Dir switch
        {
            Direction.North => (Row - 1, Col),
            Direction.East => (Row, Col + 1),
            Direction.South => (Row + 1, Col),
            Direction.West => (Row, Col - 1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public Direction TurnLeft() => Dir switch
    {
        Direction.North => Direction.West,
        Direction.West => Direction.South,
        Direction.South => Direction.East,
        Direction.East => Direction.North,
        _ => throw new ArgumentOutOfRangeException()
    };

    public Direction TurnRight() => Dir switch
    {
        Direction.North => Direction.East,
        Direction.East => Direction.South,
        Direction.South => Direction.West,
        Direction.West => Direction.North,
        _ => throw new ArgumentOutOfRangeException()
    };
}