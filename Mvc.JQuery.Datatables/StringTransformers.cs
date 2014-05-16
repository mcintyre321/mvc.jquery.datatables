using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc.JQuery.Datatables
{
    public static class StringTransformers
    {
        internal static object GetStringedValue(Type propertyType, object value)
        {
            if (Transformers.ContainsKey(propertyType))
                return Transformers[propertyType](propertyType, value);
            return (value as object ?? "").ToString();
        }

        static StringTransformers()
        {
            RegisterFilter<DateTimeOffset>(dateTimeOffset => dateTimeOffset.ToLocalTime().ToString("g"));
            RegisterFilter<DateTime>(dateTime => dateTime.ToString("g"));
            RegisterFilter<IHtmlString>(s => s.ToHtmlString());
            RegisterFilter<IEnumerable<string>>(s => s.ToArray());
            RegisterFilter<IEnumerable<int>>(s => s.ToArray());
            RegisterFilter<IEnumerable<long>>(s => s.ToArray());
            RegisterFilter<IEnumerable<decimal>>(s => s.ToArray());
            RegisterFilter<IEnumerable<bool>>(s => s.ToArray());
            RegisterFilter<IEnumerable<double>>(s => s.ToArray());
            RegisterFilter<IEnumerable<object>>(s => s.Select(o => GetStringedValue(o.GetType(), o)).ToArray());
            RegisterFilter<bool>(s => s);
            RegisterFilter<object>(o => (o ?? "").ToString());
        }

        private static readonly Dictionary<Type, StringTransformer> Transformers = new Dictionary<Type, StringTransformer>();

        public delegate object GuardedValueTransformer<TVal>(TVal value);

        public delegate object StringTransformer(Type type, object value);

        public static void RegisterFilter<TVal>(GuardedValueTransformer<TVal> filter)
        {
            if (Transformers.ContainsKey(typeof(TVal)))
                Transformers[typeof(TVal)] = Guard(filter);
            else
                Transformers.Add(typeof(TVal), Guard(filter));
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
