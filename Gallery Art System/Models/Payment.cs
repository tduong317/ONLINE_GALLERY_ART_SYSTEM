﻿using System;
using System.Collections.Generic;

namespace Gallery_Art_System.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string? Method { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
