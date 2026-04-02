using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrGuestSalary;
using Interface.Repository.Payroll;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Payroll
{
    public class PrGuestSalaryMstService : BaseService<PrGuestSalaryMst>, IPrGuestSalaryMstService
    {
        #region Config

        private IPrGuestSalaryMstRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public PrGuestSalaryMstService(IPrGuestSalaryMstRepository iRepository,
            IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm>> SearchAsync(DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion

        #region GetGuestSalaryMstById

        public async Task<PrGuestSalaryMstVm> GetGuestSalaryMstByIdAsync(long id)
        {
            var data = await Repository.GetPrGuestSalaryMstByIdAsync(id);
            var model = _iMapper.Map<PrGuestSalaryMstVm>(data);
            return model;
        }

        #endregion

        #region GuestSalaryDetailHtml

        public async Task<string> GuestSalaryDetailHtml(long id)
        {
            var data = await Repository.GetPrGuestSalaryMstByIdAsync(id);
            var model = _iMapper.Map<PrGuestSalaryMstVm>(data);
            var approvalStr = model.IsApproved ? "Approved" : "Not Approved";
            var fullHtml = "";

            fullHtml += "<div style='padding-bottom: 10px;'>";
            fullHtml += $@"<table class='table' id='GuestSalaryDetailTablePrint' style='width:100%;text-align: center'>
                                <tbody>
                                    <tr>
                                        <td style='width:25%'> Year </td>
                                        <td style='width:25%'>{model.Year} </td>
                                        <td style='width:25%'> Month </td>
                                        <td style='width:25%'> {Utility.GetMonthName(model.Month)}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:25%'> Approval </td>
                                        <td style='width:25%'>{approvalStr}</td>
                                        <td style='width:25%'> Approve Date </td>
                                        <td style='width:25%'>{Utility.ConvertDateToStr(model.ApprovedDate)}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:25%'> Approve By </td>
                                        <td style='width:25%'> {model.ApprovedByFullName} </td>
                                        <td style='width:25%'> Remarks </td>
                                        <td style='width:25%'> {model.Remarks} </td>
                                    </tr>
                                </tbody>
                            </table>";
            fullHtml += "</div>";

            fullHtml += "<table class='table table-bordered' id='GuestSalaryInfoTablePrint' style='width:100%;text-align: center'>";
            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl.</th>";
            fullHtml += "<th style='width:50%'>Employee</th>";
            fullHtml += "<th style='width:10%'>Amount</th>";
            fullHtml += "<th style='width:35%'>Remarks</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (model.PrGuestSalaryDtls.Count > 0)
            {
                foreach (var (v, i) in model.PrGuestSalaryDtls.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td>{i + 1}</td>";
                    fullHtml += $@"<td class='text-start'><b>{v.EmployeeName}</b></td>";
                    fullHtml += $@"<td class='text-center'>{v.Amount}</td>";
                    fullHtml += $@"<td class='text-center'>{v.Remarks}</td>";

                    fullHtml += "</tr>";
                }
            }
            fullHtml += "</tbody>";
            fullHtml += "</table>";

            return fullHtml;
        }

        #endregion
    }
}
