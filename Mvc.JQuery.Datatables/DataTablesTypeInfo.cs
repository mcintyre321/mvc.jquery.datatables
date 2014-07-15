using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Mvc.JQuery.Datatables
{
    public static class DataTablesTypeInfoHelper
    {
        public static string ToDisplayName(this DataTablesAttribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.DisplayName) || attribute.DisplayNameResourceType == null)
                return attribute.DisplayName;
            var value = ResourceHelper.GetResourceLookup<string>(attribute.DisplayNameResourceType, attribute.DisplayName);
            return value;
        }
    }

    public static class DataTablesTypeInfo<T>
    {
        public static DataTablesPropertyInfo[] Properties { get; private set; }

        static DataTablesTypeInfo()
        {
            var infos = from pi in typeof (T).GetProperties()
                        let da = (DataTablesAttribute)(pi.GetCustomAttributes(typeof(DataTablesAttribute), false).SingleOrDefault() ?? new DataTablesAttribute())
                             orderby da.Order
                        select new DataTablesPropertyInfo(pi, da);
            Properties = infos.ToArray();
        }

        public static Dictionary<string, object> ToDictionary(T row)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pi in Properties)
            {
                dictionary[pi.Item1.Name] = pi.Item1.GetValue(row, null);
            }
            return dictionary;
        }
    }

    

    public class DataTablesPropertyInfo
    {
        public DataTablesPropertyInfo(PropertyInfo item1, DataTablesAttribute item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public PropertyInfo Item1 { get; private set; }
        public DataTablesAttribute Item2 { get; private set; }

    }

    public class DataTablesAttribute : Attribute
    {
        public DataTablesAttribute()
        {
            this.Sortable = true;
            this.Searchable = true;
            this.Visible = true;
            this.Order = int.MaxValue;
        }

        public bool Searchable { get; set; }
        public bool Sortable { get; set; }
        public int Order { get; set; }
        public string DisplayName { get; set; }
        public Type DisplayNameResourceType { get; set; }
        public SortDirection SortDirection { get; set; }
        public string MRenderFunction { get; set; }
        public String CssClass { get; set; }
        public String CssClassHeader { get; set; }

        public bool Visible { get; set; }
    }

    public enum SortDirection
    {
        None,
        Ascending,
        Descending
    }
}