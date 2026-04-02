using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryDtl;
using Domain.ViewModel.Payroll.PrSalaryMst;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Repository.Payroll;
using Interface.Repository.Pf;
using Interface.Repository.Restaurant;
using Interface.Services.Admin;
using Interface.Services.Hr;
using Interface.Services.Payroll;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Payroll;

public class PrSalaryMstService : BaseService<PrSalaryMst>, IPrSalaryMstService
{
    #region Config
    private readonly IPrSalaryMstRepository Repository;
    private readonly IPrSalaryPartService _iPartService;
    private readonly IPrEmpSalaryPartService _iPrEmpSalaryPartService;
    private readonly IMonthlyAttSheetMstService _iMonthlyAttSheetMstService;
    private readonly IEmployeeService _iEmployeeService;
    private readonly IPrArrearMstService _iPrArrearMstService;
    private readonly IEmpLoanMstService _iEmpLoanMstService;
    private readonly IPfFundOpenningService _iPfFundOpenningService;
    private readonly IPfSettingRepository _iPfSettingRepository;
    private readonly IPrArrearDtlRepository _iArrearDtlRepository;
    private readonly IEmpLoanDtlRepository _iEmpLoanDtlRepository;
    private readonly IPrGuestSalaryMstRepository _iPrGuestSalaryMstRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IFoodOrderRepository _iFoodOrderRepository;
    private readonly IRsOrderPaymentRepository _iOrderPaymentRepository;
    private readonly ICustomerRepository _iCustomerRepository;
    private readonly IDepartmentService _iDepartmentService;

    public PrSalaryMstService(IPrSalaryMstRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IPrEmpSalaryPartService iPrEmpSalaryPartService,
        IMonthlyAttSheetMstService iMonthlyAttSheetMstService, IEmployeeService iEmployeeService, IPrSalaryPartService iPartService,
        IPrArrearMstService iPrArrearMstService, IEmpLoanMstService iEmpLoanMstService, IPfFundOpenningService iPfFundOpenningService,
        IPfSettingRepository iPfSettingRepository, IPrArrearDtlRepository iArrearDtlRepository, IEmpLoanDtlRepository iEmpLoanDtlRepository,
        IPrGuestSalaryMstRepository iPrGusetSalaryMstRepository,
        IFoodOrderRepository iFoodOrderRepository,
        ICustomerRepository iCustomerRepository,
        IRsOrderPaymentRepository iOrderPaymentRepository,
        IDepartmentService iDepartmentService) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iMonthlyAttSheetMstService = iMonthlyAttSheetMstService;
        _iPrEmpSalaryPartService = iPrEmpSalaryPartService;
        _iEmployeeService = iEmployeeService;
        _iPartService = iPartService;
        _iPrArrearMstService = iPrArrearMstService;
        _iEmpLoanMstService = iEmpLoanMstService;
        _iPfFundOpenningService = iPfFundOpenningService;
        _iPfSettingRepository = iPfSettingRepository;
        _iArrearDtlRepository = iArrearDtlRepository;
        _iEmpLoanDtlRepository = iEmpLoanDtlRepository;
        _iPrGuestSalaryMstRepository = iPrGusetSalaryMstRepository;
        _iFoodOrderRepository = iFoodOrderRepository;
        _iCustomerRepository = iCustomerRepository;
        _iOrderPaymentRepository = iOrderPaymentRepository;
        _iDepartmentService = iDepartmentService;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm>> SearchAsync(DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region CalculatePayroll

    public async Task<PrSalaryMstVm> CalculatePayroll(PrSalaryMstVm vm)
    {
        if (!(vm.Year > 2000) && !(vm.Month > 0)) throw new Exception("Please Select Month And Year");

        var monthlySheet = await _iMonthlyAttSheetMstService.GetFirstOrDefaultAsync(c => c.Year == vm.Year && c.Month == vm.Month, m => m.MonthlyAttendanceSheetDtls);

        if (monthlySheet == null) throw new Exception("Sheet Not Created Yet");

        var salaryResult = new PrSalaryMstVm();

        salaryResult.Year = monthlySheet.Year;
        salaryResult.Month = monthlySheet.Month;
        salaryResult.DateFrom = monthlySheet.DateFrom;
        salaryResult.DateTo = monthlySheet.DateTo;
        salaryResult.SalaryDateStr = vm.SalaryDateStr;

        var salaryDate = (DateTime)(!string.IsNullOrEmpty(salaryResult.SalaryDateStr) ? DU.Utility.ConvertStrToDate(salaryResult.SalaryDateStr) : DateTime.Now);

        var enableParts = _iPartService.Get(c => c.IsEnable);
        salaryResult.EnablePrSalaryParts = _iMapper.Map<List<PrSalaryPartVm>>(enableParts);

        var presentEmployees = monthlySheet.MonthlyAttendanceSheetDtls.Where(c => c.PresentDays > 0).ToList();

        var salaryMonth = new DateTime(vm.Year, vm.Month, 1);
        var preMonth = salaryMonth.AddMonths(-1);

        var prArrear = await _iPrArrearMstService.GetFirstOrDefaultAsync(c => c.Year == preMonth.Year && c.Month == preMonth.Month && c.IsApproved, d => d.PrArrearDtls);

        var dtlList = new List<PrSalaryDtlVm>();

        if (presentEmployees.Count > 0)
        {
            var employeeListIds = presentEmployees.Select(x => x.EmployeeId).ToList();
            var employeeCustomerList = await _iCustomerRepository.GetAsync(x => employeeListIds.Contains((long)x.EmployeeId) && !x.IsDeleted);

            foreach (var employeeSheet in presentEmployees)
            {
                var dtlModel = new PrSalaryDtlVm();

                var employee = await _iEmployeeService.GetFirstOrDefaultAsync(c => c.Id == employeeSheet.EmployeeId && c.IsEnable && !c.IsDeleted, d => d.Department, ds => ds.Designation);
                if (employee == null)
                {
                    continue;
                }
                if (employee.Salary == 0) continue;
                var employeeParts = await _iPrEmpSalaryPartService.GetAsync(c => c.EmployeeId == employeeSheet.EmployeeId, p => p.SalaryPart);

                double additionValue = 0;
                double deductionValue = 0;

                dtlModel.EmployeeId = employee.Id;
                dtlModel.EmployeeName = employee.Name;
                dtlModel.EmployeeCode = employee.Code;
                dtlModel.EmpDepartmentName = employee.Department?.Name;
                dtlModel.EmpDepartmentCode = employee.Department?.Code;
                dtlModel.EmpDesignationName = employee.Designation?.Name;
                dtlModel.EmpDesignationCode = employee.Designation?.Code;
                dtlModel.SetDepartmentSlNo();

                //var perDaySalary = (employee.Salary / employeeSheet.TotalDays);
                //employee.Salary = Math.Ceiling(perDaySalary * employeeSheet.PayDays);

                if (employee.EmployeeStatus == (short)EmployeeStatusEnum.Guest)
                {
                    var prevGuestSalary = await _iPrGuestSalaryMstRepository.GetFirstOrDefaultAsync(c => c.Year == vm.Year && c.Month == vm.Month && c.IsApproved, d => d.PrGuestSalaryDtls);
                    if (prevGuestSalary == null) continue;

                    if (prevGuestSalary != null && prevGuestSalary.PrGuestSalaryDtls != null && prevGuestSalary.PrGuestSalaryDtls.Count > 0)
                    {
                        var guestEmp = prevGuestSalary.PrGuestSalaryDtls.FirstOrDefault(c => c.EmployeeId == employee.Id);

                        var guestSalary = guestEmp != null ? guestEmp.Amount : 0;

                        dtlModel.NetSalary = guestSalary;
                        dtlModel.DeductionValue = 0;
                        dtlList.Add(dtlModel);
                        continue;
                    }
                }

                #region PartValueAssign

                var basicSalary = enableParts.FirstOrDefault(c => c.PartCode == "BASIC");

                if (basicSalary != null && basicSalary.IsEmpWise)
                {
                    var basicEmpSalary = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == "BASIC");
                    //dtlModel.BasicSalary = AssignEmpPartValue(basicEmpSalary, employee.Salary);
                    dtlModel.BasicSalary = employee.Salary;

                    if (basicEmpSalary != null && basicEmpSalary.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.BasicSalary;
                    }
                    else
                    {
                        deductionValue += dtlModel.BasicSalary;
                    }

                }
                else if (basicSalary != null)
                {
                    //dtlModel.BasicSalary = AssignPartValue(basicSalary, employee.Salary);
                    dtlModel.BasicSalary = employee.Salary;

                    if (basicSalary != null && basicSalary.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.BasicSalary;
                    }
                    else
                    {
                        deductionValue += dtlModel.BasicSalary;
                    }
                }



                #region ColA__E                    

                var colA = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColA);

                if (colA != null && colA.IsEmpWise)
                {
                    var colEmpA = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColA);
                    dtlModel.ColA = AssignEmpPartValue(colEmpA, dtlModel.BasicSalary);

                    if (colEmpA != null && colEmpA.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColA;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColA;
                    }

                }
                else if (colA != null)
                {
                    dtlModel.ColA = AssignPartValue(colA, dtlModel.BasicSalary);

                    if (colA != null && colA.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColA;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColA;
                    }
                }


                var colB = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColB);

