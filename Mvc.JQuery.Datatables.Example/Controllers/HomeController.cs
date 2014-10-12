using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Mvc.JQuery.Datatables.Models;
using Newtonsoft.Json.Linq;
using Resources;

namespace Mvc.JQuery.Datatables.Example.Controllers
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

        public DataTablesResult<UserView> GetUsers(DataTablesParam dataTableParam)
        {
            return DataTablesResult.Create(FakeDatabase.Users.Select(user => new UserView()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Position = user.Position == null ? "" : user.Position.ToString(),
                Number = user.Number,
                Hired = user.Hired,
                IsAdmin = user.IsAdmin,
                Salary = user.Salary
            }), dataTableParam,
            uv => new 
            {
                Name = "<b>" + uv.Name + "</b>",
                Hired = uv.Hired == null ? "&lt;pending&gt;" : uv.Hired.Value.ToShortDateString() + " (" + FriendlyDateHelper.GetPrettyDate(uv.Hired.Value) + ") "
            });
        }

        //public DataTablesResult<User> GetUsersUntyped(DataTablesParam dataTableParam)
        //{
        //    var users = FakeDatabase.Users;

        //    return DataTablesResult.Create(users, dataTableParam);
        //}


    }


   

    public class UserView
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
        [DefaultToStartOfYear]
        public DateTime?  Hired { get; set; }

        public Numbers Number { get; set; }

        [DataTablesExclude]
        public string ThisColumnIsExcluded { get { return "asdf"; } }

        
    }

    public class DefaultToStartOfYearAttribute : DataTablesAttributeBase
    {
        public override void ApplyTo(ColDef colDef, PropertyInfo pi)
        {
            colDef.SearchCols = colDef.SearchCols ?? new JObject();
            colDef.SearchCols["sSearch"] = new DateTime(DateTime.Now.Year, 1, 1).ToString("g") + "~" + DateTimeOffset.Now.Date.AddDays(1).ToString("g");
        }
    }
}