using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    [Table("Banner")]
    public partial class Banner
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public bool Status { get; set; }
    }
}
