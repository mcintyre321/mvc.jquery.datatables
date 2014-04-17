using System;
using System.Web.Mvc;

namespace Mvc.JQuery.Datatables
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>
    public class DataTablesModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProvider = bindingContext.ValueProvider;

            DataTablesParam obj = new DataTablesParam(GetValue<int>(valueProvider, "iColumns"));

            obj.iDisplayStart = GetValue<int>(valueProvider, "iDisplayStart");
            obj.iDisplayLength = GetValue<int>(valueProvider, "iDisplayLength");
            obj.sSearch = GetValue<string>(valueProvider, "sSearch");
            obj.bEscapeRegex = GetValue<bool>(valueProvider, "bEscapeRegex");
            obj.iSortingCols = GetValue<int>(valueProvider, "iSortingCols");
            obj.sEcho = GetValue<int>(valueProvider, "sEcho");
            
            for (int i = 0; i < obj.iColumns; i++)
            {
                obj.bSortable.Add(GetValue<bool>(valueProvider, "bSortable_" + i));
                obj.bSearchable.Add(GetValue<bool>(valueProvider, "bSearchable_" + i));
                obj.sSearchColumns.Add(GetValue<string>(valueProvider, "sSearch_" + i));
                obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, "bEscapeRegex_" + i));
                obj.iSortCol.Add(GetValue<int>(valueProvider, "iSortCol_" + i));
                obj.sSortDir.Add(GetValue<string>(valueProvider, "sSortDir_" + i));
            }
            return obj;            
        }

        private static T GetValue<T>(IValueProvider valueProvider, string key)
        {
            ValueProviderResult valueResult = valueProvider.GetValue(key);
            return (valueResult==null)
                ? default(T)
                : (T)valueResult.ConvertTo(typeof(T));
        }
    }
}