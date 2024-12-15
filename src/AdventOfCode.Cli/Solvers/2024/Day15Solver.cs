namespace AdventOfCode.Cli.Solvers._2024;

public class Day15Solver : ISolver
{
    private static readonly Direction Up = new(-1, 0);
    private static readonly Direction Down = new(1, 0);
    private static readonly Direction Left = new(0, -1);
    private static readonly Direction Right = new(0, 1);

    private readonly record struct Direction(int RowDelta, int ColDelta);

    private readonly record struct Position(int Row, int Col)
    {
        public Position Move(Direction dir) => new(Row + dir.RowDelta, Col + dir.ColDelta);
    }

    public string SolvePartA(string[] input)
    {
        var (map, moves) = ParseInput(input);
        
        var robot = FindRobot(map);
        Move(robot, moves, map);
        return CalculateScore(map, 'O').ToString();
    }

    public string SolvePartB(string[] input)
    {
        var (map, moves) = ParseInput(input);

        var expandedMap = ExpandMap(map);
        var robot = FindRobot(expandedMap);
        Move(robot, moves, expandedMap);
        return CalculateScore(expandedMap, '[').ToString();
    }

    private static (char[,], Queue<Direction>) ParseInput(string[] input)
    {
        var separatorIndex = Array.IndexOf(input, string.Empty);

        if (separatorIndex == -1)
        {
            throw new Exception("I'm not even supposed to be here today");
        }

        var rawMap = input.Take(separatorIndex).ToArray();
        var rawMoves = string.Join("", input.Skip(separatorIndex + 1));

        var map = new char[rawMap.Length, rawMap[0].Length];
        for (var row = 0; row < map.GetLength(0); row++)
        for (var col = 0; col < map.GetLength(1); col++)
            map[row, col] = rawMap[row][col];
        
        var moves = new Queue<Direction>(rawMoves.Select(c => c switch
        {
            '<' => Left,
            '>' => Right,
            '^' => Up,
            'v' => Down,
            _ => throw new InvalidOperationException($"Invalid direction: {c}")
        }));

        return (map, moves);
    }

    private static bool CanMoveTo(Position pos, char[,] map) => GetAt(pos, map) == '.';

    private static char? GetAt(Position pos, char[,] map)
    {
        if (pos.Row < 0 || pos.Col < 0 ||
            pos.Row >= map.GetLength(0) || pos.Col >= map.GetLength(1))
            return null;
        return map[pos.Row, pos.Col];
    }

    private static (Position left, Position right) GetBoxPositions(Position pos, char[,] map)
    {
        return map[pos.Row, pos.Col] == '['
            ? (pos, pos with { Col = pos.Col + 1 })
            : (pos with { Col = pos.Col - 1 }, pos);
    }

    private static bool CheckWideBox(Position box, Direction dir, char[,] map)
    {
        var (left, right) = GetBoxPositions(box, map);

        if (dir == Left)
        {
            var nextChar = GetAt(left.Move(dir), map);
            return nextChar switch
            {
                null => false,
                ']' => CheckWideBox(left.Move(dir), dir, map),
                '#' => false,
                '.' => true,
                _ => false
            };
        }

        if (dir == Right)
        {
            var nextChar = GetAt(right.Move(dir), map);
            return nextChar switch
            {
                null => false,
                '[' => CheckWideBox(right.Move(dir), dir, map),
                '#' => false,
                '.' => true,
                _ => false
            };
        }

        // UP or DOWN
        var nextLeft = GetAt(left.Move(dir), map);
        var nextRight = GetAt(right.Move(dir), map);

        if (nextLeft == '#' || nextRight == '#')
            return false;

        var checkNeeded = false;

        if (nextLeft is '[' or ']')
        {
            checkNeeded = true;
            if (!CheckWideBox(left.Move(dir), dir, map))
                return false;
        }

        if (nextRight is '[' or ']')
        {
            checkNeeded = true;
            if (!CheckWideBox(right.Move(dir), dir, map))
                return false;
        }

        return checkNeeded || (nextLeft == '.' && nextRight == '.');
    }

    private static bool TryMoveHorizontal(Position lead, Position trail, Direction dir, char[,] map)
    {
        var nextPos = lead.Move(dir);
        switch (GetAt(nextPos, map))
        {
            case null or '#':
                return false;
            case '[' or ']':
                if (!MoveWideBox(nextPos, dir, map))
                    return false;
                break;
        }

        MoveItem(lead, dir, map);
        MoveItem(trail, dir, map);
        return true;
    }

