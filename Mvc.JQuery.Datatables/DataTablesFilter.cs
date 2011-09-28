using System;
using System.Linq;
using Mvc.JQuery.Datatables.DynamicLinq;

namespace Mvc.JQuery.Datatables
{
    public class DataTablesFilter
    {
        public IQueryable FilterPagingSortingSearch(DataTablesParam DTParams, IQueryable data, out int totalRecordsDisplay, Tuple<string, Type>[] columns)
        {

            if (!String.IsNullOrEmpty(DTParams.sSearch))
            {
                string searchString = "";
                bool first = true;
                for (int i = 0; i < DTParams.iColumns; i++)
                {
                    if (DTParams.bSearchable[i])
                    {
                        string columnName = columns[i].Item1;

                        if (!first)
                            searchString += " or ";
                        else
                            first = false;

                        if (columns[i].Item2 == typeof(int))
                        {
                            searchString += columnName + ".ToString().StartsWith(\"" + DTParams.sSearch + "\")";
                        }
                        else
                        {
                            searchString += columnName + ".Contains(\"" + DTParams.sSearch + "\")";
                        }
                    }
                }
                data = data.Where(searchString);
            }
            string sortString = "";
            for (int i = 0; i < DTParams.iSortingCols; i++)
            {
                
                int columnNumber = DTParams.iSortCol[i];
                string columnName = columns[columnNumber].Item1;
                string sortDir = DTParams.sSortDir[i];
                if (i != 0)
                    sortString += ", ";
                sortString += columnName + " " + sortDir;
            }

            totalRecordsDisplay = data.Count();

            data = data.OrderBy(sortString);
            data = data.Skip(DTParams.iDisplayStart).Take(DTParams.iDisplayLength);

            return data;
        }
    }
}