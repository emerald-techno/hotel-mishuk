using Domain.Entities.Accounting;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranMst;
using Interface.Base;
using Interface.Repository.Accounts;
using Interface.Repository.HotelManagement;
using Interface.UnitOfWork;
using System.Transactions;

namespace Interface.Services.Accounts
{
    public interface IAccTranMstService : IService<AccTranMst>
    {
        Task<AccTranMstVm> GetJournalDataAsync(long id);
        Task<string> GetVoucherAutoCode(string voucherTypeCode, DateTime vcDate);
        Task<long> AddAccTran(AccTranMstVm vm);
        Task<DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm>> SearchAsync(DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm> model);
        Task<string> GetLedgerReportHtml(AccReportVm vm, bool isPrint);
        Task<string> GetTrialBalanceHtml(AccReportVm vm);
        Task<string> GetHeadWiseBalanceHtml(AccReportVm vm);
        Task<string> GetVoucherDetaliHtml(long id);
        Task<string> GetReceiptAndPaymentsReportHtml(AccReportVm vm);
        Task<AccTranMstVm?> GetOpeningDataAsync(long finYearId);
        Task<long> UpdateOpeningTransaction(AccTranMstVm vm);
        Task<string> GetProfitOrLossStatementHtml(AccReportVm vm);

        Task<AccTranMstVm> GetClosingDataAsync(long finYearId);
        Task<long> AddQuickVc(QuickVoucherVm vm);
        Task<string> GetQuickLedgerReportHtml(AccReportVm vm, bool isPrint);
        Task<QuickVoucherVm> GetQuickVoucherByVcId(long vcId);
        Task<bool> UpdateQuickVc(QuickVoucherVm vm);

        Task<bool> VoucherRemoveAsync(long voucherId);

    }
}
