using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class EmpDisciplinary : IAuditable
    {
        public long Id { get; set; }
        public short ActionType { get; set; } // 1=Sow Cause, 2 = Warning, 3= Suspend , 4= Others
        public DateTime DisciplineDate { get; set; }

        [StringLength(220)]
        public string ActionReason { get; set; }

        [StringLength(400)]
        public string ActionDesc { get; set; }

        [StringLength(200)]
        public string Committee { get; set; }

        [StringLength(200)]
        public string ActionFileUrl { get; set; }
        public short SlNo { get; set; } = 1;

        [StringLength(350)]
        public string Remarks { get; set; }

        //-----------------------------------------

        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
