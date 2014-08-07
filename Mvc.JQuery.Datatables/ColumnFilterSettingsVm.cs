using System.Collections;
using System.Linq;
using System.Web.Script.Serialization;

namespace Mvc.JQuery.Datatables
{
    public class ColumnFilterSettingsVm : Hashtable
    {
        private readonly DataTableConfigVm _vm;

        public ColumnFilterSettingsVm(DataTableConfigVm vm)
        {
            _vm = vm;
            this["bUseColVis"] = true;
            this["sPlaceHolder"] = "head:after";
        }

        public override string ToString()
        {
            var noColumnFilter = new FilterDef(null);
            this["aoColumns"] = _vm.Columns
                //.Where(c => c.Visible || c.Filter["sSelector"] != null)
                .Select(c => c.Searchable?c.Filter:noColumnFilter).ToArray();
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}