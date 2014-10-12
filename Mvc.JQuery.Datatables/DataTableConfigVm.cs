using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Mvc.JQuery.Datatables.Models;
using Mvc.JQuery.Datatables.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mvc.JQuery.Datatables
{
    public class DataTableConfigVm
    {
        public bool HideHeaders { get; set; }
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
            this.ShowSearch = true;
            this.ShowPageSizes = true;
            this.TableTools = true;
            ColumnFilterVm = new ColumnFilterSettingsVm(this);
            AjaxErrorHandler = 
                "function(jqXHR, textStatus, errorThrown)" + 
                "{ " + 
                    "window.alert('error loading data: ' + textStatus + ' - ' + errorThrown); " + 
                    "console.log(arguments);" + 
                "}";
        }

        public bool ShowSearch { get; set; }

        public string Id { get; private set; }

        public string AjaxUrl { get; private set; }

        public IEnumerable<ColDef> Columns { get; private set; }

        public IDictionary<string, object> JsOptions { get { return m_JsOptions; } }

       

        public string ColumnDefsString
        {
            get
            {
                return ConvertColumnDefsToJson(Columns.ToArray());
            }
        }
        public bool ColumnFilter { get; set; }

        public ColumnFilterSettingsVm ColumnFilterVm { get; set; }

        public bool TableTools { get; set; }

        public bool AutoWidth { get; set; }

        public JToken SearchCols
        {
            get
            {
                var initialSearches = Columns
                    .Select(c => c.Searchable & c.SearchCols != null ? c.SearchCols : null as object).ToArray();
                return new JArray(initialSearches);
            }
        }



        public string Dom
        {
            get { 
                var sdom = "";
                if (ColVis) sdom += "C";
                if (TableTools) sdom += "T<\"clear\">";
                if (ShowPageSizes) sdom += "l";
                if (ShowSearch) sdom += "f";
                sdom += "tipr";
                return sdom;
            }
        }

        public bool ColVis { get; set; }

        public string ColumnSortingString
        {
            get
            {
                return ConvertColumnSortingToJson(Columns);
            }
        }

        public bool ShowPageSizes { get; set; }

        public bool StateSave { get; set; }

        public string Language { get; set; }

        public string DrawCallback { get; set; }
        public LengthMenuVm LengthMenu { get; set; }
        public int? PageLength { get; set; }
        public string GlobalJsVariableName { get; set; }

        private bool _columnFilter;

        public bool FixedLayout { get; set; }
        public string AjaxErrorHandler { get; set; }

        public class _FilterOn<TTarget>
        {
            private readonly TTarget _target;
            private readonly ColDef _colDef;

            public _FilterOn(TTarget target, ColDef colDef)
            {
                _target = target;
                _colDef = colDef;

            }

            public TTarget Select(params string[] options)
            {
                _colDef.Filter.type = "select";
                _colDef.Filter.values = options.Cast<object>().ToArray();
                return _target;
            }
            public TTarget NumberRange()
            {
                _colDef.Filter.type = "number-range";
                return _target;
            }

            public TTarget DateRange()
            {
                _colDef.Filter.type = "date-range";
                return _target;
            }

            public TTarget Number()
            {
                _colDef.Filter.type = "number";
                return _target;
            }

            public TTarget CheckBoxes(params string[] options)
            {
                _colDef.Filter.type = "checkbox";
                _colDef.Filter.values = options.Cast<object>().ToArray();
                return _target;
            }

            public TTarget Text()
            {
                _colDef.Filter.type = "text";
                return _target;
            }

            public TTarget None()
            {
                _colDef.Filter = null;
                return _target;
            }
        }
        public _FilterOn<DataTableConfigVm> FilterOn<T>()
        {
            return FilterOn<T>(null); 
        }
        public _FilterOn<DataTableConfigVm> FilterOn<T>(object jsOptions)
        {
            IDictionary<string, object> optionsDict = DataTableConfigVm.ConvertObjectToDictionary(jsOptions);
            return FilterOn<T>(optionsDict); 
        }
        ////public _FilterOn<DataTableConfigVm> FilterOn<T>(IDictionary<string, object> jsOptions)
        ////{
        ////    return new _FilterOn<DataTableConfigVm>(this, this.FilterTypeRules, (c, t) => t == typeof(T), jsOptions);
        ////}
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName)
        {
            return FilterOn(columnName, null);
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, object jsOptions)
        {
            IDictionary<string, object> optionsDict = ConvertObjectToDictionary(jsOptions);
            return FilterOn(columnName, optionsDict); 
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, object jsOptions, object jsInitialSearchCols)
        {
            IDictionary<string, object> optionsDict = ConvertObjectToDictionary(jsOptions);
            IDictionary<string, object> initialSearchColsDict = ConvertObjectToDictionary(jsInitialSearchCols);
            return FilterOn(columnName, optionsDict, initialSearchColsDict);
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, IDictionary<string, object> jsOptions)
        {
            return FilterOn(columnName, jsOptions, null);
        }
        public _FilterOn<DataTableConfigVm> FilterOn(string columnName, IDictionary<string, object> jsOptions, IDictionary<string, object> jsInitialSearchCols)
        {
            var colDef = this.Columns.Single(c => c.Name == columnName);
            if (jsOptions != null)
            {
                foreach (var jsOption in jsOptions)
                {
                    colDef.Filter[jsOption.Key] = jsOption.Value;
                }
            }
            if (jsInitialSearchCols != null)
            {
                colDef.SearchCols = new JObject();
                foreach (var jsInitialSearchCol in jsInitialSearchCols)
                {
                    colDef.SearchCols[jsInitialSearchCol.Key] = new JValue(jsInitialSearchCol.Value);
                }
            }
            return new _FilterOn<DataTableConfigVm>(this, colDef);
        }

        private static string ConvertDictionaryToJsonBody(IDictionary<string, object> dict)
        {
            // Converting to System.Collections.Generic.Dictionary<> to ensure Dictionary will be converted to Json in correct format
            var dictSystem = new Dictionary<string, object>(dict);
            var json = JsonConvert.SerializeObject((object)dictSystem, Formatting.None, new RawConverter());
            return json.Substring(1, json.Length - 2);
        }

        private static string ConvertColumnDefsToJson(ColDef[] columns)
        {
            var nonSortableColumns = columns.Select((x, idx) => x.Sortable ? -1 : idx).Where( x => x > -1).ToArray();
            var nonVisibleColumns = columns.Select((x, idx) => x.Visible ? -1 : idx).Where(x => x > -1).ToArray();
            var nonSearchableColumns = columns.Select((x, idx) => x.Searchable ? -1 : idx).Where(x => x > -1).ToArray();
            var mRenderColumns = columns.Select((x, idx) => string.IsNullOrEmpty(x.MRenderFunction) ? new { x.MRenderFunction, Index = -1 } : new { x.MRenderFunction, Index = idx }).Where(x => x.Index > -1).ToArray();
            var CssClassColumns = columns.Select((x, idx) => string.IsNullOrEmpty(x.CssClass) ? new { x.CssClass, Index = -1 } : new { x.CssClass, Index = idx }).Where(x => x.Index > -1).ToArray();
            


            var defs = new List<dynamic>();

            if (nonSortableColumns.Any())
                defs.Add(new { bSortable = false, aTargets = nonSortableColumns });
            if (nonVisibleColumns.Any())
                defs.Add(new { bVisible = false, aTargets = nonVisibleColumns });
            if (nonSearchableColumns.Any())
                defs.Add(new { bSearchable = false, aTargets = nonSearchableColumns }); 
            if (mRenderColumns.Any())
                foreach (var mRenderColumn in mRenderColumns)
                {
                    defs.Add(new { mRender = "%" + mRenderColumn.MRenderFunction + "%", aTargets = new[] {mRenderColumn.Index} });
                }
            if (CssClassColumns.Any())
                foreach (var CssClassColumn in CssClassColumns)
                {
                    defs.Add(new { className = CssClassColumn.CssClass, aTargets = new[] { CssClassColumn.Index } });
                }

            for(var i=0;i<columns.Length;i++)
            {
                if (columns[i].Width != null)
                {
                    defs.Add(new { width = columns[i].Width, aTargets = new[] { i} });
                }
            }
             

            if (defs.Count > 0)
                return new JavaScriptSerializer().Serialize(defs).Replace("\"%", "").Replace("%\"", "");

            return "[]";
        }

        private static string ConvertColumnSortingToJson(IEnumerable<ColDef> columns)
        {
            var sortList = columns.Select((c, idx) => c.SortDirection == SortDirection.None ? new dynamic[] { -1, "" } : (c.SortDirection == SortDirection.Ascending ? new dynamic[] { idx, "asc" } : new dynamic[] { idx, "desc" })).Where(x => x[0] > -1).ToArray();

            if (sortList.Length > 0) 
                return new JavaScriptSerializer().Serialize(sortList);

            return "[]";
        }

        private static IDictionary<string, object> ConvertObjectToDictionary(object obj)
        {
            // Doing this way because RouteValueDictionary converts to Json in wrong format
            return new Dictionary<string, object>(new RouteValueDictionary(obj));
        }
    }
}