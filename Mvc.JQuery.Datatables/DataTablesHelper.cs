using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Mvc.JQuery.Datatables
{
    public static class DataTablesHelper
    {
        
        public static MvcHtmlString DataTable<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, IDataTablesResult<TResult>>> exp, params string[] columns)
        {
            if (columns == null || columns.Length == 0) columns = typeof (TResult).GetProperties().Where(p => p.GetGetMethod() != null).Select(p => p.Name).ToArray();

            var mi = exp.MethodInfo();
             var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var ajaxUrl = urlHelper.Action(mi.Name, controllerName);
            return DataTable(html, id, ajaxUrl, columns);
        }

        private static MvcHtmlString DataTable(HtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            var vm = new DataTableVm(id, ajaxUrl, columns);
            return html.Partial("DataTable", vm);
        }
    }
}