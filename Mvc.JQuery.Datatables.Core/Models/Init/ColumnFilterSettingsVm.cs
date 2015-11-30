using System.Collections;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace Mvc.JQuery.DataTables.Models.Init
{
    public class ColumnFilterSettingsVm : Hashtable
    {
        private readonly DataTableConfigVm _vm;

        public ColumnFilterSettingsVm(DataTableConfigVm vm)
        {
            _vm = vm;
            
            this["sPlaceHolder"] = "head:after";
        }

        public JObject columnBuilders = new JObject();

        public override string ToString()
        {
            var noColumnFilter = new FilterDef(null);
            this["bUseColVis"] = _vm.ColVis;
            this["aoColumns"] = _vm.Columns
                //.Where(c => c.Visible || c.Filter["sSelector"] != null)
                .Select(c => c.Searchable?c.Filter:noColumnFilter).ToArray();
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}