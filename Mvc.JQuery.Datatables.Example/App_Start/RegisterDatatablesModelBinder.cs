using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

[assembly: PreApplicationStartMethod(typeof(Mvc.JQuery.Datatables.Example.App_Start.RegisterDatatablesModelBinder), "Start")]

namespace Mvc.JQuery.Datatables.Example.App_Start {
    public static class RegisterDatatablesModelBinder {
        public static void Start() {
            ModelBinders.Binders.Add(typeof(Mvc.JQuery.Datatables.DataTablesParam), new Mvc.JQuery.Datatables.DataTablesModelBinder());
        }
    }
}
