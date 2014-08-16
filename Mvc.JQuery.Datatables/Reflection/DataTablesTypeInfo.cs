using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace Mvc.JQuery.Datatables.Reflection
{
    internal static class DataTablesTypeInfo
    {
        static ConcurrentDictionary<Type, DataTablesPropertyInfo[]> propertiesCache = new ConcurrentDictionary<Type, DataTablesPropertyInfo[]>(); 
        internal static DataTablesPropertyInfo[] Properties(Type type)
        {
            return propertiesCache.GetOrAdd(type, t =>
            {
                var infos = from pi in t.GetProperties()
                            let attributes = (pi.GetCustomAttributes()).OfType<DataTablesAttributeBase>().ToArray()
                            orderby attributes.OfType<DataTablesAttribute>().Select(a => a.Order as int?).SingleOrDefault() ?? int.MaxValue
                            select new DataTablesPropertyInfo(pi, attributes);
                return infos.ToArray();
            });
            
        }
    }
    static class DataTablesTypeInfo<T>
    {
        internal static DataTablesPropertyInfo[] Properties { get; private set; }

        static DataTablesTypeInfo()
        {
            Properties = DataTablesTypeInfo.Properties(typeof (T));
        }

        public static Dictionary<string, object> ToDictionary(T row)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pi in Properties)
            {
                dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
            }
            return dictionary;
        }

        public static OrderedDictionary ToOrderedDictionary(T row)
        {
            var dictionary = new OrderedDictionary();
            foreach (var pi in Properties)
            {
                dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
            }
            return dictionary;
        }


    }
}