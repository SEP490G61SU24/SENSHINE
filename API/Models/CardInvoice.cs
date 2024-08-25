using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class CardInvoice
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int InvoiceId { get; set; }

        public virtual Card Card { get; set; } = null!;
        public virtual Invoice Invoice { get; set; } = null!;
    }
}
