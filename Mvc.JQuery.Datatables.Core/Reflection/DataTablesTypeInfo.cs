using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Mvc.JQuery.Datatables.Models;

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
                            where pi.GetCustomAttribute<DataTablesExcludeAttribute>() == null
                            let attributes = (pi.GetCustomAttributes()).OfType<DataTablesAttributeBase>().ToArray()
                            orderby attributes.OfType<DataTablesAttribute>().Select(a => a.Order as int?).SingleOrDefault() ?? int.MaxValue
                            select new DataTablesPropertyInfo(pi, attributes);
                return infos.ToArray();
            });
            
        }
    }
    public static class DataTablesTypeInfo<T>
    {
        internal static DataTablesPropertyInfo[] Properties { get; private set; }
        internal static DataTablesPropertyInfo RowID { get; private set; }

        static DataTablesTypeInfo()
        {
            Properties = DataTablesTypeInfo.Properties(typeof (T));
            RowID = Properties.SingleOrDefault(x => x.Attributes.Any(y => y is DataTablesRowIdAttribute));
        }

        public static Dictionary<string, object> ToDictionary(T row)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pi in Properties)
            {
                dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
            }
            if (RowID != null)
            {
                dictionary["DT_RowID"] = RowID.PropertyInfo.GetValue(row, null);
                if (!RowID.Attributes.OfType<DataTablesRowIdAttribute>().First().EmitAsColumnName)
                {
                    dictionary.Remove(RowID.PropertyInfo.Name);
                }
            }
            return dictionary;
        }

        public static Func<T, Dictionary<string, object>> ToDictionary(ResponseOptions<T> options = null)
        {
            if (options == null || options.DT_RowID == null)
            {
                return ToDictionary;
            }
            else
            {
                return row =>
                {
                    var dictionary = new Dictionary<string, object>();
                    dictionary["DT_RowID"] = options.DT_RowID(row);
                    foreach (var pi in Properties)
                    {
                        dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
                    }
                    return dictionary;
                };
            }
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