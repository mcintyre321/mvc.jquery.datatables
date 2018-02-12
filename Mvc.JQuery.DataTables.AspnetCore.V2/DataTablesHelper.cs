using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Mvc.JQuery.DataTables.Models;
using Mvc.JQuery.DataTables.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mvc.JQuery.DataTables
{
    public static class DataTablesHelper
    {
        public static DataTableConfigVm DataTableVm<TController, TResult>(this IHtmlHelper html, string id,
            Expression<Func<TController, DataTablesResult<TResult>>> exp, IEnumerable<ColDef> columns = null)
        {
            if (columns == null || !columns.Any())
            {
                columns = typeof(TResult).ColDefs();
            }

            var mi = exp.MethodInfo();
            var controllerName = typeof (TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));
            var urlHelper = new UrlHelper(html.ViewContext);
            var ajaxUrl = urlHelper.Action(new UrlActionContext { Action = mi.Name, Controller = controllerName, });
            return new DataTableConfigVm(id, ajaxUrl, columns);
        }

        public static DataTableConfigVm DataTableVm(this IHtmlHelper html, Type t, string id, string uri)
        {
            return new DataTableConfigVm(id, uri.ToString(), t.ColDefs());
        }
        public static DataTableConfigVm DataTableVm<T>(string id, string uri)
        {
            return new DataTableConfigVm(id, uri.ToString(), typeof(T).ColDefs());
        }


        public static DataTableConfigVm DataTableVm<TResult>(this IHtmlHelper html, string id, string uri)
        {
            return DataTableVm(html, typeof (TResult), id, uri);
        }

        public static DataTableConfigVm DataTableVm(this IHtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            return new DataTableConfigVm(id, ajaxUrl, columns.Select(c => new ColDef(c, typeof(string))
            {

            }));
        }

      
    }
}