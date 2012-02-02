using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Mvc.JQuery.Datatables
{
    public static class DataTablesHelper
    {
        public static IHtmlString DataTableIncludes(this HtmlHelper helper, bool css = true, bool filters = true)
        {
            const string cssHtml = @"<link type=""text/css"" href=""@Url.Content(""~/Content/DataTables-1.8.2/media/css/demo_table.css"" rel=""stylesheet""/>";

            const string jsHtml = @"<script src=""/Scripts/DataTables-1.9.0/media/js/jquery.dataTables.js"" type=""text/javascript""></script>";
            const string filtersHtml = @"<script src=""/Scripts/jquery.dataTables.columnFilter.js"" type=""text/javascript""></script>";
            return helper.Raw(jsHtml + (filters ? filtersHtml : "") + (css ? cssHtml : ""));

        }

        public static MvcHtmlString DataTable<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, DataTablesResult<TResult>>> exp, params string[] columns)
        {
            if (columns == null || columns.Length == 0) columns = typeof(TResult).GetProperties().Where(p => p.GetGetMethod() != null).Select(p => p.Name).ToArray();

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