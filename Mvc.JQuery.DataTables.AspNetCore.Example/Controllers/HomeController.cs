using Microsoft.AspNetCore.Mvc;
using Mvc.JQuery.DataTables.Example.Domain;
using Mvc.JQuery.DataTables.Example.Helpers;
using Mvc.JQuery.DataTables.Models;
using Mvc.JQuery.DataTables.Serialization;
using System.Linq;

namespace Mvc.JQuery.DataTables.Example.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var getDataUrl = Url.Action(nameof(HomeController.GetUsers));
            var vm = DataTablesHelper.DataTableVm<UserTableRowViewModel>("idForTableElement", getDataUrl);


            //but you can also set them in a typed fashion:

            vm.Filter = true; //Enable filtering 
            vm.ShowFilterInput = true; //Show or hide the search-across-all-columns input

            //Use the advanced per-column filter plugin
            vm.UseColumnFilterPlugin = true;

            vm //Fluently configure, or use attributes on the ViewModel 
                .FilterOn("Position", new { sSelector = "#custom-filter-placeholder-position" }, new { sSearch = "Tester" }).Select("Engineer", "Tester", "Manager")
                .FilterOn("Id").NumberRange()
                .FilterOn("Salary", new { sSelector = "#custom-filter-placeholder-salary" }).NumberRange();


            //You can set options on the datatable in an untyped way:
            //  vm.JsOptions.Add("iDisplayLength", 25);
            //  vm.JsOptions.Add("aLengthMenu", new object[] { new[] {5, 10, 25, 250, -1} , new object[] { 5, 10, 25, 250, "All"} });
            vm.JsOptions.Add("fnCreatedRow", new Raw(@"function( nRow, aData, iDataIndex ) {
        $(nRow).attr('data-id', aData[0]);
    }"));

            //Enable localstorage based saving filter/sort/paging state 
            vm.StateSave = true;

            //you can change the page length options... 
            vm.LengthMenu = LengthMenuVm.Default(); // 5,10,25,50,All
            vm.LengthMenu.RemoveAll(t => t.Item2 == 5);//Take out 5
            vm.PageLength = 25; //... and set a default

            //Enable column visibility 
            vm.ColVis = true;
            vm.ShowVisibleColumnPicker = true; //Displays a control for showing/hiding columns


            //Localizable
            if (Request.Query["lang"] == "de")
            {
                //vm.Language = "{ 'sUrl': '" + Url.Content("~/Content/jquery.dataTables.lang.de-DE.txt") + "' }";
                vm.Language = new Language
                {
                    sProcessing = "Bitte warten...",
                    sLengthMenu = "_MENU_ Einträge anzeigen",
                    sZeroRecords = "Keine Einträge vorhanden.",
                    sInfo = "_START_ bis _END_ von _TOTAL_ Einträgen",
                    sInfoEmpty = "0 bis 0 von 0 Einträgen",
                    sInfoFiltered = "(gefiltert von _MAX_  Einträgen)",
                    sInfoPostFix = "",
                    sSearch = "Suchen",
                    sUrl = "",
                    oPaginate = new Paginate()
                    {
                        sFirst = "Erster",
                        sPrevious = "Zurück",
                        sNext = "Weiter",
                        sLast = "Letzter"
                    }
                }.ToJsonString();
            }
            return View(vm);
        }


        public IActionResult JSUnitTests()
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
                            : rowViewModel.Hired.Value.ToString("d") + " " +
                              rowViewModel.Hired.Value.ToString("t") + " (" +
                              FriendlyDateHelper.GetPrettyDate(rowViewModel.Hired.Value) + ") ",
                    Thumb = "<img src='" + rowViewModel.Thumb + "' />"
                });
        }

    }
}