﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Undefault
{
    public static class ComparisonConfigurator
    {
        private static readonly object ConfigurationGate;
        private static readonly HashSet<Type> ConfiguredEqualityComparerTypes;

        static ComparisonConfigurator()
        {
            ConfigurationGate = new object();
            ConfiguredEqualityComparerTypes = new HashSet<Type>();
        }

        public static void Configure<T>(IEqualityComparer<T> equalityComparer)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(equalityComparer, null));
            if (EqualityComparer<IEqualityComparer<T>>.Default.Equals(EqualityComparer<T>.Default, equalityComparer))
                return;
            lock (ConfigurationGate)
            {
                ConfiguredEqualityComparerTypes.Add(TypeOf<T>.Instance);
                FieldFor<T>.EqualityComparer.SetValue(null, equalityComparer);
                FieldFor<T>.Comparer.SetValue(null, new EqualityComparerCompatibleComparerDecorator<T>(Comparer<T>.Default, equalityComparer));
            }
        }

        public static void Configure<T>(IComparer<T> comparer)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(comparer, null));
            if (Equals(Comparer<T>.Default, comparer)) return;
            lock (ConfigurationGate)
            {
                if (ConfiguredEqualityComparerTypes.Contains(TypeOf<T>.Instance))
                    comparer = new EqualityComparerCompatibleComparerDecorator<T>(comparer, EqualityComparer<T>.Default);
                FieldFor<T>.Comparer.SetValue(null, comparer);
            }
        }

        public static void Configure<T>(Comparison<T> comparison)
        {
            Contract.Requires<ArgumentNullException>(!ReferenceEquals(comparison,null));
            Configure(new AnonymousComparer<T>(comparison));
        }

        public static void Revert<T>()
        {
            lock (ConfigurationGate)
            {
                FieldFor<T>.EqualityComparer.SetValue(null, null);
                FieldFor<T>.Comparer.SetValue(null, null);
                ConfiguredEqualityComparerTypes.Remove(TypeOf<T>.Instance);
            }
        }

        private static class Constant
        {
            public const string FieldName = "defaultComparer";
            public const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        }
        
        private static class FieldFor<T>
        { 
            private static FieldInfo comparer, equalityComparer;

            public static FieldInfo Comparer
            {
                get { return comparer ?? (comparer = TypeOf<Comparer<T>>.Instance.GetField(Constant.FieldName, Constant.PrivateStatic)); }
            }

            public static FieldInfo EqualityComparer
            {
                get
                {
                    return equalityComparer ??
                           (equalityComparer = TypeOf<EqualityComparer<T>>.Instance.GetField(Constant.FieldName, Constant.PrivateStatic));
                }
            }
        }
    }
}
