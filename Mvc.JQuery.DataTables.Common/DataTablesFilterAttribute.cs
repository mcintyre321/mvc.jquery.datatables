using Mvc.JQuery.DataTables.Models;
//using System.Linq;

namespace Mvc.JQuery.DataTables
{
    public class DataTablesFilterAttribute : DataTablesAttributeBase
    {
        private readonly string filterType;
        //private readonly object[] options;
        public DataTablesFilterAttribute()
        {
        }

        public DataTablesFilterAttribute(DataTablesFilterType filterType)
            :this(GetFilterTypeString(filterType))
        {

        }

        /// <summary>
        /// Sets sSelector on the column (i.e. selector for custom positioning)
        /// </summary>
        public string Selector { get; set; }

        private static string GetFilterTypeString(DataTablesFilterType filterType)
        {
            return filterType.ToString().ToLower().Replace("range", "-range");
        }

        public DataTablesFilterAttribute(string filterType)
        {
            this.filterType = filterType;
        }

        public override void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi)
        {
            colDef.Filter = new FilterDef(pi.PropertyType);
            if (Selector != null)
            {
                colDef.Filter["sSelector"] = Selector;
            }
            if (filterType == "none")
            {
                colDef.Filter = null;
            }
            else
            {
                if (filterType != null) colDef.Filter.type = filterType;
                //if (options != null && options.Any()) colDef.Filter.values = options;
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