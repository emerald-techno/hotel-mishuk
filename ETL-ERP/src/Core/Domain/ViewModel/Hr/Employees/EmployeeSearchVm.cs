using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Hr.Employees
{
    public class EmployeeSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? RetirementDate { get; set; }
        public string PhotoUrl { get; set; }
        public string SignatureFile { get; set; }
        public string NIDFile { get; set; }
        public DateTime? Dob { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }
        public string BloodGroup { get; set; }
        public string MaritalStatus { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SpouseName { get; set; }
        public int EmployeeStatus { get; set; }
        public string EmployeeStatusText => EmployeeStatus switch { 1 => "Permanent", 2 => "Contractual", 3 => "Adhoc", 4 => "Guest", _ => "" };
        public string PreAddress { get; set; }
        public string PerAddress { get; set; }
        public string Remarks { get; set; }
        public double Salary { get; set; }
        public int SalaryType { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsEnable { get; set; }
        public short EnableStatus { get; set; }

        //----------------------------------------

        public IEnumerable<SelectListItem> DesignationLookUp { get; set; }
        public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }
        public IEnumerable<SelectListItem> AcademicDepartmentLookUp { get; set; }
        public IEnumerable<SelectListItem> EmployeeIsEnableLookUp { get; set; }
        public long? DesignationId { get; set; }
        public long? DepartmentId { get; set; }
        public long? AcademicDptId { get; set; }
        public long? DutyShiftId { get; set; }
        public string PermanentDutyShiftName { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string AcademicDepartmentName { get; set; }
    }
}
