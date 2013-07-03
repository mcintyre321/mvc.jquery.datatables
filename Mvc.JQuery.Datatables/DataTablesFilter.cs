using System;
using System.Collections.Generic;
using System.Linq;
using Mvc.JQuery.Datatables.DynamicLinq;

namespace Mvc.JQuery.Datatables
{
    public class DataTablesFilter
    {
        public IQueryable FilterPagingSortingSearch(DataTablesParam dtParameters, IQueryable data, ColInfo[] columns)
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
                        if (string.IsNullOrWhiteSpace(filterClause) == false)
                        {
                            data = data.Where(filterClause, parameters.ToArray());
                        }
                    }
                }
            }
            string sortString = "";
            for (int i = 0; i < dtParameters.iSortingCols; i++)
            {

                int columnNumber = dtParameters.iSortCol[i];
                string columnName = columns[columnNumber].Name;
                string sortDir = dtParameters.sSortDir[i];
                if (i != 0)
                    sortString += ", ";
                sortString += columnName + " " + sortDir;
            }
            if (string.IsNullOrWhiteSpace(sortString))
            {
                sortString = columns[0].Name;
            }
            data = data.OrderBy(sortString);


            return data;
        }

        public delegate string ReturnedFilteredQueryForType(string query, string columnName, Type columnType, List<object> parametersForLinqQuery);



        static readonly List<ReturnedFilteredQueryForType> Filters = new List<ReturnedFilteredQueryForType>()
        {
            Guard(IsBoolType, TypeFilters.BoolFilter),
            Guard(IsDateTimeType, TypeFilters.DateTimeFilter),
            Guard(IsDateTimeOffsetType, TypeFilters.DateTimeOffsetFilter),
            Guard(IsNumericType, TypeFilters.NumericFilter),
            Guard(arg => arg == typeof (string), TypeFilters.StringFilter),
        };


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
            Filters.Add(Guard(arg => arg is T, filter));
        }

        private static string GetFilterClause(string query, ColInfo column, List<object> parametersForLinqQuery)
        {
            foreach (var filter in Filters)
            {
                var filteredQuery = filter(query, column.Name, column.Type, parametersForLinqQuery);
                if (filteredQuery != null)
                {
                    return filteredQuery;
                }
            }
            var parts = query.Split('~').SelectMany(s => s.Split('|'))
                .Select(q => string.Format("({1} == null ? \"\" : {1}.ToString()).{0}", TypeFilters.FilterMethod(q, parametersForLinqQuery, column.Type), column.Name));
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
        public static bool IsBoolType(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }
        public static bool IsDateTimeType(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }
        public static bool IsDateTimeOffsetType(Type type)
        {
            return  type == typeof(DateTimeOffset) ||   type == typeof(DateTimeOffset?);
        }

    }
}