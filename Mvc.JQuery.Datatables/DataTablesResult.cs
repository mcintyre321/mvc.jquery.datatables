using System;
using System.Linq;
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
        public static DataTablesResult Create(object queryable, DataTablesParam dataTableParam)
        {
            try
            {
                var openCreateMethod =
                    typeof(DataTablesResult).GetMethods().Single(x => x.Name == "Create" && x.GetGenericArguments().Count() == 1);
                var queryableType = queryable.GetType().GetGenericArguments()[0];
                var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
                return (DataTablesResult)closedCreateMethod.Invoke(null, new[] { queryable });
            }
            catch (Exception ex)
            {
                throw new Exception("Was the object passed in a Something<T>?", ex);
            }
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
            var properties = typeof(T).GetProperties();

            var content = GetResults(q, dataTableParam, properties.Select(p => Tuple.Create(p.Name, p.PropertyType)).ToArray());
            this.Data = content;
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }


        private DataTablesData GetResults(IQueryable q, DataTablesParam param, Tuple<string, Type>[] searchColumns)
        {

            int totalRecords = q.Count();

            var data = q;

            int totalRecordsDisplay;

            DataTablesFilter filters = new DataTablesFilter();


            data = filters.FilterPagingSortingSearch(param, data, out totalRecordsDisplay, searchColumns);
            var dataArray = data.Cast<T>().ToArray().AsQueryable().Select(_transform).Cast<object>();
            var type = typeof(TRes);
            var properties = type.GetProperties();

            var toArrayQuery = from i in dataArray
                               let values =
                                   properties.Select(p => (p.GetGetMethod().Invoke(i, null) ?? "").ToString())
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
    }
 

}