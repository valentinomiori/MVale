using System;
using BenchmarkDotNet.Attributes;

namespace MVale.Core.Utils.Benchmark
{
    public class ByteHexUtil_Benchmark
    {
        private byte[] data;

        [Params(1, 10, 100, 1000, 1000 * 1024)]
        public int length;

        [GlobalSetup]
        public void Setup()
        {
            data = new byte[length];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public string BitConverterPlusReplace()
            => ByteHexUtil.SafeToHex(this.data);

        [Benchmark]
        public string Custom()
            => ByteHexUtil.ToHex(this.data);
    }
}