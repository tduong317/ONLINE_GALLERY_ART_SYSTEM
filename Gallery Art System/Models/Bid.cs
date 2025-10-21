using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class Bid
    {
        public int BidId { get; set; }
        public int AuctionId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? BidTime { get; set; }

        public virtual Auction Auction { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
