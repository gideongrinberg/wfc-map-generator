using System;
using System.Diagnostics;

using WaveFunctionCollapse;

public class Program
{
    static void Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        long seconds = 0;

        for (int i = 0; i < 10; i++)
        {
            WFCGenerator wfc = new WFCGenerator(n: 6);
            wfc.Run();
            seconds += stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Ran model for the {i}th time");
        }

        Console.WriteLine(seconds / 10);
    }
}