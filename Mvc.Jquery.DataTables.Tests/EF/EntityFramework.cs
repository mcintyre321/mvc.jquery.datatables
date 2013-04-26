using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using Mvc.JQuery.Datatables;
using NUnit.Framework;

namespace Mvc.JQuery.DataTables.Tests.EF
{


    [TestFixture]
    public class EntityFramework
    {
        public class SomeContext : DbContext
        {
            public DbSet<SomeModel> Models { get; set; }
        }

        protected const string DbFile = "test.sdf";
        protected const string Password = "1234567890";

        [SetUp]
        public void InitTest()
        {
            DataContext = new SomeContext();
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", "", string.Format("Data Source=\"{0}\";Password={1}", DbFile, Password));

            DataContext.Database.Initialize(true);
            for (var i = 0; i < 100; i++)
            {
                DataContext.Models.Add(new SomeModel() {DisplayName = "Name " + i});

            }
            DataContext.SaveChanges();
        }

        protected SomeContext DataContext { get; set; }

        [Test]
        public void ItMakesAppropriateQueries()
        {
            //arrange
            var source = DataContext.Models;
            var dataTablesParam = new DataTablesParam();
            dataTablesParam.iColumns = 2;
            dataTablesParam.bSearchable.Add(false);
            dataTablesParam.bSearchable.Add(true);

            dataTablesParam.sSearch = "Name 10";

            //act
            var result = new DataTablesResult<SomeModel, SomeModel>(source, dataTablesParam, model => model);
            var data = (DataTablesData) result.Data;
        
            //assert
            //TODO: need to check that appropriate SQL queries are made somehow.

        }

        [TearDown]
        public void CleanupTest()
        {
            DataContext.Dispose();

            if (File.Exists(DbFile))
            {
                File.Delete(DbFile);
            }
        }
    }
}
