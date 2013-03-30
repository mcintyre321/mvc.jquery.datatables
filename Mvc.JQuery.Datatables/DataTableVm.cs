using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Mvc.JQuery.Datatables
{
    public class ColDef
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Type Type { get; set; }

        public static ColDef Create(string name, string p1, Type propertyType)
        {
            return new ColDef() {Name = name, DisplayName = p1, Type = propertyType};
        }
    }
    public class DataTableVm
    {
        IDictionary<string, object> m_JsOptions = new Dictionary<string, object>();

        static DataTableVm()
        {
            DefaultTableClass = "table table-bordered table-striped";
        }

        public static string DefaultTableClass { get; set; }
        public string TableClass { get; set; }

        public DataTableVm(string id, string ajaxUrl, IEnumerable<ColDef> columns)
        {
            AjaxUrl = ajaxUrl;
            this.Id = id;
            this.Columns = columns;
            FilterTypeRules = new FilterRuleList();
            FilterTypeRules.AddRange(StaticFilterTypeRules);
            this.ShowSearch = true;
            this.ShowPageSizes = true;
            this.TableTools = true;
        }

        public bool ShowSearch { get; set; }

        public string Id { get; private set; }

        public string AjaxUrl { get; private set; }

        public IEnumerable<ColDef> Columns { get; private set; }

        public IDictionary<string, object> JsOptions { get { return m_JsOptions; } }

        public string JsOptionsString
        {
            get
            {
                return (new JavaScriptSerializer()).Serialize((object)JsOptions).TrimStart('{').TrimEnd('}');
            }
        }

        public bool ColumnFilter { get; set; }

        public bool TableTools { get; set; }

        public bool AutoWidth { get; set; }

        public string ColumnFiltersString
        {
            get { return string.Join(",", Columns.Select(c => GetFilterType(c.Name, c.Type))); }
        }

        public string Dom
        {
            get { 
                var sdom = "";
                if (TableTools) sdom += "T<\"clear\">";
                if (ShowPageSizes) sdom += "l";
                if (ShowSearch) sdom += "f";
                sdom += "tipr";
                return sdom;
            }
        }

        public bool ShowPageSizes { get; set; }


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
            (c, t) => t == typeof(bool) ? "{type: 'checkbox', values : ['True', 'False']}" : null,
            (c, t) => t.IsEnum ?  ("{type: 'checkbox', values : ['" + string.Join("','", Enum.GetNames(t)) + "']}") : null,
            (c, t) => "{type: 'text'}", //by default, text filter on everything
        };

        private static List<Type> DateTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?) };

        public class _FilterOn<TTarget>
        {
            private readonly TTarget _target;
            private readonly FilterRuleList _list;
            private readonly Func<string, Type, bool> _predicate;

            public _FilterOn(TTarget target, FilterRuleList list, Func<string, Type, bool> predicate)
            {
                _target = target;
                _list = list;
                _predicate = predicate;
            }

            public TTarget Select(params string[] options)
            {
                var escapedOptions = options.Select(o => o.Replace("'", "\\'"));
                AddRule("{type: 'select', values: ['" + string.Join("','", escapedOptions) + "']}");
                return _target;
            }
            public TTarget NumberRange()
            {
                AddRule("{type: 'number-range'}");
                return _target;
            }

            public TTarget DateRange()
            {
                AddRule("{type: 'date-range'}");
                return _target;
            }

            public TTarget Number()
            {
                AddRule("{type: 'number'}");
                return _target;
            }

            public TTarget CheckBoxes(params string[] options)
            {
                var escapedOptions = options.Select(o => o.Replace("'", "\\'"));
                AddRule("{type: 'checkbox', values: ['" + string.Join("','", escapedOptions) + "']}");
                return _target;
            }

            public TTarget None()
            {
                AddRule("null");
                return _target;
            }

            private void AddRule(string result)
            {
                _list.Insert(0, (c, t) => _predicate(c, t) ? result : null);
            }
        }
        public _FilterOn<DataTableVm> FilterOn<T>()
        {
            return new _FilterOn<DataTableVm>(this, this.FilterTypeRules, (c, t) => t == typeof(T));
        }
        public _FilterOn<DataTableVm> FilterOn(string columnName)
        {
            return new _FilterOn<DataTableVm>(this, this.FilterTypeRules, (c, t) => c == columnName);
        }

    }

    public class FilterRuleList : List<Func<string, Type, string>>
    {
       
    } 
}