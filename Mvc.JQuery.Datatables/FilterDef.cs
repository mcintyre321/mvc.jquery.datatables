using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.JQuery.Datatables
{
    public class FilterDef : Hashtable
    {
        internal object[] values { set { this["values"] = value; } }
        internal string type { set { this["type"] = value; } }


        public FilterDef(Type t)
        {
            SetDefaultValuesAccordingToColumnType(t);
        }

        private static List<Type> DateTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?) };


        private void SetDefaultValuesAccordingToColumnType(Type t)
        {
            if (t==null)
            {
                type = "null";
            }
            else if (DateTypes.Contains(t))
            {
                type = "date-range";
            }
            else if (t == typeof (bool))
            {
                type = "select";
                values = new object[] {"True", "False"};
            }
            else if (t == typeof (bool?))
            {
                type = "select";
                values = new object[] {"True", "False", "null"};
            }
            else if (t.IsEnum)
            {
                type = "checkbox";
                values = Enum.GetNames(t).Cast<object>().ToArray();
            }
            else
            {
                type = "text";
            }
        }
    }
}