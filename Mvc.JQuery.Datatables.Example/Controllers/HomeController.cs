using System.Linq;
using System.Web.Mvc;
using Mvc.JQuery.DataTables;
using Mvc.JQuery.DataTables.Example.Domain;
using Mvc.JQuery.DataTables.Example.Helpers;

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
                    Hired =
                        rowViewModel.Hired == null
                            ? "&lt;pending&gt;"
                            : rowViewModel.Hired.Value.ToShortDateString() + " " +
                              rowViewModel.Hired.Value.ToShortTimeString() + " (" +
                              FriendlyDateHelper.GetPrettyDate(rowViewModel.Hired.Value) + ") ",
                    Thumb = "<img src='" + rowViewModel.Thumb + "' />"
                });
        }
    }
}