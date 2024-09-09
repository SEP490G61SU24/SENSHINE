using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class BedSlot
    {
        public int Id { get; set; }
        public int BedId { get; set; }
        public int SlotId { get; set; }
        public DateTime SlotDate { get; set; }
        public string? Status { get; set; }

        public virtual Bed Bed { get; set; } = null!;
        public virtual Slot Slot { get; set; } = null!;
    }
}
