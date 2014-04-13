using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.JQuery.Datatables.Example;

[assembly: PreApplicationStartMethod(typeof(RegisterDatatablesModelBinder), "Start")]

namespace Mvc.JQuery.Datatables.Example
{

    public static class RegisterDatatablesModelBinder
    {
        public static void Start()
        {
            if (!ModelBinders.Binders.ContainsKey(typeof (DataTablesParam)))
                ModelBinders.Binders.Add(typeof (DataTablesParam), new Mvc.JQuery.Datatables.DataTablesModelBinder());
        }
    }
}