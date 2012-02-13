using System;
using System.Collections.Generic;
using System.Linq;
using Mvc.JQuery.Datatables.DynamicLinq;

namespace Mvc.JQuery.Datatables
{
    public class DataTablesFilter
    {
        public IQueryable FilterPagingSortingSearch(DataTablesParam dtParameters, IQueryable data, out int totalRecordsDisplay, Tuple<string, Type>[] columns)
        {
            if (!String.IsNullOrEmpty(dtParameters.sSearch))
            {
                var parts = new List<string>();
                for (var i = 0; i < dtParameters.iColumns; i++)
                {
                    if (dtParameters.bSearchable[i])
                    {
                        parts.Add(GetFilterClause(dtParameters.sSearch, columns[i]));
                    }
                }
                data = data.Where(string.Join(" or ", parts));
            }
            for (int i = 0; i < dtParameters.sSearchColumns.Count; i++)
            {
                if (dtParameters.bSearchable[i])
                {
                    var searchColumn = dtParameters.sSearchColumns[i];
                    if (!string.IsNullOrWhiteSpace(searchColumn))
                    {
                        data = data.Where(GetFilterClause(dtParameters.sSearchColumns[i], columns[i]));
                    }
                }
            }
            string sortString = "";
            for (int i = 0; i < dtParameters.iSortingCols; i++)
            {

                int columnNumber = dtParameters.iSortCol[i];
                string columnName = columns[columnNumber].Item1;
                string sortDir = dtParameters.sSortDir[i];
                if (i != 0)
                    sortString += ", ";
                sortString += columnName + " " + sortDir;
            }

            totalRecordsDisplay = data.Count();

            data = data.OrderBy(sortString);
            data = data.Skip(dtParameters.iDisplayStart).Take(dtParameters.iDisplayLength);

            return data;
        }

        public delegate string ReturnedFilteredQueryForType(string query, string columnName, Type columnType);

        static string FilterMethod(string q)
        {
            if (q.StartsWith("^"))
            {
                return "ToLower().StartsWith(\"" + q.ToLower().Substring(1).Replace("\"", "\"\"") + "\")";
            }
            else
            {
                return "ToLower().Contains(\"" + q.ToLower().Replace("\"", "\"\"") + "\")";
            }
        }

        static readonly List<ReturnedFilteredQueryForType> Filters = new List<ReturnedFilteredQueryForType>()
        {
            Guard<DateTimeOffset>((q, c) => string.Format("{1}.ToString(\"g\").{0}", FilterMethod(q), c)),
        };
        public delegate string GuardedFilter(string query, string columnName);
        static ReturnedFilteredQueryForType Guard<T>(GuardedFilter filter)
        {
            return (q, c, t) =>
            {
                if (typeof(T) != t)
                {
                    return null;
                }
                return filter(q, c);
            };
        }
        public static void RegisterFilter<T>(GuardedFilter filter)
        {
            Filters.Add(Guard<T>(filter));
        }

        private static string GetFilterClause(string query, Tuple<string, Type> column)
        {
            foreach (var filter in Filters)
            {
                var filteredQuery = filter(query, column.Item1, column.Item2);
                if (filteredQuery != null)
                {
                    return filteredQuery;
                }
            }
            return string.Format("{1}.ToString().{0}", FilterMethod(query), column.Item1);
        }
    }
}