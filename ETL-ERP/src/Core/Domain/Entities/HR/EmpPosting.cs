using Domain.Entities.Admin;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class EmpPosting : IAuditable
    {
        public long Id { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }


        [Required]
        [StringLength(1)]
        public string PostingType { get; set; } // R = Regular, S = OSD, P = Promotion, I = Increment, B = Promotion & Increment, O = Other


        public double NetSalary { get; set; }




        public double PreNetSalary { get; set; }

        [StringLength(120)]
        public string PostingDoc { get; set; }

        public short SlNo { get; set; }


        [StringLength(350)]
        public string Remarks { get; set; }


        //-------------------FK----------------------
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public long? DepartmentId { get; set; }
        public Department Department { get; set; }

        public long? DesignationId { get; set; }
        public Designation Designation { get; set; }

        public long? PreDepartmentId { get; set; }
        public Department PreDepartment { get; set; }
        public long? PreDesignationId { get; set; }
        public Designation PreDesignation { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
