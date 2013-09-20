using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mvc.JQuery.Datatables
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataTablesSortableAttribute : Attribute
    {
        public DataTablesSortableAttribute() : this(true)
        { }
        public DataTablesSortableAttribute(bool sortable)
        {
            this.Sortable = sortable;
        }

        public bool Sortable { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataTablesVisibleAttribute : Attribute
    {
        public DataTablesVisibleAttribute()
            : this(true)
        { }
        public DataTablesVisibleAttribute(bool visible)
        {
            this.Visible = visible;
        }

        public bool Visible { get; set; }
    }

}
