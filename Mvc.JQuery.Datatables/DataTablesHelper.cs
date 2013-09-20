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
                var propInfos = TypeExtensions.GetSortedProperties<TResult>();
                var columnList = new List<ColDef>();
                foreach (var propertyInfo in propInfos)
                {
                    var displayNameAttribute = (DisplayNameAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault();
                    var displayName = displayNameAttribute == null ? propertyInfo.Name : displayNameAttribute.DisplayName;

                    var sortableAttribute = (DataTablesSortableAttribute)propertyInfo.GetCustomAttributes(typeof(DataTablesSortableAttribute), false).FirstOrDefault();
                    var sortable = sortableAttribute == null ? true : sortableAttribute.Sortable;

                    var visibleAttribute = (DataTablesVisibleAttribute)propertyInfo.GetCustomAttributes(typeof(DataTablesVisibleAttribute), false).FirstOrDefault();
                    var visible = visibleAttribute == null ? true : visibleAttribute.Visible;

                    columnList.Add(new ColDef()
                    {
                        Name = propertyInfo.Name,
                        DisplayName = displayName,
                        Sortable = sortable,
                        Visible = visible,
                        Type = propertyInfo.PropertyType
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