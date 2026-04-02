using Domain.Entities.Accounting;
using Domain.Entities.Payroll;
using Domain.Entities.Pf;
using Domain.ViewModel.Accounting.AccTranMst;

namespace Interface.Services.Accounts;

public interface IAutoVoucherService
{
    Task<AccTranMst> GetAdvanceGivenQuickVoucher(EmpLoanMst advancePayment, long currentUserId, string ledgerCode);
    Task<AccTranMst> GetAdvanceReceivedQuickVoucher(EmpLoanDtl advanceReceive, long currentUserId, string ledgerCode);
    Task<AccTranMst> GetEmpFoodBillQuickVoucher(PrSalaryDtl prSalaryDtl, long currentUserId, string ledgerCode);
    Task<AccTranMst> GetDepartmentSalaryPayQuickVoucher(PayrollAutoVoucherVm payment, long currentUserId);
    Task<AccTranMst> GetEmpSalaryPayQuickVoucher(PrSalaryDtl payment, long currentUserId, string ledgerCode);
    Task<AccTranMst> GetEmpAttDeductionQuickVoucher(PrSalaryDtl prSalaryDtl, long currentUserId, string ledgerCode);
}
