using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc.JQuery.Datatables.Example.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }


        public IDataTablesResult<UserView> GetUsers(DataTablesParam dataTableParam)
        {
            var users = new List<User>
            (
                Enumerable.Range(1, 100).Select(i => new User(){Id = i, Email = "user" + i + "@gmail.com", Name = "User" + i})
            ).AsQueryable();

            return DataTablesResult.Create(users, dataTableParam, user => new UserView()
            {
                Id = user.Id,
                Name = user.Name
            });
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class UserView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}