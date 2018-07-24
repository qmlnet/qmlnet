using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Qml.Net.Benchmarks
{
    public class Config: ManualConfig
    {
        public Config()
        {
            Add(Job.Core.WithGcForce(true));
        }
    }
}