using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Hr.EmpReference
{
    public class EmpReferenceVm
    {
        public long Id { get; set; }

        [DisplayName("Reference Name: ")]
        [StringLength(60, ErrorMessage = "Reference Name can not be more than 60 characters")]
        public string RefName { get; set; }
        public int RefType { get; set; } // 1 = Reference, 2 = Nomine, 3 = Emergency Contact

        [DisplayName("Occupation: ")]
        [StringLength(30, ErrorMessage = "Occupation can not be more than 30 characters")]
        public string Occupation { get; set; }
        public DateTime? Dob { get; set; }
        public string DobStr { get; set; }

        [StringLength(120, ErrorMessage = "Address can not be more than 120 characters")]
        [DisplayName("Address: ")]
        public string Address { get; set; }

        [StringLength(15, ErrorMessage = "Mobile number can not be more than 15 characters")]
        [DisplayName("Mobile No: ")]
        public string Mobile { get; set; }

        [StringLength(20, ErrorMessage = "Phone number can not be more than 20 characters")]
        [DisplayName("Phone Number: ")]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Relation can not be more than 50 characters")]
        [DisplayName("Relation: ")]
        public string Relation { get; set; }
        public int SlNo { get; set; }
        public double OwnerShip { get; set; }

        [DisplayName("Remarks: ")]
        [StringLength(350, ErrorMessage = "Remarks can not be more than 350 characters")]
        public string Remarks { get; set; }

        //-----------------------------------------------------------------------------------
        public long EmployeeId { get; set; }
        public bool IsAjaxPost { get; set; }
    }
}
