using BenchmarkDotNet.Running;

namespace Qml.Net.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ReferenceBenchmarks>();
        }
    }
}
