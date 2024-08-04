using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class CardCombo
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int ComboId { get; set; }
        public int? SessionDone { get; set; }

        public virtual Card Card { get; set; } = null!;
        public virtual Combo Combo { get; set; } = null!;
    }
}
