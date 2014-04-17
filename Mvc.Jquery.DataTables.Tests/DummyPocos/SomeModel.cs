using System;
using System.ComponentModel.DataAnnotations;

namespace Mvc.JQuery.Datatables.Tests
{
    public class SomeModel
    {
        [Key]
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int Category { get; set; }
        public double Scale { get; set; }
        public DateTime When { get; set; }
        public bool Discounted { get; set; }
    }
}