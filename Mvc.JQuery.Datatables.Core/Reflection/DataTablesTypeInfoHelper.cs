namespace Mvc.JQuery.Datatables.Reflection
{
    internal static class DataTablesTypeInfoHelper
    {
        public static string ToDisplayName(this DataTablesAttribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.DisplayName) || attribute.DisplayNameResourceType == null)
                return attribute.DisplayName;
            var value = ResourceHelper.GetResourceLookup<string>(attribute.DisplayNameResourceType, attribute.DisplayName);
            return value;
        }
    }
}