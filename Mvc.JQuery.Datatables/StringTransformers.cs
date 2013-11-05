using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc.JQuery.Datatables
{
    static class StringTransformers
    {
        internal static object GetStringedValue(Type propertyType, object value)
        {
            foreach (var transformer in Transformers)
            {
                var result = transformer(propertyType, value);
                if (result != null) return result;
            }
            return (value as object ?? "").ToString();
        }
        private static readonly List<StringTransformer> Transformers = new List<StringTransformer>()
        {
            Guard<DateTimeOffset>(dateTimeOffset => dateTimeOffset.ToLocalTime().ToString("g")),
            Guard<DateTime>(dateTime => dateTime.ToLocalTime().ToString("g")),
            Guard<IHtmlString>(s => s.ToHtmlString()),
            Guard<IEnumerable<string>>(s => s.ToArray()),
            Guard<IEnumerable<int>>(s => s.ToArray()),
            Guard<IEnumerable<long>>(s => s.ToArray()),
            Guard<IEnumerable<decimal>>(s => s.ToArray()),
            Guard<IEnumerable<bool>>(s => s.ToArray()),
            Guard<IEnumerable<double>>(s => s.ToArray()),
            Guard<IEnumerable<object>>(s => s.Select(o => GetStringedValue(o.GetType(), o)).ToArray()),
            Guard<bool>(s => s),
            Guard<object>(o => (o ?? "").ToString())
        };

        public delegate object GuardedValueTransformer<TVal>(TVal value);


        public delegate object StringTransformer(Type type, object value);

        public static void RegisterFilter<TVal>(GuardedValueTransformer<TVal> filter)
        {
            Transformers.Add(Guard<TVal>(filter));
        }

        private static StringTransformer Guard<TVal>(GuardedValueTransformer<TVal> transformer)
        {
            return (t, v) =>
            {
                if (!typeof(TVal).IsAssignableFrom(t))
                {
                    return null;
                }
                return transformer((TVal)v);
            };
        }

        public static Dictionary<string, object> TransformDictionary(Dictionary<string, object> dict)
        {
            var output = new Dictionary<string, object>();
            foreach (var row in dict)
            {
                output[row.Key] = row.Value == null ? "" : GetStringedValue(row.Value.GetType(), row.Value);
            }
            return output;
        }
    }
}