using System.Collections.Generic;
using System.Linq;
using Mvc.JQuery.DataTables.Models;
using Mvc.JQuery.DataTables.Reflection;

namespace Mvc.JQuery.DataTables
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>

    public class DataTablesParam
    {
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public int iColumns { get; set; }
        public string sSearch { get; set; }
        public bool bEscapeRegex { get; set; }
        public int iSortingCols { get; set; }
        public int sEcho { get; set; }
        public List<string> sColumnNames { get; set; }
        public List<bool> bSortable { get; set; }
        public List<bool> bSearchable { get; set; }
        public List<string> sSearchValues { get; set; }
        public List<int> iSortCol { get; set; }
        public List<string> sSortDir { get; set; }
        public List<bool> bEscapeRegexColumns { get; set; }

        public DataTablesParam()
        {
            sColumnNames = new List<string>();
            bSortable = new List<bool>();
            bSearchable = new List<bool>();
            sSearchValues = new List<string>();
            iSortCol = new List<int>();
            sSortDir = new List<string>();
            bEscapeRegexColumns = new List<bool>();
        }

        public DataTablesParam(int iColumns)
        {
            this.iColumns = iColumns;
            sColumnNames = new List<string>(iColumns);
            bSortable = new List<bool>(iColumns);
            bSearchable = new List<bool>(iColumns);
            sSearchValues = new List<string>(iColumns);
            iSortCol = new List<int>(iColumns);
            sSortDir = new List<string>(iColumns);
            bEscapeRegexColumns = new List<bool>(iColumns);
        }

        public DataTablesResponseData GetDataTablesResponse<TSource>(IQueryable<TSource> data)
        {
            var totalRecords = data.Count(); // annoying this, as it causes an extra evaluation..

            var filters = new DataTablesFiltering();

            var outputProperties = DataTablesTypeInfo<TSource>.Properties;

            var filteredData = filters.ApplyFiltersAndSort(this, data, outputProperties);
            var totalDisplayRecords = filteredData.Count();

            var skipped = filteredData.Skip(this.iDisplayStart);
            var page = (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToArray();

            var result = new DataTablesResponseData()
            {
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayRecords,
                sEcho = this.sEcho,
                aaData = page.Cast<object>().ToArray(),
            };

            return result;
        }
    }
    //public enum DataType
    //{
    //    tInt,
    //    tString,
    //    tnone
    //}
}