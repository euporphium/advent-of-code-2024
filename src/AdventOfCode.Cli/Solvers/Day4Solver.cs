namespace AdventOfCode.Cli.Solvers;

public class Day4Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var rows = input.Length;
        var columns = input[0].Length;

        var sum = 0;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                sum += CountAllFromPoint(i, j);
            }
        }

        return sum.ToString();

        int CountAllFromPoint(int i, int j)
        {
            if (input[i][j] != 'X') return 0;

            var count = 0;
        
            // up
            if (i - 3 >= 0 && input[i - 1][j] == 'M' && input[i - 2][j] == 'A' && input[i - 3][j] == 'S')
                count++;
        
            // down
            if (i + 3 < rows && input[i + 1][j] == 'M' && input[i + 2][j] == 'A' && input[i + 3][j] == 'S')
                count++;
        
            // left
            if (j - 3 >= 0 && input[i][j - 1] == 'M' && input[i][j - 2] == 'A' && input[i][j - 3] == 'S')
                count++;
        
            // right
            if (j + 3 < columns && input[i][j + 1] == 'M' && input[i][j + 2] == 'A' && input[i][j + 3] == 'S')
                count++;
        
            // up-left
            if (i - 3 >= 0 && j - 3 >= 0 && input[i - 1][j - 1] == 'M' && input[i - 2][j - 2] == 'A' && input[i - 3][j - 3] == 'S')
                count++;
        
            // up-right
            if (i - 3 >= 0 && j + 3 < columns && input[i - 1][j + 1] == 'M' && input[i - 2][j + 2] == 'A' && input[i - 3][j + 3] == 'S')
                count++;
        
            // down-left
            if (i + 3 < rows && j - 3 >= 0 && input[i + 1][j - 1] == 'M' && input[i + 2][j - 2] == 'A' && input[i + 3][j - 3] == 'S')
                count++;
        
            // down-right
            if (i + 3 < rows && j + 3 < columns && input[i + 1][j + 1] == 'M' && input[i + 2][j + 2] == 'A' && input[i + 3][j + 3] == 'S')
                count++;
        
            return count;
        }
    }

    
    public string SolvePartB(string[] input)
    {
        var rows = input.Length;
        var cols = input[0].Length;
        var count = 0;

        // For each possible center point of the X
        for (var i = 1; i < rows - 1; i++)
        {
            for (var j = 1; j < cols - 1; j++)
            {
                if (input[i][j] != 'A') continue;

                // Check upper-left + lower-right diagonal combined with upper-right + lower-left diagonal
                var topLeftMAS = i > 0 && j > 0 && input[i-1][j-1] == 'M' && input[i+1][j+1] == 'S';
                var topLeftSAM = i > 0 && j > 0 && input[i-1][j-1] == 'S' && input[i+1][j+1] == 'M';
                var topRightMAS = i > 0 && j < cols-1 && input[i-1][j+1] == 'M' && input[i+1][j-1] == 'S';
                var topRightSAM = i > 0 && j < cols-1 && input[i-1][j+1] == 'S' && input[i+1][j-1] == 'M';

                if ((topLeftMAS || topLeftSAM) && (topRightMAS || topRightSAM))
                {
                    count++;
                }
            }
        }

        return count.ToString();
    }
}