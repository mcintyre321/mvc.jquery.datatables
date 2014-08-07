using System;

namespace Mvc.JQuery.Datatables
{
    public class ColInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public ColInfo(string name, Type propertyType)
        {
            Name = name;
            Type = propertyType;

        }
    }
}