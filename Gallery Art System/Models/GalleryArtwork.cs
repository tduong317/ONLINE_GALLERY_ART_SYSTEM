using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class GalleryArtwork
    {
        public int GalleryId { get; set; }
        public int ArtworkId { get; set; }
        public DateTime? AddedAt { get; set; }

        public virtual Artwork Artwork { get; set; } = null!;
        public virtual Gallery Gallery { get; set; } = null!;
    }
}
