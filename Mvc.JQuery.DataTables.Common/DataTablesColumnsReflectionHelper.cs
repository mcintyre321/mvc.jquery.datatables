using System;
using System.Collections.Generic;
using System.Linq;
using Mvc.JQuery.DataTables.Models;
using Mvc.JQuery.DataTables.Reflection;

namespace Mvc.JQuery.DataTables
{
    public static class DataTablesColumnsReflectionHelper{
        public static ColDef[] ColDefs (this Type t)
        {
            var propInfos = DataTablesTypeInfo.Properties(t);
            var columnList = new List<ColDef>();
            
            foreach (var dtpi in propInfos)
            {

                var colDef = new ColDef(dtpi.PropertyInfo.Name, dtpi.PropertyInfo.PropertyType);
                foreach (var att in dtpi.Attributes)
                {
                    att.ApplyTo(colDef, dtpi.PropertyInfo);
                }
                
                columnList.Add(colDef);
            }
            return columnList.ToArray();
        }
        public static ColDef[] ColDefs<TResult>()
        {
            return ColDefs(typeof(TResult));
        }

     
    }
}