using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Slot
    {
        public Slot()
        {
            Appointments = new HashSet<Appointment>();
            BedSlots = new HashSet<BedSlot>();
            UserSlots = new HashSet<UserSlot>();
        }

        public int Id { get; set; }
        public string SlotName { get; set; } = null!;
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<BedSlot> BedSlots { get; set; }
        public virtual ICollection<UserSlot> UserSlots { get; set; }
    }
}
