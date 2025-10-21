using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gallery_Art_System.Models
{
    public partial class Admin
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; } = null!;
        public string? Password { get; set; } = null!;
    }
}
