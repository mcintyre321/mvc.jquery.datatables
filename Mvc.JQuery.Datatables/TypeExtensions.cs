using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

public static class TypeExtensions
{
    public static IEnumerable<PropertyInfo> GetSortedProperties(this Type t)
    {
        return from pi in t.GetProperties()
               let da = (DisplayAttribute)pi.GetCustomAttributes(typeof(DisplayAttribute), false).SingleOrDefault()
               let order = ((da != null && da.Order != 0) ? da.Order : int.MaxValue)
               orderby order
               select pi;
    }

    public static IEnumerable<PropertyInfo> GetSortedProperties<T>()
    {
        return typeof(T).GetSortedProperties();
    }
    public static IEnumerable<PropertyInfo> GetProperties(this Type t)
    {
        return from pi in t.GetProperties()
               select pi;
    }

    public static IEnumerable<PropertyInfo> GetProperties<T>()
    {
        return typeof(T).GetSortedProperties();
    }
}