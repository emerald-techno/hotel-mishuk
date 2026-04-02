using Domain.ViewModel.Accounting.AccTranMst;

namespace Interface.Repository.Accounts;

public interface IAccountReportRepository
{
    /*Ledger Report*/
    Task<List<AccLedgerReportVm>> GetLedgerReportData(AccReportVm vm);
    Task<List<AccLedgerReportVm>> GetQuickVoucherLedgerReportData(AccReportVm vm);
    Task<string> GetLedgerReportHtml(AccReportVm vm, bool isPrint = false);

    /*Trial Balance*/
    Task<List<AccTrialBalanceVm>> GetTrailBalanceData(AccReportVm vm);
    Task<string> GetTrialBalanceHtml(AccReportVm vm);
    Task<string> GetHeadWiseBalanceHtml(AccReportVm vm);
    Task<string> GetVoucherDetailHtml(long id);

    /*Receipt and Payments Reports*/
    Task<List<ReceiptAndPaymentVM>> GetReceiptAndPaymentsData(AccReportVm vm);
    Task<string> GetReceiptAndPaymentsHtml(AccReportVm vm);

    Task<List<ProfitLossVM>> GetProfitOrLossStatementData(AccReportVm vm);
    Task<string> GetProfitOrLossStatementHtml(AccReportVm vm);

    Task<string> GetQuickViewLedgerReportHtml(AccReportVm vm, bool isPrint = false);
}