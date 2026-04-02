using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Leave.LeaveSetup
{
    public class LeaveSetupVm
    {
        public long Id { get; set; }

        [Required]
        [StringLength(1)]
        public string LeaveLimitType { get; set; }  // Y = Year, L = Job Life
        public int LeaveBalance { get; set; } = 0; // If 0 No limit
        public int MaxLeave { get; set; } = 0; // Max leave at a time 
        public int MinLeave { get; set; } = 0;
        public bool IsCarryForward { get; set; }
        public int MaxCarryForward { get; set; } = 0; // 0 Means No Limit
        public int LeaveAfter { get; set; } = 0; // If 0 then from join date
        public DateTime ActiveDate { get; set; }
        public long LeaveTypeId { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }

        [Required]
        [StringLength(1)]
        public string Gender { get; set; }// A = All, F = Female
        public string GenderText => Gender switch { "A" => "All", "F" => "Female", _ => "N/A" };
        public bool IsActive { get; set; }
        public IEnumerable<SelectListItem> LeaveLimitTypeLookUp { get; set; }
        public IEnumerable<SelectListItem> LeaveTypeLookUp { get; set; }
    }
}
