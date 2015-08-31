using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using Mvc.JQuery.DataTables;
using Mvc.JQuery.DataTables.Tests.DummyPocos;
using NUnit.Framework;

namespace Mvc.JQuery.DataTables.Tests.Fixtures
{
    public class EntityFramework : Linq, IDisposable
    {
        public class SomeContext : DbContext
        {
            public DbSet<SomeModel> Models { get; set; }
        }

        private readonly IDbConnectionFactory _defaultConnectionFactory;
        private SomeContext _dataContext;
        protected SomeContext DataContext
        {
            get { return _dataContext; }
            set
            {
                _dataContext = value;
                SomeModelQueryable = (_dataContext==null)?null:_dataContext.Models.AsQueryable();
            }
        }

        protected const string DbFile = "Test.sdf";
        protected const string Password = "1234567890";

        public EntityFramework()
            : this(new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", "",
                string.Format("Data Source=\"{0}\";Password={1}", DbFile, Password))) { }

        public EntityFramework(IDbConnectionFactory connectionFactory)
        {
            _defaultConnectionFactory = Database.DefaultConnectionFactory;
            Database.DefaultConnectionFactory = connectionFactory;
            var oldQueryable = SomeModelQueryable;
            DataContext = new SomeContext();
            DataContext.Database.Initialize(true);
            foreach (var sm in oldQueryable)
            {
                DataContext.Models.Add(sm);
            }
            DataContext.SaveChanges();
        }

        [Test, TestCaseSource(typeof(MyFactoryClass), "TestCases")]
        public override int[] ExecuteParams(DataTablesParam dataTablesParam)
        {
            DataContext.Dispose(); //reset datacontext in order to clear local 
            DataContext = new SomeContext();
            int[] returnVar = base.ExecuteParams(dataTablesParam);
            Assert.AreEqual(returnVar.Length, DataContext.Models.Local.Count, "records loaded in memory");
            return returnVar;
        }

        #region IDisposable implementation
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (DataContext != null) { DataContext.Dispose(); }
                    if (File.Exists(DbFile)) { File.Delete(DbFile); }
                    Database.DefaultConnectionFactory = _defaultConnectionFactory;
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
