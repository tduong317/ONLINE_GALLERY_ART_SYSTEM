using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int? UserId { get; set; }

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public string? Author { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public string? SaleType { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public string? Created { get; set; }

        public string? Size { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Status { get; set; }

        // Quan hệ
        public virtual User? User { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<Auction> Auctions { get; set; }
        public virtual ICollection<ExhibitionRequest> ExhibitionRequests { get; set; }
        public virtual ICollection<GalleryArtwork> GalleryArtworks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
