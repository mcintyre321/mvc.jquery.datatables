using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mvc.JQuery.DataTables
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
            var noColumnFilter = null as FilterDef;
            this["bUseColVis"] = _vm.ColVis;
            this["aoColumns"] = _vm.Columns
                //.Where(c => c.Visible || c.Filter["sSelector"] != null)
                .Select(c => c.Searchable && c.Filter != null ? c.Filter : noColumnFilter).ToArray();
            return JsonConvert.SerializeObject(this);
        }
    }
}