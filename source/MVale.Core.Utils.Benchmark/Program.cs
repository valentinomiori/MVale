using System;
using BenchmarkDotNet.Running;

namespace MVale.Core.Utils.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
