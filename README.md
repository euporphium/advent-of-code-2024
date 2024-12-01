# Advent of Code 2024 - C# Solutions

A C# command-line application for solving Advent of Code 2024 puzzles with built-in performance tracking.

## Setup

1. Clone the repository
2. Create a `appsettings.json` file in the project root with your input data directory:
```json
{
  "AdventOfCode": {
    "InputDataDirectory": "path/to/your/input/files"
  }
}
```

## Project Structure

- `ISolver`: Interface that all daily solutions implement
- `SimpleReader`: Handles reading input files
- `Performance`: Utility class for timing operations
- Daily solvers in `Solvers` directory (e.g., `Day1Solver.cs`)

## Running Solutions

Use the CLI to run solutions for specific days:

```bash
dotnet run solve --day <day> --part <a|b> --input <filename>
```

Example:
```bash
dotnet run solve --day 1 --part b --input actual.txt
```

The application will:
1. Load the specified input file
2. Find the appropriate solver for the day
3. Run either part A or B
4. Display timing information for both file reading and solution computation
5. Output the answer

## Features

- Automatic solver discovery via dependency injection
- Performance tracking for both I/O and computation time
- Clean separation of concerns between file reading and solving
- Built on Cocona CLI framework for robust command-line parsing

## Adding New Solutions

1. Create a new solver class in the `Solvers` directory:
```csharp
public class DayNSolver : ISolver
{
    public string SolvePartA(string[] input) => "solution";
    public string SolvePartB(string[] input) => "solution";
}
```

2. The application will automatically discover and register new solvers.

## Technologies Used

- .NET 9
- Cocona (CLI framework)