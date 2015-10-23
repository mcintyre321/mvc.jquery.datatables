using System.Web;
using System.Web.Mvc;
using Mvc.JQuery.DataTables.Example;
using Mvc.JQuery.DataTables;

[assembly: PreApplicationStartMethod(typeof(RegisterDataTablesModelBinder), "Start")]

namespace Mvc.JQuery.DataTables.Example
{

    public static class RegisterDataTablesModelBinder
    {
        public static void Start()
        {
            if (!ModelBinders.Binders.ContainsKey(typeof (DataTablesParam)))
                ModelBinders.Binders.Add(typeof (DataTablesParam), new DataTablesModelBinder());
        }
    }
}