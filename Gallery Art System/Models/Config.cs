using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    [Table("Config")]
    public class Config
    { 
        [Key]
        public int Id { get; set; }
        public string? Mail { get; set; }
        public string? MailPort { get; set; }
        public string? MailInfo { get; set; }
        public string? Contact { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Hotline { get; set; }
        public string? SocialLink1 { get; set; }
        public string? SocialLink2 { get; set; }
        public string? SocialLink3 { get; set; }
        public string? LinkVideo1 { get; set; }
        public string? LinkVideo2 { get; set; }
        public string? LinkVideo3 { get; set; }
        public string? Image { get; set; }
    }
}
