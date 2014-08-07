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
using Mvc.JQuery.Datatables.Models;
using Mvc.JQuery.Datatables.Reflection;

namespace Mvc.JQuery.Datatables
{
    public static class DataTablesHelper
    {
        

        public static DataTableConfigVm DataTableVm<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, DataTablesResult<TResult>>> exp, IEnumerable<ColDef> columns = null)
        {
            if (columns == null || !columns.Any())
            {
                columns = ColDefs<TResult>();
            }

            var mi = exp.MethodInfo();
            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var ajaxUrl = urlHelper.Action(mi.Name, controllerName);
            return new DataTableConfigVm(id, ajaxUrl, columns);
        }

        public static DataTableConfigVm DataTableVm(this HtmlHelper html, Type t, string id, Uri uri)
        {
            return new DataTableConfigVm(id, uri.ToString(), ColDefs(t));
        }

        public static DataTableConfigVm DataTableVm<TResult>(this HtmlHelper html, string id, Uri uri)
        {
            return DataTableVm(html, typeof (TResult), id, uri);
        }

        public static ColDef[] ColDefs (this Type t)
        {
            var propInfos = DataTablesTypeInfo.Properties(t);
            var columnList = new List<ColDef>();
            
            foreach (var dtpi in propInfos)
            {

                var colDef = new ColDef(dtpi.PropertyInfo.Name, dtpi.PropertyInfo.PropertyType);
                foreach (var att in dtpi.Attributes)
                {
                    att.ApplyTo(colDef, dtpi.PropertyInfo);
                }
                
                columnList.Add(colDef);
            }
            return columnList.ToArray();
        }
        public static ColDef[] ColDefs<TResult>()
        {
            return ColDefs(typeof(TResult));
        }

        public static DataTableConfigVm DataTableVm(this HtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            return new DataTableConfigVm(id, ajaxUrl, columns.Select(c => ColDef.Create(c, (string)null, typeof(string))));
        }
    }
}