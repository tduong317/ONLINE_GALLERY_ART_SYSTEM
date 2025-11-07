using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    public partial class Auction
    {
        public Auction()
        {
            Bids = new HashSet<Bid>();
        }

        public int AuctionId { get; set; }
        public int ArtworkId { get; set; }

        public int? UserId { get; set; }
        public decimal StartPrice { get; set; }
        public decimal? CurrentPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Status { get; set; }
        public string? ApprovalStatus { get; set; }
        
        public virtual Artwork Artwork { get; set; } = null!;
        public virtual User? User { get; set; } = null!;
        public virtual ICollection<Bid> Bids { get; set; }
    }
}
