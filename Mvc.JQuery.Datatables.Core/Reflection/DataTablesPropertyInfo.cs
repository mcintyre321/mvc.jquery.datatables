using System;
using System.Reflection;

namespace Mvc.JQuery.DataTables.Reflection
{
    class DataTablesPropertyInfo
    {
        public DataTablesPropertyInfo(System.Reflection.PropertyInfo propertyInfo, DataTablesAttributeBase[] attributeses)
        {
            PropertyInfo = propertyInfo;
            Attributes = attributeses;
        }

        public System.Reflection.PropertyInfo PropertyInfo { get; private set; }
        public DataTablesAttributeBase[] Attributes { get; private set; }
        public Type Type {
            get { return this.PropertyInfo.PropertyType; }
        }
    }
}