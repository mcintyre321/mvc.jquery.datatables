using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Mvc.JQuery.Datatables.Models.Init
{
    public class ColDef
    {
        public ColDef(string name, Type type)
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
            this.Searchable = true;
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

        public JObject SearchCols { get; set; }
        public Attribute[] CustomAttributes { get; set; }
        public string Width { get; set; }
    }
}