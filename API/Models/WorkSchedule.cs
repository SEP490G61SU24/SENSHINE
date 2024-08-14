using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class WorkSchedule
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Status { get; set; }

        public virtual User? Employee { get; set; }
    }
}
