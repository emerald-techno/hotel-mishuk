using AutoMapper;
using ClosedXML.Excel;
using Domain.Entities.Admin;
using Domain.Entities.HR;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Hr.Employees;
using Interface.Repository.Admin;
using Interface.Repository.Common;
using Interface.Repository.Hr;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Services.Base;
using System.Data;
using System.Transactions;

namespace Services.Hr
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        #region Config

        private IEmployeeRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IAutoCodeRepository _iAutoCodeRepository;
        private readonly IDesignationRepository _iDesignationRepository;
        private readonly IDepartmentRepository _iDepartmentRepository;
        private readonly IEmpEducationService _iEmpEducationService;
        private readonly IEmpExperienceService _iEmpExperienceService;
        private readonly IEmpReferenceService _iEmpReferenceService;
        private readonly IEmpPostingService _iEmpPostingService;
        private readonly IEmpTrainingService _iEmpTrainingService;
        private readonly IEmpDisciplinaryService _iEmpDisciplinaryService;

        public EmployeeService(IEmployeeRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository, IDesignationRepository iDesignationRepository, IDepartmentRepository iDepartmentRepository, IEmpEducationService iEmpEducationService, IEmpExperienceService iEmpExperienceService, IEmpReferenceService iEmpReferenceService, IEmpPostingService iEmpPostingService, IEmpTrainingService iEmpTrainingService, IEmpDisciplinaryService iEmpDisciplinaryService) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iAutoCodeRepository = iAutoCodeRepository;
            _iDesignationRepository = iDesignationRepository;
            _iDepartmentRepository = iDepartmentRepository;
            _iEmpEducationService = iEmpEducationService;
            _iEmpExperienceService = iEmpExperienceService;
            _iEmpReferenceService = iEmpReferenceService;
            _iEmpPostingService = iEmpPostingService;
            _iEmpTrainingService = iEmpTrainingService;
            _iEmpDisciplinaryService = iEmpDisciplinaryService;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>>
            SearchAsync(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion

        #region EmployeeAdd

        public async Task<bool> EmployeeAddAsync(EmployeeVm vm)
        {
            var employeeModel = _iMapper.Map<Employee>(vm);
            employeeModel.Code = await GetEmployeeCode();
            employeeModel.Dob = !string.IsNullOrEmpty(vm.DobStr) ? Utility.ConvertStrToDate(vm.DobStr) : null;
            employeeModel.JoinDate = (DateTime)(!string.IsNullOrEmpty(vm.JoinDateStr) ? Utility.ConvertStrToDate(vm.JoinDateStr) : vm.JoinDate);
            employeeModel.ProbationDate = !string.IsNullOrEmpty(vm.ProbationDateStr) ? Utility.ConvertStrToDate(vm.ProbationDateStr) : null;
            employeeModel.ConfirmationDate = !string.IsNullOrEmpty(vm.ConfirmationDateStr) ? Utility.ConvertStrToDate(vm.ConfirmationDateStr) : null;
            employeeModel.RetirementDate = !string.IsNullOrEmpty(vm.RetirementDateStr) ? Utility.ConvertStrToDate(vm.RetirementDateStr) : null;

            employeeModel.IsEnable = true;
            employeeModel.ActionById = CurrentUserId;
            employeeModel.ActionDate = Utility.GetBdDateTimeNow();

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await Repository.AddAsync(employeeModel);
            var isAdded = await _iUnitOfWork.CompleteAsync();
            if (!isAdded) return false;
            ts.Complete();
            return true;
        }

        #endregion

        #region GetEmployeeData

        public async Task<EmployeeVm> GetEmployeeDataAsync(long id)
        {
            var data = await Repository.GetEmployeeByIdAsync(id);
            //data.EmpLeaveInfo = await _iEmpLeaveAppService.EmployeeWiseLeaveBalance(data.Id, DateTime.Now.Year);
            return data;
        }

        #endregion

        #region GetEmployeeCode

        public async Task<string> GetEmployeeCode()
        {
            var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.Employees.ToString(), "Code", "EMP", 6);
            return data;
        }

        #endregion

        #region GetEmployeeDataListFromExcel

        private IEnumerable<EmployeeExcelModel> GetEmployeeDataListFromExcel(IFormFile importFile)
        {
            if (importFile == null || importFile.Length <= 0) return null;
            var employeeModels = new List<EmployeeExcelModel>();

            var dt = Utility.ConvertExcelOrCsVToDataTable(importFile);

            foreach (DataRow dataRow in dt.Rows)
            {
                var model = new EmployeeExcelModel
                {
                    Name = dataRow["Name"].ToString().Trim(),
                    Code = dataRow["Code"].ToString().Trim(),
                    Designation = dataRow["Designation"].ToString().Trim(),
                    Department = dataRow["Department"].ToString().Trim(),
                    Gender = dataRow["Gender"].ToString().Trim(),
                    BasicSalary = dataRow["BasicSalary"].ToString().Trim()
                };

                if (string.IsNullOrEmpty(model.Name)) continue;
                if (string.IsNullOrEmpty(model.Code)) continue;
                if (string.IsNullOrEmpty(model.Designation)) continue;
                if (string.IsNullOrEmpty(model.Department)) continue;
                if (string.IsNullOrEmpty(model.Gender)) continue;
                if (string.IsNullOrEmpty(model.BasicSalary)) continue;

                employeeModels.Add(model);
            }

            return employeeModels;
        }

        #endregion

        #region ExcelFileDownload

        public async Task<byte[]> ExcelFileDownload(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> model)
        {
            var dataTable = await Repository.SearchAllAsync(model);
            var data = dataTable.data;

            byte[] fileContents;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employees");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Name";
                worksheet.Cell(currentRow, 2).Value = "Code";
                worksheet.Cell(currentRow, 3).Value = "Designation";
                worksheet.Cell(currentRow, 4).Value = "Department";
                worksheet.Cell(currentRow, 5).Value = "Academic Department";
                worksheet.Cell(currentRow, 6).Value = "Job Status";
                worksheet.Cell(currentRow, 7).Value = "Joining Date";
                worksheet.FirstColumn().Width = 30;
                worksheet.ColumnWidth = 20;
                for (int i = 1; i <= 7; i++)
                {
                    worksheet.Cell(currentRow, i).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, i).Style.Font.FontSize = 12;
                }

                if (data.Count > 0)
                {
                    foreach (var (v, i) in data.GetItemWithIndex())
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = v.Name;
                        worksheet.Cell(currentRow, 2).Value = v.Code;
                        worksheet.Cell(currentRow, 3).Value = v.DesignationName;
                        worksheet.Cell(currentRow, 4).Value = v.DepartmentName;
                        worksheet.Cell(currentRow, 5).Value = v.AcademicDepartmentName;
                        worksheet.Cell(currentRow, 6).Value = v.EmployeeStatusText;
                        worksheet.Cell(currentRow, 7).Value = v.JoinDate.ToString("dd/MM/yyyy");
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    fileContents = stream.ToArray();
                }
            }
            return fileContents;
        }

        #endregion

        #region PrintHtml

        public async Task<string> EmployeeListPrintHtml(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> model)
        {
            var fullHtml = "";
            var dataTable = await Repository.SearchAllAsync(model);
            var data = dataTable.data;

            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;repeat-header:yes;'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl.</th>";
            fullHtml += "<th style='width:20%'>Employee Name</th>";
            fullHtml += "<th style='width:10%'>Employee Code</th>";
            fullHtml += "<th style='width:15%'>Designation</th>";
            fullHtml += "<th style='width:10%'>Job Status</th>";
            fullHtml += "<th style='width:10%'>Join Date</th>";
            fullHtml += "<th style='width:15%'>Department</th>";
            fullHtml += "<th style='width:15%'>Academic Department</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";

            fullHtml += "<tbody>";
            if (data.Count > 0)
            {
                foreach (var (v, i) in data.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td>{i + 1}</td>";
                    fullHtml += $@"<td class='text-start'>{v.Name}</td>";
                    fullHtml += $@"<td class='text-center'>{v.Code}</td>";
                    fullHtml += $@"<td class='text-center'>{v.DesignationName}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmployeeStatusText}</td>";
                    fullHtml += $@"<td class='text-center'>{v.JoinDate.ConvertDateToStr()}</td>";
                    fullHtml += $@"<td class='text-center'>{v.DepartmentName}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.AcademicDepartmentName}</ td >";

                    fullHtml += "</tr>";
                }
            }
            fullHtml += "</tbody>";
            fullHtml += "</table>";

            return fullHtml;
        }

        public async Task<string> EmployeeCVPrintHtml(long id, short refType)
        {
            var data = await Repository.GetEmployeeByIdAsync(id);
            var empExperience = await _iEmpExperienceService.GetEmpExperienceByEmpId(id);
            var empEducation = await _iEmpEducationService.GetEmpEducationByEmpId(id);
            var empReference = await _iEmpReferenceService.GetReferenceByEmpId(id, refType);
            var empPosting = await _iEmpPostingService.GetPostingByEmpId(id);
            var empTraining = await _iEmpTrainingService.GetTrainingByEmpId(id);
            var empDisciplinaryService = await _iEmpDisciplinaryService.GetEmpDisciplinaryByEmpId(id);
            var model = _iMapper.Map<EmployeeVm>(data);
            var fullHtml = "";

            // Basic information
            fullHtml += "<div style=''>";
            fullHtml += "<p style='padding-bottom:10px;'>";
            fullHtml += $@"<b style='font-size:22px;'>{model.Name}</b>";
            fullHtml += "</p>";

            fullHtml += $@"<table class='cv-info-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:30%;padding:3px 0px;'>Employee Code</td>
                                        <td style='width:70%;padding:3px 0px;'>: {model.Code}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding:3px 0px;'>Designation</td>
                                        <td style='padding:3px 0px;'>: {model.DesignationName}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding:3px 0px;'>Department</td>
                                        <td style='padding:3px 0px;'>: {model.DepartmentName}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding:3px 0px;'>Academic Department</td>
                                        <td style='padding:3px 0px;'>: {model.AcademicDepartmentName}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding:3px 0px;'>Mobile No.</td>
                                        <td style='padding:3px 0px;'>: {model.Mobile}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding:3px 0px;'>Email</td>
                                        <td style='padding:3px 0px;'>: {model.Email}</td>
                                    </tr>
                                </tbody>
                            </table>";
            fullHtml += "</div>";

            // Personal Info
            fullHtml += "<div style='padding-top: 20px;'>";
            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;font-size: 12px;'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th colspan='2' style='font-size:15px;' class='text-start'>Personal Info</th>";
            fullHtml += "<th colspan='2' style='font-size:15px;' class='text-start'>Employee Info</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            fullHtml += "<tr>";
            fullHtml += $"<td style='width:15%;'>Gender</td><td style='width:35%;'>{model.GenderText}</td>";
            fullHtml += $"<td style='width:15%;'>Status</td><td style='width:35%;'>{model.EmployeeStatusText}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $"<td>Nationality</td><td>{model.Nationality}</td>";
            fullHtml += $"<td>Confirmation Date</td><td>{Utility.ConvertDateToStr(model.ConfirmationDate)}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $"<td>Religion</td><td>{model.ReligionText}</td>";
            fullHtml += $"<td>Probation Date</td><td>{Utility.ConvertDateToStr(model.ProbationDate)}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $"<td>Marital Status</td><td>{model.MaritalStatusText}</td>";
            fullHtml += $"<td>Join Date</td><td>{model.JoinDate.ConvertDateToStr()}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $"<td>Permanent Address</td><td>{model.PerAddress}</td>";
            fullHtml += $"<td>Bank Account No.</td><td>{model.BankAccNo}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $"<td>Present Address</td><td>{model.PreAddress}</td>";
            fullHtml += $"<td>Basic Salary</td><td>{model.Salary}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $"<td colspan='2'></td>";
            fullHtml += $"<td>PF Member</td><td>{(model.IsPfMember ? "Member" : "Not A Member")}</td>";
            fullHtml += "</tr>";

            fullHtml += "</tbody>";
            fullHtml += "</table>";
            fullHtml += "</div>";

            // Experience
            if (empExperience.Count > 0)
            {
                fullHtml += "<div style='padding-top: 20px;'>";
                fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";
                fullHtml += "<thead>";
                fullHtml += "<tr><th colspan='4' style='font-size:15px;' class='text-start'>Experience</th></tr>";
                fullHtml += "<tr>";
                fullHtml += "<th style='width:25%'>Company Name</th>";
                fullHtml += "<th style='width:25%'>Designation</th>";
                fullHtml += "<th style='width:25%'>From Date</th>";
                fullHtml += "<th style='width:25%'>From Date</th>";
                fullHtml += "</tr>";
                fullHtml += "</thead>";

                fullHtml += "<tbody>";

                foreach (var v in empExperience)
                {
                    fullHtml += "<tr>";
                    fullHtml += $@"<td>{v.CompanyName}</td>";
                    fullHtml += $@"<td>{v.Designation}</td>";
                    fullHtml += $@"<td>{Utility.ConvertDateToStr(v.DateFrom)}</td>";
                    fullHtml += $@"<td>{Utility.ConvertDateToStr(v.DateTo)}</td>";
                    fullHtml += "</tr>";
                }

                fullHtml += "</tbody>";
                fullHtml += "</table>";
                fullHtml += "</div>";
            }

            // Education
            if (empEducation.Count > 0)
            {
                fullHtml += "<div style='padding-top: 20px;'>";
                fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";
                fullHtml += "<thead>";
                fullHtml += "<tr><th colspan='7' style='font-size:15px;' class='text-start'>Education</th></tr>";
                fullHtml += "<tr>";
                fullHtml += "<th style='width:25%'>Course</th>";
                fullHtml += "<th style='width:25%'>Institute</th>";
                fullHtml += "<th style='width:25%'>Board/University</th>";
                fullHtml += "<th style='width:25%'>From Year</th>";
                fullHtml += "<th style='width:25%'>To Year</th>";
                fullHtml += "<th style='width:25%'>Subject/Group</th>";
                fullHtml += "<th style='width:25%'>Passing year</th>";
                fullHtml += "</tr>";
                fullHtml += "</thead>";

                fullHtml += "<tbody>";

                foreach (var v in empEducation)
                {
                    fullHtml += "<tr>";
                    fullHtml += $@"<td>{v.CourseName}</td>";
                    fullHtml += $@"<td>{v.InstitutionName}</td>";
                    fullHtml += $@"<td>{v.BoardUniversity}</td>";
                    fullHtml += $@"<td>{v.YearFrom}</td>";
                    fullHtml += $@"<td>{v.YearTo}</td>";
                    fullHtml += $@"<td>{v.SubjectGroup}</td>";
                    fullHtml += $@"<td>{v.PassingYear}</td>";
                    fullHtml += "</tr>";
                }

                fullHtml += "</tbody>";
                fullHtml += "</table>";
                fullHtml += "</div>";
            }

            // Training
            if (empTraining.Count > 0)
            {
                fullHtml += "<div style='padding-top: 20px;'>";
                fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";
                fullHtml += "<thead>";
                fullHtml += "<tr><th colspan='6' style='font-size:15px;' class='text-start'>Training</th></tr>";
                fullHtml += "<tr>";
                fullHtml += "<th style='width:25%'>Course</th>";
                fullHtml += "<th style='width:25%'>Institute</th>";
                fullHtml += "<th style='width:25%'>Location</th>";
                fullHtml += "<th style='width:25%'>Start Date</th>";
                fullHtml += "<th style='width:25%'>End Date</th>";
                fullHtml += "<th style='width:25%'>Result</th>";
                fullHtml += "</tr>";
                fullHtml += "</thead>";

                fullHtml += "<tbody>";

                foreach (var v in empTraining)
                {
                    fullHtml += "<tr>";
                    fullHtml += $@"<td>{v.CourseTitle}</td>";
                    fullHtml += $@"<td>{v.InstituteName}</td>";
                    fullHtml += $@"<td>{v.Location}</td>";
                    fullHtml += $@"<td>{Utility.ConvertDateToStr(v.DateFrom)}</td>";
                    fullHtml += $@"<td>{Utility.ConvertDateToStr(v.DateTo)}</td>";
                    fullHtml += $@"<td>{v.Result}</td>";
                    fullHtml += "</tr>";
                }

                fullHtml += "</tbody>";
                fullHtml += "</table>";
                fullHtml += "</div>";
            }

            // Posting
            if (empPosting.Count > 0)
            {
                fullHtml += "<div style='padding-top: 20px;'>";
                fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";
                fullHtml += "<thead>";
                fullHtml += "<tr><th colspan='3' style='font-size:15px;' class='text-start'>Posting</th></tr>";
                fullHtml += "<tr>";
                fullHtml += "<th style='width:33%'>Previous Salary</th>";
                fullHtml += "<th style='width:33%'>Current Salary</th>";
                fullHtml += "<th style='width:34%'>Posting Type</th>";
                fullHtml += "</tr>";
                fullHtml += "</thead>";

                fullHtml += "<tbody>";

                foreach (var v in empPosting)
                {
                    fullHtml += "<tr>";
                    fullHtml += $@"<td>{v.PreNetSalary}</td>";
                    fullHtml += $@"<td>{v.NetSalary}</td>";
                    fullHtml += $@"<td>{v.PostingTypeText}</td>";
                    fullHtml += "</tr>";
                }

                fullHtml += "</tbody>";
                fullHtml += "</table>";
                fullHtml += "</div>";
            }

            // Disciplinary action
            if (empReference.Count > 0)
            {
                fullHtml += "<div style='padding-top: 20px;'>";
                fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";
                fullHtml += "<thead>";
                fullHtml += "<tr><th colspan='3' style='font-size:15px;' class='text-start'>Disciplinary Action</th></tr>";
                fullHtml += "<tr>";
                fullHtml += "<th style='width:33%'>Action Type</th>";
                fullHtml += "<th style='width:33%'>Reason</th>";
                fullHtml += "<th style='width:34%'>Committee</th>";
                fullHtml += "</tr>";
                fullHtml += "</thead>";

                fullHtml += "<tbody>";

                foreach (var v in empDisciplinaryService)
                {
                    fullHtml += "<tr>";
                    fullHtml += $@"<td>{v.ActionType}</td>";
                    fullHtml += $@"<td>{v.ActionReason}</td>";
                    fullHtml += $@"<td>{v.Committee}</td>";
                    fullHtml += "</tr>";
                }

                fullHtml += "</tbody>";
                fullHtml += "</table>";
                fullHtml += "</div>";
            }

            // Reference
            if (empReference.Count > 0)
            {
                fullHtml += "<div style='padding-top: 20px;'>";
                fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";
                fullHtml += "<thead>";
                fullHtml += "<tr><th colspan='4' style='font-size:15px;' class='text-start'>Reference</th></tr>";
                fullHtml += "<tr>";
                fullHtml += "<th style='width:33%'>Reference By</th>";
                fullHtml += "<th style='width:33%'>Occupation</th>";
                fullHtml += "<th style='width:34%'>Mobile</th>";
                fullHtml += "<th style='width:34%'>Relation</th>";
                fullHtml += "</tr>";
                fullHtml += "</thead>";

                fullHtml += "<tbody>";

                foreach (var v in empReference)
                {
                    fullHtml += "<tr>";
                    fullHtml += $@"<td>{v.RefName}</td>";
                    fullHtml += $@"<td>{v.Occupation}</td>";
                    fullHtml += $@"<td>{v.Mobile}</td>";
                    fullHtml += $@"<td>{v.Relation}</td>";
                    fullHtml += "</tr>";
                }

                fullHtml += "</tbody>";
                fullHtml += "</table>";
                fullHtml += "</div>";
            }

            return fullHtml;
        }

        #endregion

        #region Import

        public async Task<bool> ImportAsync(IFormFile importFile)
        {
            if (importFile == null || importFile.Length <= 0) return false;
            var employeeModels = GetEmployeeDataListFromExcel(importFile);

            if (employeeModels.Count() == 0) throw new Exception("No Data Found In Excel..!");

            var employeeList = new List<Employee>();

            foreach (var emp in employeeModels)
            {
                var employeeModel = new Employee();

                employeeModel.ActionById = CurrentUserId;
                employeeModel.ActionDate = Utility.GetBdDateTimeNow();

                if (string.IsNullOrEmpty(emp.Name)) 
                    throw new Exception($"Name Not Found");

                if (string.IsNullOrEmpty(emp.Code)) 
                    throw new Exception($"{emp.Name} Code Not Found");

                var existDesignation = await _iDesignationRepository.GetFirstOrDefaultAsync(c => c.Name.Equals(emp.Designation));
                if (existDesignation == null) 
                    throw new Exception($"{emp.Designation} Designation Not Found");

                var existDepartment = await _iDepartmentRepository.GetFirstOrDefaultAsync(c => c.Name.Equals(emp.Department) || c.Name.Contains(emp.Department));
                if (existDepartment == null) 
                    throw new Exception($"{emp.Department} Department Not Found");

                var gender = !string.IsNullOrEmpty(emp.Gender) && emp.Gender == "Male" ? "M" : !string.IsNullOrEmpty(emp.Gender) && emp.Gender == "Female" ? "F" : "";
                if (string.IsNullOrEmpty(gender)) 
                    throw new Exception($"{emp.Name} Gender Not Found");

                employeeModel.Name = emp.Name;
                employeeModel.Code = emp.Code;
                employeeModel.Gender = gender;
                employeeModel.DesignationId = existDesignation.Id;
                employeeModel.DepartmentId = existDepartment?.Id;
                employeeModel.Salary = Convert.ToDouble(emp.BasicSalary);
                employeeModel.SalaryType = 1;
                employeeModel.IsEnable = true;
                employeeModel.FatherName = "Auto Entry";
                employeeModel.JoinDate = DateTime.Now;
                employeeModel.EmployeeStatus = 1;

                var existEmployee = Repository.GetFirstOrDefault(c => c.Name.Equals(emp.Name) && c.Code.Equals(emp.Code));

                if (existEmployee != null) continue;

                employeeList.Add(employeeModel);
            }

            //var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await Repository.AddRangeAsync(employeeList);
            var isAdded = await _iUnitOfWork.CompleteAsync();
            if (!isAdded) return false;

            //ts.Complete();
            return true;

        }

        #endregion     
    }
}