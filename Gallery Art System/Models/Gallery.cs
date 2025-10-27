using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    public partial class Gallery
    {
        public Gallery()
        {
            GalleryArtworks = new HashSet<GalleryArtwork>();
        }

        public int GalleryId { get; set; }
        public int UserId { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string? Name { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<GalleryArtwork> GalleryArtworks { get; set; }
    }
}
