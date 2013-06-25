using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mvc.JQuery.Datatables
{
    static class TypeFilters
    {
        internal static string FilterMethod(string q)
        {
            if (q.StartsWith("^"))
            {
                return "ToLower().StartsWith(\"" + q.ToLower().Substring(1).Replace("\"", "\"\"") + "\")";
            }
            else
            {
                return "ToLower().Contains(\"" + q.ToLower().Replace("\"", "\"\"") + "\")";
            }
        }
        public static string NumericFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                var clause = null as string;
                try
                {
                    parametersForLinqQuery.Add(Convert.ChangeType(parts[0], columnType));
                    clause = string.Format("{0} >= @{1}", columnname, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }

                try
                {
                    parametersForLinqQuery.Add(Convert.ChangeType(parts[1], columnType));
                    if (clause != null) clause += " and ";
                    clause += string.Format("{0} <= @{1}", columnname, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }
                return clause ?? "true";

            }
            else
            {
                try
                {
                    parametersForLinqQuery.Add(Convert.ChangeType(query, columnType));
                    return string.Format("{0} == @{1}", columnname, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }
                return "false";
            }
        }

        public static string DateTimeOffsetFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                DateTimeOffset start, end;
                DateTimeOffset.TryParse(parts[0] ?? "", out start);
                if (!DateTimeOffset.TryParse(parts[1] ?? "", out end)) end = DateTimeOffset.MaxValue;

                parametersForLinqQuery.Add(start);
                parametersForLinqQuery.Add(end);
                return string.Format("{0} >= @{1} and {0} <= @{2}", columnname, parametersForLinqQuery.Count - 2, parametersForLinqQuery.Count - 1);
            }
            else
            {
                return string.Format("{1}.ToLocalTime().ToString(\"g\").{0}", FilterMethod(query), columnname);
            }
        }
        public static string DateTimeFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                DateTime start, end;
                DateTime.TryParse(parts[0] ?? "", out start);
                if (!DateTime.TryParse(parts[1] ?? "", out end)) end = DateTime.MaxValue;

                parametersForLinqQuery.Add(start);
                parametersForLinqQuery.Add(end);
                return string.Format("{0} >= @{1} and {0} <= @{2}", columnname, parametersForLinqQuery.Count - 2, parametersForLinqQuery.Count - 1);
            }
            else
            {
                return string.Format("{1}.ToLocalTime().ToString(\"g\").{0}", FilterMethod(query), columnname);
            }
        }

        public static string StringFilter(string q, string columnname, Type columntype, List<object> parametersforlinqquery)
        {
            if (q.StartsWith("^"))
            {
                return columnname + ".ToLower().StartsWith(\"" + q.ToLower().Replace("\"", "\"\"") + "\")";
            }
            else
            {
                return columnname + ".ToLower().Contains(\"" + q.ToLower().Replace("\"", "\"\"") + "\")";
            }
        }

    }
}