using System;
using System.Threading;

namespace MVale.Core.Math
{
    /// <summary>
    /// A class that performs advanced rounding of floating point numbers.
    /// <see href="https://en.wikipedia.org/wiki/Rounding"/>
    /// </summary>
    public static class Rounding
    {
        /// <summary>
        /// An enum that indicates the rounding mode.
        /// <para>
        /// For the examples below, sgn(x) refers to the sign function applied to the original number, x.
        /// </para>
        /// </summary>
        public enum Modes : byte
        {
            /// <summary>
            /// <para> Rounding down - Tags: <see cref="ModeTags.Directed"/>, <see cref="ModeTags.Functional"/> </para>
            /// round down (or take the floor, or round toward negative infinity): y is the largest integer that does not exceed x.
            /// <para>
            /// y = floor(x) = ⌊x⌋ = -⌈-x⌉
            /// </para>
            /// For example, 23.7 gets rounded to 23, and −23.2 gets rounded to −24.
            /// </summary>
            Down                = 0b001 | ModeTags.Directed     | ModeTags.Functional,
            /// <summary>
            /// <para> Rounding up - Tags: <see cref="ModeTags.Directed"/>, <see cref="ModeTags.Functional"/> </para>
            /// round up (or take the ceiling, or round toward positive infinity): y is the smallest integer that is not less than x.
            /// <para> y = ceil(x) = ⌈x⌉ = -⌊-x⌋ </para>
            /// For example, 23.2 gets rounded to 24, and −23.7 gets rounded to −23.
            /// </summary>
            Up                  = 0b010 | ModeTags.Directed     | ModeTags.Functional,
            /// <summary>
            /// <para> Rounding toward zero - Tags: <see cref="ModeTags.Directed"/>, <see cref="ModeTags.Functional"/> </para>
            /// round toward zero (or truncate, or round away from infinity):
            /// y is the integer that is closest to x such that it is between 0 and x (included);
            /// i.e. y is the integer part of x, without its fraction digits.
            /// <para> y = truncate(x) = sgn(x)⌊|x|⌋ = -sgn(x)⌈-|x|⌉ = { ⌊x⌋ se x &gt;= 0, ⌈x⌉ se x &lt; 0 }  </para>
            /// For example, 23.7 gets rounded to 23, and −23.7 gets rounded to −23.
            /// </summary>
            TowardZero          = 0b011 | ModeTags.Directed     | ModeTags.Functional,
            /// <summary>
            /// <para> Rounding away from zero - Tags: <see cref="ModeTags.Directed"/>, <see cref="ModeTags.Functional"/> </para>
            /// round away from zero (or round toward infinity):
            /// y is the integer that is closest to 0 (or equivalently, to x) such that x is between 0 and y (included).
            /// <para> y = sgn(x)⌈|x|⌉ = -sgn(x)⌊|-x|⌋ = { ⌈x⌉ se x &gt;= 0, ⌊x⌋ se x &lt; 0 } </para>
            /// For example, 23.2 gets rounded to 24, and −23.2 gets rounded to −24.
            /// </summary>
            AwayFromZero        = 0b100 | ModeTags.Directed     | ModeTags.Functional,
            /// <summary>
            /// <para> Round half down - Tags: <see cref="ModeTags.Nearest"/>, <see cref="ModeTags.Functional"/> </para>
            /// <para>
            /// One may also use round half down (or round half toward negative infinity) as opposed to the more common round half up.
            /// </para>
            /// If the fraction of x is exactly 0.5, then y = x − 0.5
            /// <para> y = ⌈x - 0.5⌉ = -⌊-x + 0.5⌋ = ⌊⌈2x⌉ / 2⌋ </para>
            /// For example, 23.5 gets rounded to 23, and −23.5 gets rounded to −24.
            /// </summary>
            HalfDown            = 0b001 | ModeTags.Nearest      | ModeTags.Functional,
            /// <summary>
            /// <para> Round half up - Tags: <see cref="ModeTags.Nearest"/>, <see cref="ModeTags.Functional"/> </para>
            /// <para>
            /// The following tie-breaking rule, called round half up (or round half toward positive infinity),
            /// is widely used in many disciplines. That is, half-way values of x are always rounded up.
            /// </para>
            /// If the fraction of x is exactly 0.5, then y = x + 0.5
            /// <para> y = ⌊x + 0.5⌋ = -⌈-x - 0.5⌉ = ⌈⌊2x⌋ / 2⌉ </para>
            /// For example, 23.5 gets rounded to 24, and −23.5 gets rounded to −23.
            /// </summary>
            HalfUp              = 0b010 | ModeTags.Nearest      | ModeTags.Functional,
            /// <summary>
            /// <para> Round half toward zero - Tags: <see cref="ModeTags.Nearest"/>, <see cref="ModeTags.Functional"/> </para>
            /// One may also round half toward zero (or round half away from infinity) as opposed to the conventional round half away from zero.
            /// <para>
            /// If the fraction of x is exactly 0.5, then y = x − 0.5 if x is positive, and y = x + 0.5 if x is negative.
            /// </para>
            /// <para> y = sgn(x)⌈|x| - 0.5⌉ = -sgn(x)⌊-|x| + 0.5⌋ </para>
            /// For example, 23.5 gets rounded to 23, and −23.5 gets rounded to −23.
            /// <para>
            /// This method treats positive and negative values symmetrically, and therefore is free of overall positive/negative bias
            /// if the original numbers are positive or negative with equal probability.
            /// It does, however, still have bias toward zero.
            /// </para>
            /// </summary>
            HalfTowardZero      = 0b011 | ModeTags.Nearest      | ModeTags.Functional,
            /// <summary>
            /// <para> Round half away from zero - Tags: <see cref="ModeTags.Nearest"/>, <see cref="ModeTags.Functional"/> </para>
            /// The other tie-breaking method commonly taught and used is the round half away from zero (or round half toward infinity), namely:
            /// <para>
            /// If the fraction of x is exactly 0.5, then y = x + 0.5 if x is positive, and y = x − 0.5 if x is negative.
            /// </para>
            /// <para> y = sgn(x)⌊|x| + 0.5⌋ = -sgn(x)⌈-|x| - 0.5⌉ </para>
            /// For example, 23.5 gets rounded to 24, and −23.5 gets rounded to −24.
            /// <para>
            /// This can be more efficient on binary computers because only the first omitted bit needs to be considered
            /// to determine if it rounds up (on a 1) or down (on a 0).
            /// This is one method used when rounding to significant figures due to its simplicity.
            /// </para>
            /// <para>
            /// This method, also known as commercial rounding, treats positive and negative values symmetrically,
            /// and therefore is free of overall positive/negative bias if the original numbers are positive or negative with equal probability.
            /// It does, however, still have bias away from zero.
            /// </para>
            /// It is often used for currency conversions and price roundings
            /// (when the amount is first converted into the smallest significant subdivision of the currency, such as cents of a euro)
            /// as it is easy to explain by just considering the first fractional digit,
            /// independently of supplementary precision digits or sign of the amount
            /// (for strict equivalence between the paying and recipient of the amount).
            /// </summary>
            HalfAwayFromZero    = 0b100 | ModeTags.Nearest      | ModeTags.Functional,
            /// <summary>
            /// <para> Round half to even - Tags: <see cref="ModeTags.Nearest"/>, <see cref="ModeTags.Functional"/> </para>
            /// <para>
            /// A tie-breaking rule without positive/negative bias and without bias toward/away from zero is round half to even.
            /// By this convention, if the fractional part of x is 0.5, then y is the even integer nearest to x.
            /// Thus, for example, +23.5 becomes +24, as does +24.5; however, −23.5 becomes −24, as does −24.5.
            /// This function minimizes the expected error when summing over rounded figures,
            /// even when the inputs are mostly positive or mostly negative, provided they are neither mostly even nor mostly odd.
            /// </para>
            /// <para>
            /// This variant of the round-to-nearest method is also called convergent rounding,
            /// statistician's rounding, Dutch rounding, Gaussian rounding, odd–even rounding,[6] or bankers' rounding.
            /// </para>
            /// <para>
            /// This is the default rounding mode used in IEEE 754 operations for results in binary floating-point formats
            /// (see also nearest integer function), and the more sophisticated mode used when rounding to significant figures.
            /// </para>
            /// <para>
            /// By eliminating bias, repeated rounded addition or subtraction of independent numbers will give
            /// a result with an error that tends to grow in proportion to the square root of the number of operations rather than linearly.
            /// See random walk for more.
            /// </para>
            /// <para>
            /// However, this rule distorts the distribution by increasing the probability of evens relative to odds.
            /// Typically this is less important than the biases that are eliminated by this method.
            /// </para>
            /// </summary>
            HalfToEven          = 0b101 | ModeTags.Nearest      | ModeTags.Functional,
            /// <summary>
            /// <para> Round half to odd - Tags: <see cref="ModeTags.Nearest"/>, <see cref="ModeTags.Functional"/> </para>
            /// <para>
            /// A similar tie-breaking rule to round half to even is round half to odd.
            /// In this approach, if the fraction of x is 0.5, then y is the odd integer nearest to x.
            /// Thus, for example, +23.5 becomes +23, as does +22.5; while −23.5 becomes −23, as does −22.5.
            /// </para>
            /// <para>
            /// This method is also free from positive/negative bias and bias toward/away from zero,
            /// provided the numbers to be rounded are neither mostly even nor mostly odd.
            /// It also shares the round half to even property of distorting the original distribution,
            /// as it increases the probability of odds relative to evens.
            /// </para>
            /// <para>
            /// This variant is almost never used in computations,
            /// except in situations where one wants to avoid increasing the scale of floating-point numbers,
            /// which have a limited exponent range.
            /// With round half to even, a non-infinite number would round to infinity,
            /// and a small denormal value would round to a normal non-zero value.
            /// Effectively, this mode prefers preserving the existing scale of tie numbers,
            /// avoiding out-of-range results when possible for numeral systems of even radix (such as binary and decimal).
            /// </para>
            /// </summary>
            HalfToOdd           = 0b110 | ModeTags.Nearest      | ModeTags.Functional,
            /// <summary>
            /// <para> Alternating tie-breaking - Tags: <see cref="ModeTags.Randomized"/> </para>
            /// <para>
            /// One method, more obscure than most, is to alternate direction when rounding a number with 0.5 fractional part.
            /// All others are rounded to the closest integer.
            /// </para>
            /// <para>
            /// Whenever the fractional part is 0.5, alternate rounding up or down: for the first occurrence of a 0.5 fractional part,
            /// round up, for the second occurrence, round down, and so on.
            /// Alternatively, the first 0.5 fractional part rounding can be determined by a random seed.
            /// "Up" and "down" can be any two rounding methods that oppose each other - toward and away from positive infinity
            /// or toward and away from zero.
            /// </para>
            /// <para>
            /// If occurrences of 0.5 fractional parts occur significantly more than a restart of the occurrence "counting",
            /// then it is effectively bias free. With guaranteed zero bias, it is useful if the numbers are to be summed or averaged.
            /// </para>
            /// </summary>
            AlternatingTieBreak = 0b001 | ModeTags.Randomized,
            /// <summary>
            /// <para> Random tie-breaking - Tags: <see cref="ModeTags.Randomized"/> </para>
            /// <para>
            /// If the fractional part of x is 0.5, choose y randomly between x + 0.5 and x − 0.5, with equal probability.
            /// All others are rounded to the closest integer.
            /// </para>
            /// <para>
            /// Like round-half-to-even and round-half-to-odd, this rule is essentially free of overall bias,
            /// but it is also fair among even and odd y values.
            /// An advantage over alternate tie-breaking is that the last direction of rounding
            /// on the 0.5 fractional part does not have to be "remembered".
            /// </para>
            /// </summary>
            RandomTieBreak      = 0b010 | ModeTags.Randomized,
            /// <summary>
            /// <para> Stochastic rounding - Tags: <see cref="ModeTags.Randomized"/> </para>
            /// <para>
            /// Rounding as follows to one of the closest integer toward negative infinity and the closest integer toward positive infinity,
            /// with a probability dependent on the proximity is called stochastic rounding and will give an unbiased result on average.
            /// </para>
            /// <para> y = { ⌊x⌋ with probability 1 - (x - ⌊x⌋) = ⌊x⌋ - x +1, ⌊x⌋ + 1 with probability } </para>
            /// <para>
            /// For example, 1.6 would be rounded to 1 with probability 0.4 and to 2 with probability 0.6.
            /// </para>
            /// <para>
            /// Stochastic rounding can be accurate in a way that a rounding function can never be.
            /// For example, suppose one started with 0 and added 0.3 to that one hundred times while rounding
            /// the running total between every addition.
            /// The result would be 0 with regular rounding, but with stochastic rounding,
            /// the expected result would be 30, which is the same value obtained without rounding.
            /// This can be useful in machine learning where the training may use low precision arithmetic iteratively.
            /// Stochastic rounding is also a way to achieve 1-dimensional dithering.
            /// </para>
            /// </summary>
            Stochastic          = 0b011 | ModeTags.Randomized
        }

