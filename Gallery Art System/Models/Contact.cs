using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery_Art_System.Models
{
    public partial class Contact
    {
        public int MessageId { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "nvarchar(200)")]
        public string Email { get; set; } = null!;
        [Column(TypeName = "nvarchar(200)")]
        public string? Subject { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Message { get; set; } = null!;
        public DateTime? SentAt { get; set; }

    }
}
