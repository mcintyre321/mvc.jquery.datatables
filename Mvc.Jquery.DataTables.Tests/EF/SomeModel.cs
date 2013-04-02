using System.ComponentModel.DataAnnotations;

namespace Mvc.JQuery.DataTables.Tests.EF
{
    public class SomeModel
    {
        [Key]
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }
}