namespace MathNet.Spatial.Internals
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class ImmutableList
    {
        internal static ImmutableList<T> Create<T>(IEnumerable<T> data)
        {
            return ImmutableList<T>.Empty.AddRange(data.AsCollection());
        }

        private static ICollection<T> AsCollection<T>(this IEnumerable<T> source)
        {
            if (source is ICollection<T> collection)
            {
                return collection;
            }

            if (source is ImmutableList<T> list)
            {
                return list.GetRawData();
            }

            return source.ToList();
        }
    }
}