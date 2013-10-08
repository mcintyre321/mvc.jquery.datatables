using System;

namespace Mvc.JQuery.Datatables.Example.Controllers
{
    static class FriendlyDateHelper
    {
        public static string GetPrettyDate(DateTime d)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(d);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int) s.TotalDays;

            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} days ago",
                                     dayDiff);
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} weeks ago",
                                     Math.Ceiling((double) dayDiff/7));
            }
            return string.Format("{0} months ago",
                                 Math.Ceiling((double) dayDiff/31));
        }
    }
}