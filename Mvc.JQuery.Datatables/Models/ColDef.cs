using System;
using System.Collections.Generic;

namespace Mvc.JQuery.Datatables.Models
{
    public class ColDef
    {
        protected internal ColDef(string name, Type type)
        {
            Name = name;
            Type = type;
            Filter = new FilterDef(Type);
            DisplayName = name;
            Visible = true;
            Sortable = true;
            SortDirection = SortDirection.None;
            MRenderFunction = (string) null;
            CssClass = "";
            CssClassHeader = "";
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Visible { get; set; }
        public bool Sortable { get; set; }
        public Type Type { get; set; }
        public bool Searchable { get; set; }
        public String CssClass { get; set; }
        public String CssClassHeader { get; set; }
        public SortDirection SortDirection { get; set; }
        public string MRenderFunction { get; set; }
        public FilterDef Filter { get; set; }

        IDictionary<string, object> m_JsInitialSearchCols = new Dictionary<string, object>();
        public IDictionary<string, object> JsInitialSearchCols { get { return m_JsInitialSearchCols; } }
        public Attribute[] CustomAttributes { get; set; }
    }
}