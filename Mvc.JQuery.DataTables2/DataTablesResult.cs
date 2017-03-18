using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.JQuery.DataTables;
using Mvc.JQuery.DataTables.Models;
using Mvc.JQuery.DataTables.Reflection;
using Mvc.JQuery.DataTables.Util;
using Newtonsoft.Json;

namespace Mvc.JQuery.DataTables
{
    public abstract class DataTablesResult : ActionResult
    {
        /// <typeparam name="TSource"></typeparam>
        /// <param name="q">A queryable for the data. The properties of this can be marked up with [DataTablesAttribute] to control sorting/searchability/visibility</param>
        /// <param name="dataTableParam"></param>
        /// <param name="transform">//a transform for custom column rendering e.g. to do a custom date row => new { CreatedDate = row.CreatedDate.ToString("dd MM yy") } </param>
        /// <param name="responseOptions"></param>
        /// <returns></returns>
        public static DataTablesResult<TSource> Create<TSource>(IQueryable<TSource> q, DataTablesParam dataTableParam,
            Func<TSource, object> transform, ResponseOptions<TSource> responseOptions = null)
        {
            transform = transform ?? (s => s);
            var result = new DataTablesResult<TSource>(q, dataTableParam);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(row => TransformTypeInfo.MergeTransformValuesIntoDictionary(transform, row))
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, responseOptions);

            return result;
        }

        public static DataTablesResult<TSource> Create<TSource>(IQueryable<TSource> q, DataTablesParam dataTableParam,
            ResponseOptions<TSource> responseOptions = null)
        {
            var result = new DataTablesResult<TSource>(q, dataTableParam);

            var dictionaryTransform = DataTablesTypeInfo<TSource>.ToDictionary(responseOptions);
            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(dictionaryTransform)
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRules(result.Data, responseOptions);

            return result;
        }

        private static DataTablesResponseData ApplyOutputRules<TSource>(DataTablesResponseData sourceData, ResponseOptions<TSource> responseOptions)
        {
            responseOptions = responseOptions ?? new ResponseOptions<TSource>() { ArrayOutputType = ArrayOutputType.BiDimensionalArray };
            DataTablesResponseData outputData = sourceData;

            switch (responseOptions.ArrayOutputType)
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
            ResponseOptions responseOptions = null)
        {
            var s = "Create";
            var openCreateMethod = typeof(DataTablesResult).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
            var queryableType = queryable.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType, typeof(object));
            return (DataTablesResult)closedCreateMethod.Invoke(null, new object[] { queryable, dataTableParam, transform, responseOptions });
        }
    }

    public class DataTablesResult<TSource> : DataTablesResult
    {
        public DataTablesResponseData Data { get; set; }

        public DataTablesResult(IQueryable<TSource> q, DataTablesParam dataTableParam)
        {
            this.Data = dataTableParam.GetDataTablesResponse(q);
        }
        public DataTablesResult(DataTablesResponseData data)
        {
            this.Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
 

            response.Write(JsonConvert.SerializeObject(this.Data));
        }

    }
}