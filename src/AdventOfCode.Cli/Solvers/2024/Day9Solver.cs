using System.Text;

namespace AdventOfCode.Cli.Solvers._2024;

public class Day9Solver : ISolver
{
    public string SolvePartA(string[] input)
    {
        var files = ParseDiskMap(input[0]);

        var firstAvailable = files.FirstOrDefault(f => f.FreeSpace > 0);
        while (firstAvailable != null)
        {
            var toMove = files.Last();

            if (toMove.Size <= firstAvailable.FreeSpace)
            {
                var newFileBlock = new FileBlock(toMove.Id, toMove.Size, firstAvailable.FreeSpace - toMove.Size);
                files.Insert(files.IndexOf(firstAvailable) + 1, newFileBlock);
                files.Remove(toMove);
            }
            else
            {
                var newFileBlock = new FileBlock(toMove.Id, firstAvailable.FreeSpace, 0);
                toMove.Size -= newFileBlock.Size;
                files.Insert(files.IndexOf(firstAvailable) + 1, newFileBlock);
            }

            firstAvailable.FreeSpace = 0;

            firstAvailable = files.FirstOrDefault(f => f.FreeSpace > 0);
        }

        // Console.WriteLine(ExpandToVisualize(files));
        return CalculateChecksum(files).ToString(); 
    }

    public string SolvePartB(string[] input)
    {
        var files = ParseDiskMap(input[0]);

        var reversed = files.AsEnumerable().Reverse().ToList();
        foreach (var toMove in reversed)
        {
            foreach (var target in files)
            {
                if (target == toMove) break;
                if (toMove.Size > target.FreeSpace) continue;
                var newFileBlock = new FileBlock(toMove.Id, toMove.Size, target.FreeSpace - toMove.Size);
                files.Insert(files.IndexOf(target) + 1, newFileBlock);
                
                var previous = files[files.IndexOf(toMove) - 1];
                previous.FreeSpace += toMove.Size + toMove.FreeSpace;
                files.Remove(toMove);
                target.FreeSpace = 0;
                break;
            }
        }

        // Console.WriteLine(ExpandToVisualize(files));
        return CalculateChecksum(files).ToString();
    }

    private static List<FileBlock> ParseDiskMap(string diskMap)
    {
        var files = new List<FileBlock>();
        
        for (var i = 0; i < diskMap.Length; i += 2)
        {
            var fileSize = int.Parse(diskMap[i].ToString());
            var freeSpace = i + 1 < diskMap.Length ? int.Parse(diskMap[i + 1].ToString()) : 0;
            files.Add(new FileBlock(i / 2, fileSize, freeSpace));
        }
        
        return files;
    }
    
    private static long CalculateChecksum(List<FileBlock> files)
    {
        long sum = 0;
        var index = 0;
        
        foreach (var file in files)
        {
            for (var i = 0; i < file.Size; index++, i++)
            {
                sum += index * file.Id;
                index += file.FreeSpace;
            }
        }
        
        return sum;
    }
    
    // For debugging purposes
    private static string ExpandToVisualize(List<FileBlock> files)
    {
        var sb = new StringBuilder();
        foreach (var file in files)
        {
            for (var i = 0; i < file.Size; i++)
            {
                sb.Append(file.Id % 10); // limit to single digit
            }

            for (var i = 0; i < file.FreeSpace; i++)
            {
                sb.Append('.');
            }
        }

        return sb.ToString();
    }
}

public class FileBlock(int id, int size, int freeSpace)
{
    public int Id { get; } = id;
    public int Size { get; set; } = size;
    public int FreeSpace { get; set; } = freeSpace;
}