                if (colB != null && colB.IsEmpWise)
                {
                    var colEmpB = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColB);
                    dtlModel.ColB = AssignEmpPartValue(colEmpB, dtlModel.BasicSalary);

                    if (colEmpB != null && colEmpB.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColB;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColB;
                    }

                }
                else if (colB != null)
                {
                    dtlModel.ColB = AssignPartValue(colB, dtlModel.BasicSalary);

                    if (colB != null && colB.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColB;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColB;
                    }
                }

                var colC = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColC);

                if (colC != null && colC.IsEmpWise)
                {
                    var colEmpC = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColC);
                    dtlModel.ColC = AssignEmpPartValue(colEmpC, dtlModel.BasicSalary);

                    if (colEmpC != null && colEmpC.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColC;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColC;
                    }

                }
                else if (colC != null)
                {
                    dtlModel.ColC = AssignPartValue(colC, dtlModel.BasicSalary);

                    if (colC != null && colC.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColC;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColC;
                    }
                }


                var colD = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColD);

                if (colD != null && colD.IsEmpWise)
                {
                    var colEmpD = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColD);
                    dtlModel.ColD = AssignEmpPartValue(colEmpD, dtlModel.BasicSalary);

                    if (colEmpD != null && colEmpD.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColD;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColD;
                    }

                }
                else if (colC != null)
                {
                    dtlModel.ColD = AssignPartValue(colD, dtlModel.BasicSalary);

                    if (colD != null && colD.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColD;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColD;
                    }
                }


                //var colE = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColE);

                //if (colE != null && colE.IsEmpWise)
                //{
                //    var colEmpE = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColE);
                //    dtlModel.ColE = AssignEmpPartValue(colEmpE, dtlModel.BasicSalary);

                //    if (colEmpE != null && colEmpE.PartType == PrSalaryPartType.Addition)
                //    {
                //        additionValue += dtlModel.ColE;
                //    }
                //    else
                //    {
                //        deductionValue += dtlModel.ColE;
                //    }

                //}
                //else if (colE != null)
                //{
                //    dtlModel.ColE = AssignPartValue(colE, dtlModel.BasicSalary);

                //    if (colE != null && colE.PartType == PrSalaryPartType.Addition)
                //    {
                //        additionValue += dtlModel.ColE;
                //    }
                //    else
                //    {
                //        deductionValue += dtlModel.ColE;
                //    }
                //}

                var colEFoodBill = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColE && c.PartLink == PrSalaryPartLink.FoodBill);

                if (colEFoodBill != null && AppUtility.IsAutoFoodBill == true)
                {
                    var customer = employeeCustomerList.FirstOrDefault(x => x.EmployeeId == employee.Id);
                    if (customer != null)
                    {
                        var queryDate = new DateTime(2024, 10, 1);

                        var anyUnpaidFoodBillList = await _iFoodOrderRepository.GetAsync(x => x.CustomerId == customer.Id && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment
                        && x.OrderStatus != RsOrderStatusEnum.Canceled && x.OrderDate.Date >= queryDate && x.OrderDate.Date < salaryMonth.AddMonths(1).Date);

                        var unpaidIds = anyUnpaidFoodBillList.Select(x => x.Id).ToList();
                        var paidList = _iOrderPaymentRepository.Get(c => unpaidIds.Contains(c.OrderId) && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
                        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

                        var remainFoodBill = anyUnpaidFoodBillList.Sum(x => x.NetAmount) - (alreadyPaidAmount);

                        dtlModel.ColE = remainFoodBill > 0 ? remainFoodBill : 0;
                    }
                    else
                    {
                        dtlModel.ColE = 0;
                    }
                }
                else
                {
                    dtlModel.ColE = 0;
                }

                if (colEFoodBill != null && colEFoodBill.PartType == PrSalaryPartType.Addition)
                {
                    additionValue += dtlModel.ColE;
                }
                else
                {
                    deductionValue += dtlModel.ColE;
                }

                #endregion

                #region PF

                var colFPf = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColF && c.PartLink == PrSalaryPartLink.Pf);

                if (colFPf != null)
                {
                    //var existEmpPf = await _iPfFundOpenningService.GetFirstOrDefaultAsync(c => c.EmployeeId == employee.Id && DateTime.Now.Date > c.PfStartDate.Date);

                    if (employee.EmployeeStatus == (short)EmployeeStatusEnum.Permanent && employee.ConfirmationDate != null && salaryMonth > employee.ConfirmationDate && employee.IsPfMember)
                    {
                        var pfSettings = _iPfSettingRepository.GetFirstOrDefault();

                        double employeeCon = default;
                        //double companyCon = default;

                        if (pfSettings.PfSource == "G")
                        {
                            employeeCon = PercentCalculation(pfSettings.EmpCon, employee.Salary);
                            //companyCon = PercentCalculation(pfSettings.CompCon, employee.Salary);
                        }
                        else if (pfSettings.PfSource == "B")
                        {
                            employeeCon = PercentCalculation(pfSettings.EmpCon, dtlModel.BasicSalary);
                            //companyCon = PercentCalculation(pfSettings.CompCon, dtlModel.BasicSalary);
                        }

                        dtlModel.ColF = employeeCon;
                    }
                    else
                    {
                        dtlModel.ColF = 0;
                    }
                }

                if (colFPf != null && colFPf.PartType == PrSalaryPartType.Addition)
                {
                    additionValue += dtlModel.ColF;
                }
                else
                {
                    deductionValue += dtlModel.ColF;
                }

                #endregion

                #region ColG__H

                var colG = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColG);
                if (colG != null && colG.IsEmpWise)
                {
                    var colEmpG = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColG);
                    dtlModel.ColG = AssignEmpPartValue(colEmpG, dtlModel.BasicSalary);

                    if (colEmpG != null && colEmpG.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColG;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColG;
                    }

                }
                else if (colG != null)
                {
                    dtlModel.ColG = AssignPartValue(colG, dtlModel.BasicSalary);

                    if (colG != null && colG.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColG;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColG;
                    }
                }


                var colH = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColH);
                if (colH != null && colH.IsEmpWise)
                {
                    var colEmpH = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColH);
                    dtlModel.ColH = AssignEmpPartValue(colEmpH, dtlModel.BasicSalary);

                    if (colEmpH != null && colEmpH.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColH;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColH;
                    }

                }
                else if (colH != null)
                {
                    dtlModel.ColH = AssignPartValue(colH, dtlModel.BasicSalary);

                    if (colH != null && colH.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColH;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColH;
                    }
                }

                #endregion

                #region ArrearCol

                var colIArrear = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColI && c.PartLink == PrSalaryPartLink.Arrear);

                if (colIArrear != null)
                {
                    var existEmpArrears = prArrear != null && prArrear.PrArrearDtls.Count > 0 ? prArrear.PrArrearDtls.Where(c => c.EmployeeId == employee.Id && !c.IsPaid).ToList() : null;

                    dtlModel.ColI = existEmpArrears != null && existEmpArrears.Count > 0 ? existEmpArrears.Sum(c => c.Amount) : 0;
                }

                if (colIArrear != null && colIArrear.PartType == PrSalaryPartType.Addition)
                {
                    additionValue += dtlModel.ColI;
                }
                else
                {
                    deductionValue += dtlModel.ColI;
                }

                #endregion

                #region LoanCol

                var colJLoan = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColJ && c.PartLink == PrSalaryPartLink.Loan);

                //var existEmpLoan = await _iEmpLoanMstService.GetFirstOrDefaultAsync(c => c.EmployeeId == employee.Id 
                //&& salaryDate.Date > c.FirstInsDate.Date && c.Status == (short)LoanStatusEnum.Running, d => d.EmpLoanDtls);

                var existEmpLoanList = await _iEmpLoanMstService.GetAsync(c => c.EmployeeId == employee.Id
                && salaryDate.Date > c.FirstInsDate.Date && c.Status == (short)LoanStatusEnum.Running, d => d.EmpLoanDtls);

                if (colJLoan != null && AppUtility.IsLoanCutFromPayroll == true)
                {

                    if (existEmpLoanList != null && existEmpLoanList.Count > 0)
                    {
                        //var instalment = existEmpLoan.EmpLoanDtls.FirstOrDefault(c => !c.IsPaid && c.InsDate.Year == salaryResult.Year && c.InsDate.Month == salaryResult.Month);

                        var instalmentList = existEmpLoanList.SelectMany(x => x.EmpLoanDtls).Where(c => !c.IsPaid && !c.IsDeleted).OrderBy(s => s.Serial).ToList();

                        //var thisMonthInstalment = instalmentList.FirstOrDefault(x => x.InsDate.Month == salaryDate.Month);
                        var thisMonthInstalmentList = instalmentList.Where(x => x.InsDate.Month == salaryDate.Month).ToList();

                        //dtlModel.ColJ = thisMonthInstalment != null ? thisMonthInstalment.InsAmount - thisMonthInstalment.InterestAmount : 0;

                        dtlModel.ColJ = thisMonthInstalmentList != null && thisMonthInstalmentList.Count > 0 ? thisMonthInstalmentList.Sum(c => c.InsAmount) - thisMonthInstalmentList.Sum(c => c.InterestAmount) : 0;
                    }

                }
                else
                {
                    dtlModel.ColJ = 0;
                }

                if (colJLoan != null && colJLoan.PartType == PrSalaryPartType.Addition)
                {
                    additionValue += dtlModel.ColJ;
                }
                else
                {
                    deductionValue += dtlModel.ColJ;
                }

                #endregion

                #region LoanInterest

                //var colKLoanInterst = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColK && c.PartLink == PrSalaryPartLink.LoanInterest);

                //if (colKLoanInterst != null)
                //{
                //    if (existEmpLoan != null && existEmpLoan.Status != (short)LoanStatusEnum.Complete && existEmpLoan.EmpLoanDtls.Count > 0)
                //    {
                //        //var instalment = existEmpLoan.EmpLoanDtls.FirstOrDefault(c => !c.IsPaid && c.InsDate.Year == salaryResult.Year && c.InsDate.Month == salaryResult.Month);

                //        var instalmentList = existEmpLoan.EmpLoanDtls.Where(c => !c.IsPaid && !c.IsDeleted).OrderBy(s => s.Serial).ToList();

                //        var instalment = instalmentList.FirstOrDefault();

                //        dtlModel.ColK = instalment != null ? instalment.InterestAmount : 0;
                //    }

                //}

                //if (colKLoanInterst != null && colKLoanInterst.PartType == PrSalaryPartType.Addition)
                //{
                //    additionValue += dtlModel.ColK;
                //}
                //else
                //{
                //    deductionValue += dtlModel.ColK;
                //}

                #endregion

                #region ColL__T

                var colL = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColL);
                if (colL != null && colL.IsEmpWise)
                {
                    var colEmpL = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColL);
                    dtlModel.ColL = AssignEmpPartValue(colEmpL, dtlModel.BasicSalary);

                    if (colEmpL != null && colEmpL.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColL;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColL;
                    }

                }
                else if (colL != null)
                {
                    dtlModel.ColL = AssignPartValue(colL, dtlModel.BasicSalary);

                    if (colL != null && colL.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColL;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColL;
                    }
                }


                var colM = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColM);
                if (colM != null && colM.IsEmpWise)
                {
                    var colEmpM = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColM);
                    dtlModel.ColM = AssignEmpPartValue(colEmpM, dtlModel.BasicSalary);

                    if (colEmpM != null && colEmpM.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColM;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColM;
                    }

                }
                else if (colM != null)
                {
                    dtlModel.ColM = AssignPartValue(colM, dtlModel.BasicSalary);

                    if (colM != null && colM.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColM;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColM;
                    }
                }


                var colN = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColN);
                if (colN != null && colN.IsEmpWise)
                {
                    var colEmpN = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColN);
                    dtlModel.ColN = AssignEmpPartValue(colEmpN, dtlModel.BasicSalary);

                    if (colEmpN != null && colEmpN.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColN;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColN;
                    }

                }
                else if (colN != null)
                {
                    dtlModel.ColN = AssignPartValue(colN, dtlModel.BasicSalary);

                    if (colN != null && colN.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColN;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColN;
                    }
                }


                var colO = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColO);
                if (colO != null && colO.IsEmpWise)
                {
                    var colEmpO = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColO);
                    dtlModel.ColO = AssignEmpPartValue(colEmpO, dtlModel.BasicSalary);

                    if (colEmpO != null && colEmpO.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColO;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColO;
                    }

                }
                else if (colO != null)
                {
                    dtlModel.ColO = AssignPartValue(colO, dtlModel.BasicSalary);

                    if (colO != null && colO.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColO;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColO;
                    }
                }


                var colP = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColP);
                if (colP != null && colP.IsEmpWise)
                {
                    var colEmpP = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColP);
                    dtlModel.ColP = AssignEmpPartValue(colEmpP, dtlModel.BasicSalary);

                    if (colEmpP != null && colEmpP.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColP;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColP;
                    }

                }
                else if (colP != null)
                {
                    dtlModel.ColP = AssignPartValue(colP, dtlModel.BasicSalary);

                    if (colP != null && colP.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColP;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColP;
                    }
                }


                var colQ = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColQ);
                if (colQ != null && colQ.IsEmpWise)
                {
                    var colEmpQ = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColQ);
                    dtlModel.ColQ = AssignEmpPartValue(colEmpQ, dtlModel.BasicSalary);

                    if (colEmpQ != null && colEmpQ.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColQ;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColQ;
                    }

                }
                else if (colQ != null)
                {
                    dtlModel.ColQ = AssignPartValue(colQ, dtlModel.BasicSalary);

                    if (colQ != null && colQ.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColQ;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColQ;
                    }
                }


                var colR = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColR);
                if (colR != null && colR.IsEmpWise)
                {
                    var colEmpR = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColR);
                    dtlModel.ColR = AssignEmpPartValue(colEmpR, dtlModel.BasicSalary);

                    if (colEmpR != null && colEmpR.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColR;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColR;
                    }

                }
                else if (colR != null)
                {
                    dtlModel.ColR = AssignPartValue(colR, dtlModel.BasicSalary);

                    if (colR != null && colR.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColR;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColR;
                    }
                }


                var colS = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColS);
                if (colS != null && colS.IsEmpWise)
                {
                    var colEmpS = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColS);
                    dtlModel.ColS = AssignEmpPartValue(colEmpS, dtlModel.BasicSalary);

                    if (colEmpS != null && colEmpS.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColS;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColS;
                    }

                }
                else if (colS != null)
                {
                    dtlModel.ColS = AssignPartValue(colS, dtlModel.BasicSalary);

                    if (colS != null && colS.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColS;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColS;
                    }
                }


                var colT = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColT);
                if (colT != null && colT.IsEmpWise)
                {
                    var colEmpT = employeeParts.FirstOrDefault(c => c.SalaryPart.PartCode == PrSalaryPartCol.ColT);
                    dtlModel.ColT = AssignEmpPartValue(colEmpT, dtlModel.BasicSalary);

                    if (colEmpT != null && colEmpT.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColT;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColT;
                    }

                }
                else if (colT != null)
                {
                    dtlModel.ColT = AssignPartValue(colT, dtlModel.BasicSalary);

                    if (colT != null && colT.PartType == PrSalaryPartType.Addition)
                    {
                        additionValue += dtlModel.ColT;
                    }
                    else
                    {
                        deductionValue += dtlModel.ColT;
                    }
                }

                #endregion

                #region SalaryDeduct

                var colUSalaryDeduct = enableParts.FirstOrDefault(c => c.PartCode == PrSalaryPartCol.ColU && c.PartLink == PrSalaryPartLink.SalaryDeduct);

                if (colUSalaryDeduct != null)
                {
                    var absentDeduction = additionValue / employeeSheet.TotalDays * (employeeSheet.AbsentDays + employeeSheet.UnPaidLeaveDays);
                    dtlModel.ColU = Math.Round(absentDeduction);
                }

                if (colUSalaryDeduct != null && colUSalaryDeduct.PartType == PrSalaryPartType.Addition)
                {
                    additionValue += dtlModel.ColU;
                }
                else
                {
                    deductionValue += dtlModel.ColU;
                }

                #endregion

                #endregion

                //var absentDeduction = (additionValue / employeeSheet.TotalDays) * employeeSheet.AbsentDays;
                //dtlModel.AbsentDeductionValue = absentDeduction;

                //dtlModel.DeductionValue = Math.Round(deductionValue + absentDeduction);

                //dtlModel.GrossSalary = additionValue;
                //dtlModel.NetSalary = Math.Round(additionValue - (absentDeduction + deductionValue));

                dtlModel.DeductionValue = Math.Round(deductionValue);

                dtlModel.GrossSalary = additionValue;
                dtlModel.NetSalary = Math.Round(additionValue - deductionValue);

                dtlList.Add(dtlModel);
            }

        }

        salaryResult.PrSalaryDtls = dtlList.OrderBy(c => c.EmpDesignationCode).ThenByDescending(c => c.BasicSalary).ToList();
        salaryResult.TotalSalary = salaryResult.PrSalaryDtls.Sum(c => c.NetSalary);

        return salaryResult;

    }

    #endregion

    #region ApprovePrSalary

    public async Task<bool> ApprovePrSalary(long id, string remarks)
    {
        if (id == 0) return false;
        var payroll = await Repository.GetFirstOrDefaultAsync(c => c.Id == id, d => d.PrSalaryDtls);

        var prExistArrear = await _iPrArrearMstService.GetFirstOrDefaultAsync(c => c.Year == payroll.Year && c.Month == payroll.Month && c.IsApproved, d => d.PrArrearDtls);

        if (payroll == null) return false;

        var isUpdated = false;
        var isArrearAdded = false;
        var isArrearDtlAdded = false;

        var arrearModel = new PrArrearMst();
        arrearModel.Year = payroll.Year;
        arrearModel.Month = payroll.Month;
        arrearModel.ActionById = CurrentUserId;
        arrearModel.ActionDate = Utility.GetBdDateTimeNow();

        var arrearDtlList = new List<PrArrearDtl>();
        //var instalmentList = new List<EmpLoanDtl>();

        if (payroll.PrSalaryDtls != null && payroll.PrSalaryDtls.Count > 0)
        {
            foreach (var empSalary in payroll.PrSalaryDtls)
            {
                if (empSalary.IsHeldUp)
                {
                    var arrearDtlModel = new PrArrearDtl();
                    arrearDtlModel.EmployeeId = empSalary.EmployeeId;
                    arrearDtlModel.Amount = empSalary.GrossSalary;
                    arrearDtlModel.ArrearFor = "S";
                    arrearDtlModel.SalaryId = empSalary.Id;
                    arrearDtlModel.ArrearId = prExistArrear != null ? prExistArrear.Id : 0;

                    arrearDtlList.Add(arrearDtlModel);
                }

                //var existEmpLoan = await _iEmpLoanMstService.GetFirstOrDefaultAsync(c => c.EmployeeId == empSalary.EmployeeId && DateTime.Now.Date > c.FirstInsDate.Date, d => d.EmpLoanDtls);

                //if (existEmpLoan != null && existEmpLoan.Status == (short)LoanStatusEnum.Running && existEmpLoan.EmpLoanDtls.Count > 0)
                //{
                //    var instalments = existEmpLoan.EmpLoanDtls.Where(c => !c.IsPaid && !c.IsDeleted).OrderBy(s => s.Serial).ToList();

                //    var instalment = instalments.FirstOrDefault();

                //    if (instalment == null) continue;
                //    instalment.IsPaid = true;
                //    instalmentList.Add(instalment);
                //}
            }
        }

        payroll.ApprovedById = CurrentUserId;
        payroll.ApprovedDate = Utility.GetBdDateTimeNow();
        payroll.ApprovedRemarks = remarks;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.UpdateAsync(payroll);
        isUpdated = await _iUnitOfWork.CompleteAsync();

        if (prExistArrear != null && arrearDtlList.Count > 0)
        {
            _iArrearDtlRepository.AddRange(arrearDtlList);
            isArrearDtlAdded = await _iUnitOfWork.CompleteAsync();
        }
        else if (prExistArrear == null && arrearDtlList.Count > 0)
        {
            arrearModel.PrArrearDtls = arrearDtlList;
            _iPrArrearMstService.Add(arrearModel);
            isArrearAdded = await _iUnitOfWork.CompleteAsync();
        }

        //if (instalmentList != null && instalmentList.Count > 0)
        //{
        //    _iEmpLoanDtlRepository.UpdateRange(instalmentList);
        //    isInsPaid = await _iUnitOfWork.CompleteAsync();
        //}

        if (!isUpdated && (!isArrearDtlAdded || !isArrearAdded))
        {
            return false;
        }

        ts.Complete();
        return true;
    }

    #endregion

    #region GetPrSalaryById

    public async Task<PrSalaryMstVm> GetPrSalaryByIdAsync(long id)
    {
        var data = await Repository.GetPrSalaryByIdAsync(id);
        var model = _iMapper.Map<PrSalaryMstVm>(data);

        var enableParts = _iPartService.Get(c => c.IsEnable);
        model.EnablePrSalaryParts = _iMapper.Map<List<PrSalaryPartVm>>(enableParts);

        if (model.PrSalaryDtls.Count > 0)
        {
            foreach (var dtl in model.PrSalaryDtls)
            {
                var filterData = data.PrSalaryDtls.FirstOrDefault(c => c.Id == dtl.Id);

                dtl.EmployeeName = filterData?.Employee.Name;
                dtl.EmployeeCode = filterData?.Employee.Code;
                dtl.EmpDepartmentName = filterData?.Employee?.Department?.Name;
                dtl.EmpDesignationName = filterData?.Employee?.Designation?.Name;
            }
        }

        return model;
    }

    #endregion

    #region AssignPartValue

    private double AssignPartValue(PrSalaryPart salaryPart, double percentTotal)
    {
        double result = 0;

        if (salaryPart != null)
        {
            if (salaryPart.ValueType == PrSalaryPartValueType.Percent)
            {
                result = PercentCalculation(salaryPart.Value, percentTotal);
            }
            else
            {
                result = salaryPart?.Value ?? 0;
            }
        }

        return result;
    }

    #endregion

    #region AssignEmpPartValue

    private double AssignEmpPartValue(PrEmpSalaryPart salaryPart, double percentTotal)
    {
        double result = 0;

        if (salaryPart != null)
        {
            if (salaryPart.ValueType == PrSalaryPartValueType.Percent)
            {
                result = PercentCalculation(salaryPart.Value, percentTotal);
            }
            else
            {
                result = salaryPart?.Value ?? 0;
            }
        }

        return result;
    }

    #endregion

    #region PercentCalculation

    private double PercentCalculation(double percentValue, double totalNumber)
    {
        var result = Math.Ceiling(percentValue * totalNumber / 100);
        return result;
    }

    #endregion

    #region PrintHtml

    public async Task<string> PayrollHtml(long id)
    {

        try
        {
            var data = await Repository.GetPrSalaryByIdAsync(id);
            var model = _iMapper.Map<PrSalaryMstVm>(data);

            var enableParts = _iPartService.Get(c => c.IsEnable);
            model.EnablePrSalaryParts = _iMapper.Map<List<PrSalaryPartVm>>(enableParts);

            if (model.PrSalaryDtls.Count > 0)
            {
                foreach (var dtl in model.PrSalaryDtls)
                {
                    var filterData = data.PrSalaryDtls.FirstOrDefault(c => c.Id == dtl.Id);

                    dtl.EmployeeName = filterData?.Employee.Name;
                    dtl.EmployeeCode = filterData?.Employee.Code;
                    dtl.EmpDepartmentName = filterData?.Employee?.Department.Name;
                    dtl.EmpDesignationName = filterData?.Employee?.Designation.Name;

                    dtl.SetDepartmentSlNo();
                }
            }

            var enableAdditionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "A").ToList();
            var enableDiductionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "D").ToList();
            var addCount = enableAdditionParts.Count();
            var diductCount = enableDiductionParts.Count();

            string fullHtml = "";

            fullHtml += "<div class='text-center' style='repeat-header:yes;'>";

            fullHtml += $@"<b style='font-size:14px;'>THE MONTH OF {Utility.GetMonthName(model.Month).ToUpper()}-{model.Year}</b>";

            fullHtml += "</div>";

            fullHtml += "<table class='PayrollPrintTable' style='width:100%;repeat-header:yes;'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th rowspan='2' style='width:2%'>Sl.</th>";
            fullHtml += "<th rowspan='2' style='width:10%'>Employee Name</th>";
            fullHtml += "<th rowspan='2'> Employee Code</th>";
            fullHtml += "<th rowspan='2'>Designation</th>";
            fullHtml += "<th colspan='" + addCount + "'>Addition</th>";
            fullHtml += "<th rowspan='2'>Groos Salary</th>";
            fullHtml += "<th colspan='" + diductCount + "'>Deduction</th>";
            fullHtml += "<th rowspan='2'>Deduction Total</th>";
            fullHtml += "<th rowspan='2'>Net Salary</th>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            if (enableAdditionParts.Count > 0)
            {
                enableAdditionParts = enableAdditionParts.OrderBy(c => c.SlNo).ToList();

                foreach (var addItem in enableAdditionParts)
                {
                    fullHtml += "<th>" + addItem.PartName + "</th>";
                }
            }
            if (enableDiductionParts.Count > 0)
            {
                enableDiductionParts = enableDiductionParts.OrderBy(c => c.SlNo).ToList();

                foreach (var deductItem in enableDiductionParts)
                {
                    fullHtml += "<th>" + deductItem.PartName + "</th>";
                }
            }
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (model.PrSalaryDtls.Count > 0)
            {
                fullHtml += PayrollTableBodyHtml(model);
            }

            fullHtml += "</tbody>";

            fullHtml += "</table>";

            return fullHtml;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private string PayrollTableBodyHtml(PrSalaryMstVm model)
    {
        var fullHtml = "";

        var enableAdditionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "A").ToList();
        var enableDiductionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "D").ToList();
        var addCount = enableAdditionParts.Count();
        var diductCount = enableDiductionParts.Count();

        enableAdditionParts = enableAdditionParts.OrderBy(c => c.SlNo).ToList();
        enableDiductionParts = enableDiductionParts.OrderBy(c => c.SlNo).ToList();

        var totalSalarySpan = addCount + diductCount + 6;

        foreach (var (employee, i) in model.PrSalaryDtls.GetItemWithIndex())
        {
            fullHtml += "<tr style='background-color:white'>";

            fullHtml += "<td style='width:2%'>" + (i + 1) + "</td>";

            fullHtml += $@"<td>{employee.EmployeeName}</td>";
            fullHtml += $@"<td>{employee.EmployeeCode}</td>";
            fullHtml += $@"<td>{employee.EmpDesignationName}</td>";

            foreach (var item in enableAdditionParts)
            {
                if (item.PartCode == "BASIC")
                {
                    fullHtml += $@"<td class='text-end'>{employee.BasicSalary}</td>";
                }
                else if (item.PartCode == nameof(employee.ColA))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColA}</td>";
                }
                else if (item.PartCode == nameof(employee.ColB))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColB}</td>";
                }
                else if (item.PartCode == nameof(employee.ColC))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColC}</td>";
                }
                else if (item.PartCode == nameof(employee.ColD))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColD}</td>";
                }
                else if (item.PartCode == nameof(employee.ColE))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColE}</td>";
                }
                else if (item.PartCode == nameof(employee.ColF))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColF}</td>";
                }
                else if (item.PartCode == nameof(employee.ColG))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColG}</td>";
                }
                else if (item.PartCode == nameof(employee.ColH))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColH}</td>";
                }
                else if (item.PartCode == nameof(employee.ColI))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColI}</td>";
                }
                else if (item.PartCode == nameof(employee.ColK))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColK}</td>";
                }
                else if (item.PartCode == nameof(employee.ColL))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColL}</td>";
                }
                else if (item.PartCode == nameof(employee.ColM))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColM}</td>";
                }
                else if (item.PartCode == nameof(employee.ColN))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColN}</td>";
                }
                else if (item.PartCode == nameof(employee.ColO))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColO}</td>";
                }
                else if (item.PartCode == nameof(employee.ColP))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColP}</td>";
                }
                else if (item.PartCode == nameof(employee.ColQ))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColQ}</td>";
                }
                else if (item.PartCode == nameof(employee.ColR))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColR}</td>";
                }
                else if (item.PartCode == nameof(employee.ColS))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColS}</td>";
                }
                else if (item.PartCode == nameof(employee.ColT))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColT}</td>";
                }
                else if (item.PartCode == nameof(employee.ColU))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColU}</td>";
                }

            }

            fullHtml += $@"<td class='text-end'>{employee.GrossSalary}</td>";

            foreach (var item in enableDiductionParts)
            {
                if (item.PartCode == nameof(employee.ColA))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColA}</td>";
                }
                else if (item.PartCode == nameof(employee.ColB))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColB}</td>";
                }
                else if (item.PartCode == nameof(employee.ColC))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColC}</td>";
                }
                else if (item.PartCode == nameof(employee.ColD))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColD}</td>";
                }
                else if (item.PartCode == nameof(employee.ColE))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColE}</td>";
                }
                else if (item.PartCode == nameof(employee.ColF))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColF}</td>";
                }
                else if (item.PartCode == nameof(employee.ColG))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColG}</td>";
                }
                else if (item.PartCode == nameof(employee.ColH))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColH}</td>";
                }
                else if (item.PartCode == nameof(employee.ColJ))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColJ}</td>";
                }
                else if (item.PartCode == nameof(employee.ColK))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColK}</td>";
                }
                else if (item.PartCode == nameof(employee.ColL))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColL}</td>";
                }
                else if (item.PartCode == nameof(employee.ColM))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColM}</td>";
                }
                else if (item.PartCode == nameof(employee.ColN))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColN}</td>";
                }
                else if (item.PartCode == nameof(employee.ColO))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColO}</td>";
                }
                else if (item.PartCode == nameof(employee.ColP))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColP}</td>";
                }
                else if (item.PartCode == nameof(employee.ColQ))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColQ}</td>";
                }
                else if (item.PartCode == nameof(employee.ColR))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColR}</td>";
                }
                else if (item.PartCode == nameof(employee.ColS))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColS}</td>";
                }
                else if (item.PartCode == nameof(employee.ColT))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColT}</td>";
                }
                else if (item.PartCode == nameof(employee.ColU))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColU}</td>";
                }

            }

            fullHtml += $@"<td class='text-end'>{employee.TotalDeduction}</td>";
            fullHtml += $@"<td class='text-end'>{employee.NetSalary}</td>";

            fullHtml += "</tr>";
        }

        #region Summery Row
        fullHtml += "<tr>";
        //fullHtml += $@"<td colspan='4' class='text-end'><b>Total</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.BasicSalary).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColA).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColB).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColC).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColD).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.GrossSalary).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColE).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColJ).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColU).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.TotalDeduction).ToString("N2")}</b></td>
        //               <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.NetSalary).ToString("N2")}</b></td>";

        fullHtml += "<td colspan='4' class='text-end'><b>Total</b></td>";

        // Addition parts totals
        foreach (var item in enableAdditionParts)
        {
            double sum = 0;
            if (item.PartCode == "BASIC")
                sum = model.PrSalaryDtls.Sum(x => x.BasicSalary);
            else if (item.PartCode == PrSalaryPartCol.ColA)
                sum = model.PrSalaryDtls.Sum(x => x.ColA);
            else if (item.PartCode == PrSalaryPartCol.ColB)
                sum = model.PrSalaryDtls.Sum(x => x.ColB);
            else if (item.PartCode == PrSalaryPartCol.ColC)
                sum = model.PrSalaryDtls.Sum(x => x.ColC);
            else if (item.PartCode == PrSalaryPartCol.ColD)
                sum = model.PrSalaryDtls.Sum(x => x.ColD);
            else if (item.PartCode == PrSalaryPartCol.ColE)
                sum = model.PrSalaryDtls.Sum(x => x.ColE);
            else if (item.PartCode == PrSalaryPartCol.ColF)
                sum = model.PrSalaryDtls.Sum(x => x.ColF);
            else if (item.PartCode == PrSalaryPartCol.ColG)
                sum = model.PrSalaryDtls.Sum(x => x.ColG);
            else if (item.PartCode == PrSalaryPartCol.ColH)
                sum = model.PrSalaryDtls.Sum(x => x.ColH);
            else if (item.PartCode == PrSalaryPartCol.ColI)
                sum = model.PrSalaryDtls.Sum(x => x.ColI);
            else if (item.PartCode == PrSalaryPartCol.ColK)
                sum = model.PrSalaryDtls.Sum(x => x.ColK);
            else if (item.PartCode == PrSalaryPartCol.ColL)
                sum = model.PrSalaryDtls.Sum(x => x.ColL);
            else if (item.PartCode == PrSalaryPartCol.ColM)
                sum = model.PrSalaryDtls.Sum(x => x.ColM);
            else if (item.PartCode == PrSalaryPartCol.ColN)
                sum = model.PrSalaryDtls.Sum(x => x.ColN);
            else if (item.PartCode == PrSalaryPartCol.ColO)
                sum = model.PrSalaryDtls.Sum(x => x.ColO);
            else if (item.PartCode == PrSalaryPartCol.ColP)
                sum = model.PrSalaryDtls.Sum(x => x.ColP);
            else if (item.PartCode == PrSalaryPartCol.ColQ)
                sum = model.PrSalaryDtls.Sum(x => x.ColQ);
            else if (item.PartCode == PrSalaryPartCol.ColR)
                sum = model.PrSalaryDtls.Sum(x => x.ColR);
            else if (item.PartCode == PrSalaryPartCol.ColS)
                sum = model.PrSalaryDtls.Sum(x => x.ColS);
            else if (item.PartCode == PrSalaryPartCol.ColT)
                sum = model.PrSalaryDtls.Sum(x => x.ColT);
            else if (item.PartCode == PrSalaryPartCol.ColU)
                sum = model.PrSalaryDtls.Sum(x => x.ColU);

            fullHtml += $@"<td class='text-end'><b>{sum:N2}</b></td>";
        }

        // Gross salary total
        fullHtml += $@"<td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.GrossSalary):N2}</b></td>";

        // Deduction parts totals
        foreach (var item in enableDiductionParts)
        {
            double sum = 0;
            if (item.PartCode == PrSalaryPartCol.ColA)
                sum = model.PrSalaryDtls.Sum(x => x.ColA);
            else if (item.PartCode == PrSalaryPartCol.ColB)
                sum = model.PrSalaryDtls.Sum(x => x.ColB);
            else if (item.PartCode == PrSalaryPartCol.ColC)
                sum = model.PrSalaryDtls.Sum(x => x.ColC);
            else if (item.PartCode == PrSalaryPartCol.ColD)
                sum = model.PrSalaryDtls.Sum(x => x.ColD);
            else if (item.PartCode == PrSalaryPartCol.ColE)
                sum = model.PrSalaryDtls.Sum(x => x.ColE);
            else if (item.PartCode == PrSalaryPartCol.ColF)
                sum = model.PrSalaryDtls.Sum(x => x.ColF);
            else if (item.PartCode == PrSalaryPartCol.ColG)
                sum = model.PrSalaryDtls.Sum(x => x.ColG);
            else if (item.PartCode == PrSalaryPartCol.ColH)
                sum = model.PrSalaryDtls.Sum(x => x.ColH);
            else if (item.PartCode == PrSalaryPartCol.ColJ)
                sum = model.PrSalaryDtls.Sum(x => x.ColJ);
            else if (item.PartCode == PrSalaryPartCol.ColK)
                sum = model.PrSalaryDtls.Sum(x => x.ColK);
            else if (item.PartCode == PrSalaryPartCol.ColL)
                sum = model.PrSalaryDtls.Sum(x => x.ColL);
            else if (item.PartCode == PrSalaryPartCol.ColM)
                sum = model.PrSalaryDtls.Sum(x => x.ColM);
            else if (item.PartCode == PrSalaryPartCol.ColN)
                sum = model.PrSalaryDtls.Sum(x => x.ColN);
            else if (item.PartCode == PrSalaryPartCol.ColO)
                sum = model.PrSalaryDtls.Sum(x => x.ColO);
            else if (item.PartCode == PrSalaryPartCol.ColP)
                sum = model.PrSalaryDtls.Sum(x => x.ColP);
            else if (item.PartCode == PrSalaryPartCol.ColQ)
                sum = model.PrSalaryDtls.Sum(x => x.ColQ);
            else if (item.PartCode == PrSalaryPartCol.ColR)
                sum = model.PrSalaryDtls.Sum(x => x.ColR);
            else if (item.PartCode == PrSalaryPartCol.ColS)
                sum = model.PrSalaryDtls.Sum(x => x.ColS);
            else if (item.PartCode == PrSalaryPartCol.ColT)
                sum = model.PrSalaryDtls.Sum(x => x.ColT);
            else if (item.PartCode == PrSalaryPartCol.ColU)
                sum = model.PrSalaryDtls.Sum(x => x.ColU);

            fullHtml += $@"<td class='text-end'><b>{sum:N2}</b></td>";
        }

        // Total deduction and net salary
        fullHtml += $@"<td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.TotalDeduction):N2}</b></td>";
        fullHtml += $@"<td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.NetSalary):N2}</b></td>";

        fullHtml += "</tr>";
        #endregion

        return fullHtml;
    }

    public async Task<string> PayrollBankHtml(long id)
    {

        try
        {
            var data = await Repository.GetPrSalaryByIdAsync(id);
            var model = _iMapper.Map<PrSalaryMstVm>(data);

            if (model.PrSalaryDtls.Count > 0)
            {
                foreach (var dtl in model.PrSalaryDtls)
                {
                    var filterData = data.PrSalaryDtls.FirstOrDefault(c => c.Id == dtl.Id);

                    dtl.EmployeeName = filterData?.Employee.Name;
                    dtl.EmpAccount = filterData?.Employee.BankAccNo;
                    dtl.NetSalary = filterData.NetSalary;
                }
            }

            var totalSalary = model.PrSalaryDtls.Sum(c => c.NetSalary);

            string fullHtml = "";

            fullHtml += $@"<div style='padding-bottom:15px;font-size:14px;'>
                            <p> Dear Sir,</p>
                            <p> This is to advise you to kindly transfer the following amounts to the individual Accounts of the Teacher, Officer & staffs of Bank Asia, Hfrcmch Branch, Dhaka on account of Salary for Officer & staffs of Bank Asia, Hfrcmch Branch, Dhaka on account of Salary for the month of {Utility.GetMonthName(model.Month)}-{model.Year}  by debiting the Holy Family Red Crescent Medical College Operation Account No 08333000134 & cheque no.1084884 Total: Tk {totalSalary} Only</p>
                        </div>";

            fullHtml += "<table class='table' style='width:100%;repeat-header:yes;'>";

            fullHtml += "<thead>";

            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl.</th>";
            fullHtml += "<th style='width:45%'>Employee Name</th>";
            fullHtml += "<th style='width:30%'> Employee Account</th>";
            fullHtml += "<th style='width:20%'>Net Salary</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (model.PrSalaryDtls.Count > 0)
            {
                foreach (var (employee, i) in model.PrSalaryDtls.GetItemWithIndex())
                {
                    fullHtml += "<tr style='background-color:white'>";

                    fullHtml += "<td style='width:2%'>" + (i + 1) + "</td>";
                    fullHtml += $@"<td>{employee.EmployeeName}</td>";
                    fullHtml += $@"<td>{employee.EmpAccount}</td>";
                    fullHtml += $@"<td class='text-end'>{employee.NetSalary}</td>";

                    fullHtml += "</tr>";
                }
                fullHtml += "<tr style='background-color:white'>";
                fullHtml += "<td colspan='3' class='text-end'><b>Total</b></td>";
                fullHtml += "<td class='text-end'><b> " + totalSalary + " </b></td>";
                fullHtml += "</tr>";
            }


            fullHtml += "</tbody>";

            fullHtml += "</table>";

            return fullHtml;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    #endregion

    #region DepartmentWisePayrollHtml

    public async Task<string> DepartmentWisePayrollHtml(int year, int month, long? departmentId, bool isPrint = false)
    {
        try
        {
            var data = await Repository.GetPrSalaryByMonthAsync(year, month);
            var model = _iMapper.Map<PrSalaryMstVm>(data);
            model.DepartmentId = departmentId;

            var enableParts = _iPartService.Get(c => c.IsEnable);
            model.EnablePrSalaryParts = _iMapper.Map<List<PrSalaryPartVm>>(enableParts);

            if (model.PrSalaryDtls.Count > 0)
            {
                foreach (var dtl in model.PrSalaryDtls)
                {
                    var filterData = data.PrSalaryDtls.FirstOrDefault(c => c.Id == dtl.Id);

                    dtl.EmployeeName = filterData?.Employee.Name;
                    dtl.EmployeeCode = filterData?.Employee.Code;
                    dtl.EmpDepartmentId = filterData?.Employee?.DepartmentId;
                    dtl.EmpDepartmentName = filterData?.Employee?.Department.Name;
                    dtl.EmpDepartmentCode = filterData?.Employee?.Department.Code;
                    dtl.EmpDesignationName = filterData?.Employee?.Designation.Name;
                    dtl.DptLadgerCode = _iDepartmentService.GetLedgerCodeByDptCode(dtl.EmpDepartmentCode);
                    dtl.SetDepartmentSlNo();
                }
            }

            var enableAdditionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "A").ToList();
            var enableDiductionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "D").ToList();
            var addCount = enableAdditionParts.Count();
            var diductCount = enableDiductionParts.Count();

            var totalSalarySpan = addCount + diductCount + 6;

            if (isPrint == true)
            {
                totalSalarySpan = addCount + diductCount + 7;
            }

            string fullHtml = "";

            fullHtml += "<div class='text-center' style='repeat-header:yes;'>";

            fullHtml += $@"<b style='font-size:14px;'>THE MONTH OF {Utility.GetMonthName(model.Month).ToUpper()}-{model.Year}</b>";

            fullHtml += "</div>";

            if (model.DepartmentId > 0)
            {
                model.PrSalaryDtls = model.PrSalaryDtls.Where(x => x.EmpDepartmentId == model.DepartmentId).ToList();
            }


            var groupedDepartments = model.PrSalaryDtls.GroupBy(x => new { x.EmpDepartmentId, x.EmpDepartmentName, x.EmpDepartmentSlNo })
                                                        .OrderBy(g => g.Key.EmpDepartmentSlNo);

            foreach (var departmentGroup in groupedDepartments)
            {
                var departmentName = departmentGroup.Key.EmpDepartmentName;

                fullHtml += "<table class='table table-striped table-bordered table-md' style='width:100%;repeat-header:yes;margin-bottom:10px;'>";

                fullHtml += "<thead>";

                fullHtml += $@"<tr style='background-color:lightgray;'>
                                <td colspan='{totalSalarySpan}' class='text-center' style='font-size:22px;'><b>{departmentName}</b></td>
                            </tr>";

                fullHtml += "<tr>";
                fullHtml += "<th rowspan='2' style='width:2%'>Sl.</th>";
                fullHtml += "<th rowspan='2' style='width:10%'>Employee Name</th>";
                fullHtml += "<th rowspan='2'>Designation</th>";
                fullHtml += "<th colspan='" + addCount + "' class='text-center'>Addition</th>";
                fullHtml += "<th rowspan='2' class='text-center'>Gross Salary</th>";
                fullHtml += "<th colspan='" + diductCount + "' class='text-center'>Deduction</th>";
                fullHtml += "<th rowspan='2' class='text-center'>Deduction Total</th>";
                fullHtml += "<th rowspan='2' class='text-center'>Net Salary</th>";

                if (isPrint == true)
                {
                    fullHtml += "<th rowspan='2' class='text-center'>Signature</th>";
                }

                fullHtml += "</tr>";

                fullHtml += "<tr>";
                if (enableAdditionParts.Count > 0)
                {
                    enableAdditionParts = enableAdditionParts.OrderBy(c => c.SlNo).ToList();

                    foreach (var addItem in enableAdditionParts)
                    {
                        fullHtml += "<th class='text-center'>" + addItem.PartName + "</th>";
                    }
                }
                if (enableDiductionParts.Count > 0)
                {
                    enableDiductionParts = enableDiductionParts.OrderBy(c => c.SlNo).ToList();

                    foreach (var deductItem in enableDiductionParts)
                    {
                        fullHtml += "<th class='text-center'>" + deductItem.PartName + "</th>";
                    }
                }
                fullHtml += "</tr>";

                fullHtml += "</thead>";

                fullHtml += "<tbody>";

                if (model.PrSalaryDtls.Count > 0)
                {
                    foreach (var (employee, i) in departmentGroup.GetItemWithIndex())
                    {
                        fullHtml += "<tr style='background-color:white'>";

                        fullHtml += "<td style='width:2%'>" + (i + 1) + "</td>";

                        fullHtml += $@"<td>{employee.EmployeeName}</td>";
                        fullHtml += $@"<td>{employee.EmpDesignationName}</td>";

                        foreach (var item in enableAdditionParts)
                        {
                            if (item.PartCode == "BASIC")
                            {
                                fullHtml += $@"<td class='text-end'>{employee.BasicSalary}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColA))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColA}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColB))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColB}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColC))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColC}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColD))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColD}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColE))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColE}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColF))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColF}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColG))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColG}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColH))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColH}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColI))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColI}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColK))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColK}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColL))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColL}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColM))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColM}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColN))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColN}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColO))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColO}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColP))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColP}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColQ))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColQ}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColR))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColR}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColS))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColS}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColT))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColT}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColU))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColU}</td>";
                            }

                        }

                        fullHtml += $@"<td class='text-end'>{employee.GrossSalary}</td>";

                        foreach (var item in enableDiductionParts)
                        {
                            if (item.PartCode == nameof(employee.ColA))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColA}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColB))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColB}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColC))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColC}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColD))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColD}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColE))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColE}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColF))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColF}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColG))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColG}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColH))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColH}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColJ))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColJ}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColK))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColK}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColL))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColL}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColM))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColM}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColN))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColN}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColO))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColO}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColP))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColP}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColQ))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColQ}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColR))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColR}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColS))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColS}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColT))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColT}</td>";
                            }
                            else if (item.PartCode == nameof(employee.ColU))
                            {
                                fullHtml += $@"<td class='text-end'>{employee.ColU}</td>";
                            }

                        }

                        fullHtml += $@"<td class='text-end'>{employee.TotalDeduction}</td>";
                        fullHtml += $@"<td class='text-end'>{employee.NetSalary}</td>";
                        if (isPrint == true)
                        {
                            fullHtml += !employee.IsPaid ? $@"<td></td>" : "<td class='text-center'>Paid</td>";
                        }
                        fullHtml += "</tr>";
                    }


                    #region Summery Row
                    //fullHtml += "<tr style='background-color:aliceblue;'>";
                    //fullHtml += $@"<td colspan='3' class='text-end'><b>Total</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.BasicSalary).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.ColA).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.ColB).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.ColC).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.ColD).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.GrossSalary).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.ColE).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.ColJ).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.ColU).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.TotalDeduction).ToString("N2")}</b></td>
                    //   <td class='text-end'><b>{departmentGroup.Sum(x => x.NetSalary).ToString("N2")}</b></td>";
                    //fullHtml += "</tr>";

                    fullHtml += "<tr style='background-color:aliceblue;'>";                    
                    fullHtml += "<td colspan='3' class='text-end'><b>Total</b></td>";

                    // Addition parts totals
                    foreach (var item in enableAdditionParts)
                    {
                        double sum = 0;
                        if (item.PartCode == "BASIC")
                            sum = departmentGroup.Sum(x => x.BasicSalary);
                        else if (item.PartCode == PrSalaryPartCol.ColA)
                            sum = departmentGroup.Sum(x => x.ColA);
                        else if (item.PartCode == PrSalaryPartCol.ColB)
                            sum = departmentGroup.Sum(x => x.ColB);
                        else if (item.PartCode == PrSalaryPartCol.ColC)
                            sum = departmentGroup.Sum(x => x.ColC);
                        else if (item.PartCode == PrSalaryPartCol.ColD)
                            sum = departmentGroup.Sum(x => x.ColD);
                        else if (item.PartCode == PrSalaryPartCol.ColE)
                            sum = departmentGroup.Sum(x => x.ColE);
                        else if (item.PartCode == PrSalaryPartCol.ColF)
                            sum = departmentGroup.Sum(x => x.ColF);
                        else if (item.PartCode == PrSalaryPartCol.ColG)
                            sum = departmentGroup.Sum(x => x.ColG);
                        else if (item.PartCode == PrSalaryPartCol.ColH)
                            sum = departmentGroup.Sum(x => x.ColH);
                        else if (item.PartCode == PrSalaryPartCol.ColI)
                            sum = departmentGroup.Sum(x => x.ColI);
                        else if (item.PartCode == PrSalaryPartCol.ColK)
                            sum = departmentGroup.Sum(x => x.ColK);
                        else if (item.PartCode == PrSalaryPartCol.ColL)
                            sum = departmentGroup.Sum(x => x.ColL);
                        else if (item.PartCode == PrSalaryPartCol.ColM)
                            sum = departmentGroup.Sum(x => x.ColM);
                        else if (item.PartCode == PrSalaryPartCol.ColN)
                            sum = departmentGroup.Sum(x => x.ColN);
                        else if (item.PartCode == PrSalaryPartCol.ColO)
                            sum = departmentGroup.Sum(x => x.ColO);
                        else if (item.PartCode == PrSalaryPartCol.ColP)
                            sum = departmentGroup.Sum(x => x.ColP);
                        else if (item.PartCode == PrSalaryPartCol.ColQ)
                            sum = departmentGroup.Sum(x => x.ColQ);
                        else if (item.PartCode == PrSalaryPartCol.ColR)
                            sum = departmentGroup.Sum(x => x.ColR);
                        else if (item.PartCode == PrSalaryPartCol.ColS)
                            sum = departmentGroup.Sum(x => x.ColS);
                        else if (item.PartCode == PrSalaryPartCol.ColT)
                            sum = departmentGroup.Sum(x => x.ColT);
                        else if (item.PartCode == PrSalaryPartCol.ColU)
                            sum = departmentGroup.Sum(x => x.ColU);

                        fullHtml += $@"<td class='text-end'><b>{sum:N2}</b></td>";
                    }

                    // Gross salary total
                    fullHtml += $@"<td class='text-end'><b>{departmentGroup.Sum(x => x.GrossSalary):N2}</b></td>";

                    // Deduction parts totals
                    foreach (var item in enableDiductionParts)
                    {
                        double sum = 0;
                        if (item.PartCode == PrSalaryPartCol.ColA)
                            sum = departmentGroup.Sum(x => x.ColA);
                        else if (item.PartCode == PrSalaryPartCol.ColB)
                            sum = departmentGroup.Sum(x => x.ColB);
                        else if (item.PartCode == PrSalaryPartCol.ColC)
                            sum = departmentGroup.Sum(x => x.ColC);
                        else if (item.PartCode == PrSalaryPartCol.ColD)
                            sum = departmentGroup.Sum(x => x.ColD);
                        else if (item.PartCode == PrSalaryPartCol.ColE)
                            sum = departmentGroup.Sum(x => x.ColE);
                        else if (item.PartCode == PrSalaryPartCol.ColF)
                            sum = departmentGroup.Sum(x => x.ColF);
                        else if (item.PartCode == PrSalaryPartCol.ColG)
                            sum = departmentGroup.Sum(x => x.ColG);
                        else if (item.PartCode == PrSalaryPartCol.ColH)
                            sum = departmentGroup.Sum(x => x.ColH);
                        else if (item.PartCode == PrSalaryPartCol.ColJ)
                            sum = departmentGroup.Sum(x => x.ColJ);
                        else if (item.PartCode == PrSalaryPartCol.ColK)
                            sum = departmentGroup.Sum(x => x.ColK);
                        else if (item.PartCode == PrSalaryPartCol.ColL)
                            sum = departmentGroup.Sum(x => x.ColL);
                        else if (item.PartCode == PrSalaryPartCol.ColM)
                            sum = departmentGroup.Sum(x => x.ColM);
                        else if (item.PartCode == PrSalaryPartCol.ColN)
                            sum = departmentGroup.Sum(x => x.ColN);
                        else if (item.PartCode == PrSalaryPartCol.ColO)
                            sum = departmentGroup.Sum(x => x.ColO);
                        else if (item.PartCode == PrSalaryPartCol.ColP)
                            sum = departmentGroup.Sum(x => x.ColP);
                        else if (item.PartCode == PrSalaryPartCol.ColQ)
                            sum = departmentGroup.Sum(x => x.ColQ);
                        else if (item.PartCode == PrSalaryPartCol.ColR)
                            sum = departmentGroup.Sum(x => x.ColR);
                        else if (item.PartCode == PrSalaryPartCol.ColS)
                            sum = departmentGroup.Sum(x => x.ColS);
                        else if (item.PartCode == PrSalaryPartCol.ColT)
                            sum = departmentGroup.Sum(x => x.ColT);
                        else if (item.PartCode == PrSalaryPartCol.ColU)
                            sum = departmentGroup.Sum(x => x.ColU);

                        fullHtml += $@"<td class='text-end'><b>{sum:N2}</b></td>";
                    }

                    // Total deduction and net salary
                    fullHtml += $@"<td class='text-end'><b>{departmentGroup.Sum(x => x.TotalDeduction):N2}</b></td>";
                    fullHtml += $@"<td class='text-end'><b>{departmentGroup.Sum(x => x.NetSalary):N2}</b></td>";

                    fullHtml += "</tr>";
                    #endregion
                }

                fullHtml += "</tbody>";

                fullHtml += "</table>";

            }


            #region Summary

            var ledgerGroups = model.PrSalaryDtls.GroupBy(x => x.DptLadgerCode).ToList();

            if (ledgerGroups.Count > 0)
            {
                fullHtml += "<div style='width:100%'>";

                foreach (var ledgerGroup in ledgerGroups)
                {
                    if (isPrint)
                        fullHtml += "<div style='width:25%;float:left;padding-right:5px;'>";
                    else
                        fullHtml += "<div style='width:23%;float:left;padding-right:5px;'>";

                    fullHtml += "<table class='table table-striped table-bordered table-md' style='width:100%;margin-bottom:10px;'>";

                    fullHtml += "<thead>";

                    fullHtml += "<tr>";

                    if (ledgerGroup.Key == AccLadgerCode.HotelMisuk)
                        fullHtml += "<td colspan='2'><b>Hotel Mishuk</b></td>";
                    else if (ledgerGroup.Key == AccLadgerCode.RestaurantLedger)
                        fullHtml += "<td colspan='2'><b>Restaurant</b></td>";
                    else if (ledgerGroup.Key == AccLadgerCode.StaffKitchenLedger)
                        fullHtml += "<td colspan='2'><b>Stuff Kitchen</b></td>";
                    else if (ledgerGroup.Key == AccLadgerCode.AmariResortLedger)
                        fullHtml += "<td colspan='2'><b>Amari Resort</b></td>";

                    fullHtml += "</tr>";

                    fullHtml += "</thead>";

                    var ledgerDepartmentGroup = ledgerGroup
                        .GroupBy(x => new { x.EmpDepartmentId, x.EmpDepartmentName })
                        .ToList();

                    fullHtml += "<tbody>";

                    if (ledgerDepartmentGroup.Count > 0)
                    {
                        foreach (var dptGroup in ledgerDepartmentGroup)
                        {
                            fullHtml += "<tr>";
                            fullHtml += $"<td style='width: 60%;'>{dptGroup.Key.EmpDepartmentName}</td>";
                            fullHtml += $"<td class='text-end' style='width: 40%;'>{dptGroup.Sum(x => x.NetSalary).ToString("N2")}</td>";
                            fullHtml += "</tr>";
                        }

                        fullHtml += "<tr>";
                        fullHtml += $"<td style='width: 60%;'><b>Total</b></td>";
                        fullHtml += $"<td class='text-end' style='width: 40%;'><b>{ledgerGroup.Sum(x => x.NetSalary).ToString("N2")}</b></td>";
                        fullHtml += "</tr>";
                    }
                    else
                    {
                        fullHtml += "<tr>";
                        fullHtml += $"<td colspan='2'><b>No Data Found</b></td>";
                        fullHtml += "</tr>";
                    }

                    fullHtml += "</tbody>";

                    fullHtml += "</table>";

                    fullHtml += "</div>";
                }

                fullHtml += "<table class='table table-striped table-bordered table-md' style='margin-bottom:10px;'>";

                fullHtml += "<thead>";

                foreach (var ledgerGroup in ledgerGroups)
                {
                    fullHtml += "<tr>";

                    if (ledgerGroup.Key == AccLadgerCode.HotelMisuk)
                        fullHtml += "<th style='width: 60%;'><b>Hotel Mishuk</b></th>";
                    else if (ledgerGroup.Key == AccLadgerCode.RestaurantLedger)
                        fullHtml += "<th style='width: 60%;'><b>Restaurant</b></th>";
                    else if (ledgerGroup.Key == AccLadgerCode.StaffKitchenLedger)
                        fullHtml += "<th style='width: 60%;'><b>Stuff Kitchen</b></th>";
                    else if (ledgerGroup.Key == AccLadgerCode.AmariResortLedger)
                        fullHtml += "<th style='width: 60%;'><b>Amari Resort</b></th>";

                    fullHtml += $"<th style='width: 30%;'><b>{ledgerGroup.Sum(x => x.NetSalary).ToString("N2")}</b></th>";

                    fullHtml += "</tr>";
                }

                fullHtml += $"<tr><th>TOTAL</th><th>{model.PrSalaryDtls.Sum(x => x.NetSalary).ToString("N2")}</th></tr>";

                fullHtml += "</thead>";

                fullHtml += "</table>";

                fullHtml += "</div>";


            }



            #endregion

            return fullHtml;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private string DepartmentPayrollTableBodyHtml(PrSalaryMstVm model)
    {
        var fullHtml = "";

        var enableAdditionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "A").ToList();
        var enableDiductionParts = model.EnablePrSalaryParts.Where(c => c.PartType == "D").ToList();
        var addCount = enableAdditionParts.Count();
        var diductCount = enableDiductionParts.Count();

        enableAdditionParts = enableAdditionParts.OrderBy(c => c.SlNo).ToList();
        enableDiductionParts = enableDiductionParts.OrderBy(c => c.SlNo).ToList();

        var totalSalarySpan = addCount + diductCount + 7;

        foreach (var (employee, i) in model.PrSalaryDtls.GetItemWithIndex())
        {
            fullHtml += "<tr style='background-color:white'>";

            fullHtml += "<td style='width:2%'>" + (i + 1) + "</td>";

            fullHtml += $@"<td>{employee.EmployeeName}</td>";
            fullHtml += $@"<td>{employee.EmployeeCode}</td>";
            //fullHtml += $@"<td>{employee.EmpDepartmentName}</td>";
            fullHtml += $@"<td>{employee.EmpDesignationName}</td>";

            foreach (var item in enableAdditionParts)
            {
                if (item.PartCode == "BASIC")
                {
                    fullHtml += $@"<td class='text-end'>{employee.BasicSalary}</td>";
                }
                else if (item.PartCode == nameof(employee.ColA))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColA}</td>";
                }
                else if (item.PartCode == nameof(employee.ColB))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColB}</td>";
                }
                else if (item.PartCode == nameof(employee.ColC))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColC}</td>";
                }
                else if (item.PartCode == nameof(employee.ColD))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColD}</td>";
                }
                else if (item.PartCode == nameof(employee.ColE))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColE}</td>";
                }
                else if (item.PartCode == nameof(employee.ColF))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColF}</td>";
                }
                else if (item.PartCode == nameof(employee.ColG))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColG}</td>";
                }
                else if (item.PartCode == nameof(employee.ColH))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColH}</td>";
                }
                else if (item.PartCode == nameof(employee.ColI))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColI}</td>";
                }
                else if (item.PartCode == nameof(employee.ColK))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColK}</td>";
                }
                else if (item.PartCode == nameof(employee.ColL))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColL}</td>";
                }
                else if (item.PartCode == nameof(employee.ColM))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColM}</td>";
                }
                else if (item.PartCode == nameof(employee.ColN))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColN}</td>";
                }
                else if (item.PartCode == nameof(employee.ColO))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColO}</td>";
                }
                else if (item.PartCode == nameof(employee.ColP))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColP}</td>";
                }
                else if (item.PartCode == nameof(employee.ColQ))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColQ}</td>";
                }
                else if (item.PartCode == nameof(employee.ColR))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColR}</td>";
                }
                else if (item.PartCode == nameof(employee.ColS))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColS}</td>";
                }
                else if (item.PartCode == nameof(employee.ColT))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColT}</td>";
                }
                else if (item.PartCode == nameof(employee.ColU))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColU}</td>";
                }

            }

            fullHtml += $@"<td class='text-end'>{employee.GrossSalary}</td>";

            foreach (var item in enableDiductionParts)
            {
                if (item.PartCode == nameof(employee.ColA))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColA}</td>";
                }
                else if (item.PartCode == nameof(employee.ColB))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColB}</td>";
                }
                else if (item.PartCode == nameof(employee.ColC))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColC}</td>";
                }
                else if (item.PartCode == nameof(employee.ColD))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColD}</td>";
                }
                else if (item.PartCode == nameof(employee.ColE))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColE}</td>";
                }
                else if (item.PartCode == nameof(employee.ColF))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColF}</td>";
                }
                else if (item.PartCode == nameof(employee.ColG))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColG}</td>";
                }
                else if (item.PartCode == nameof(employee.ColH))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColH}</td>";
                }
                else if (item.PartCode == nameof(employee.ColJ))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColJ}</td>";
                }
                else if (item.PartCode == nameof(employee.ColK))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColK}</td>";
                }
                else if (item.PartCode == nameof(employee.ColL))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColL}</td>";
                }
                else if (item.PartCode == nameof(employee.ColM))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColM}</td>";
                }
                else if (item.PartCode == nameof(employee.ColN))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColN}</td>";
                }
                else if (item.PartCode == nameof(employee.ColO))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColO}</td>";
                }
                else if (item.PartCode == nameof(employee.ColP))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColP}</td>";
                }
                else if (item.PartCode == nameof(employee.ColQ))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColQ}</td>";
                }
                else if (item.PartCode == nameof(employee.ColR))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColR}</td>";
                }
                else if (item.PartCode == nameof(employee.ColS))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColS}</td>";
                }
                else if (item.PartCode == nameof(employee.ColT))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColT}</td>";
                }
                else if (item.PartCode == nameof(employee.ColU))
                {
                    fullHtml += $@"<td class='text-end'>{employee.ColU}</td>";
                }

            }

            fullHtml += $@"<td class='text-end'>{employee.TotalDeduction}</td>";
            fullHtml += $@"<td class='text-end'>{employee.NetSalary}</td>";

            fullHtml += "</tr>";
        }

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='4' class='text-end' style='background-color:aliceblue;'><b>Total</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.BasicSalary).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColA).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColB).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColC).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColD).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.GrossSalary).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColE).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColJ).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.ColU).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.TotalDeduction).ToString("N2")}</b></td>
                       <td class='text-end'><b>{model.PrSalaryDtls.Sum(x => x.NetSalary).ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        return fullHtml;
    }

    #endregion
}
