using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MVale.Core.Test.Math
{
    [TestFixture(TestOf = typeof(Core.Math.Rounding))]
    public class Rounding_Test
    {
        public static IEnumerable<TestCaseData> DeterministicTestCases
        {
            get
            {
                static IEnumerable<TestCaseData> Create(
                    Core.Math.Rounding.Modes mode,
                    params (double d, double result)[] args)
                {
                    foreach ((double d, double result) in args)
                        yield return new TestCaseData(d, mode, result);
                }

                return Create(
                    Core.Math.Rounding.Modes.Down,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +1.0),
                    (+1.5, +1.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, -0.0),
                    (+0.5, -0.0),
                    (+0.2, -0.0),
                    (+0.0, -0.0),
                    (-0.0, -0.0),
                    (-0.2, -1.0),
                    (-0.5, -1.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -2.0),
                    (-1.5, -2.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity))
                .Concat(Create(
                    Core.Math.Rounding.Modes.Up,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +2.0),
                    (+1.2, +2.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +1.0),
                    (+0.2, +1.0),
                    (+0.0, +0.0),
                    (-0.0, +0.0),
                    (-0.2, +0.0),
                    (-0.5, +0.0),
                    (-0.8, +0.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -1.0),
                    (-1.8, -1.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.TowardZero,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +1.0),
                    (+1.5, +1.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, +0.0),
                    (+0.5, +0.0),
                    (+0.2, +0.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -0.0),
                    (-0.5, -0.0),
                    (-0.8, -0.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -1.0),
                    (-1.8, -1.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.AwayFromZero,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +2.0),
                    (+1.2, +2.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +1.0),
                    (+0.2, +1.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -1.0),
                    (-0.5, -1.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -2.0),
                    (-1.5, -2.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.HalfDown,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +1.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +0.0),
                    (+0.2, +0.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -0.0),
                    (-0.5, -1.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -2.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.HalfUp,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +2.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +1.0),
                    (+0.2, +0.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -0.0),
                    (-0.5, -0.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -1.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.HalfTowardZero,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +1.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +0.0),
                    (+0.2, +0.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -0.0),
                    (-0.5, -0.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -1.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.HalfAwayFromZero,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +2.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +1.0),
                    (+0.2, +0.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -0.0),
                    (-0.5, -1.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -2.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.HalfToEven,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +2.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +0.0),
                    (+0.2, +0.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -0.0),
                    (-0.5, -0.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -2.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)))
                .Concat(Create(
                    Core.Math.Rounding.Modes.HalfToOdd,
                    (double.PositiveInfinity, double.PositiveInfinity),
                    (double.MaxValue, double.MaxValue),
                    (+2.0, +2.0),
                    (+1.8, +2.0),
                    (+1.5, +1.0),
                    (+1.2, +1.0),
                    (+1.0, +1.0),
                    (+0.8, +1.0),
                    (+0.5, +1.0),
                    (+0.2, +0.0),
                    (+0.0, +0.0),
                    (-0.0, -0.0),
                    (-0.2, -0.0),
                    (-0.5, -1.0),
                    (-0.8, -1.0),
                    (-1.0, -1.0),
                    (-1.2, -1.0),
                    (-1.5, -1.0),
                    (-1.8, -2.0),
                    (-2.0, -2.0),
                    (double.MinValue, double.MinValue),
                    (double.NegativeInfinity, double.NegativeInfinity)));
            }
        }

        public static IEnumerable<(double, double[])> TieBreakTestCases
        {
            get => new List<(double, double[])>()
            {
                (double.PositiveInfinity, new double[] { double.PositiveInfinity }),
                (double.MaxValue, new double[] { double.MaxValue }),
                (+2.0, new double[] { +2.0 }),
                (+1.8, new double[] { +2.0 }),
                (+1.5, new double[] { +1.0, +2.0 }),
                (+1.2, new double[] { +1.0 }),
                (+1.0, new double[] { +1.0 }),
                (+0.8, new double[] { +1.0 }),
                (+0.5, new double[] { +0.0, +1.0 }),
                (+0.2, new double[] { +0.0 }),
                (+0.0, new double[] { +0.0 }),
                (-0.0, new double[] { -0.0 }),
                (-0.2, new double[] { -0.0 }),
                (-0.5, new double[] { -0.0, -1.0 }),
                (-0.8, new double[] { -1.0 }),
                (-1.0, new double[] { -1.0 }),
                (-1.2, new double[] { -1.0 }),
                (-1.5, new double[] { -1.0, -2.0 }),
                (-1.8, new double[] { -2.0 }),
                (-2.0, new double[] { -2.0 }),
                (double.MinValue, new double[] { double.MinValue }),
                (double.NegativeInfinity, new double[] { double.NegativeInfinity })
            };
        }

        public static void Repeat(int n, Action action)
        {
            for (int i = 0; i < n; i++)
            {
                action.Invoke();
            }
        }

        public static IEnumerable<T> Repeat<T>(int n, Func<T> func)
        {
            for (int i = 0; i < n; i++)
            {
                yield return func.Invoke();
            }
        }

        [Test, TestCaseSource(nameof(DeterministicTestCases))]
        public void Round_Test(double d, Core.Math.Rounding.Modes mode, double result)
        {
            Assert.AreEqual(result, Core.Math.Rounding.Round(d, mode));
        }

        [Test]
        public void RoundAlternatingTieBreak_Test()
        {
            foreach ((var input, var results) in TieBreakTestCases)
            {
                var average = Repeat(results.Length > 1 ? 16 : 4, () =>
                {
                    var result = Core.Math.Rounding.Round(input, Core.Math.Rounding.Modes.AlternatingTieBreak);
                    Assert.Contains(result, results, $"Failed for: {input}.");
                    return result;
                })
                .Average();

                if (!(results.Length > 1))
                    continue;

                Assert.Greater(average, results.Min());
                Assert.Less(average, results.Max());
            }
        }

        [Test]
        public void RoundRandomTieBreak_Test()
        {
            foreach ((var input, var results) in TieBreakTestCases)
            {
                var average = Repeat(results.Length > 1 ? 32 : 4, () =>
                {
                    var result = Core.Math.Rounding.Round(input, Core.Math.Rounding.Modes.RandomTieBreak);
                    Assert.Contains(result, results, $"Failed for: {input}.");
                    return result;
                })
                .Average();

                if (!(results.Length > 1))
                    continue;

                Assert.Greater(average, results.Min());
                Assert.Less(average, results.Max());
            }
        }

        [Test]
        public void RoundStochastic_Test()
        {
            static IEnumerable<double> GetCases()
            {
                return new double[] { +1.8, +1.5, +1.2, +0.8, +0.5, +0.2, -0.2, -0.5, -0.8, -1.2, -1.5, -1.8, };
            }

            static IEnumerable<double> GetExactCases()
            {
                return new double[] { +2.0, +1.0, +0.0, -0.0, -1.0, -2.0 };
            }

            static double Round(double d)
            {
                return Core.Math.Rounding.Round(d, Core.Math.Rounding.Modes.Stochastic);
            }

            const int repeats = 1000;
            const double delta = 0.05;

            Assert.AreEqual(double.PositiveInfinity, Round(double.PositiveInfinity));
            Assert.AreEqual(double.MaxValue, Round(double.MaxValue));

            foreach (var n in GetCases())
            {
                var average = Repeat(repeats, () => Round(n)).Average();

                Assert.AreEqual(n, average, delta);
            }

            foreach (var n in GetExactCases())
            {
                Assert.AreEqual(n, Round(n));
            }

            Assert.AreEqual(double.MinValue, Round(double.MinValue));
            Assert.AreEqual(double.NegativeInfinity, Round(double.NegativeInfinity));
        }
    }
}