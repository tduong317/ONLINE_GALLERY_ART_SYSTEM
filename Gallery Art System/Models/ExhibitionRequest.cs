using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class ExhibitionRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public int ArtworkId { get; set; }
        public int ExhibitionId { get; set; }
        public string? Status { get; set; }

        public virtual Artwork Artwork { get; set; } = null!;
        public virtual Exhibition Exhibition { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
