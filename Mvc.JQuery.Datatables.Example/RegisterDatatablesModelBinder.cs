using System.Web;
using System.Web.Mvc;
using Mvc.JQuery.Datatables.Example;
using Mvc.JQuery.DataTables;

[assembly: PreApplicationStartMethod(typeof(RegisterDatatablesModelBinder), "Start")]

namespace Mvc.JQuery.Datatables.Example
{

    public static class RegisterDatatablesModelBinder
    {
        public static void Start()
        {
            if (!ModelBinders.Binders.ContainsKey(typeof (DataTablesParam)))
                ModelBinders.Binders.Add(typeof (DataTablesParam), new DataTablesModelBinder());
        }
    }
}