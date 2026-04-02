using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Leave.LeaveCf
{
    public class LeaveCfSearchVm
    {
        public long Id { get; set; }
        public int LeaveYear { get; set; } // Default Current Year
        public int LeaveBalance { get; set; } = 0; // If 0 No limit
        public int CfBalance { get; set; } = 0; // Carry Forwarded Leave
        public int LeaveEnjoyed { get; set; } = 0;
        public int LeaveSale { get; set; } = 0;
        public DateTime LastActionDate { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsDeleted { get; set; }

        //----- FK ------
        public long LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long DesignationId { get; set; }
        public string DesignationName { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
        public IEnumerable<SelectListItem> LeaveTypeLookUp { get; set; }
        public IEnumerable<SelectListItem> LeaveYearLookUp { get; set; }
    }
}