        public static class ModeTags
        {
            /// <summary>
            /// <para><strong> Directed rounding to an integer </strong></para>
            /// <para>
            /// These four methods are called directed rounding, as the displacements from the original number x to the rounded value y
            /// are all directed toward or away from the same limiting value (0, +∞, or −∞).
            /// Directed rounding is used in interval arithmetic and is often required in financial calculations.
            /// </para>
            /// <para>
            /// If x is positive, round-down is the same as round-toward-zero, and round-up is the same as round-away-from-zero.
            /// If x is negative, round-down is the same as round-away-from-zero, and round-up is the same as round-toward-zero.
            /// In any case, if x is an integer, y is just x.
            /// </para>
            /// <para>
            /// Where many calculations are done in sequence, the choice of rounding method can have a very significant effect on the result.
            /// A famous instance involved a new index set up by the Vancouver Stock Exchange in 1982.
            /// It was initially set at 1000.000 (three decimal places of accuracy),
            /// and after 22 months had fallen to about 520 — whereas stock prices had generally increased in the period.
            /// The problem was caused by the index being recalculated thousands of times daily,
            /// and always being rounded down to 3 decimal places, in such a way that the rounding errors accumulated.
            /// Recalculating with better rounding gave an index value of 1098.892 at the end of the same period.
            /// </para>
            /// </summary>
            public const Modes Directed   = (Modes) 0b00010000;

