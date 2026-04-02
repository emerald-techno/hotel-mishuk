using Domain.ModelInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Leave.LeaveSetup
{
    public class LeaveSetupSearchVm : IDataTableSearch
    {
        public string LeaveTypeName { get; set; }
        public string LeaveLimitType { get; set; }  // Y = Year, L = Job Life
        public string LeaveLimitTypeText => LeaveLimitType switch { "Y" => "Year", "L" => "Job Life", _ => "N/A" };
        public int LeaveBalance { get; set; } = 0; // If 0 No limit
        public string Gender { get; set; }// A = All, F = Female
        public string GenderText => Gender switch { "A" => "All", "F" => "Female", _ => "N/A" };
        public bool IsCarryForward { get; set; }
        public string IsCarryForwardText => IsCarryForward switch { false => "No", true => "Yes" };
        public int LeaveAfter { get; set; } = 0; // If 0 then from join date
        public bool IsActive { get; set; }

        //--------------------------------------
        public long Id { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }

    }
}
