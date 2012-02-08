using System.Collections.Generic;

namespace Mvc.JQuery.Datatables
{
    public class DataTableVm
    {
        public DataTableVm(string id, string ajaxUrl, IEnumerable<string> columns)
        {
            AjaxUrl = ajaxUrl;
            this.Id = id;
            this.Columns = columns;
        }

        public string Id { get; private set; }

        public string AjaxUrl { get; private set; }

        public IEnumerable<string> Columns { get; private set; }

        public bool ColumnFilter { get; set; }
    }
}