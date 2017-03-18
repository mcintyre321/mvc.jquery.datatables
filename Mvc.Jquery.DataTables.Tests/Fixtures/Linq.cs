using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mvc.JQuery.DataTables;
using Mvc.JQuery.DataTables.Tests.DummyPocos;
using NUnit.Framework;

namespace Mvc.JQuery.DataTables.Tests.Fixtures
{
    [TestFixture]
    public class Linq
    {
        private const int TotalRecords = 100;

        protected IQueryable<SomeModel> SomeModelQueryable { get; set; }

        public Linq()
        {
            DateTime startDate = new DateTime(2000, 1, 1);
            var dataSet = new List<SomeModel>(TotalRecords);
            for (var i = 1; i < TotalRecords; i++)
            {
                dataSet.Add(new SomeModel()
                {
                    Id = i,
                    DisplayName = "Name " + i,
                    Category = i % 4,
                    Scale = Math.Abs(50 - i),
                    When = startDate.AddDays(i)
                });
            }
            SomeModelQueryable = dataSet.AsQueryable();
        }

        [Test, TestCaseSource(typeof(MyFactoryClass), "TestCases")]
        public virtual int[] ExecuteParams(DataTablesParam dataTablesParam)
        {
            var result = new DataTablesResult<SomeModel>(SomeModelQueryable, dataTablesParam);
            var data = result.Data;
            return data.aaData.Select(row => ((SomeModel)row).Id).ToArray();
        }

        //[Test, TestCaseSource(typeof(MyFactoryClass), "TestCases")]
        public virtual int[] ExecuteParamsAndTransform(DataTablesParam dataTablesParam)
        {
            var result = DataTablesResult.Create(SomeModelQueryable,
                dataTablesParam,
                m => new { 
                    FriendlyWhen = m.When.ToShortDateString(),
                });
            var data = result.Data;
            return data.aaData.Select(d=>Convert.ToInt32(((IList)d)[0])).ToArray();
        }

    }
    public static class MyFactoryClass
    {
        static int PropertyCount(Type T)
        {
            return T.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Count(pi=>pi.CanRead);
        }

        static int DefaultTestCasesLength = 5;

        public static IEnumerable TestCases
        {
            get
            {
                int propertyCount = PropertyCount(typeof(DataTablesParam));
                var dataTablesParam = GetEmptyParam(propertyCount);
                dataTablesParam.sSortDir[0] = "asc";
                dataTablesParam.iSortingCols = 1;
                yield return new TestCaseData(dataTablesParam)
                    .Returns(Enumerable.Range(1, DefaultTestCasesLength).ToArray())
                    .SetName("SimpleOrder")
                    .SetDescription("Simple Ordering");

                dataTablesParam = GetEmptyParam(propertyCount);
                dataTablesParam.sSearch = "Name 10";
                yield return new TestCaseData(dataTablesParam)
                    .Returns(new int[] { 10 })
                    .SetName("SingleRecordSearch")
                    .SetDescription("Single Record Text Search");

                dataTablesParam = GetEmptyParam(propertyCount);
                dataTablesParam.iSortingCols = 1;
                dataTablesParam.iSortCol[0] = 2;
                dataTablesParam.sSearchValues[3] = "25~35";
                dataTablesParam.iDisplayStart = 6;
                yield return new TestCaseData(dataTablesParam)
                    .Returns(new int[] { 17, 21, 25, 77, 81 })
                    .SetName("SortFilterPage")
                    .SetDescription("Combination of Sort, Filter & Paginate");
            }
        }

        static DataTablesParam GetEmptyParam(int columns)
        {
            var returnVar = new DataTablesParam(columns);
            returnVar.iDisplayLength = DefaultTestCasesLength;
            returnVar.iSortingCols = 1;
            returnVar.sEcho = 1;
            returnVar.sSearch = "";
            returnVar.bEscapeRegexColumns.AddRange(Enumerable.Repeat(false, columns));
            returnVar.bSearchable.AddRange(Enumerable.Repeat(true, columns));
            returnVar.bSortable.AddRange(Enumerable.Repeat(true, columns));
            returnVar.iSortCol.AddRange(Enumerable.Repeat(0, columns));
            returnVar.sSearchValues.AddRange(Enumerable.Repeat("", columns));
            returnVar.sSortDir.AddRange(Enumerable.Repeat<string>(null, columns));

            return returnVar;
        }
    }

}
