using Mvc.JQuery.Datatables.Models;
using Mvc.JQuery.Datatables.Reflection;
using Mvc.JQuery.Datatables.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Mvc.JQuery.Datatables
{
    public abstract class DataTablesResult  : ActionResult
    {
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTransform"></typeparam>
        /// <param name="q">A queryable for the data. The properties of this can be marked up with [DataTablesAttribute] to control sorting/searchability/visibility</param>
        /// <param name="dataTableParam"></param>
        /// <param name="transform">//a transform for custom column rendering e.g. to do a custom date row => new { CreatedDate = row.CreatedDate.ToString("dd MM yy") } </param>
        /// <returns></returns>
        public static DataTablesResult<TSource> Create<TSource, TTransform>(IQueryable<TSource> q, DataTablesParam dataTableParam, 
            Func<TSource, TTransform> transform, ArrayOutputType? arrayOutput = null)
        {
            var result = new DataTablesResult<TSource>(q, dataTableParam);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(row => TransformTypeInfo.MergeTransformValuesIntoDictionary(transform, row))
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, arrayOutput);
                
            return result;
        }

        public static DataTablesResult<TSource> Create<TSource>(IQueryable<TSource> q, DataTablesParam dataTableParam, 
            ArrayOutputType? arrayOutput = null)
        {
            var result = new DataTablesResult<TSource>(q, dataTableParam);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(DataTablesTypeInfo<TSource>.ToDictionary)
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, arrayOutput);

            return result;
        }

        private static DataTablesData ApplyOutputRules(DataTablesData sourceData, ArrayOutputType? arrayOutput = null)
        {
            DataTablesData outputData = sourceData;

            switch (arrayOutput)
            {
                case ArrayOutputType.ArrayOfObjects:
                    // Nothing is needed
                    break;
                case ArrayOutputType.BiDimensionalArray:
                default:
                    outputData = sourceData.Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray());
                    break;
            }

            return outputData;
        }

        /// <param name="transform">Should be a Func<T, TTransform></param>
        public static DataTablesResult Create(IQueryable queryable, DataTablesParam dataTableParam, object transform, 
            ArrayOutputType? arrayOutput = null)
        {
            var s = "Create";
            var openCreateMethod = typeof(DataTablesResult).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 2);
            var queryableType = queryable.GetType().GetGenericArguments()[0];
            var transformType = transform.GetType().GetGenericArguments()[1];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType, transformType);
            return (DataTablesResult)closedCreateMethod.Invoke(null, new object[] { queryable, dataTableParam, transform, arrayOutput });
        }

        public static DataTablesResult Create(IQueryable queryable, DataTablesParam dataTableParam, 
            ArrayOutputType? arrayOutput = null)
        {
            var s = "Create";
            var openCreateMethod = typeof(DataTablesResult).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
            var queryableType = queryable.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
            return (DataTablesResult)closedCreateMethod.Invoke(null, new object[] { queryable, dataTableParam, arrayOutput });
        }

        public static DataTablesResult<T> CreateResultUsingEnumerable<T>(IEnumerable<T> q, DataTablesParam dataTableParam, 
            ArrayOutputType? arrayOutput = null)
        {
            return Create(q.AsQueryable(), dataTableParam, arrayOutput);
        }
    }

    public class DataTablesResult<TSource> : DataTablesResult
    {
        public DataTablesData Data { get; set; }

        internal DataTablesResult(IQueryable<TSource> q, DataTablesParam dataTableParam)
        {
            this.Data = GetResults(q, dataTableParam);
        }
        internal DataTablesResult(DataTablesData data)
        {
            this.Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            var scriptSerializer = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            };

            response.Write(scriptSerializer.Serialize(this.Data));
        }

        DataTablesData GetResults(IQueryable<TSource> data, DataTablesParam param)
        {
            var totalRecords = data.Count(); // annoying this, as it causes an extra evaluation..

            var filters = new DataTablesFiltering();

            var outputProperties = DataTablesTypeInfo<TSource>.Properties;

            var filteredData = filters.ApplyFiltersAndSort(param, data, outputProperties);
            var totalDisplayRecords = filteredData.Count();

            var skipped = filteredData.Skip(param.iDisplayStart);
            var page = (param.iDisplayLength <= 0 ? skipped : skipped.Take(param.iDisplayLength)).ToArray();
            
            var result = new DataTablesData
            {
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayRecords,
                sEcho = param.sEcho,
                aaData = page.Cast<object>().ToArray(),
            };

            return result;
        }
    }
}