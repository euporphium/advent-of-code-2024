using System.Diagnostics;

namespace AdventOfCode.Cli;

internal static class Performance
{
    public static void LogTime(string actionName, Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        Log(actionName, sw);
    }

    public static T LogTime<T>(string actionName, Func<T> action)
    {
        var sw = Stopwatch.StartNew();
        var result = action();
        sw.Stop();
        Log(actionName, sw);
        return result;
    }
    
    public static async Task LogTimeAsync(string actionName, Func<Task> action)
    {
        var sw = Stopwatch.StartNew();
        await action();
        sw.Stop();
        Log(actionName, sw);
    }

    public static async Task<T> LogTimeAsync<T>(string actionName, Func<Task<T>> action)
    {
        var sw = Stopwatch.StartNew();
        var result = await action();
        sw.Stop();
        Log(actionName, sw);
        return result;
    }

    private static void Log(string actionName, Stopwatch sw)
    {
        Console.WriteLine($"[{actionName}] completed in {sw.ElapsedMilliseconds}ms"); 
    }
}