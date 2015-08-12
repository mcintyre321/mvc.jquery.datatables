using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc.JQuery.Datatables.Models
{
    public class ResponseOptions
    {
        public virtual ArrayOutputType? ArrayOutputType { get; set; }

        public static ResponseOptions<TSource> For<TSource>(IQueryable<TSource> data, 
            Action<ResponseOptions<TSource>> setOptions) where TSource : class
        {
            var responseOptions = new ResponseOptions<TSource>();
            setOptions(responseOptions);
            return responseOptions;
        }
    }

    public class ResponseOptions<TSource> : ResponseOptions
    {
        public Func<TSource, object> DT_RowID
        {
            get
            {
                return dt_rowid;
            }
            set
            {
                dt_rowid = value;
                if (value != null)
                {
                    ArrayOutputType = Models.ArrayOutputType.ArrayOfObjects;
                }
            }
        }
        private Func<TSource, object> dt_rowid;

        public override ArrayOutputType? ArrayOutputType
        {
            get { return base.ArrayOutputType; }
            set
            {
                if (DT_RowID != null && value != Models.ArrayOutputType.ArrayOfObjects)
                {
                    throw new ArgumentOutOfRangeException("ArrayOutputType", "ArrayOutputType must be ArrayOfObjects when DT_RowID is set");
                }
                base.ArrayOutputType = value;
            }
        }
    }
}
