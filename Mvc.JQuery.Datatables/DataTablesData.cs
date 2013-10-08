
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.JQuery.Datatables
{
    public class DataTablesData
    {
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public int sEcho { get; set; }
        public object[] aaData { get; set; }

        public DataTablesData Transform<TData, TTransform>(Func<TData, TTransform> transformRow)
        {
            var data = new DataTablesData 
            {
                aaData = aaData.Cast<TData>().Select(transformRow).Cast<object>().ToArray(),
                iTotalDisplayRecords = iTotalDisplayRecords,
                iTotalRecords = iTotalRecords,
                sEcho = sEcho
            };
            return data;
        }
    }

}