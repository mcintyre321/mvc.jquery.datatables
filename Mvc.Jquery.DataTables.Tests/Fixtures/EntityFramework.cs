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
    public abstract class EntityFramework : Linq
    {
        public class SomeContext : DbContext
        {
            public DbSet<SomeModel> Models { get; set; }
        }

        protected readonly SomeContext DataContext;

        public EntityFramework(SomeContext dataContext)
        {
            DataContext = dataContext;
            if (DataContext.Models.Any())
            {
                DataContext.Database.ExecuteSqlCommand("DELETE FROM SomeModels");
            }
            foreach (var sm in SomeModelQueryable)
            {
                DataContext.Models.Add(sm);
            }
            DataContext.SaveChanges();

            SomeModelQueryable = DataContext.Models;
        }

    }
}
