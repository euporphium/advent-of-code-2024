using Cocona;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommand("solve", (int year) =>
{
    Console.WriteLine($"\"Solving\" for {year}");
});

app.Run();