﻿using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Bed
    {
        public Bed()
        {
            Appointments = new HashSet<Appointment>();
            BedSlots = new HashSet<BedSlot>();
        }

        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BedNumber { get; set; } = null!;
        public string StatusWorking { get; set; } = null!;

        public virtual Room Room { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<BedSlot> BedSlots { get; set; }
    }
}
