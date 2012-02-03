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


        public DataTablesResult<UserView> GetUsers(DataTablesParam dataTableParam)
        {
            var domains = "gmail.com,yahoo.com,hotmail.com".Split(',').ToArray();
            var users = new List<User>
            (
                Enumerable.Range(1, 100).Select(i => 
                    new User()
                    {
                        Id = i, 
                        Email = "user" + i + "@" + domains[i % domains.Length], 
                        Name = "User" + i
                    })
            ).AsQueryable();

            return DataTablesResult.Create(users, dataTableParam, user => new UserView()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }
        public DataTablesResult GetUsersUntyped(DataTablesParam dataTableParam)
        {
            var domains = "gmail.com,yahoo.com,hotmail.com".Split(',').ToArray();
            var users = new List<User>
            (
                Enumerable.Range(1, 100).Select(i =>
                    new User()
                    {
                        Id = i,
                        Email = "user" + i + "@" + domains[i % domains.Length],
                        Name = "User" + i
                    })
            );

            return DataTablesResult.Create(users, dataTableParam);
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

        public string Email { get; set; }
    }
}