using System;
using BasicUsage;

Console.WriteLine("Purity.Analyzer Demo");
Console.WriteLine("====================");
Console.WriteLine();

// Pure examples that compile successfully
Console.WriteLine("Pure calculations:");
Console.WriteLine($"Add(5, 3) = {PureExamples.Add(5, 3)}");
Console.WriteLine($"Multiply(4, 7) = {PureExamples.Multiply(4, 7)}");
Console.WriteLine($"Circle area (r=5) = {PureExamples.CalculateCircleArea(5):F2}");
Console.WriteLine($"Format name = {PureExamples.FormatName("John", "Doe")}");
Console.WriteLine($"Factorial(5) = {PureExamples.Factorial(5)}");
Console.WriteLine($"Combine strings = {PureExamples.CombineStrings("Hello", "World", " ")}");
Console.WriteLine($"Count matches = {PureExamples.CountMatches(new[] { "apple", "apricot", "banana" }, "ap")}");
Console.WriteLine();

Console.WriteLine("✅ All pure methods executed successfully!");
Console.WriteLine();
Console.WriteLine("To see violations:");
Console.WriteLine("1. Remove the '<Compile Remove=\"ViolationDemos.cs\" />' line from BasicUsage.csproj");
Console.WriteLine("2. Run 'dotnet build' to see all 10+ purity violations");
