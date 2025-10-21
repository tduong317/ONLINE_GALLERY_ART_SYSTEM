using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class Exhibition
    {
        public Exhibition()
        {
            ExhibitionRequests = new HashSet<ExhibitionRequest>();
        }

        public int ExhibitionId { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public virtual ICollection<ExhibitionRequest> ExhibitionRequests { get; set; }
    }
}
