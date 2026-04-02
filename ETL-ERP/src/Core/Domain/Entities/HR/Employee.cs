using Domain.Entities.Admin;
using Domain.Entities.Attendance;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class Employee : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(90)]
        public string Name { get; set; }

        [Required]
        [StringLength(30)]
        public string Code { get; set; }

        [StringLength(35)]
        public string Email { get; set; }


        [StringLength(20)]
        public string Mobile { get; set; }

        public DateTime JoinDate { get; set; }
        public DateTime? RetirementDate { get; set; }
        public DateTime? ProbationDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }

        [StringLength(120)]
        public string PhotoUrl { get; set; }

        [StringLength(120)]
        public string SignatureUrl { get; set; }

        [StringLength(120)]
        public string NIDUrl { get; set; }

        [StringLength(40)]
        public string NID { get; set; }

        public DateTime? Dob { get; set; }

        [Required]
        [StringLength(1)]
        public string Gender { get; set; } //M = Male, F = Female

        [StringLength(50)]
        public string Nationality { get; set; }

        [StringLength(30)]
        public string Religion { get; set; }

        [StringLength(20)]
        public string BloodGroup { get; set; }

        [StringLength(1)]
        public string MaritalStatus { get; set; } // U = Unmaried, M = Married

        [StringLength(60)]
        public string FatherName { get; set; }

        [StringLength(60)]
        public string MotherName { get; set; }

        [StringLength(60)]
        public string SpouseName { get; set; }
        public short EmployeeStatus { get; set; } // 1 = Permanent, 2 = Contractual

        [StringLength(120)]
        public string PreAddress { get; set; }

        [StringLength(120)]
        public string PerAddress { get; set; }

        [StringLength(350)]
        public string Remarks { get; set; }
        public double Salary { get; set; }
        public short SalaryType { get; set; } // 1 = Monthly, 2 = Class Wise
        public bool IsEnable { get; set; }
        public DateTime? DisableDate { get; set; }

        [StringLength(350)]
        public string DisableReason { get; set; }

        [StringLength(100)]
        public string BankAccNo { get; set; }
        public bool IsPfMember { get; set; }


        //--------------FK--------------

        public long DesignationId { get; set; }
        public Designation Designation { get; set; }

        public long? DepartmentId { get; set; }
        public Department Department { get; set; }

        public long? AcademicDptId { get; set; }
        public Department AcademicDpt { get; set; }

        public long? DutyShiftId { get; set; }
        public DutyShift DutyShift { get; set; }

        public long? UserId { get; set; }
        public ApplicationUser User { get; set; }

        public long? DisableById { get; set; }
        public ApplicationUser DisableBy { get; set; }

        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }


        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
