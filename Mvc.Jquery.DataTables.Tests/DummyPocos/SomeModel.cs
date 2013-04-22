using System;
using System.ComponentModel.DataAnnotations;

namespace Mvc.JQuery.DataTables.Tests
{
    public class SomeModel
    {
        [Key]
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int Category { get; set; }
        public double Scale { get; set; }
    }
}