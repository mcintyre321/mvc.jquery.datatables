using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Mvc.JQuery.DataTables
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>
    public class DataTablesModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProvider = bindingContext.ValueProvider;
            int columns = GetValue<int>(valueProvider, "iColumns");
            //TODO: Consider whether this should be pushed to a worker thread...
            if (columns == 0)
            {
                var bindV10Model = BindV10Model(valueProvider);
                bindingContext.Result = ModelBindingResult.Success(bindV10Model);
                return Task.FromResult(bindV10Model);
            }
            else
            {
                var bindLegacyModel = BindLegacyModel(valueProvider, columns);
                bindingContext.Result = ModelBindingResult.Success(bindLegacyModel);
                return Task.FromResult(bindLegacyModel);
            }
        }

        private object BindV10Model(IValueProvider valueProvider)
        {
            DataTablesParam obj = new DataTablesParam();
            obj.iDisplayStart = GetValue<int>(valueProvider, "start");
            obj.iDisplayLength = GetValue<int>(valueProvider, "length");
            obj.sSearch = GetValue<string>(valueProvider, "search[value]");
            obj.bEscapeRegex = GetValue<bool>(valueProvider, "search[regex]");
            obj.sEcho = GetValue<int>(valueProvider, "draw");

            int colIdx = 0;
            while (true)
            {
                string colPrefix = String.Format("columns[{0}]", colIdx);
                string colName = GetValue<string>(valueProvider, colPrefix+"[data]");
                if (String.IsNullOrWhiteSpace(colName)) {
                    break;
                }
                obj.sColumnNames.Add(colName);
                obj.bSortable.Add(GetValue<bool>(valueProvider, colPrefix+"[orderable]"));
                obj.bSearchable.Add(GetValue<bool>(valueProvider, colPrefix+"[searchable]"));
                obj.sSearchValues.Add(GetValue<string>(valueProvider, colPrefix+"[search][value]"));
                obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, colPrefix+"[searchable][regex]"));
                colIdx++;
            }
            obj.iColumns = colIdx;
            colIdx = 0;
            while (true)
            {
                string colPrefix = String.Format("order[{0}]", colIdx);
                int? orderColumn = GetValue<int?>(valueProvider, colPrefix+"[column]");
                if (orderColumn.HasValue)
                {
                    obj.iSortCol.Add(orderColumn.Value);
                    obj.sSortDir.Add(GetValue<string>(valueProvider, colPrefix+"[dir]"));
                    colIdx++;
                }
                else
                {
                    break;
                }
            }
            obj.iSortingCols = colIdx;
            return obj;
        }

        private DataTablesParam BindLegacyModel(IValueProvider valueProvider, int columns)
        {
            DataTablesParam obj = new DataTablesParam(columns);

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
                obj.sSearchValues.Add(GetValue<string>(valueProvider, "sSearch_" + i));
                obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, "bEscapeRegex_" + i));
                obj.iSortCol.Add(GetValue<int>(valueProvider, "iSortCol_" + i));
                obj.sSortDir.Add(GetValue<string>(valueProvider, "sSortDir_" + i));
            }
            return obj;
        }

        private static T GetValue<T>(IValueProvider valueProvider, string key)
        {
            ValueProviderResult valueResult = valueProvider.GetValue(key);
            return (valueResult == null)
                ? default(T)
                : ConvertTo<T>(valueResult);
        }

        // <summary>
        /// Attempts to convert the values in <paramref name="result"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> for conversion.</typeparam>
        /// <param name="result">The <see cref="ValueProviderResult"/>.</param>
        /// <returns>
        /// The converted value, or the default value of <typeparamref name="T"/> if the value could not be converted.
        /// </returns>
        private static T ConvertTo<T>(ValueProviderResult result)
        {
            object valueToConvert = null;
            if (result.Values.Count == 1)
            {
                valueToConvert = result.Values[0];
            }
            else if (result.Values.Count > 1)
            {
                valueToConvert = result.Values.ToArray();
            }
            return ModelBindingHelper.ConvertTo<T>(valueToConvert, result.Culture);
        }
    }

    public class DataTablesModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(DataTablesParam)) // only encode string types
            {
                return new DataTablesModelBinder();
            }

            return null;
        }
    }
}