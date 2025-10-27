using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class Artwork
    {
        public Artwork()
        {
            Auctions = new HashSet<Auction>();
            ExhibitionRequests = new HashSet<ExhibitionRequest>();
            GalleryArtworks = new HashSet<GalleryArtwork>();
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
        }

        public int ArtworkId { get; set; }
        public int? ArtistId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public string? SaleType { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Status { get; set; }

        public virtual User? Artist { get; set; } = null!;
        public virtual Category? Category { get; set; }
        public virtual ICollection<Auction> Auctions { get; set; }
        public virtual ICollection<ExhibitionRequest> ExhibitionRequests { get; set; }
        public virtual ICollection<GalleryArtwork> GalleryArtworks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
