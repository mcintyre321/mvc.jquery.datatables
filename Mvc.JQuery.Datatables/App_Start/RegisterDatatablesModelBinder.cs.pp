using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.RegisterDatatablesModelBinder), "Start")]

namespace $rootnamespace$.App_Start {
    public static class RegisterDatatablesModelBinder {
        public static void Start() {
            ModelBinders.Binders.Add(typeof(DataTablesParam), new DataTablesModelBinder());
        }
    }
}
