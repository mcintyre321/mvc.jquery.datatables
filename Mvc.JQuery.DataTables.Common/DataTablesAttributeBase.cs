using System;
using System.Reflection;
using Mvc.JQuery.DataTables.Models;

namespace Mvc.JQuery.DataTables
{
    public abstract class DataTablesAttributeBase : Attribute
    {
        public abstract void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi);
    }
}