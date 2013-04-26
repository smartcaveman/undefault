namespace Undefault
{
    internal static class TypeOf<T>
    {
        static TypeOf()
        {
            Instance = typeof(T);
        }

        /// <summary>Provides slightly more performant access to the type of the generic parameter</summary>
        public static readonly System.Type Instance;
    }
}