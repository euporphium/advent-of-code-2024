using System.Text;

namespace AdventOfCode.Cli.Solvers._2024;

public class Day17Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var result =  new Day17(input).Run();
        return string.Join(",", result);
    }

    public string SolvePartB(string[] input)
    {
        return new Day17(input).FindQuineInput();
    }
}

internal class Day17
{
    private int _registerA;
    private int _registerB;
    private int _registerC;

    private int[] _program;
    private int _instructionPointer;

    public Day17(string[] input)
    {
        _registerA = int.Parse(input[0].Split(": ")[1]);
        _registerB = int.Parse(input[1].Split(": ")[1]);
        _registerC = int.Parse(input[2].Split(": ")[1]);

        _program = input[4].Split(": ")[1].Split(",").Select(int.Parse).ToArray();
        _instructionPointer = 0;
    }

    public List<int> Run()
    {
        var outBuffer = new List<int>();
        while (_instructionPointer < _program.Length - 1)
        {
            var opcode = _program[_instructionPointer];
            var operand = _program[_instructionPointer + 1];

            switch (opcode)
            {
                case 0: // adv
                    // _registerA /= (int)Math.Pow(2, Combo(operand));
                    _registerA >>= Combo(operand); 
                    break;
                case 1: // bxl
                    _registerB ^= operand;
                    break;
                case 2: //bst
                    _registerB = Combo(operand) % 8;
                    break;
                case 3: // jnz
                    if (_registerA != 0)
                    {
                        _instructionPointer = operand;
                        continue;
                    }

                    break;
                case 4: // bxc
                    _registerB ^= _registerC;
                    break;
                case 5: // out
                    var value = Combo(operand) % 8;
                    outBuffer.Add(value);
                    break;
                case 6: // bdv
                    _registerB = _registerA >> Combo(operand);
                    break;
                case 7: // cdv
                    _registerC = _registerA >> Combo(operand);
                    break;
            }

            _instructionPointer += 2;
        }

        return outBuffer;
    }
    
    public string FindQuineInput()
{
    return Solve(_program.Length - 1, 0).ToString()!;
}

private long? Solve(int position, long result)
{
    if (position < 0)
    {
        return result;
    }

    // Try each possible 3-bit value
    for (int digit = 0; digit < 8; digit++)
    {
        long a = (result << 3) | (uint)digit;
        long b = 0;
        long c = 0;
        int ip = 0;
        bool outputFound = false;

        while (ip < _program.Length - 1 && !outputFound)
        {
            var opcode = _program[ip];
            var operand = _program[ip + 1];

            long o = operand <= 3 ? operand :
                    operand == 4 ? a :
                    operand == 5 ? b :
                    operand == 6 ? c : throw new Exception("Invalid operand");

            switch (opcode)
            {
                case 0: // adv
                    a >>= (int)o;  // Cast to int for shift operator
                    break;
                case 1: // bxl
                    b ^= operand;
                    break;
                case 2: // bst
                    b = o & 7;
                    break;
                case 3: // jnz
                    if (a != 0)
                    {
                        ip = operand - 2;  // -2 because we'll add 2 below
                    }
                    break;
                case 4: // bxc
                    b ^= c;
                    break;
                case 5: // out
                    outputFound = true;
                    var output = o & 7;
                    if (output == _program[position])
                    {
                        var nextResult = Solve(position - 1, (result << 3) | (uint)digit);
                        if (nextResult.HasValue)
                            return nextResult;
                    }
                    break;
                case 6: // bdv
                    b = a >> (int)o;  // Cast to int for shift operator
                    break;
                case 7: // cdv
                    c = a >> (int)o;  // Cast to int for shift operator
                    break;
            }
            ip += 2;
        }
    }

    return null;
}

    private int Combo(int operand)
    {
        return operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4 => _registerA,
            5 => _registerB,
            6 => _registerC,
            _ => throw new Exception("Invalid operand")
        };
    }
}