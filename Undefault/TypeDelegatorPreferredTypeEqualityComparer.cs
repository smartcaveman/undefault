using System;
using System.Collections.Generic;
using System.Reflection;

namespace Undefault
{
    public sealed class TypeDelegatorPreferredTypeEqualityComparer : EqualityComparer<Type>
    {
        private static readonly TypeDelegatorPreferredTypeEqualityComparer Instance;

        static TypeDelegatorPreferredTypeEqualityComparer()
        {
            Instance = new TypeDelegatorPreferredTypeEqualityComparer();
        }

        private TypeDelegatorPreferredTypeEqualityComparer()
        {
        }

        public static IEqualityComparer<Type> GetInstance()
        {
            return Instance;
        }

        public static void MakeDefault()
        {
            if (!Equals(Default, GetInstance()))
            {
                ComparisonConfigurator.ConfigureEqualityComparer(Instance);
            }
        }

        public override bool Equals(Type one, Type other)
        {
            return ReferenceEquals(one, null)
                       ? ReferenceEquals(other, null)
                       : !ReferenceEquals(other, null)
                         && ((one is TypeDelegator || !(other is TypeDelegator))
                                 ? one.Equals(other)
                                 : other.Equals(one));
        }

        public override int GetHashCode(Type type)
        {
            return ReferenceEquals(type, null) ? 0 : type.GetHashCode();
        }
    }
}