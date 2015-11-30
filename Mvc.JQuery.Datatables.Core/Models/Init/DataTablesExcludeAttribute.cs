using System;

namespace Mvc.JQuery.DataTables
{
    /// <summary>
    /// Prevent a public property from being included as a column in a DataTable row model
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataTablesExcludeAttribute : Attribute
    {
        
    }
}