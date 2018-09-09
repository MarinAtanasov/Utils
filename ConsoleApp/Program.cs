using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();

            var a = new double[512][];
            Enumerable.Range(0, a.Length).ToList().ForEach(row =>
            {
                a[row] = Enumerable.Range(0, 512).Select(col => (250.0 - col) / 10).ToArray();
            });
            var b = new double[a[0].Length][];
            Enumerable.Range(0, b.Length).ToList().ForEach(row =>
            {
                b[row] = Enumerable.Range(0, 512).Select(col => (250.0 - col) / -10).ToArray();
            });

            var left = new Matrix(a);
            var right = new Matrix(b);
            stopwatch.Restart();
            var result = left * right;
            Console.WriteLine($"Multiplied in {stopwatch.Elapsed.TotalMilliseconds} ms.");

            stopwatch.Restart();
            Enumerable.Range(0, 10).ToList().ForEach(x => result = left * right);

            Console.WriteLine("Executed in: {0} ms.", stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
