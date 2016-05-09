using System;
using System.Linq;
using System.Web.Mvc;
using Mvc.JQuery.DataTables.Example.Domain;
using Mvc.JQuery.DataTables.Models;

namespace Mvc.JQuery.DataTables.Example.Controllers
{
    public class FacetController : Controller
    { 

        public ActionResult Index()
        {
            return View();
        }
 
        public DataTablesResult<UserFacetRowViewModel> GetFacetedUsers(DataTablesParam dataTableParam)
        {
            return DataTablesResult.Create(FakeDatabase.Users.Select(user => new UserFacetRowViewModel()
            {
                Email = user.Email,
                Position = user.Position == null ? "" : user.Position.ToString(),
                Hired = user.Hired,
                IsAdmin = user.IsAdmin,
                Content = "https://randomuser.me/api/portraits/thumb/men/" + user.Id + ".jpg"
            }), dataTableParam,
                rowViewModel => new
                {
                    Content = "<div>" +
                              "  <div>Email: " + rowViewModel.Email + (rowViewModel.IsAdmin ? " (admin)" : "") + "</div>" +
                              "  <div>Hired: " + rowViewModel.Hired + "</div>" +
                              "  <img src='" + rowViewModel.Content + "' />" +
                              "</div>"
                });
        }


        public class UserFacetRowViewModel
        {
            [DataTables(DisplayName = "E-Mail", Searchable = true, Visible = false)]
            [DataTablesFilter(Selector = "#" + nameof(Email) + "Filter")]
            public string Email { get; set; }

            [DataTables(Width = "70px", Visible = false)]
            [DataTablesFilter(Selector =  "#" +nameof(IsAdmin) + "Filter")]
            public bool IsAdmin { get; set; }

              
            [DataTables(Width = "70px", Visible = false)]
            [DataTablesFilter(Selector = "#" + nameof(Position) + "Filter")]
            public string Position { get; set; }

            [DataTablesFilter(DataTablesFilterType.DateTimeRange, Selector = "#" + nameof(Hired) + "Filter")]
            [DefaultToStartOf2014]
            [DataTables(Visible = false)]
            public DateTime? Hired { get; set; }
             
            [DataTables(Sortable = false, Searchable = false)]
            [DataTablesFilter(DataTablesFilterType.None)]
            public string Content { get; set; }
        }
    }
}