    private static bool TryMoveVertical(Position left, Position right, Direction dir, char[,] map)
    {
        var nextLeft = GetAt(left.Move(dir), map);
        var nextRight = GetAt(right.Move(dir), map);

        if (nextLeft == '#' || nextRight == '#')
            return false;

        var leftIsBox = nextLeft is '[' or ']';
        var rightIsBox = nextRight is '[' or ']';
        var sameBox = nextLeft == '[' && nextRight == ']';

        if (leftIsBox && !CheckWideBox(left.Move(dir), dir, map))
            return false;
        if (rightIsBox && !sameBox && !CheckWideBox(right.Move(dir), dir, map))
            return false;

        if (leftIsBox)
            MoveWideBox(left.Move(dir), dir, map);
        if (rightIsBox && !sameBox)
            MoveWideBox(right.Move(dir), dir, map);

        MoveItem(left, dir, map);
        MoveItem(right, dir, map);
        return true;
    }

    private static void Move(Position current, Queue<Direction> moves, char[,] map)
    {
        while (moves.Count != 0)
        {
            var direction = moves.Dequeue();
            var nextPos = current.Move(direction);

            switch (GetAt(nextPos, map))
            {
                case '.':
                case 'O' when MoveBox(nextPos, direction, map):
                case '[' or ']' when MoveWideBox(nextPos, direction, map):
                    MoveItem(current, direction, map);
                    current = nextPos;
                    continue;

                default:
                    continue;
            }
        }
    }

    private static void MoveItem(Position from, Direction dir, char[,] map)
    {
        var to = from.Move(dir);
        (map[to.Row, to.Col], map[from.Row, from.Col]) = (map[from.Row, from.Col], '.');
    }

    private static bool MoveBox(Position box, Direction dir, char[,] map)
    {
        if (CanMoveTo(box.Move(dir), map))
        {
            MoveItem(box, dir, map);
            return true;
        }

        var next = box.Move(dir);
        if (GetAt(next, map) == 'O' && MoveBox(next, dir, map))
        {
            MoveItem(box, dir, map);
            return true;
        }

        return false;
    }

    private static bool MoveWideBox(Position box, Direction dir, char[,] map)
    {
        var (left, right) = GetBoxPositions(box, map);

        if (dir == Left)
            return TryMoveHorizontal(left, right, dir, map);
        if (dir == Right)
            return TryMoveHorizontal(right, left, dir, map);

        return TryMoveVertical(left, right, dir, map);
    }

    private static Position FindRobot(char[,] map)
    {
        for (var row = 0; row < map.GetLength(0); row++)
        for (var col = 0; col < map.GetLength(1); col++)
            if (map[row, col] == '@')
                return new Position(row, col);

        throw new InvalidOperationException("No robot found in map");
    }

    private static long CalculateScore(char[,] map, char targetChar)
    {
        long sum = 0;
        for (var row = 0; row < map.GetLength(0); row++)
        for (var col = 0; col < map.GetLength(1); col++)
            if (map[row, col] == targetChar)
                sum += 100 * row + col;

        return sum;
    }

    private static char[,] ExpandMap(char[,] original)
    {
        var result = new char[original.GetLength(0), original.GetLength(1) * 2];

        for (var row = 0; row < original.GetLength(0); row++)
        {
            for (var col = 0; col < original.GetLength(1); col++)
            {
                var (first, second) = ExpandChar(original[row, col]);
                result[row, col * 2] = first;
                result[row, col * 2 + 1] = second;
            }
        }

        return result;
    }

    private static (char, char) ExpandChar(char c) => c switch
    {
        '#' => ('#', '#'),
        'O' => ('[', ']'),
        '.' => ('.', '.'),
        '@' => ('@', '.'),
        _ => throw new ArgumentOutOfRangeException(nameof(c))
    };

    private static void PrintMap(char[,] map)
    {
        for (var r = 0; r < map.GetLength(0); r++)
        {
            for (var c = 0; c < map.GetLength(1); c++)
            {
                Console.Write(map[r, c]);
            }

            Console.WriteLine();
        }
    }
}