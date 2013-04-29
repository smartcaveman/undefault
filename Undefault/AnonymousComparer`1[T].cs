using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Undefault
{
    internal sealed class AnonymousComparer<T> : Comparer<T>, IEquatable<Comparer<T>>, IEquatable<AnonymousComparer<T>>
    {
        private readonly Comparison<T> comparison;

        public AnonymousComparer(Comparison<T> comparison)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(comparison, null));
            this.comparison = comparison;
        }

        public override int Compare(T x, T y)
        {
            return this.comparison(x, y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AnonymousComparer<T>);
        }
        
        public bool Equals(Comparer<T> other)
        {
            return Equals(other as AnonymousComparer<T>);
        }
        
        public bool Equals(AnonymousComparer<T> other)
        {
            return !ReferenceEquals(other, null) && this.comparison.Equals(other.comparison);
        }
        
        public override int GetHashCode()
        {
            return this.comparison.GetHashCode();
        }
        
        public override string ToString()
        {
            return new { comparison.Method, comparison.Target }.ToString();
        }

    }
}