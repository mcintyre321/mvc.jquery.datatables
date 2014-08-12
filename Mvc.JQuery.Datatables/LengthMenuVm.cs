using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.JQuery.Datatables
{
    public class LengthMenuVm : List<Tuple<string, int>>
    {
        public LengthMenuVm()
        {
            
           
        }
        /// <summary>
        /// Create a lengthmenuvm with options for 5,10,25,50,All
        /// </summary>
        /// <returns></returns>
        public static LengthMenuVm Default()
        {
            return new LengthMenuVm {Tuple.Create("5", 5), Tuple.Create("10", 10), Tuple.Create("25", 25), Tuple.Create("50", 50), Tuple.Create("All", -1)};
        }

        public override string ToString()
        {
            return "[[" + string.Join(", ", this.Select(pair => pair.Item2)) + "],[\"" + string.Join("\", \"", this.Select(pair => pair.Item1.Replace("\"", "\"\""))) + "\"]]";
            
        }
    }
}