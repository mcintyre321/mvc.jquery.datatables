using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Mvc.JQuery.Datatables
{
    public static class DataTablesHelper
    {
        public static IHtmlString DataTableIncludes(this HtmlHelper helper, bool jqueryUi = false, bool filters = true, bool tableTools = true)
        {
            StringBuilder output = new StringBuilder();
            Action<string> addJs = s => output.AppendLine(@"<script src=""" + s + @""" type=""text/javascript""></script>");
            Action<string> addCss = s => output.AppendLine(@"<link type=""text/css"" href=""" + s + @""" rel=""stylesheet""/>");

            addCss("/Content/DataTables/media/css/" + (jqueryUi ? ("jquery.dataTables_themeroller.css") : "jquery.dataTables.css"));
            addJs("/Content/DataTables/media/js/jquery.dataTables.js");
            if (filters) addJs("/Content/jquery.dataTables.columnFilter.js");
            if (tableTools)
            {
                addJs("/Content/DataTables/extras/TableTools/media/js/ZeroClipboard.js");
                addJs("/Content/DataTables/extras/TableTools/media/js/TableTools.js");
                addCss("/Content/DataTables/extras/TableTools/media/css/TableTools.css");
            }
            return helper.Raw(output.ToString());
        }

        public static DataTableConfigVm DataTableVm<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, DataTablesResult<TResult>>> exp, IEnumerable<ColDef> columns = null)
        {
            if (columns == null || !columns.Any())
            {
                //var propInfos = typeof (TResult).GetProperties().Where(p => p.GetGetMethod() != null).ToList();
                var propInfos = DataTablesTypeInfo<TResult>.Properties;
                var columnList = new List<ColDef>();
                foreach (var pi in propInfos)
                {
                    columnList.Add(new ColDef()
                    {
                        Name = pi.Item1.Name,
                        DisplayName = pi.Item2.DisplayName ?? pi.Item1.Name,
                        Sortable = pi.Item2.Sortable,
                        Visible = pi.Item2.Visible,
                        Searchable = pi.Item2.Searchable,
                        Type = pi.Item1.PropertyType
                    });
                }
                columns = columnList.ToArray();
            }

            var mi = exp.MethodInfo();
            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var ajaxUrl = urlHelper.Action(mi.Name, controllerName);
            return new DataTableConfigVm(id, ajaxUrl, columns);
        }

        public static DataTableConfigVm DataTableVm(this HtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            return new DataTableConfigVm(id, ajaxUrl, columns.Select(c => ColDef.Create(c, (string)null, typeof(string))));
        }
    }
}