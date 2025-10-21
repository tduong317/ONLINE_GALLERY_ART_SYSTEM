﻿using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int ArtworkId { get; set; }
        public int UserId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Artwork Artwork { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
