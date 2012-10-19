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

        public static DataTableVm DataTableVm<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, DataTablesResult<TResult>>> exp, params Tuple<string, string, Type>[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                var propInfos = typeof (TResult).GetProperties().Where(p => p.GetGetMethod() != null).ToList();
                var columnList = new List<Tuple<string, string, Type>>();
                foreach (var propertyInfo in propInfos)
                {
                    var displayNamettribute = (DisplayNameAttribute)propertyInfo.GetCustomAttributes(typeof (DisplayNameAttribute), false).FirstOrDefault();
                    if(displayNamettribute != null)
                    {
                        columnList.Add(Tuple.Create(propertyInfo.Name, displayNamettribute.DisplayName, propertyInfo.PropertyType));
                    }
                    else
                    {
                        columnList.Add(Tuple.Create(propertyInfo.Name, propertyInfo.Name, propertyInfo.PropertyType));
                    }
                }
                columns = columnList.ToArray();
            }

            var mi = exp.MethodInfo();
            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var ajaxUrl = urlHelper.Action(mi.Name, controllerName);
            return new DataTableVm(id, ajaxUrl, columns);
        }

        public static DataTableVm DataTableVm(this HtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            return new DataTableVm(id, ajaxUrl, columns.Select(c => Tuple.Create(c, (string)null, typeof(string))));
        }
    }
}