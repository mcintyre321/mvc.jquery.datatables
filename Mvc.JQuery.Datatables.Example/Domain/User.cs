using System;

namespace Mvc.JQuery.DataTables.Example.Domain
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public PositionTypes? Position { get; set; }

        
        public DateTime? Hired { get; set; }

        public Numbers Number { get; set; }

        public bool IsAdmin { get; set; }

        public decimal Salary { get; set; }
    }
}