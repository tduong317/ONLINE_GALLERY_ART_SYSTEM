using System;
using System.Collections.Generic;

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
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<GalleryArtwork> GalleryArtworks { get; set; }
    }
}
