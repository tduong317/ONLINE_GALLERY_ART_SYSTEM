using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    public partial class Exhibition
    {
        public Exhibition()
        {
            ExhibitionRequests = new HashSet<ExhibitionRequest>();
        }

        public int ExhibitionId { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "nvarchar(300)")]
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public virtual ICollection<ExhibitionRequest> ExhibitionRequests { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

    }
}
