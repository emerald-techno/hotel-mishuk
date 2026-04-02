using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class EmpReference : IAuditable
    {
        public long Id { get; set; }

        [StringLength(60)]
        public string RefName { get; set; }

        public short RefType { get; set; } // 1 = Reference, 2 = Nomine, 3 = Emergency Contact

        [StringLength(30)]
        public string Occupation { get; set; }

        public DateTime? Dob { get; set; }

        [StringLength(120)]
        public string Address { get; set; }

        [StringLength(15)]
        public string Mobile { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Relation { get; set; }

        [StringLength(120)]
        public string PhotoUrl { get; set; }

        public short SlNo { get; set; }
        public double OwnerShip { get; set; }

        [StringLength(350)]
        public string Remarks { get; set; }

        //-------------------------FK-----------------------
        public Employee Employee { get; set; }
        public long EmployeeId { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
