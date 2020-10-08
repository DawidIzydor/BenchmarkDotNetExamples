using BenchmarkDotNet.Running;

namespace BenchmarkExamples
{
    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<GenerateSortedArrayBenchmark>();
        }
    }
}