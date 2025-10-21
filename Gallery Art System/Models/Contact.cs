using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class Contact
    {
        public int MessageId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Subject { get; set; }
        public string Message { get; set; } = null!;
        public DateTime? SentAt { get; set; }
    }
}
