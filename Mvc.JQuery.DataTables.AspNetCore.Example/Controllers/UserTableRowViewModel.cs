using Mvc.JQuery.DataTables.Example.App_GlobalResources;
using Mvc.JQuery.DataTables.Example.Domain;
using Mvc.JQuery.DataTables.Models;
using System;

namespace Mvc.JQuery.DataTables.Example.Controllers
{
    public class UserTableRowViewModel
    {
       
        [DataTables(DisplayName = "Name", DisplayNameResourceType = typeof(UserViewResource), MRenderFunction = "encloseInEmTag")]
        public string Name { get; set; }

        [DataTables(SortDirection = SortDirection.Ascending, Order = 0)]
        public int Id { get; set; }

        [DataTables(DisplayName = "E-Mail", Searchable = true)]
        public string Email { get; set; }

        [DataTables( Sortable = false, Width = "70px")]
        public bool IsAdmin { get; set; }

        [DataTables(Visible = false)]
        public bool AHiddenColumn { get; set; }


        [DataTables(Visible = false)]
        public decimal Salary { get; set; }
        
        public string Position { get; set; }

        [DataTablesFilter(DataTablesFilterType.DateTimeRange)]
        [DefaultToStartOf2014]
        public DateTime?  Hired { get; set; }

        public Numbers Number { get; set; }

        [DataTablesExclude]
        public string ThisColumnIsExcluded { get { return "asdf"; } }

        [DataTables(Sortable = false, Searchable = false)]
        [DataTablesFilter(DataTablesFilterType.None)]
        public string Thumb { get; set; }
    }
}