using System;

namespace MVale.Core.Collections.Comparers
{
    [Serializable]
    public class RoundedFloatEqualityComparer :
        EqualityComparer<double>,
        IEqualityComparer<float>
    {
#region Comparer Instances
        public static readonly RoundedFloatEqualityComparer Down
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.Down);

        public static readonly RoundedFloatEqualityComparer Up
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.Up);

        public static readonly RoundedFloatEqualityComparer TowardZero
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.TowardZero);

        public static readonly RoundedFloatEqualityComparer AwayFromZero
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.AwayFromZero);

        public static readonly RoundedFloatEqualityComparer HalfDown
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.HalfDown);

        public static readonly RoundedFloatEqualityComparer HalfUp
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.HalfUp);

        public static readonly RoundedFloatEqualityComparer HalfTowardZero
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.HalfTowardZero);

        public static readonly RoundedFloatEqualityComparer HalfAwayFromZero
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.HalfAwayFromZero);

        public static readonly RoundedFloatEqualityComparer HalfToEven
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.HalfToEven);

        public static readonly RoundedFloatEqualityComparer HalfToOdd
            = new RoundedFloatEqualityComparer(Math.Rounding.Modes.HalfToOdd);
#endregion

        public Math.Rounding.Modes Mode { get; }

        private RoundedFloatEqualityComparer(Math.Rounding.Modes mode)
        {
            Mode = mode;
        }

        public override bool Equals(double x, double y)
        {
            return this.ApplyTransformation(x) == this.ApplyTransformation(y);
        }

        public override int GetHashCode(double obj)
        {
            return this.ApplyTransformation(obj).GetHashCode();
        }

        public virtual double ApplyTransformation(double d)
        {
            return Math.Rounding.Round(d, this.Mode);
        }

        public virtual bool Equals(float x, float y)
            => this.Equals((double) x, (double) y);

        public virtual int GetHashCode(float obj)
            => this.GetHashCode((double) obj);
    }
}