            /// <summary>
            /// <para><strong> Rounding to the nearest integer </strong></para>
            /// <para>
            /// Rounding a number x to the nearest integer requires some tie-breaking rule for those cases when x is exactly half-way
            /// between two integers — that is, when the fraction part of x is exactly 0.5.
            /// </para>
            /// <para>
            /// If it were not for the 0.5 fractional parts,
            /// the round-off errors introduced by the round to nearest method would be symmetric:
            /// for every fraction that gets rounded down (such as 0.268),
            /// there is a complementary fraction (namely, 0.732) that gets rounded up by the same amount.
            /// </para>
            /// <para>
            /// When rounding a large set of fixed-point numbers with uniformly distributed fractional parts,
            /// the rounding errors by all values, with the omission of those having 0.5 fractional part,
            /// would statistically compensate each other.
            /// This means that the expected (average) value of the rounded numbers is equal to the expected value of the original numbers
            /// when numbers with fractional part 0.5 from the set are removed.
            /// </para>
            /// <para>
            /// In practice, floating-point numbers are typically used,
            /// which have even more computational nuances because they are not equally spaced.
            /// </para>
            /// </summary>
            public const Modes Nearest    = (Modes) 0b00100000;

            /// <summary>
            /// These are true functions.
            /// </summary>
            public const Modes Functional = (Modes) 0b01000000;

