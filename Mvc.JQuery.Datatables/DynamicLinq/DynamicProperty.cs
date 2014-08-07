using System;
using System.Reflection.Emit;

namespace Mvc.JQuery.Datatables.DynamicLinq
{
    public class DynamicProperty
    {
        string name;
        Type type;

        public DynamicProperty(string name, Type type, CustomAttributeBuilder attributeBuilder = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");
            this.name = name;
            this.type = type;
            this.CustomAttributeBuilder = attributeBuilder;

        }

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return type; }
        }

        public CustomAttributeBuilder CustomAttributeBuilder
        {
            get;
            private set;
        }

    }
}