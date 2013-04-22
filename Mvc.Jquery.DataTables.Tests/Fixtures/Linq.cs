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
        protected const int SomeModelPropertyCount = 4;
        protected const int SomeViewPropertyCount = 4;
        private const int TotalRecords = 100;
        private const int DisplayLength = 5; 

        protected IQueryable<SomeModel> SomeModelQueryable { get; set; }
        private Func<DataTablesParam, DataTablesData> _executeParams;

        public Linq()
        {
            var dataSet = new List<SomeModel>(TotalRecords);
            var startDate = new DateTime(2013, 2, 1);
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
        
        private Exception ExecuteParamsFail = null;
        [TestFixtureSetUp]
        public void SetExecuteParamsMethod()
        {
            var dataTablesParam = EmptyParam();
            dataTablesParam.sSearch = "Name 10";
            try
            {
                ExecuteParamsReturningViewModel(dataTablesParam);
                _executeParams = ExecuteParamsReturningViewModel;
            }
            catch (Exception e1)
            {
                try
                {
                    ExecuteParamsReturningModel(dataTablesParam);
                    _executeParams = ExecuteParamsReturningModel;
                    ExecuteParamsFail = e1;
                }
                catch (Exception e2)
                {
                    _executeParams = null;
                    ExecuteParamsFail = e2;
                }
            }
        }
        [Test(Description = "DataTablesResult.Create working?")]
        public void CanCreateDataTablesResult()
        {
            Assert.That(ExecuteParamsFail == null,
                string.Format("failed {0}{1}{2}",
                    (_executeParams == null)?"all create atttempts - other tests to fail on ignore":"transform argument mapping to view model. Will use (model=>model) for all tests",
                    Environment.NewLine,
                    ExecuteParamsFail
                ));
        }
        [Test(Description = "Simple Ordering")]
        public void SimpleOrdering()
        {
            if (_executeParams == null) { Assert.Ignore("Unable to create new DataTableResult"); }
            //arrange
            var dataTablesParam = EmptyParam();
            dataTablesParam.sSortDir[0] = "asc";
            dataTablesParam.iSortingCols = 1;
            Assert.AreEqual(_executeParams(dataTablesParam).RecordIds(), Enumerable.Range(1, DisplayLength).ToArray());
        }

        [Test(Description = "Single Record Text Search")]
        public void SingleRecordSearch()
        {
            if (_executeParams == null) { Assert.Ignore("Unable to create new DataTableResult"); }
            //arrange
            var dataTablesParam = EmptyParam();
            //dataTablesParam.sSortDir[0] = "asc";
            dataTablesParam.sSearch = "Name 10";

            Assert.AreEqual(_executeParams(dataTablesParam).RecordIds(), new int[] { 10 });
        }
        [Test(Description = "Combination of Sort, Filter & Paginate")]
        public void SortFilterPage()
        {
            if (_executeParams == null) { Assert.Ignore("Unable to create new DataTableResult"); }
            var dataTablesParam = EmptyParam();
            dataTablesParam.iSortingCols = 1;
            dataTablesParam.iSortCol[0] = 2;
            dataTablesParam.sSearchColumns[3] = "25~35";
            dataTablesParam.iDisplayStart = 6;
            var result = _executeParams(dataTablesParam).RecordIds();
            Assert.AreEqual(_executeParams(dataTablesParam).RecordIds(), new int[] { 17,21,25,77,81 });
        }
        private DataTablesData ExecuteParamsReturningModel(DataTablesParam dataTablesParam)
        {

            var result = DataTablesResult.Create(SomeModelQueryable, //DataContext.Models,
                dataTablesParam,
                model => model);
            return (DataTablesData)result.Data;
        }
        private DataTablesData ExecuteParamsReturningViewModel(DataTablesParam dataTablesParam)
        {
            var result = DataTablesResult.Create(SomeModelQueryable,
                dataTablesParam,
                model => new SomeView
                {
                    Name = model.DisplayName,
                    Category = model.Category,
                    Scale = model.Scale,
                    Id = model.Id
                });
            return (DataTablesData)result.Data;
        }

        protected static DataTablesParam EmptyParam(int columns = SomeModelPropertyCount)
        {
            return new DataTablesParam
            {
                bEscapeRegex = false,
                bEscapeRegexColumns = Populate<bool>(false, columns),
                bSearchable = Populate<bool>(true, columns), 
                bSortable = Populate<bool>(true, columns),
                iColumns = columns,
                iDisplayLength = DisplayLength,
                iSortingCols=1,
                iSortCol = Populate<int>(0, columns),
                sEcho = 1,
                sSearchColumns = Populate<string>("", columns),
                sSortDir = Populate<string>(null, columns),
                sSearch = ""
            };
        }
        protected static List<Tlist> Populate<Tlist>(Tlist value, int capacity = SomeModelPropertyCount)
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
    public static class LinqTestStaticMethods
    {
        internal static int[] RecordIds(this DataTablesData data)
        {
            return Array.ConvertAll<object, int>(data.aaData, d => int.Parse((string)((IEnumerable<object>)d).First()));
        }
    }
}
