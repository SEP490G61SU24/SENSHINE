using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Room
    {
        public Room()
        {
            Beds = new HashSet<Bed>();
        }

        public int Id { get; set; }
        public int SpaId { get; set; }
        public string RoomName { get; set; } = null!;

        public virtual Spa Spa { get; set; } = null!;
        public virtual ICollection<Bed> Beds { get; set; }
    }
}
