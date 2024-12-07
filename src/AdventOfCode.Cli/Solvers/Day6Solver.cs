namespace AdventOfCode.Cli.Solvers;

public class Day6Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var tracker = new GuardTracker(input);
        var explored = tracker.Patrol();

        return explored.ToString();
    }

    public string SolvePartB(string[] input)
    {
        var tracker = new GuardTracker(input);
        var count = tracker.FindLoopCreatingObstacleOptions();
        
        return count.ToString();
    }
}

internal class GuardTracker
{
    private readonly char[][] _grid;
    private readonly List<Coordinate> _obstacles = [];
    private readonly HashSet<Coordinate> _explored = [];
    private Guard _guard;
    
    private readonly HashSet<Coordinate> _empty = [];
    private Coordinate? _shenanigan;

    public GuardTracker(string[] input)
    {
        var rows = input.Length;
        var cols = input[0].Length;

        _grid = new char[rows][];
        for (var i = 0; i < rows; i++)
        {
            _grid[i] = new char[cols];
            for (var j = 0; j < cols; j++)
            {
                var value = input[i][j];
                _grid[i][j] = value;

                var coordinate = new Coordinate(i, j);
                switch (value)
                {
                    case '.':
                        _empty.Add(coordinate);
                        break;
                    case '#':
                        _obstacles.Add(coordinate);
                        break;
                    case '^':
                        SetGuard(coordinate, Direction.Up);
                        break;
                    case '>':
                        SetGuard(coordinate, Direction.Right);
                        break;
                    case 'V':
                    case 'v':
                        SetGuard(coordinate, Direction.Down);
                        break;
                    case '<':
                        SetGuard(coordinate, Direction.Left);
                        break;
                    default:
                        throw new ArgumentException("Unexpected character in input");
                }
            }
        }

        if (_guard == null)
        {
            throw new ArgumentException("Guard not found in input");
        }
    }

    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public int Patrol()
    {
        var next = GetNext();
        while (next != null)
        {
            if (_obstacles.Contains(next) || next == _shenanigan)
            {
                SetGuard(_guard.Position, TurnRight(_guard.Orientation));
            }
            else
            {
                SetGuard(next, _guard.Orientation);
            }

            next = GetNext();
        }

        return _explored.Count;
    }

    public int FindLoopCreatingObstacleOptions()
    {
        var sum = 0;
        var next = GetNext();
        while (next != null)
        {
            if (_obstacles.Contains(next))
            {
                SetGuard(_guard.Position, TurnRight(_guard.Orientation));
            }
            else
            {
                // explore with shenanigan
                var guard = _guard;
                _shenanigan = next;
                SetGuard(_guard.Position, TurnRight(_guard.Orientation));
                if (HasLoop())
                {
                    Console.WriteLine($"Loop found. Total: {++sum}");
                }
                
                // explore without shenanigan
                _guard = guard;
                _shenanigan = null;
                SetGuard(next, _guard.Orientation);
            }

            next = GetNext();
        }

        return sum;
    }

    private bool HasLoop()
    {
        var encountered = new HashSet<Guard>();
        var next = GetNext();
        while (next != null)
        {
            // PrintGrid();
            if (encountered.Contains(_guard))
            {
                return true;
            }
            
            if (_obstacles.Contains(next) || next == _shenanigan)
            {
                encountered.Add(_guard);
                SetGuard(_guard.Position, TurnRight(_guard.Orientation));
            }
            else
            {
                SetGuard(next, _guard.Orientation);
            }

            next = GetNext();
        }

        return false;
    }

    private void PrintGrid()
    {
        Console.Clear();
        for (var i = 0; i < _grid.Length; i++)
        {
            for (var j = 0; j < _grid[i].Length; j++)
            {
                var coordinate = new Coordinate(i, j);
                if (_guard.Position.Equals(coordinate))
                {
                    Console.Write(_guard.Orientation switch
                    {
                        Direction.Up => '^',
                        Direction.Right => '>',
                        Direction.Down => 'v',
                        Direction.Left => '<',
                        _ => throw new InvalidOperationException("Invalid guard orientation")
                    });
                }
                else if (_obstacles.Contains(coordinate))
                {
                    Console.Write('#');
                }
                else if (_explored.Contains(coordinate))
                {
                    Console.Write('X');
                }
                else if (coordinate == _shenanigan)
                {
                    Console.Write('O');
                }
                else
                {
                    Console.Write('.');
                }
            }

            Console.WriteLine();
        }
    }

    private Coordinate? GetNext()
    {
        var next = _guard.Orientation switch
        {
            Direction.Up => _guard.Position with { X = _guard.Position.X - 1 },
            Direction.Right => _guard.Position with { Y = _guard.Position.Y + 1 },
            Direction.Down => _guard.Position with { X = _guard.Position.X + 1 },
            Direction.Left => _guard.Position with { Y = _guard.Position.Y - 1 },
            _ => throw new InvalidOperationException("Invalid guard orientation")
        };

        if (next.X < 0 || next.X >= _grid.Length || next.Y < 0 || next.Y >= _grid[0].Length)
        {
            return null;
        }

        return next;
    }

    private static Direction TurnRight(Direction current) => (Direction)((int)(current + 1) % 4);

    private void SetGuard(Coordinate position, Direction orientation)
    {
        _guard = new Guard(position, orientation);
        _explored.Add(position);
    }

    private record Coordinate(int X, int Y);

    private record Guard(Coordinate Position, Direction Orientation);
}