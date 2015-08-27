using System;
using System.Reflection;
using Mvc.JQuery.Datatables.Models;

namespace Mvc.JQuery.Datatables.Models.Init
{
    public abstract class DataTablesAttributeBase : Attribute
    {
        public abstract void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi);
    }
}