using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class UserSlot
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SlotId { get; set; }
        public DateTime SlotDate { get; set; }
        public string? Status { get; set; }

        public virtual Slot Slot { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
