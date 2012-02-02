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
                bool first = true;
                List<string> parts = new List<string>();
                for (int i = 0; i < dtParameters.iColumns; i++)
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

        private static string GetFilterClause(string query, Tuple<string, Type> column)
        {
            if (column.Item2 == typeof (int))
            {
                return (column.Item1 + ".ToString().StartsWith(\"" + query + "\")");
            }
            else
            {
                return (column.Item1 + ".Contains(\"" + query + "\")");
            }
        }
    }
}