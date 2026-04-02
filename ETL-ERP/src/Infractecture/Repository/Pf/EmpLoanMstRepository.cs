using AutoMapper;
using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.Pf.EmpLoan;
using Interface.Repository.Pf;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Pf
{
    public class EmpLoanMstRepository : BaseRepository<EmpLoanMst>, IEmpLoanMstRepository, IDisposable
    {
        #region Config

        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;
        private readonly IApplicationReadDbConnection _iReadDbConnection;
        public EmpLoanMstRepository(ApplicationDbContext db, IMapper iMapper, IApplicationReadDbConnection iReadDbConnection) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
            _iReadDbConnection = iReadDbConnection;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm>> SearchAsync(DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm> vm)
        {
            var searchResult = Context.EmpLoanMsts
                .Include(c => c.Employee)
                .Include(c => c.EmpLoanDtls)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Arrear not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Employee.Name.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.FirstInsDate)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<EmpLoanMstSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeName = filterData?.Employee.Name;
                    searchDto.InstalmentCount = filterData?.EmpLoanDtls.Count() ?? 0;
                }
            }
            return vm;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion

        #region EmployeeLoanReportHtml
        public async Task<List<EmployeeLoanReportVm>> GetEmployeeLoanReport(EmployeeLoanReportVm vm)
        {
            var reportDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrQueryDate)).ToString("dd/MMM/yyyy");
            vm.StrQueryDate = (string.IsNullOrEmpty(vm.StrQueryDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;

            var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
            var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

            string fromDateFilter = !string.IsNullOrEmpty(vm.StrFromDate) ?  $" and convert(date,ld.InsDate) >= '{fromDate}'" : "";
            string toDateFilter = !string.IsNullOrEmpty(vm.StrFromDate) ? $" and convert(date,ld.InsDate) <= '{toDate}'": "";
            string employeeFilter = vm.EmployeeId> 0 ? $" and lm.EmployeeId = {vm.EmployeeId}" : "";

            string query = $@"select e.id EmployeeId,e.Code EmployeeCode,e.Name EmployeeName,e.Mobile,e.Email,sd.Name Designation,dp.Name Department, lm.Id LoanId, 
                                lm.LoanAmount,lm.InterestRate,lm.InsAmount InstallmentAmount,convert(Date,lm.FirstInsDate)FirstInsDate,convert(date,lm.LoanPassDate)LoanPassDate,convert(date,lm.LoanPayDate)LoanPayDate
                                ,count(ld.id)  TotalInstallment,sum(case when ld.IsPaid = 'true' then 1 else 0 end) PaidInstallment,count(ld.id) - sum(case when ld.IsPaid = 'true' then 1 else 0 end) RemainingInstallment
                                ,sum(case when ld.IsPaid = 'true' then ld.LoanAmount else 0 end) PaidLoanAmount,sum(case when ld.IsPaid = 'false' then ld.LoanAmount else 0 end) RemainingLoanAmount
                                ,sum(case when ld.IsPaid = 'true' then ld.InterestAmount else 0 end) PaidInterestAmount,sum(case when ld.IsPaid = 'false' then ld.InterestAmount else 0 end) RemainingInterestAmount
                                ,sum(case when ld.IsPaid = 'true' then ld.InsAmount else 0 end) PaidAmount,sum(case when ld.IsPaid = 'false' then ld.InsAmount else 0 end) RemainingAmount
                                ,sum(ld.InsAmount) TotalPayableAmount,sum(ld.InterestAmount) TotalInterestAmount,
                                (select MIN(ld2.InsDate) 
                                   from EmpLoanDtls ld2 
                                   where ld2.LoanId = lm.Id 
                                     and ld2.IsPaid = 0 
                                     and ld2.IsDeleted = 0) as NextInstallmentDate
                                from EmpLoanDtls ld
                                inner join EmpLoanMsts lm on lm.Id = ld.LoanId
                                inner join Employees e on e.Id = lm.EmployeeId
                                left join Designations sd on sd.Id = e.DesignationId
                                left join Departments dp on dp.Id = e.DepartmentId
                                where ld.IsDeleted = 0 and lm.IsDeleted = 0 and ld.IsPaid = 0 {fromDateFilter} {toDateFilter} {employeeFilter}
                                group by e.id,e.Code,e.Name,e.Mobile,e.Email,sd.Name,dp.Name,lm.Id,lm.LoanAmount,lm.InterestRate,lm.InsAmount,
                                convert(Date,lm.FirstInsDate),convert(date,lm.LoanPassDate),convert(date,lm.LoanPayDate)";

            var data = await _iReadDbConnection.QueryAsync<EmployeeLoanReportVm>(query);
            return data.ToList();
        }
        public async Task<string> EmployeeLoanReportHtml(EmployeeLoanReportVm vm)
        {
            try
            {
                string fullHtml = "";
                List<EmployeeLoanReportVm> objDataList = await GetEmployeeLoanReport(vm);

                if (objDataList.Count > 0)
                {
                    fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                    fullHtml += "<thead>";

                    fullHtml += $@"<tr style='height:30px;'><td colspan='13' class='text-center'>Date: {DateTime.Today:dd/MMM/yyyy}</td></tr>";

                    fullHtml += "<tr style='height:30px;' class='text-center'>";

                    fullHtml += $@"<th style='width:40px;' >SL</th>
                            <th style='width:80px;'> Employee Code</th>
                            <th style='width:110px;'> Employee Name </th>
                            <th style='width:80px;'> Loan Date</th>
                            <th style='width:130px;'> Loan Amount </th>
                            <th style='width:60px;'> Interest Rate</th>
                            <th style='width:60px;'> Interest Amount</th>
                            <th style='width:100px;'> Total Payable</th>
                            <th style='width:90px;'> Total Installment</th>
                            <th style='width:100px;'> Paid Installment</th>
                            <th style='width:100px;'> Remaining Installment</th>
                            <th style='width:90px;'> Return Amount</th>
                            <th style='width:90px;'> Loan Due/ Next Ins. Date</th>";

                    fullHtml += "</tr>";

                    fullHtml += "</thead>";
                    fullHtml += "<tbody>";

                    for (int i = 0; i < objDataList.Count; i++)
                    {
                        EmployeeLoanReportVm objItem = objDataList[i];
                        fullHtml += "<tr>";

                        fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>";
                        fullHtml += $@"<td style='text-align:center;'><b>{objItem.EmployeeCode}</b></td>                              
                                <td style='text-align:center;'>{objItem.EmployeeName} <br/> {objItem.Designation}</td>
                                <td style='text-align:center;'>{objItem.LoanPassDate:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objItem.LoanAmount}</td>
                                <td style='text-align:center;'>{objItem.InterestRate}</td>
                                <td style='text-align:center;'>{objItem.TotalInterestAmount}</td>
                                <td style='text-align:center;'>{objItem.TotalPayableAmount:N2}</td>
                                <td style='text-align:center;'>{objItem.TotalInstallment}</td>
                                <td style='text-align:center;'>{objItem.PaidInstallment}</td>
                                <td style='text-align:center;'>{objItem.RemainingInstallment}</td>
                                <td style='text-align:center;'>{objItem.PaidAmount:F2}</td>
                                <td style='text-align:center;'>{objItem.RemainingAmount:F2} <br /> {objItem.NextInstallmentDate:dd/MMM/yyyy} </td>";
                        fullHtml += "</tr>";

                    }

                    fullHtml += "</tbody>";

                    fullHtml += "<tfoot>";
                    fullHtml += $@"<tr><td colspan='4' style='text-align:right;'><b>TOTAL</b></td>                              
                                <td style='text-align:center;'>{objDataList.Sum(o => o.LoanAmount)}</td>
                                <td style='text-align:center;'>{objDataList.Average(o => o.InterestRate):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalInterestAmount):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalPayableAmount):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalInstallment)}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.PaidInstallment)}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.RemainingInstallment)}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.PaidAmount):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.RemainingAmount):F2}</td>";
                    fullHtml += "</tr>";
                    fullHtml += "</tfoot>";


                    fullHtml += "</table>";

                }


                return fullHtml;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }

}
