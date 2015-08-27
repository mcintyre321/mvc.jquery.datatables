using System;
using System.Reflection;
using Mvc.JQuery.Datatables.Models;

namespace Mvc.JQuery.Datatables
{
    public abstract class DataTablesAttributeBase : Attribute
    {
        public abstract void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi);
    }
}