            /// <summary>
            /// Those that use randomness.
            /// </summary>
            public const Modes Randomized = (Modes) 0b10000000;

            /// <summary>
            /// Get a value indicating if the mode is deterministic.
            /// </summary>
            /// <param name="mode"></param>
            /// <returns><see langword="true"/> if the mode is deterministic, <see langword="false"/> otherwise.</returns>
            public static bool IsDeterministic(Modes mode)
            {
                return (mode & Randomized) == 0;
            }
        }

        private static readonly ThreadLocal<ulong> _alternatingTieBreakState
            = new ThreadLocal<ulong>(() => (ulong) Random.Next(2));

        private static readonly ThreadLocal<Random> _random
            = new ThreadLocal<Random>(() => new Random(System.DateTime.Now.Millisecond));

        private static ulong AlternatingTieBreakState
        {
            get => _alternatingTieBreakState.Value;
            set => _alternatingTieBreakState.Value = value;
        }

        private static Random Random => _random.Value;

#region Internal Methods
        internal static double RoundDown(double d)
        {
            return System.Math.Floor(d);
        }

        internal static double RoundUp(double d)
        {
            return System.Math.Ceiling(d);
        }

        internal static double RoundTowardZero(double d)
        {
            return System.Math.Truncate(d);
        }

        internal static double RoundAwayFromZero(double d)
        {
            if (d >= 0)
                return RoundUp(d);
            else
                return RoundDown(d);
        }

        internal static double RoundHalfDown(double d)
        {
            return RoundUp(d - 0.5);
        }

        internal static double RoundHalfUp(double d)
        {
            return RoundDown(d + 0.5);
        }

        internal static double RoundHalfTowardZero(double d)
        {
            return System.Math.Sign(d) * RoundUp(System.Math.Abs(d) - 0.5);
        }

        internal static double RoundHalfAwayFromZero(double d)
        {
            return System.Math.Sign(d) * RoundDown(System.Math.Abs(d) + 0.5);
        }

        internal static double RoundHalfToEven(double d)
        {
            if (double.IsInfinity(d) || double.IsNaN(d))
                return d;

            double fraction = d % 1.0;

            if (fraction == 0.0)
                return d;

            if (-0.5 < fraction && fraction < +0.5)
                return d - fraction;

            if (fraction < -0.5 || +0.5 < fraction)
                return d - fraction + System.Math.Sign(fraction);

            return d - fraction + System.Math.Truncate(d % 2);
        }

