using System;

namespace MVale.Core
{
    public struct Range : IEquatable<Range>
    {
        public Index Start { get; }

        public Index End { get; }

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

#if NETCOREAPP3_0_OR_GREATER
        public static implicit operator global::System.Range(Range other) => new global::System.Range(other.Start, other.End);

        public static implicit operator Range(global::System.Range other) => new Range(other.Start, other.End);
#endif

        public static bool operator ==(Range a, Range b) => a.Equals(b);

        public static bool operator !=(Range a, Range b) => !(a == b);

        public override bool Equals(object obj)
        {
            return obj is Range other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return InternalHashCodeUtil.Combine(this.Start, this.End);
        }

        public override string ToString()
        {
            return this.Start.ToString() + ".." + this.End.ToString();
        }

        public bool Equals(Range other)
        {
            return this.Start.Equals(other.Start) && this.End.Equals(other.End);
        }

        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            int start = this.Start.GetOffset(length);
            int end = this.End.GetOffset(length);

            unchecked
            {
                if ((uint)end > (uint)length || (uint)start > (uint)end)
                    throw new ArgumentOutOfRangeException(nameof(length));
            }

            return (start, end - start);
        }
    }
}