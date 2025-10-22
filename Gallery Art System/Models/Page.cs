using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    public partial class Page
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(500, ErrorMessage = "Name must be less than 500")]
        public string Name { get; set; } = null!;
        public string? Tag { get; set; }
        public string? Content { get; set; }
        public string? Detail { get; set; }
        public string? Level { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keyword { get; set; }
        public int? Type { get; set; }
        public string? Link { get; set; }
        public string? Target { get; set; }
        public int Index { get; set; }
        public int Position { get; set; }
        public int Ord { get; set; }
        public bool Status { get; set; }
        [NotMapped]
        public List<Page> Children { get; set; } = new List<Page>();
    }
}
