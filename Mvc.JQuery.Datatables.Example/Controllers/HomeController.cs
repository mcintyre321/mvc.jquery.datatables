using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Mvc.JQuery.DataTables.Models;
using Mvc.JQuery.DataTables;
using Newtonsoft.Json.Linq;
using Resources;

namespace Mvc.JQuery.DataTables.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JSUnitTests()
        {
            return View();
        }

        public DataTablesResult<UserTableRowViewModel> GetUsers(DataTablesParam dataTableParam)
        {
            return DataTablesResult.Create(FakeDatabase.Users.Select(user => new UserTableRowViewModel()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Position = user.Position == null ? "" : user.Position.ToString(),
                Number = user.Number,
                Hired = user.Hired,
                IsAdmin = user.IsAdmin,
                Salary = user.Salary,
                Thumb = "https://randomuser.me/api/portraits/thumb/men/" + user.Id + ".jpg"
            }), dataTableParam,
            rowViewModel => new 
            {
                Name = "<b>" + rowViewModel.Name + "</b>",
                Hired = rowViewModel.Hired == null ? "&lt;pending&gt;" : rowViewModel.Hired.Value.ToShortDateString() + " " + rowViewModel.Hired.Value.ToShortTimeString() + " (" + FriendlyDateHelper.GetPrettyDate(rowViewModel.Hired.Value) + ") ",
                Thumb = "<img src='" + rowViewModel.Thumb + "' />"
            });
        }

        //public DataTablesResult<User> GetUsersUntyped(DataTablesParam dataTableParam)
        //{
        //    var users = FakeDatabase.Users;

        //    return DataTablesResult.Create(users, dataTableParam);
        //}


    }


   

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

    public class DefaultToStartOf2014Attribute : DataTablesAttributeBase
    {
        public override void ApplyTo(ColDef colDef, PropertyInfo pi)
        {
            colDef.SearchCols = colDef.SearchCols ?? new JObject();
            colDef.SearchCols["sSearch"] = new DateTime(2014, 1, 1).ToString("g") + "~" + DateTimeOffset.Now.Date.AddDays(1).ToString("g");
        }
    }
}