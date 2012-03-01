using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.JQuery.Datatables
{
    public class DataTableVm
    {
        static DataTableVm()
        {
            DefaultTableClass = "table table-bordered table-striped";
        }

        public static string DefaultTableClass { get; set; }
        public string TableClass { get; set; }

        public DataTableVm(string id, string ajaxUrl, IEnumerable<Tuple<string, Type>> columns)
        {
            AjaxUrl = ajaxUrl;
            this.Id = id;
            this.Columns = columns;
            FilterTypeRules = new FilterRuleList();
            FilterTypeRules.AddRange(StaticFilterTypeRules);
        }

        public string Id { get; private set; }

        public string AjaxUrl { get; private set; }

        public IEnumerable<Tuple<string, Type>> Columns { get; private set; }

        public bool ColumnFilter { get; set; }

        public bool TableTools { get; set; }

        public bool AutoWidth { get; set; }

        public string ColumnFiltersString
        {
            get { return string.Join(",", Columns.Select(c => GetFilterType(c.Item1, c.Item2))); }
        }


        public string GetFilterType(string columnName, Type type)
        {
            foreach (Func<string, Type, string> filterTypeRule in FilterTypeRules)
            {
                var rule = filterTypeRule(columnName, type);
                if (rule != null) return rule;
            }
            return "null";
        }

        public FilterRuleList FilterTypeRules;

        public static FilterRuleList StaticFilterTypeRules = new FilterRuleList()
        {
            (c, t) => (DateTypes.Contains(t)) ? "{type: 'date-range'}" : null,
            (c, t) => "{type: 'text'}", //by default, text filter on everything
        };

        private static List<Type> DateTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?) };

        public DataTableVm NoFilterOn(string name)
        {
            this.FilterTypeRules.Insert(0, (c, t) => c == name ? "null" : null);
            return this;
        }
        public DataTableVm NoFilterOn<T>()
        {
            this.FilterTypeRules.Insert(0, (c, t) => t == typeof(T) ? "null" : null);
            return this;
        }

    }
    public class FilterRuleList : List<Func<string, Type, string>>
    {
       
    } 
}