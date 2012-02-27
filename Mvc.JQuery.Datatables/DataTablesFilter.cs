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
                var parameters = new List<object>();
                for (var i = 0; i < dtParameters.iColumns; i++)
                {
                    if (dtParameters.bSearchable[i])
                    {
                        parts.Add(GetFilterClause(dtParameters.sSearch, columns[i], parameters));
                    }
                }
                data = data.Where(string.Join(" or ", parts), parameters.ToArray());
            }
            for (int i = 0; i < dtParameters.sSearchColumns.Count; i++)
            {
                if (dtParameters.bSearchable[i])
                {
                    var searchColumn = dtParameters.sSearchColumns[i];
                    if (!string.IsNullOrWhiteSpace(searchColumn))
                    {
                        var parameters = new List<object>();
                        data = data.Where(GetFilterClause(dtParameters.sSearchColumns[i], columns[i], parameters), parameters.ToArray());
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
            data = data.Skip(dtParameters.iDisplayStart);
            if (dtParameters.iDisplayLength > -1)
            {
                data = data.Take(dtParameters.iDisplayLength);
            }

            return data;
        }

        public delegate string ReturnedFilteredQueryForType(string query, string columnName, Type columnType, List<object> parametersForLinqQuery);

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
            Guard<DateTimeOffset>(DateFilter),
        };

        private static string DateFilter(string query, string columnname, List<object> parametersForLinqQuery)
        {
            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                DateTimeOffset start, end;
                DateTimeOffset.TryParse(parts[0] ?? "", out start);
                if (!DateTimeOffset.TryParse(parts[1] ?? "", out end)) end = DateTimeOffset.MaxValue;

                parametersForLinqQuery.Add(start);
                parametersForLinqQuery.Add(end);
                return string.Format("{0}.Ticks >= @{1}.Ticks and {0}.Ticks <= @{2}.Ticks", columnname, parametersForLinqQuery.Count - 2, parametersForLinqQuery.Count - 1);
            }
            else
            {
                return string.Format("{1}.ToLocalTime().ToString(\"g\").{0}", FilterMethod(query), columnname);
            }
        }

        public delegate string GuardedFilter(string query, string columnName, List<object> parametersForLinqQuery);
        static ReturnedFilteredQueryForType Guard<T>(GuardedFilter filter)
        {
            return (q, c, t, p) =>
            {
                if (typeof(T) != t)
                {
                    return null;
                }
                return filter(q, c, p);
            };
        }
        public static void RegisterFilter<T>(GuardedFilter filter)
        {
            Filters.Add(Guard<T>(filter));
        }

        private static string GetFilterClause(string query, Tuple<string, Type> column, List<object> parametersForLinqQuery)
        {
            foreach (var filter in Filters)
            {
                var filteredQuery = filter(query, column.Item1, column.Item2, parametersForLinqQuery);
                if (filteredQuery != null)
                {
                    return filteredQuery;
                }
            }
            return string.Format("{1}.ToString().{0}", FilterMethod(query), column.Item1);
        }
    }
}