using Mvc.JQuery.Datatables;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mvc.JQuery.DataTables.Tests
{
    [TestFixture]
    public class Linq
    {
        internal const int SomeModelPropertyCount = 4;
        internal const int SomeViewPropertyCount = 4;
        private const int TotalRecords = 100;
        private const int DisplayLength = 5; 

        protected IQueryable<SomeModel> SomeModelQueryable { get; set; }

        public Linq()
        {
            var dataSet = new List<SomeModel>(TotalRecords);
            for (var i = 1; i < TotalRecords; i++)
            {
                dataSet.Add(new SomeModel()
                {
                    Id = i,
                    DisplayName = "Name " + i,
                    Category = i % 4,
                    Scale = Math.Abs(50 - i)
                });
            }
            SomeModelQueryable = dataSet.AsQueryable();
        }
        
        [Test(Description = "Simple Ordering")]
        public void SimpleOrder()
        {
            var dataTablesParam = EmptyParam();
            dataTablesParam.sSortDir[0] = "asc";
            dataTablesParam.iSortingCols = 1;
            var result = DataTablesResult.Create(SomeModelQueryable, //DataContext.Models,
                dataTablesParam,
                model => model);
            var data = (DataTablesData)result.Data;
            Assert.AreEqual(data.RecordIds(), Enumerable.Range(1, DisplayLength).ToArray());
        }
        [Test(Description = "Simple Ordering with transform to view model")]
        public void SimpleOrderAndTransform()
        {
            var dataTablesParam = EmptyParam();
            dataTablesParam.sSortDir[0] = "asc";
            dataTablesParam.iSortingCols = 1;
            Assert.AreEqual(ExecuteParams(dataTablesParam).RecordIds(), Enumerable.Range(1, DisplayLength).ToArray());
        }

        [Test(Description = "Single Record Text Search")]
        public void SingleRecordSearch()
        {
            var dataTablesParam = EmptyParam();
            //dataTablesParam.sSortDir[0] = "asc";
            dataTablesParam.sSearch = "Name 10";

            Assert.AreEqual(ExecuteParams(dataTablesParam).RecordIds(), new int[] { 10 });
        }
        [Test(Description = "Combination of Sort, Filter & Paginate")]
        public void SortFilterPage()
        {
            var dataTablesParam = EmptyParam();
            dataTablesParam.iSortingCols = 1;
            dataTablesParam.iSortCol[0] = 2;
            dataTablesParam.sSearchColumns[3] = "25~35";
            dataTablesParam.iDisplayStart = 6;
            var result = ExecuteParams(dataTablesParam).RecordIds();
            Assert.AreEqual(ExecuteParams(dataTablesParam).RecordIds(), new int[] { 17,21,25,77,81 });
        }
        private DataTablesData ExecuteParams(DataTablesParam dataTablesParam)
        {
            var result = DataTablesResult.Create(SomeModelQueryable,
                dataTablesParam,
                model => new SomeView
                {
                    Name = model.DisplayName,
                    Cat = model.Category,
                    ViewScale = model.Scale,
                    Id = model.Id
                });
            return (DataTablesData)result.Data;
        }

        protected static DataTablesParam EmptyParam(int columns = SomeModelPropertyCount)
        {
            return new DataTablesParam
            {
                bEscapeRegex = false,
                bEscapeRegexColumns = LinqTestStaticMethods.Populate<bool>(false, columns),
                bSearchable = LinqTestStaticMethods.Populate<bool>(true, columns),
                bSortable = LinqTestStaticMethods.Populate<bool>(true, columns),
                iColumns = columns,
                iDisplayLength = DisplayLength,
                iSortingCols=1,
                iSortCol = LinqTestStaticMethods.Populate<int>(0, columns),
                sEcho = 1,
                sSearchColumns = LinqTestStaticMethods.Populate<string>("", columns),
                sSortDir = LinqTestStaticMethods.Populate<string>(null, columns),
                sSearch = ""
            };
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
