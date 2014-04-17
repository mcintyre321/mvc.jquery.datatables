using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mvc.JQuery.Datatables.Tests.TestingUtilities
{
    static class LinqExtensions
    {
        internal static IEnumerable<T> ReplaceAtIndex<T>(this IEnumerable<T> source, T newValue,params int[] indices)
        {
            int i = 0;
            var e = source.GetEnumerator();
            while (e.MoveNext())
            {
                yield return (indices.Contains(i++))
                    ? newValue
                    : e.Current;
            }
        }
    }
}
