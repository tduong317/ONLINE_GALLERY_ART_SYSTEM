using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class User
    {
        public User()
        {
            Artworks = new HashSet<Artwork>();
            Bids = new HashSet<Bid>();
            ExhibitionRequests = new HashSet<ExhibitionRequest>();
            Galleries = new HashSet<Gallery>();
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Artwork> Artworks { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<ExhibitionRequest> ExhibitionRequests { get; set; }
        public virtual ICollection<Gallery> Galleries { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
