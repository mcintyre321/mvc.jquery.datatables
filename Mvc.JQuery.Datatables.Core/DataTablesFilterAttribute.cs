using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mvc.JQuery.DataTables.Models;

namespace Mvc.JQuery.DataTables
{
    public class DataTablesFilterAttribute : DataTablesAttributeBase
    {
        private readonly string filterType;
        private readonly object[] options;

        public DataTablesFilterAttribute()
        {
        }

        public DataTablesFilterAttribute(DataTablesFilterType filterType, params object[] options)
            :this(GetFilterTypeString(filterType), options)
        {

        }

        private static string GetFilterTypeString(DataTablesFilterType filterType)
        {
            return filterType.ToString().ToLower().Replace("range", "-range");
        }

        public DataTablesFilterAttribute(string filterType, params object[] options)
        {
            this.filterType = filterType;
            this.options = options;
        }

        public override void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi)
        {
            colDef.Filter = new FilterDef(pi.GetType());
            if (filterType == "none")
            {
                colDef.Filter = null;
            }
            else
            {
                if (filterType != null) colDef.Filter.type = filterType;
                if (options != null && options.Any()) colDef.Filter.values = options;
            }
        }

    }

    public enum DataTablesFilterType
    {
        None,
        Select,
        NumberRange,
        DateRange,
        Checkbox,
        Text,
        DateTimeRange
    }
 
}