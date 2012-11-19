using System;
using System.Collections.Generic;
using System.Linq;
using Mvc.JQuery.Datatables.DynamicLinq;

namespace Mvc.JQuery.Datatables
{
    public class DataTablesFilter
    {
        public IQueryable FilterPagingSortingSearch(DataTablesParam dtParameters, IQueryable data, Tuple<string, string, Type>[] columns)
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
                        var filterClause = GetFilterClause(dtParameters.sSearchColumns[i], columns[i], parameters);
                        data = data.Where(filterClause, parameters.ToArray());
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

            data = data.OrderBy(sortString);
           

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
            Guard(IsDateType, DateFilter),
            Guard(IsNumericType, NumericFilter)

        };

        private static bool Is<T>(Type arg)
        {
            return arg is T;
        }

        private static string NumericFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                var clause = null as string;
                try
                {
                    parametersForLinqQuery.Add(Convert.ChangeType(parts[0], columnType));
                    clause = string.Format("{0} >= @{1}", columnname, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }

                try
                {
                    parametersForLinqQuery.Add(Convert.ChangeType(parts[1], columnType));
                    if (clause != null) clause += " and ";
                    clause += string.Format("{0} <= @{1}", columnname, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }
                return clause ?? "true" ;

            }
            else
            {
                return string.Format("({1} == null ? \"\" : {1}.ToString()).{0}", FilterMethod(query), columnname);
            }
        }

        private static string DateFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
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

        public delegate string GuardedFilter(string query, string columnName, Type columnType, List<object> parametersForLinqQuery);

        static ReturnedFilteredQueryForType Guard(Func<Type, bool> guard, GuardedFilter filter)
        {
            return (q, c, t, p) =>
            {
                if (!guard(t))
                {
                    return null;
                }
                return filter(q, c, t, p);
            };
        }

        public static void RegisterFilter<T>(GuardedFilter filter)
        {
            Filters.Add(Guard(Is<T>, filter));
        }

        private static string GetFilterClause(string query, Tuple<string, string, Type> column, List<object> parametersForLinqQuery)
        {
            foreach (var filter in Filters)
            {
                var filteredQuery = filter(query, column.Item1, column.Item3, parametersForLinqQuery);
                if (filteredQuery != null)
                {
                    return filteredQuery;
                }
            }
            var parts = query.Split('~').SelectMany(s => s.Split('|')).Select(q => string.Format("({1} == null ? \"\" : {1}.ToString()).{0}", FilterMethod(q), column.Item1));
            return "(" + string.Join(") OR (", parts) + ")";
        }

        public static bool IsNumericType(Type type)
        {
            if (type == null || type.IsEnum)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;

        }
        public static bool IsDateType(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateTime?) || type == typeof(DateTimeOffset?);
        }

    }
}