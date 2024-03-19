using System;
using System.Collections.Generic;
using System.Linq;

namespace NeeLaboratory.Linq
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource?> source)
            where TSource : class
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return source.Where(x => x != null)!;
        }

        public static IEnumerable<T> Direction<T>(this IEnumerable<T> self, int direction)
        {
            if (direction < 0)
            {
                return self.Reverse();
            }
            else
            {
                return self;
            }
        }
    }
}
