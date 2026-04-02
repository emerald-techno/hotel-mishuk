using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Accounting
{
    public class AccTranFile
    {
        public long Id { get; set; }

        [Required]
        [StringLength(80)]
        public string FileName { get; set; }

        public DateTime UploadDate { get; set; }

        [Required]
        [StringLength(120)]
        public string UploadFile { get; set; }

        public short SlNo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-------------------------------
        public long TranMstId { get; set; }
        public AccTranMst TranMst { get; set; }

        public long UploadById { get; set; }
        public ApplicationUser UploadBy { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
