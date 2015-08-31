using System.Linq.Expressions;

namespace Mvc.JQuery.DataTables.DynamicLinq
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}