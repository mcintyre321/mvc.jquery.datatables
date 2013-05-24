using Mvc.JQuery.Datatables;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.JQuery.DataTables.Tests
{
    [TestFixture]
    public class Linq
    {
        internal const int SomeModelPropertyCount = 5;
        internal const int SomeViewPropertyCount = 5;
        internal const int TotalRecords = 100;
        internal const int DisplayLength = 5;
        internal static DateTime StartTime = new DateTime(2013, 3, 2, 12, 15, 22);

        protected IQueryable<SomeModel> SomeModelQueryable { get; set; }

        public Linq()
        {
            var dataSet = new List<SomeModel>(TotalRecords);
            var start = StartTime;
            for (var i = 1; i < TotalRecords; i++)
            {
                dataSet.Add(new SomeModel()
                {
                    Id = i,
                    DisplayName = "Name " + i,
                    Category = i % 4,
                    Scale = Math.Abs(50 - i),
                    Time = start.AddDays(i)
                });
            }
            SomeModelQueryable = dataSet.AsQueryable();
        }

        [Test, TestCaseSource(typeof(MyFactoryClass), "TestCases")]
        public virtual int[] ExecuteParams(DataTablesParam dataTablesParam)
        {
            var result = DataTablesResult.Create(SomeModelQueryable,
                dataTablesParam,
                model => model);
            var data = (DataTablesData)result.Data;
            return data.RecordIds();
        }

        [Test, TestCaseSource(typeof(MyFactoryClass), "TestCases")]
        public virtual int[] ExecuteParamsAndTransform(DataTablesParam dataTablesParam)
        {
            var result = DataTablesResult.Create(SomeModelQueryable,
                dataTablesParam,
                model => new SomeView
                {
                    Name = model.DisplayName,
                    Cat = model.Category,
                    ViewScale = model.Scale,
                    Id = model.Id,
                    Time = model.Time
                });
            var data = (DataTablesData)result.Data;
            return data.RecordIds();
        }
    }
    public static class MyFactoryClass
    {
        public static IEnumerable TestCases
        {
            get
            {
                var dataTablesParam = DefaultParam();
                yield return new TestCaseData(dataTablesParam)
                    .Returns(Enumerable.Range(1, Linq.DisplayLength).ToArray())
                    .SetName("SimpleOrder")
                    .SetDescription("Simple Ordering");

                dataTablesParam = DefaultParam();
                dataTablesParam.sSearch = "Name 10";
                yield return new TestCaseData(dataTablesParam)
                    .Returns(new int[] { 10 })
                    .SetName("SingleRecordSearch")
                    .SetDescription("Single Record Text Search");

                dataTablesParam = DefaultParam();
                dataTablesParam.iSortCol[0] = 2;
                dataTablesParam.sSearchColumns[3] = "25~35";
                dataTablesParam.iDisplayStart = 6;
                yield return new TestCaseData(dataTablesParam)
                    .Returns(new int[] { 17, 21, 25, 77, 81 })
                    .SetName("SortFilterPage")
                    .SetDescription("Combination of Sort, Filter & Paginate");

                dataTablesParam = DefaultParam();
                dataTablesParam.sSortDir[0] = "desc";
                dataTablesParam.iSortCol[0] = 4;
                yield return new TestCaseData(dataTablesParam)
                    .Returns(Enumerable.Range(1, Linq.DisplayLength).Select(r => Linq.TotalRecords - r).ToArray())
                    .SetName("OrderDateTime")
                    .SetDescription("Order <DateTime>");

                dataTablesParam = DefaultParam();
                dataTablesParam.iSortCol[0] = 4;
                dataTablesParam.sSearchColumns[4] =  Linq.StartTime.AddDays(5).ToShortDateString() + "~";
                yield return new TestCaseData(dataTablesParam)
                    .Returns(Enumerable.Range(5, Linq.DisplayLength).ToArray())
                    .SetName("SearchDateTimeRange")
                    .SetDescription("Search for range of <DateTime>");
            }
        }

        public static DataTablesParam DefaultParam(int columns = Linq.SomeModelPropertyCount)
        {
            //info can be found at http://datatables.net/usage/server-side
            var dataTablesParam = new DataTablesParam
            {
                bEscapeRegex = false,
                bEscapeRegexColumns = LinqTestStaticMethods.Populate<bool>(false, columns),
                bSearchable = LinqTestStaticMethods.Populate<bool>(true, columns),
                bSortable = LinqTestStaticMethods.Populate<bool>(true, columns),
                iColumns = columns,
                iDisplayLength = Linq.DisplayLength,
                iSortingCols = 1,
                iSortCol = LinqTestStaticMethods.Populate<int>(0, columns),
                sEcho = 2,
                sSearchColumns = LinqTestStaticMethods.Populate<string>("", columns),
                sSortDir = LinqTestStaticMethods.Populate<string>(null, columns),
                sSearch = "",
                iDisplayStart = 0
            };
            dataTablesParam.sSortDir[0] = "asc";
            return dataTablesParam;
        }
    }

    public static class LinqTestStaticMethods
    {
        public static int[] RecordIds(this DataTablesData data)
        {
            return Array.ConvertAll<object, int>(data.aaData, d => int.Parse((string)((IEnumerable<object>)d).First()));
        }
        public static List<Tlist> Populate<Tlist>(Tlist value, int capacity = Linq.SomeModelPropertyCount)
        {
            var returnVal = new Tlist[capacity];
            if (!EqualityComparer<Tlist>.Default.Equals(value, default(Tlist)))
            {
                for (var i = 0; i < capacity; ++i)
                {
                    returnVal[i] = value;
                }
            }
            return new List<Tlist>(returnVal);
        }
    }
}
