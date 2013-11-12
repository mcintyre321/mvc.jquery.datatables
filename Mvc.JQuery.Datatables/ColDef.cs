using System;

namespace Mvc.JQuery.Datatables
{
    public class ColDef
    {
        protected internal ColDef(Type type)
        {
            Type = type;
            Filter = new FilterDef(Type);
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Visible { get; set; }
        public bool Sortable { get; set; }
        public Type Type { get; set; }
        public bool Searchable { get; set; }
        public SortDirection SortDirection { get; set; }
        public string MRenderFunction { get; set; }
        public FilterDef Filter { get; set; }

        public static ColDef Create(string name, string p1, Type propertyType, bool visible = true, bool sortable = true,
            SortDirection sortDirection = SortDirection.None, string mRenderFunction = null)
        {
            return new ColDef(propertyType)
            {
                Name = name,
                DisplayName = p1,
                Visible = visible,
                Sortable = sortable,
                SortDirection = sortDirection,
                MRenderFunction = mRenderFunction,
            };
        }


    }
}