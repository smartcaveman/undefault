using System;
using System.Collections.Generic;
using Contract = System.Diagnostics.Contracts.Contract;
namespace Undefault
{
    public sealed class EqualityComparerCompatibleComparerDecorator<T> : Comparer<T>
    {
        public EqualityComparerCompatibleComparerDecorator(IComparer<T> comparer, IEqualityComparer<T> equalityComparer)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(comparer,null));
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(equalityComparer,null));
            this.comparer = comparer;
            this.equalityComparer = equalityComparer;
        }
        private readonly IComparer<T> comparer;
        private readonly IEqualityComparer<T> equalityComparer;
        public override int Compare(T left, T right) { return this.equalityComparer.Equals(left, right) ? 0 : comparer.Compare(left, right); }
    }
}
