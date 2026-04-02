using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrArrearMst;
using Interface.Repository.Payroll;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Payroll;

public class PrArrearMstService : BaseService<PrArrearMst>, IPrArrearMstService
{
    #region Config

    private IPrArrearMstRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public PrArrearMstService(IPrArrearMstRepository iRepository,
        IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm>> SearchAsync(DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region GetPrArrearById

    public async Task<PrArrearMstVm> GetPrArrearByIdAsync(long id)
    {
        var data = await Repository.GetPrArrearByIdAsync(id);
        var model = _iMapper.Map<PrArrearMstVm>(data);
        return model;
    }

    #endregion

    #region ArrearDetailsReportHtml

    public async Task<string> ArrearDetailsReportHtml(long id)
    {
        var model = await Repository.GetPrArrearByIdAsync(id);

        var fullHtml = "";

        fullHtml += "<div style='padding-bottom: 10px;'>";
        fullHtml += $@"<table>
                                <tbody>
                                    <tr>
                                        <td style='width:15%; font-size:12px'><b>Year</b></td>
                                        <td style='width:15%; font-size:12px'>{model.Year}</td>
                                        <td style='width:15%; font-size:12px'><b>Month</b></td>
                                        <td style='width:15%; font-size:12px'>{model.Month.ToString()}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:15%; font-size:12px'><b>Approval</b></td>
                                        <td style='width:15%; font-size:12px'>{(model.IsApproved ? "Approved" : "Not Approved")}</td>
                                        <td style='width:15%; font-size:12px'><b>Approve Date</b></td>
                                        <td style='width:15%; font-size:12px'>{model.ApprovedDate} </td>
                                    </tr>
                                    <tr>
                                        <td style='width:15%; font-size:12px'><b>Approve By</b></td>
                                        <td style='width:15%; font-size:12px'>{model.ApprovedBy} </td>
                                        <td style='width:15%; font-size:12px'><b>Remarks</b></td>
                                        <td style='width:15%; font-size:12px'>{model.Remarks} </td>
                                    </tr>
                                </tbody>
                            </table>";

        fullHtml += "</div>";

        fullHtml += "<table class='table' style='width:100%;text-align: center'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:5%'>Sl.</th>";
        fullHtml += "<th style='width:20%'>Employee</th>";
        fullHtml += "<th style='width:20%'>Amount</th>";
        fullHtml += "<th style='width:20%'>Arrear For</th>";
        fullHtml += "<th style='width:10%'>Remarks</th>";
        fullHtml += "<th style='width:25%'>Status</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (model.PrArrearDtls.Count > 0)
        {
            foreach (var (v, i) in model.PrArrearDtls.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td>{i + 1}</td>";
                fullHtml += $@"<td class='text-center'><b>{v.Employee.Name}</b></td>";
                fullHtml += $@"<td class='text-center'>{v.Amount}</td>";
                fullHtml += $@"<td class='text-center'>{v.ArrearFor}</td>";
                fullHtml += $@"<td class='text-center'>{v.Remarks}</td>";
                fullHtml += $@"<td class='text-center'>{(v.IsPaid ? "Paid" : "Pending")}</td>";

                fullHtml += "</tr>";
            }
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td style='text-align:center' colspan='9'><b>Employee Provident Fund Not Found</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return fullHtml;
    }

    #endregion
}
