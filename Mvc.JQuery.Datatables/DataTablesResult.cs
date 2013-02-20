using Mvc.JQuery.Datatables.DynamicLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

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

        public static DataTablesResult Create(object queryable, DataTablesParam dataTableParam)
        {
            queryable = ((IEnumerable)queryable).AsQueryable();
            var s = "Create";

            var openCreateMethod =
                typeof(DataTablesResult).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
            var queryableType = queryable.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
            return (DataTablesResult)closedCreateMethod.Invoke(null, new[] { queryable, dataTableParam });
        }

        public static DataTablesResult<T> CreateResultUsingEnumerable<T>(IEnumerable<T> q, DataTablesParam dataTableParam)
        {
            return new DataTablesResult<T, T>(q.AsQueryable(), dataTableParam, t => t);
        }
    }

    public class DataTablesResult<T> : DataTablesResult
    {
    }

    public class DataTablesResult<T, TRes> : DataTablesResult<TRes>
    {
        private static readonly List<PropertyTransformer> PropertyTransformers = new List<PropertyTransformer>()
        {
            Guard<DateTimeOffset>(dateTimeOffset => dateTimeOffset.ToLocalTime().ToString("g")),
            Guard<DateTime>(dateTime => dateTime.ToLocalTime().ToString("g")),
            Guard<IHtmlString>(s => s.ToHtmlString()),
            Guard<object>(o => (o ?? "").ToString())
        };

        private readonly Func<T, TRes> _transform;

        public DataTablesResult(IQueryable<T> q, DataTablesParam dataTableParam, Func<T, TRes> transform)
        {
            _transform = transform;

            //var properties = typeof(TRes).GetProperties();
            var properties = TypeExtensions.GetSortedProperties<TRes>();

            var content = GetResults(q, dataTableParam, properties.Select(p => Tuple.Create(p.Name, (string)null, p.PropertyType)).ToArray());
            this.Data = content;
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public delegate object GuardedValueTransformer<TVal>(TVal value);

        public delegate object PropertyTransformer(Type type, object value);

        public static void RegisterFilter<TVal>(GuardedValueTransformer<TVal> filter)
        {
            PropertyTransformers.Add(Guard<TVal>(filter));
        }

        private static PropertyTransformer Guard<TVal>(GuardedValueTransformer<TVal> transformer)
        {
            return (t, v) =>
            {
                if (!typeof(TVal).IsAssignableFrom(t))
                {
                    return null;
                }
                return transformer((TVal)v);
            };
        }

        private DataTablesData GetResults(IQueryable<T> data, DataTablesParam param, Tuple<string, string, Type>[] searchColumns)
        {
            int totalRecords = data.Count(); //annoying this, as it causes an extra evaluation..

            var filters = new DataTablesFilter();

            var filteredData = data.Select(_transform).AsQueryable();
            filteredData = filters.FilterPagingSortingSearch(param, filteredData, searchColumns).Cast<TRes>();

            var page = filteredData.Skip(param.iDisplayStart);
            if (param.iDisplayLength > -1)
            {
                page = page.Take(param.iDisplayLength);
            }

            //var type = typeof(TRes);
            //var propertiesOriginal = type.GetProperties();

            var properties = TypeExtensions.GetSortedProperties<TRes>();

            var transformedPage = from i in page
                                  let pairs = properties.Select(p => new { p.PropertyType, Value = (p.GetGetMethod().Invoke(i, null)) })
                                  let values = pairs.Select(p => GetTransformedValue(p.PropertyType, p.Value))
                                  select values;

            var result = new DataTablesData
            {
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = filteredData.Count(),
                sEcho = param.sEcho,
                aaData = transformedPage.ToArray()
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