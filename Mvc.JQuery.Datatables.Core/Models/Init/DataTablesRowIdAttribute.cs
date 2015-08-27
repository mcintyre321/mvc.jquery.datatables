using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mvc.JQuery.Datatables.Models;

namespace Mvc.JQuery.Datatables.Models.Init
{
    public class DataTablesRowIdAttribute : DataTablesAttributeBase
    {
        public bool EmitAsColumnName { get; set; }

        public override void ApplyTo(ColDef colDef, PropertyInfo pi)
        {
            // This attribute does not affect rendering
        }

        public DataTablesRowIdAttribute()
        {
            EmitAsColumnName = true;
        }
    }
}
