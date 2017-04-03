using Mvc.JQuery.DataTables.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Mvc.JQuery.DataTables.Example.Controllers
{
    public class DefaultToStartOf2014Attribute : DataTablesAttributeBase
    {
        public override void ApplyTo(ColDef colDef, PropertyInfo pi)
        {
            colDef.SearchCols = colDef.SearchCols ?? new JObject();
            colDef.SearchCols["sSearch"] = new DateTime(2014, 1, 1).ToString("g") + "~" + DateTimeOffset.Now.Date.AddDays(1).ToString("g");
        }
    }
}