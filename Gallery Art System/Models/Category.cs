using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    public partial class Category
    {
        public Category()
        {
            Artworks = new HashSet<Artwork>();
        }

        public int CategoryId { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Artwork> Artworks { get; set; }
    }
}
