using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Mvc.JQuery.Datatables
{
    public static class DataTablesHelper
    {
        public static MvcHtmlString DataTable<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, IDataTablesResult<TResult>>> exp)
        {
            var properties = typeof (TResult).GetProperties().Where(p => p.GetGetMethod() != null).Select(p => p.Name);
            var mi = exp.MethodInfo();
             var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var vm = new DataTableVm(id, urlHelper.Action(mi.Name, controllerName), properties);
            return html.Partial("DataTable", vm);
        }
    }
}