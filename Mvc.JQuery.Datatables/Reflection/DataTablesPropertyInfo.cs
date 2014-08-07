using System.Reflection;

namespace Mvc.JQuery.Datatables.Reflection
{
    class DataTablesPropertyInfo
    {
        public DataTablesPropertyInfo(PropertyInfo propertyInfo, DataTablesAttributeBase[] attributeses)
        {
            PropertyInfo = propertyInfo;
            Attributes = attributeses;
        }

        public PropertyInfo PropertyInfo { get; private set; }
        public DataTablesAttributeBase[] Attributes { get; private set; }

    }
}