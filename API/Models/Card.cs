using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Card
    {
        public Card()
        {
            CardCombos = new HashSet<CardCombo>();
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public string CardNumber { get; set; } = null!;
        public int BranchId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }

        public virtual Spa Branch { get; set; } = null!;
        public virtual User Customer { get; set; } = null!;
        public virtual ICollection<CardCombo> CardCombos { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
