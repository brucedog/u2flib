using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoU2FSite.Repository
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
    }
}