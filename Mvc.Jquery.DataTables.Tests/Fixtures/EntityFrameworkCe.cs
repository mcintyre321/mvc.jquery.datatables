using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using Mvc.JQuery.Datatables;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Mvc.JQuery.DataTables.Tests
{
    public class EntityFrameworkCe : EntityFramework, IDisposable
    {

        protected const string DbFile = "Test.sdf";
        protected const string Password = "1234567890";

        public EntityFrameworkCe() : base(CreateDb())
        { 
        }

        private static SomeContext CreateDb()
        {
            var context = new SomeContext();
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", "",
                string.Format("Data Source=\"{0}\";Password={1}", DbFile, Password));

            context.Database.Initialize(true);
            return context;
        }

        public void Dispose()
        {
            DataContext.Dispose();
            if (File.Exists(DbFile))
            {
                File.Delete(DbFile);
            }
        }
    }
}
