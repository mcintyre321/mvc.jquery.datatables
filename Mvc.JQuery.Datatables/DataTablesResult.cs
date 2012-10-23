using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.JQuery.Datatables.DynamicLinq;

namespace Mvc.JQuery.Datatables
{
    public class DataTablesResult : JsonResult
    {
        public static DataTablesResult<TRes> Create<T, TRes>(IQueryable<T> q, DataTablesParam dataTableParam, Func<T, TRes> transform)
        {
            return new DataTablesResult<T, TRes>(q, dataTableParam, transform);
        }
        public static DataTablesResult<T> Create<T>(IQueryable<T> q, DataTablesParam dataTableParam)
        {
            return new DataTablesResult<T, T>(q, dataTableParam, t => t);
        }

        public static DataTablesResult<T> CreateResultUsingEnumerable<T>(IEnumerable<T> q, DataTablesParam dataTableParam)
        {
            return new DataTablesResult<T, T>(q.AsQueryable(), dataTableParam, t => t);
        }

        public static DataTablesResult Create(object queryable, DataTablesParam dataTableParam)
        {
            queryable = ((IEnumerable) queryable).AsQueryable();
            var s = "Create";

            var openCreateMethod =
                typeof (DataTablesResult).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
            var queryableType = queryable.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
            return (DataTablesResult) closedCreateMethod.Invoke(null, new[] {queryable, dataTableParam});
        }

		/// <summary>
		/// Creates a DataTables result from a pre-filtered collection of items.
		/// </summary>
		/// <typeparam name="T">Type of items to return</typeparam>
		/// <param name="q">Enumerable collection of pre-filtered items</param>
		/// <param name="dataTableParam">Request parameters</param>
		/// <param name="totalRecords">Total number of records before filtering</param>
		/// <param name="totalDisplayRecords">Total number of records after filtering</param>
		/// <returns>DataTablesResult</returns>
		public static DataTablesResult<T> CreatePreFiltered<T>(IEnumerable<T> q, DataTablesParam dataTableParam, int totalRecords = 0, int totalDisplayRecords = 0) {
			// reset filters
			dataTableParam.sSearch = String.Empty;
			dataTableParam.iSortingCols = 0;
			dataTableParam.iDisplayStart = 0;
			dataTableParam.iDisplayLength = -1;

			var dtr = CreateResultUsingEnumerable(q.AsQueryable(), dataTableParam);
			var dtd = dtr.Data as DataTablesData;

			if (totalRecords > 0)
				dtd.iTotalRecords = totalRecords;
			if (totalDisplayRecords > 0)
				dtd.iTotalDisplayRecords = totalDisplayRecords;

			return dtr;
		}
    }

    public class DataTablesResult<T> : DataTablesResult
    {
        
    }
    public class DataTablesResult<T, TRes> : DataTablesResult<TRes>
    {
        private readonly Func<T, TRes> _transform;

        public DataTablesResult(IQueryable<T> q, DataTablesParam dataTableParam, Func<T, TRes> transform)
        {

            _transform = transform;
            var properties = typeof(TRes).GetProperties();

            var content = GetResults(q, dataTableParam, properties.Select(p => Tuple.Create(p.Name, (string)null, p.PropertyType)).ToArray());
            this.Data = content;
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        static readonly List<PropertyTransformer> PropertyTransformers = new List<PropertyTransformer>()
        {
            Guard<DateTimeOffset>(dateTimeOffset => dateTimeOffset.ToLocalTime().ToString("g")),
            Guard<DateTime>(dateTime => dateTime.ToLocalTime().ToString("g")),
            Guard<IHtmlString>(s => s.ToHtmlString()),
            Guard<object>(o => (o ?? "").ToString())
        };

        public delegate object PropertyTransformer(Type type, object value);
        public delegate object GuardedValueTransformer<TVal>(TVal value);

        static PropertyTransformer Guard<TVal>(GuardedValueTransformer<TVal> transformer)
        {
            return (t, v) =>
            {
                if (!typeof(TVal).IsAssignableFrom(t))
                {
                    return null;
                }
                return transformer((TVal) v);
            };
        }
        public static void RegisterFilter<TVal>(GuardedValueTransformer<TVal> filter)
        {
            PropertyTransformers.Add(Guard<TVal>(filter));
        }
        private DataTablesData GetResults(IQueryable<T> data, DataTablesParam param, Tuple<string, string, Type>[] searchColumns)
        {

            int totalRecords = data.Count();


            int totalRecordsDisplay;

            var filters = new DataTablesFilter();


            var dataArray = data.Select(_transform).AsQueryable();
            dataArray = filters.FilterPagingSortingSearch(param, dataArray, out totalRecordsDisplay, searchColumns).Cast<TRes>();
            
            var type = typeof(TRes);
            var properties = type.GetProperties();

            var toArrayQuery = from i in dataArray
                               let pairs = properties.Select(p => new {p.PropertyType, Value = (p.GetGetMethod().Invoke(i, null))})
                               let values = pairs.Select(p => GetTransformedValue(p.PropertyType, p.Value))
                               select values;

            var result = new DataTablesData
            {
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecordsDisplay,
                sEcho = param.sEcho,
                aaData = toArrayQuery.ToArray()
            };

            return result;
        }

        private object GetTransformedValue(Type propertyType, object value)
        {
            foreach (var transformer in PropertyTransformers)
            {
                var result = transformer(propertyType, value);
                if (result != null) return result;
            }
            return (value as object ?? "").ToString();
        }
    }
}