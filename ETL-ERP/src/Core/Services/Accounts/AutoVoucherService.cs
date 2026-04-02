using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.HotelManagement;
using Domain.Entities.Payroll;
using Domain.Entities.Pf;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranMst;
using Interface.Repository.Common;
using Interface.Services.Accounts;
using Interface.Services.Admin;

namespace Services.Accounts;

public class AutoVoucherService : IAutoVoucherService
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly ISetCurrencyService _iSetCurrencyService;
    private readonly ISetFincYearService _iSetFincYearService;
    private readonly IAccLedgerService _iAccLedgerService;

    public AutoVoucherService(IMapper iMapper,
        IAutoCodeRepository iAutoCodeRepository,
        ISetCurrencyService iSetCurrencyService,
        ISetFincYearService iSetFincYearService,
        IAccLedgerService iAccLedgerService)
    {
        _iMapper = iMapper;
        _iAccLedgerService = iAccLedgerService;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iSetCurrencyService = iSetCurrencyService;
        _iSetFincYearService = iSetFincYearService;
    }

    #endregion

    #region GetAdvanceGivenQuickVoucher

    public async Task<AccTranMst> GetAdvanceGivenQuickVoucher(EmpLoanMst advancePayment, long currentUserId, string ledgerCode)
    {
        var model = new AccTranMst();
        model.VcDate = advancePayment.LoanPayDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Auto Voucher For Advance Paid";
        model.ActionById = currentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var accTranList = new List<AccTranDtl>();

        if (advancePayment != null)
        {
            var modelDtl = new AccTranDtl();

            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == ledgerCode);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.AdvanceMadeLedger);

            if (crLedger == null)
                throw new Exception("Mishuk Ledger Not Found..!!");
            if (drLedger == null)
                throw new Exception("No Ledger Found Against Advance Made To Stuffs..!!");

            model.AccAccountId = crLedger.Id;

            modelDtl.AmountDr = advancePayment.LoanAmount;
            modelDtl.AmountCr = advancePayment.LoanAmount;
            modelDtl.LedgerDrId = drLedger.Id;
            modelDtl.LedgerCrId = crLedger.Id;
            modelDtl.ActionById = currentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetAdvanceGivenQuickVoucher

    public async Task<AccTranMst> GetAdvanceReceivedQuickVoucher(EmpLoanDtl advanceReceive, long currentUserId, string ledgerCode)
    {
        var model = new AccTranMst();
        model.VcDate = (DateTime)advanceReceive.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Advance Recoverd";
        model.ActionById = currentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var accTranList = new List<AccTranDtl>();

        if (advanceReceive != null)
        {
            var modelDtl = new AccTranDtl();

            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.AdvanceRecoveredLedger);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == ledgerCode);

            if (crLedger == null)
                throw new Exception("No Ledger Found Against Advance Recovered Stuffs..!!");
            if (drLedger == null)
                throw new Exception("No Debit Ledger Found !!");

            model.AccAccountId = crLedger.Id;

            modelDtl.AmountDr = advanceReceive.InsAmount;
            modelDtl.AmountCr = advanceReceive.InsAmount;
            modelDtl.LedgerDrId = drLedger.Id;
            modelDtl.LedgerCrId = crLedger.Id;
            modelDtl.ActionById = currentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetEmpFoodBillQuickVoucher

    public async Task<AccTranMst> GetEmpFoodBillQuickVoucher(PrSalaryDtl prSalaryDtl, long currentUserId, string ledgerCode)
    {
        var model = new AccTranMst();
        model.VcDate = (DateTime)prSalaryDtl.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Employee Food Bill";
        model.ActionById = currentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;
        model.PrSalaryDtlId = prSalaryDtl.Id;

        var accTranList = new List<AccTranDtl>();

        if (prSalaryDtl != null)
        {
            var modelDtl = new AccTranDtl();

            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == ledgerCode);
            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.FoodPaymentReceive);

            if (crLedger == null)
                throw new Exception("No Ledger Found Against Advance Recovered Stuffs..!!");
            if (drLedger == null)
                throw new Exception("No Debit Ledger Found !!");

            model.AccAccountId = drLedger.Id;

            modelDtl.AmountDr = prSalaryDtl.ColE; //Salary Food Bill Amount
            modelDtl.AmountCr = prSalaryDtl.ColE; //Salary Food Bill Amount
            modelDtl.LedgerDrId = drLedger.Id;
            modelDtl.LedgerCrId = crLedger.Id;
            modelDtl.ActionById = currentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetFoodOrderPaymentQuickVoucher

    public async Task<AccTranMst> GetFoodOrderPaymentQuickVoucher(RsOrderPayments payment, string orderNo, long currentUserId)
    {
        var model = new AccTranMst();
        model.VcDate = payment.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Food bill receive. Order No:{orderNo}";
        model.ActionById = currentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RestaurantLedger);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.FoodPaymentReceive);


            if (crLedger == null)
                throw new Exception("No Ledger Found Against Food Bill Receive..!!");
            if (drLedger == null)
                throw new Exception("No Bank Ledger Found Against Restaurant..!!");

            model.AccAccountId = crLedger.Id;

            modelDtl.AmountDr = payment.PaidAmount;
            modelDtl.AmountCr = payment.PaidAmount;
            modelDtl.LedgerDrId = crLedger.Id;
            modelDtl.LedgerCrId = drLedger.Id;
            modelDtl.ActionById = currentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();
            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region DepartmentSalaryPayQuickVoucher

    public async Task<AccTranMst> GetDepartmentSalaryPayQuickVoucher(PayrollAutoVoucherVm payment, long currentUserId)
    {
        var model = new AccTranMst();
        model.VcDate = payment.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Auto Salary Payment";
        model.ActionById = currentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;
        //model.PayrollId = payment.PayrollId;
        
        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == payment.LedgerCode);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.SalaryPaidStaffs);

            if (crLedger == null)
                throw new Exception($"No Ledger Found Against {payment.LedgerCode}..!!");
            if (drLedger == null)
                throw new Exception("No Bank Ledger Found Against Salary Paid Stuffs..!!");

            model.AccAccountId = crLedger.Id;

            modelDtl.AmountDr = payment.PaidAmount;
            modelDtl.AmountCr = payment.PaidAmount;
            modelDtl.LedgerDrId = crLedger.Id;
            modelDtl.LedgerCrId = drLedger.Id;
            modelDtl.ActionById = currentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();
            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetEmpSalaryPayQuickVoucher

    public async Task<AccTranMst> GetEmpSalaryPayQuickVoucher(PrSalaryDtl payment, long currentUserId, string ledgerCode)
    {
        var model = new AccTranMst();
        model.VcDate = (DateTime)payment.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Auto Voucher For Salary Paid Stuffs";
        model.ActionById = currentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;
        model.PrSalaryDtlId = payment.Id;

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == ledgerCode);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.SalaryPaidStaffs);

            if (crLedger == null)
                throw new Exception($"{ledgerCode} Ledger Not Found..!!");
            if (drLedger == null)
                throw new Exception("No Ledger Found Against Salary Paid Stuffs..!!");
            
            model.AccAccountId = crLedger.Id;

            //modelDtl.AmountDr = payment.NetSalary;
            //modelDtl.AmountCr = payment.NetSalary;
            modelDtl.AmountDr = payment.GrossSalary;
            modelDtl.AmountCr = payment.GrossSalary;
            modelDtl.LedgerDrId = drLedger.Id;
            modelDtl.LedgerCrId = crLedger.Id;
            modelDtl.ActionById = currentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetAttendanceDeductQuickVoucher

    public async Task<AccTranMst> GetEmpAttDeductionQuickVoucher(PrSalaryDtl prSalaryDtl, long currentUserId, string ledgerCode)
    {
        var model = new AccTranMst();
        model.VcDate = (DateTime)prSalaryDtl.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Employee Salary Deduct";
        model.ActionById = currentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;
        model.PrSalaryDtlId = prSalaryDtl.Id;

        var accTranList = new List<AccTranDtl>();

        if (prSalaryDtl != null)
        {
            var modelDtl = new AccTranDtl();

            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == ledgerCode);
            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.SalaryDeductedStaffs);

            if (crLedger == null)
                throw new Exception("No Ledger Found Against Salary Deducted Staffs..!!");
            if (drLedger == null)
                throw new Exception("No Debit Ledger Found !!");

            model.AccAccountId = drLedger.Id;

            modelDtl.AmountDr = prSalaryDtl.ColU; //Salary Deducted Amount
            modelDtl.AmountCr = prSalaryDtl.ColU; //Salary Deducted Amount
            modelDtl.LedgerDrId = drLedger.Id;
            modelDtl.LedgerCrId = crLedger.Id;
            modelDtl.ActionById = currentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion
}
