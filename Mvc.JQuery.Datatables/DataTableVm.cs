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

        public DataTableVm(string id, string ajaxUrl, IEnumerable<Tuple<string, DataTablesColumn>> columns)
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

        public IEnumerable<Tuple<string, DataTablesColumn>> Columns { get; private set; }

        public bool ColumnFilter { get; set; }

        public bool TableTools { get; set; }

        public bool AutoWidth { get; set; }

        public string ColumnFiltersString
        {
            get { return string.Join(",", Columns.Select(c => GetFilterType(c.Item1, c.Item2.Type))); }
        }

        public string ColumnDefs
        {
            get
            {
                string s = "[";
                for (int i = 0; i < Columns.Count(); i++ )
                {
                    var col = Columns.ElementAt(i);
                    s += string.Format("{{ \"sName\": \"{0}\", \"sTitle\": \"{1}\", \"bVisible\": {2}, \"aTargets\": [{3}]}}", col.Item1, col.Item2.Title, col.Item2.Visible.ToString().ToLowerInvariant(), i);
                    if (i < Columns.Count() - 1)
                        s += ", ";
                }

                s += "]";
                return s;
            }
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
                AddRule("{type: 'select', values: ['" + string.Join("','", options) + "']}");
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