using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gallery_Art_System.Models
{
    public partial class ExhibitionRequest
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Người gửi")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tác phẩm")]
        public int ArtworkId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Triển lãm")]
        public int ExhibitionId { get; set; }

        public bool? Status { get; set; }

        public virtual Artwork Artwork { get; set; } = null!;
        public virtual Exhibition Exhibition { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
