using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Mvc.JQuery.Datatables
{
    public class ColDef
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Visible { get; set; }
        public bool Sortable { get; set; }
        public Type Type { get; set; }
        public bool Searchable { get; set; }

        public static ColDef Create(string name, string p1, Type propertyType, bool visible = true, bool sortable = true)
        {
            return new ColDef() {Name = name, DisplayName = p1, Type = propertyType, Visible = visible, Sortable = sortable};
        }
    }
    public class DataTableConfigVm
    {
        IDictionary<string, object> m_JsOptions = new Dictionary<string, object>();

        static DataTableConfigVm()
        {
            DefaultTableClass = "table table-bordered table-striped";
        }

        public static string DefaultTableClass { get; set; }
        public string TableClass { get; set; }

        public DataTableConfigVm(string id, string ajaxUrl, IEnumerable<ColDef> columns)
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
                return convertDictionaryToJsonBody(JsOptions);
            }
        }

        public string ColumnDefsString
        {
            get
            {
                return convertColumnDefsToJson(Columns);
            }
        }

        public bool ColumnFilter
        {
            get { return _columnFilter; }
            set { 
                _columnFilter = value; 
                if (value)
                {
                    foreach (var column in Columns.Where(c => c.Searchable == false))
                    {
                        this.FilterOn(column.Name).None();
                    }
                }
            }
        }

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

        public bool StateSave { get; set; }

        public string Language { get; set; }

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
        private bool _columnFilter;

        public static FilterRuleList StaticFilterTypeRules = new FilterRuleList()
        {
            (c, t) => (DateTypes.Contains(t)) ? "{type: 'date-range'}" : null,
            (c, t) => t == typeof(bool) ? "{type: 'checkbox', values : ['True', 'False']}" : null,
            (c, t) => t == typeof(bool?) ? "{type: 'checkbox', values : ['True', 'False', 'null']}" : null,
            (c, t) => t.IsEnum ?  ("{type: 'checkbox', values : ['" + string.Join("','", Enum.GetNames(t)) + "']}") : null,
            (c, t) => "{type: 'text'}", //by default, text filter on everything
        };

        private static List<Type> DateTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?) };

        public class _FilterOn<TTarget>
        {
            private readonly TTarget _target;
            private readonly FilterRuleList _list;
            private readonly Func<string, Type, bool> _predicate;
            private readonly IDictionary<string, object> _jsOptions;

            public _FilterOn(TTarget target, FilterRuleList list, Func<string, Type, bool> predicate, IDictionary<string, object> jsOptions)
            {
                _target = target;
                _list = list;
                _predicate = predicate;
                _jsOptions = jsOptions;
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

            public TTarget Text()
            {
                AddRule("{type: 'text'}");
                return _target;
            }

            public TTarget None()
            {
                AddRule("null");
                return _target;
            }

            private void AddRule(string result)
            {
                if (result != "null" && this._jsOptions != null && this._jsOptions.Count > 0)
                {
                    var _jsOptionsAsJson = DataTableConfigVm.convertDictionaryToJsonBody(this._jsOptions);
                    result = result.TrimEnd('}') + ", " + _jsOptionsAsJson + "}";
                }
                _list.Insert(0, (c, t) => _predicate(c, t) ? result : null);
            }
        }
        public _FilterOn<DataTableConfigVm> FilterOn<T>()
        {
            return FilterOn<T>(null); 
        }
        public _FilterOn<DataTableConfigVm> FilterOn<T>(object jsOptions)
        {
            IDictionary<string, object> optionsDict = DataTableConfigVm.convertObjectToDictionary(jsOptions);
            return FilterOn<T>(optionsDict); 
        }
        public _FilterOn<DataTableConfigVm> FilterOn<T>(IDictionary<string, object> jsOptions)
        {
            return new _FilterOn<DataTableConfigVm>(this, this.FilterTypeRules, (c, t) => t == typeof(T), jsOptions);
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName)
        {
            return FilterOn(columnName, null);
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, object jsOptions)
        {
            IDictionary<string, object> optionsDict = DataTableConfigVm.convertObjectToDictionary(jsOptions);
            return FilterOn(columnName, optionsDict); 
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, IDictionary<string, object> jsOptions)
        {
            return new _FilterOn<DataTableConfigVm>(this, this.FilterTypeRules, (c, t) => c == columnName, jsOptions);
        }

        private static string convertDictionaryToJsonBody(IDictionary<string, object> dict)
        {
            // Converting to System.Collections.Generic.Dictionary<> to ensure Dictionary will be converted to Json in correct format
            var dictSystem = new Dictionary<string, object>(dict);
            return (new JavaScriptSerializer()).Serialize((object)dictSystem).TrimStart('{').TrimEnd('}');
        }

        private static string convertColumnDefsToJson(IEnumerable<ColDef> columns)
        {
            var nonSortableColumns = columns.Select((x, idx) => x.Sortable ? -1 : idx).Where( x => x > -1).ToArray();
            var nonVisibleColumns = columns.Select((x, idx) => x.Visible ? -1 : idx).Where(x => x > -1).ToArray();
            var nonSearchableColumns = columns.Select((x, idx) => x.Searchable ? -1 : idx).Where(x => x > -1).ToArray();

            var defs = new List<dynamic>();

            if (nonSortableColumns.Any())
                defs.Add(new { bSortable = false, aTargets = nonSortableColumns });
            if (nonVisibleColumns.Any())
                defs.Add(new { bVisible = false, aTargets = nonVisibleColumns });
            if (nonSearchableColumns.Any())
                defs.Add(new { bSearchable = false, aTargets = nonSearchableColumns }); 

            if (defs.Count > 0) 
                return new JavaScriptSerializer().Serialize(defs);

            return "[]";
        }

        private static IDictionary<string, object> convertObjectToDictionary(object obj)
        {
            // Doing this way because RouteValueDictionary converts to Json in wrong format
            return new Dictionary<string, object>(new RouteValueDictionary(obj));
        }
    }

    public class FilterRuleList : List<Func<string, Type, string>>
    {
       
    } 
}