        internal static double RoundHalfToOdd(double d)
        {
            if (double.IsInfinity(d) || double.IsNaN(d))
                return d;

            double fraction = d % 1.0;

            if (fraction == 0.0)
                return d;

            if (-0.5 < fraction && fraction < +0.5)
                return d - fraction;

            if (fraction < -0.5 || +0.5 < fraction)
                return d - fraction + System.Math.Sign(fraction);

            return d - (d % 2) + System.Math.Sign(fraction);
        }

        internal static double RoundAlternatingTieBreak(double d)
        {
            if (double.IsInfinity(d) || double.IsNaN(d))
                return d;

            double truncated = System.Math.Truncate(d);

            if (truncated == d)
                return d;

            if ((d % 0.5) == 0.0)
                return unchecked(AlternatingTieBreakState++) % 2 == 0
                    ? truncated
                    : truncated + System.Math.Sign(d);

            return System.Math.Round(d);
        }

        internal static double RoundRandomTieBreak(double d)
        {
            if (double.IsInfinity(d) || double.IsNaN(d))
                return d;

            double truncated = System.Math.Truncate(d);

            if (truncated == d)
                return d;

            if ((d % 0.5) == 0.0)
            {
                return Random.Next(2) == 0
                    ? truncated
                    : truncated + System.Math.Sign(d);
            }

            return System.Math.Round(d);
        }

        internal static double RoundStochastic(double d)
        {
            if (double.IsInfinity(d) || double.IsNaN(d))
                return d;

            double truncated = System.Math.Truncate(d);

            if (truncated == d)
                return d;

            if (Random.NextDouble() > System.Math.Abs(d - truncated))
                return truncated;
            else
                return truncated + System.Math.Sign(d);
        }
#endregion

        /// <summary>
        /// <para> Related to <see cref="Round(double, Modes)"/>. </para>
        /// This method instead of directly calculating the result,
        /// returns a delegate that can be used to perform rounding repeatedly later on.
        /// </summary>
        /// <param name="mode"> The desired rounding mode. </param>
        /// <returns> A delegate that can perform the desired rounding. </returns>
        /// <exception cref="NotSupportedException"> If the mode is not supported. </exception>
        public static Func<double, double> GetDelegate(Modes mode)
        {
            switch (mode)
            {
                case Modes.Down:
                    return RoundDown;

                case Modes.Up:
                    return RoundUp;

                case Modes.TowardZero:
                    return RoundTowardZero;

                case Modes.AwayFromZero:
                    return RoundAwayFromZero;

                case Modes.HalfDown:
                    return RoundHalfDown;

                case Modes.HalfUp:
                    return RoundHalfUp;

                case Modes.HalfTowardZero:
                    return RoundHalfTowardZero;

                case Modes.HalfAwayFromZero:
                    return RoundHalfAwayFromZero;

                case Modes.HalfToEven:
                    return RoundHalfToEven;

                case Modes.HalfToOdd:
                    return RoundHalfToOdd;

                case Modes.AlternatingTieBreak:
                    return RoundAlternatingTieBreak;

                case Modes.RandomTieBreak:
                    return RoundRandomTieBreak;

                case Modes.Stochastic:
                    return RoundStochastic;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Round to integer.
        /// </summary>
        /// <param name="d"> The floating point number to round. </param>
        /// <param name="mode"> The desired rounding mode. </param>
        /// <returns> The result of the rounding computation. </returns>
        /// <exception cref="NotSupportedException"> If the mode is not supported. </exception>
        public static double Round(double d, Modes mode)
        {
            switch (mode)
            {
                case Modes.Down:
                    return RoundDown(d);

                case Modes.Up:
                    return RoundUp(d);

                case Modes.TowardZero:
                    return RoundTowardZero(d);

                case Modes.AwayFromZero:
                    return RoundAwayFromZero(d);

                case Modes.HalfDown:
                    return RoundHalfDown(d);

                case Modes.HalfUp:
                    return RoundHalfUp(d);

                case Modes.HalfTowardZero:
                    return RoundHalfTowardZero(d);

                case Modes.HalfAwayFromZero:
                    return RoundHalfAwayFromZero(d);

                case Modes.HalfToEven:
                    return RoundHalfToEven(d);

                case Modes.HalfToOdd:
                    return RoundHalfToOdd(d);

                case Modes.AlternatingTieBreak:
                    return RoundAlternatingTieBreak(d);

                case Modes.RandomTieBreak:
                    return RoundRandomTieBreak(d);

                case Modes.Stochastic:
                    return RoundStochastic(d);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}