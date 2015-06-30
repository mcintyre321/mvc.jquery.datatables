using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mvc.JQuery.Datatables
{
    public class ResourceHelper
    {
        public static T GetResourceLookup<T>(Type resourceType, string resourceName)
        {
            if ((resourceType != null) && (resourceName != null))
            {
                System.Reflection.PropertyInfo property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                if (property == null)
                {
                    return default(T);
                }

                return (T)property.GetValue(null, null);
            }
            return default(T);
        }
    }
    
    /// <summary>
    /// Group of helpers that gets Display attributes values from Enum members
    /// DražaM
    /// </summary>
    public static class EnumHelper
    {
        public static string DisplayName(this Enum value)
        {
            Type enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            MemberInfo member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = member.Name;
            if (attrs.Length > 0)
            {

                if (((DisplayAttribute)attrs[0]).ResourceType != null)
                {
                    outString = ((DisplayAttribute)attrs[0]).GetName();
                }
                else
                {
                    outString = ((DisplayAttribute)attrs[0]).Name;
                }
            }
            return outString;
        }

        public static List<string> AllDisplayNames(this Type tip)
        {
            List<string> exitList = new List<string>();
            foreach (string r in Enum.GetNames(tip))
            {
                exitList.Add(((Enum)Enum.Parse(tip, r)).DisplayName());
            };
            return exitList;
        }

        public static object[] EnumValLabPairs(this Type type)
        {
            var vals = Enum.GetNames(type).Cast<object>().ToArray();
            var lbls = type.AllDisplayNames().Cast<object>().ToArray();
            var result = new List<object>();

            for (var x = 0; x <= vals.Length - 1; x++) { result.Add(new { value = vals[x], label = lbls[x] }); }

            return result.ToArray<object>();
        }
    }
}
