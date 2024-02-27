using System;
using System.Runtime.CompilerServices;

namespace MVale.Core
{
    public readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        public int Value => this._value < 0 ? ~this._value : this._value;

        public bool IsFromEnd => this._value < 0;

        private Index(int _value)
        {
            this._value = _value;
        }

        public Index(int value, bool isFromEnd = false) : this(GetValue(value, isFromEnd))
        {
        }

        private static int GetValue(int value, bool isFromEnd)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            return isFromEnd ? ~value : value;
        }

#if NETCOREAPP3_0_OR_GREATER
        public static implicit operator global::System.Index(Index other) => new global::System.Index(other.Value, other.IsFromEnd);

        public static implicit operator Index(global::System.Index other) => new Index(other.Value, other.IsFromEnd);
#endif

        public static implicit operator Index(int value) => new Index(value, false);

        public static bool operator ==(Index a, Index b) => a.Equals(b);

        public static bool operator !=(Index a, Index b) => !(a == b);

        public override bool Equals(object obj)
        {
            return obj is Index other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this._value;
        }

        public override string ToString()
        {
            return this.IsFromEnd ? this.Value.ToString() : "^" + this.Value.ToString();
        }

        public bool Equals(Index other)
        {
            return this._value.Equals(other._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOffset(int length)
        {
            int offset = this._value;
            if (this.IsFromEnd)
            {
                offset += length + 1;
            }

            return offset;
        }
    }
}