using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class Category
    {
        public Category()
        {
            Artworks = new HashSet<Artwork>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Artwork> Artworks { get; set; }
    }
}
