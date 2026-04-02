using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities.Accounting;
using Domain.Entities.Admin;
using Domain.Entities.Attendance;
using Domain.Entities.HotelManagement;
using Domain.Entities.HouseKeeping;
using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.Entities.Inventory;
using Domain.Entities.Leave;
using Domain.Entities.Notification;
using Domain.Entities.Payroll;
using Domain.Enums;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Persistence.ContextModel;
using DU = Domain.Utility;

namespace Utility.CachingUtility
{
	public class DropdownService
	{
		#region CONFIG

		private readonly ApplicationDbContext _db;
		private readonly CacheStoreService _cacheStoreService;
		public DropdownService(ApplicationDbContext db, CacheStoreService cacheStoreService)
		{
			_db = db;
			_cacheStoreService = cacheStoreService;
		}

		#endregion

		/* Default */
		#region DefaultSelectListItem

		public List<SelectListItem> GetDefaultSelectListItem(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			if (isDefaultSelectAdd) items.Add(new SelectListItem { Value = "", Text = "---Select---" });
			return items;
		}

		#endregion

		/* A */
		#region ApplicationUsers

		public IEnumerable<SelectListItem> GetUserSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<ApplicationUser>>(CacheEnum.UserList.ToString()) ?? _cacheStoreService.AddOrUpdate<ApplicationUser>(CacheEnum.UserList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.FullName }));
			return items;
		}
		#endregion
		#region AcademicDepartmentSelectListItems

		public IEnumerable<SelectListItem> GetAcademicDepartmentSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Department>>(CacheEnum.DepartmentList.ToString()) ?? _cacheStoreService.AddOrUpdate<Department>(CacheEnum.DepartmentList.ToString()).Where(c => c.IsAcademic && !c.IsDeleted);
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region AcademicYearSelectListItems

		public IEnumerable<SelectListItem> GetAcademicYearSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = AcademicYearEnum.FirstYear.GetDescription(), Value = AcademicYearEnum.FirstYear.ToInt32ToString() },
				new SelectListItem { Text = AcademicYearEnum.SecoundYear.GetDescription(), Value = AcademicYearEnum.SecoundYear.ToInt32ToString() },
				new SelectListItem { Text = AcademicYearEnum.ThirdYear.GetDescription(), Value = AcademicYearEnum.ThirdYear.ToInt32ToString() },
				new SelectListItem { Text = AcademicYearEnum.FourthYear.GetDescription(), Value = AcademicYearEnum.FourthYear.ToInt32ToString() },
				new SelectListItem { Text = AcademicYearEnum.FifthYear.GetDescription(), Value = AcademicYearEnum.FifthYear.ToInt32ToString() },
			});

			return items;
		}

		#endregion
		#region AcaHstStatusSelectListItems

		public IEnumerable<SelectListItem> GetAcaHstStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = StuAcademicHstStatusEnum.Promoted.GetDescription(), Value = StuAcademicHstStatusEnum.Promoted.ToInt32ToString() },
				new SelectListItem { Text = StuAcademicHstStatusEnum.Running.GetDescription(), Value = StuAcademicHstStatusEnum.Running.ToInt32ToString() },
				new SelectListItem { Text = StuAcademicHstStatusEnum.Completed.GetDescription(), Value = StuAcademicHstStatusEnum.Completed.ToInt32ToString() },
				new SelectListItem { Text = StuAcademicHstStatusEnum.Refferd.GetDescription(), Value = StuAcademicHstStatusEnum.Refferd.ToInt32ToString() },
				new SelectListItem { Text = StuAcademicHstStatusEnum.Suspended.GetDescription(), Value = StuAcademicHstStatusEnum.Suspended.ToInt32ToString() },
			});
			return items;
		}

		#endregion
		#region AccGroupSelectListItems

		public IEnumerable<SelectListItem> GetAccGroupSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<AccGroup>>(CacheEnum.AccGroupList.ToString()) ?? _cacheStoreService.AddOrUpdate<AccGroup>(CacheEnum.AccGroupList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion
		#region AccHeadDynamicData

		public dynamic GetAccHeadDynamicData(long? groupId)
		{
			var dataList = CacheStore.Get<List<AccHead>>(CacheEnum.SubAccHeadList.ToString()) ?? _cacheStoreService.AddOrUpdate<AccHead>(CacheEnum.SubAccHeadList.ToString());
			dataList = groupId > 0 ? dataList.Where(c => c.GroupId == groupId).ToList() : dataList;
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.HeadName });
			return dynamicData;
		}

		#endregion
		#region AccHeadSelectListItems

		public IEnumerable<SelectListItem> GetAccHeadSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<AccHead>>(CacheEnum.AccHeadList.ToString()) ?? _cacheStoreService.AddOrUpdate<AccHead>(CacheEnum.AccHeadList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.HeadName }));
			return items;
		}

		#endregion
		#region AccLedgerSelectListItems

		public IEnumerable<SelectListItem> GetAccLedgerSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<AccLedger>>(CacheEnum.AccLedgerList.ToString()) ?? _cacheStoreService.AddOrUpdate<AccLedger>(CacheEnum.AccLedgerList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.LedgerCode}-{c.LedgerName}" }));
			return items;
		}

		#endregion
		#region AnswerTypeSelectListItems

		public IEnumerable<SelectListItem> GetAnswerTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = AnswerTypeEnum.Single.GetDescription(), Value = AnswerTypeEnum.Single.ToInt32ToString() },
				new SelectListItem { Text = AnswerTypeEnum.Multiple.GetDescription(), Value = AnswerTypeEnum.Multiple.ToInt32ToString() },
			});
			return items;
		}

		#endregion

		#region ApproveRequsition

		public IEnumerable<SelectListItem> GetApproveRequsitionSeletListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RequsitionInfo>>(CacheEnum.RequsitionInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<RequsitionInfo>(CacheEnum.RequsitionInfoList.ToString());
			dataList = dataList.Where(c => c.Status == (short)RequisitionStatusEnum.APPROVED).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.ReqDate)}-({c.ReqNo})-({c.Dept?.Name})-({c.ReqBy?.Name})" }));
			return items;
		}

		#endregion

		#region ApproveRequsition

		public IEnumerable<SelectListItem> GetApproveRequsitionNotIssuedSeletListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RequsitionInfo>>(CacheEnum.RequsitionInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<RequsitionInfo>(CacheEnum.RequsitionInfoList.ToString());
			dataList = dataList.Where(c => c.Status == (short)RequisitionStatusEnum.APPROVED
			&& (c.IsStatus == (short)RequisitionIssueStatusEnum.NOT || c.IsStatus == (short)RequisitionIssueStatusEnum.PARTIAL)).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.ReqDate)}-({c.ReqNo})-({c.Dept?.Name})-({c.ReqBy?.Name})" }));
			return items;
		}

		#endregion

		#region ApproveRequsition

		public IEnumerable<SelectListItem> GetOrderRequsitionSeletListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RequsitionInfo>>(CacheEnum.RequsitionInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<RequsitionInfo>(CacheEnum.RequsitionInfoList.ToString());
			dataList = dataList.Where(c => c.Status == (short)RequisitionStatusEnum.APPROVED
			&& (c.IsStatus == (short)RequisitionIssueStatusEnum.NOT || c.IsStatus == (short)RequisitionIssueStatusEnum.PARTIAL)).ToList();

			var filterList = new List<RequsitionInfo>();

			foreach (var item in dataList)
			{
				var existsOrder = _db.OrderMsts.FirstOrDefault(x => x.ReqId == item.Id);
				if (existsOrder != null)
					continue;

				filterList.Add(item);
			}

			items.AddRange(filterList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.ReqDate)}-({c.ReqNo})-({c.Dept?.Name})-({c.ReqBy?.Name})" }));
			return items;
		}

		#endregion

		/* B */
		#region BankAccLedgerSelectListItems

		public IEnumerable<SelectListItem> GetBankAccLedgerSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<AccLedger>>(CacheEnum.BankAccLedgerList.ToString()) ?? _cacheStoreService.AddOrUpdate<AccLedger>(CacheEnum.BankAccLedgerList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.LedgerCode}-{c.LedgerName}" }));
			return items;
		}

		#endregion

		#region BloodGroupSelectListItems

		public IEnumerable<SelectListItem> GetBloodGroupSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem {Value="O+", Text = "O+ (ve)" },
				new SelectListItem {Value="O-", Text = "O- (ve)"},
				new SelectListItem {Value="A+", Text = "A+ (ve)"},
				new SelectListItem {Value="A-", Text = "A- (ve)"},
				new SelectListItem {Value="B+", Text = "B+ (ve)"},
				new SelectListItem {Value="B-", Text = "B- (ve)"},
				new SelectListItem {Value="AB+", Text = "AB+ (ve)"},
				new SelectListItem {Value="AB-", Text = "AB- (ve)"},
			};
			return items;
		}

		#endregion

		#region BedTypeSelectListItems

		public IEnumerable<SelectListItem> GetBedTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtBedType>>(CacheEnum.BedTypeList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtBedType>(CacheEnum.BedTypeList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.TypeName}" }));
			return items;
		}

		#endregion

		#region BillSelectListItems

		public IEnumerable<SelectListItem> GetBillStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = BillStatusEnum.Fresh.GetDescription(), Value = BillStatusEnum.Fresh.ToInt32ToString() },
				new SelectListItem { Text = BillStatusEnum.PartialPaid.GetDescription(), Value = BillStatusEnum.PartialPaid.ToInt32ToString() },
				new SelectListItem { Text = BillStatusEnum.FullPaid.GetDescription(), Value = BillStatusEnum.FullPaid.ToInt32ToString() },

			});
			return items;
		}

		#endregion

		/* C */

		#region CompanyContributionType

		public IEnumerable<SelectListItem> CompanyContributionType(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem {Value="N", Text = "Not Contribute" },
				new SelectListItem {Value="H", Text = "Half"},
				new SelectListItem {Value="F", Text = "Full"},
			};
			return items;
		}

		#endregion

		#region CountrySelectListItems

		public IEnumerable<SelectListItem> GetCountrySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<SetCountry>>(CacheEnum.CountryList.ToString()) ?? _cacheStoreService.AddOrUpdate<SetCountry>(CacheEnum.CountryList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region GetCountrySelectListItemsWithCode
		public IEnumerable<CustomSelectListItems> GetCountrySelectListItemsWithCode(bool isDefaultSelectAdd = true)
		{
			var items = new List<CustomSelectListItems>();
			items.Add(new CustomSelectListItems { Value = "", Text = "---Select---", Code = "" });
			var dataList = CacheStore.Get<List<SetCountry>>(CacheEnum.CountryList.ToString()) ?? _cacheStoreService.AddOrUpdate<SetCountry>(CacheEnum.CountryList.ToString());
			items.AddRange(dataList.Select(c => new CustomSelectListItems() { Value = c.Id.ToString(), Text = c.Name, Code = c.Code }));
			return items;
		}
		#endregion
		#region ClientCompanySelectListItems

		public IEnumerable<SelectListItem> GetClientCompanySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<ClientCompany>>(CacheEnum.ClientCompanyList.ToString()) ?? _cacheStoreService.AddOrUpdate<ClientCompany>(CacheEnum.ClientCompanyList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region CourseLengthTypeSelectListItems

		public IEnumerable<SelectListItem> GetCourseLengthTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			//var items = new List<SelectListItem>();
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = CourseLengthEnum.Month.GetDescription(), Value = CourseLengthEnum.Month.ToInt32ToString() },
				new SelectListItem { Text = CourseLengthEnum.Year.GetDescription(), Value = CourseLengthEnum.Year.ToInt32ToString() },
			});

			return items;
		}

		#endregion

		#region CourseTypeSelectListItems

		public IEnumerable<SelectListItem> GetCourseTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = CourseTypeEnum.Full.GetDescription(), Value = CourseTypeEnum.Full.ToInt32ToString() },
				new SelectListItem { Text = CourseTypeEnum.Short.GetDescription(), Value = CourseTypeEnum.Short.ToInt32ToString() },
			});
			return items;
		}

		#endregion

		#region CurrencySelectListItems

		public IEnumerable<SelectListItem> GetCurrencySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<SetCurrency>>(CacheEnum.CurrencyList.ToString()) ?? _cacheStoreService.AddOrUpdate<SetCurrency>(CacheEnum.CurrencyList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.CurrencyName}({c.Symbol})" }));
			return items;
		}

		#endregion

		/* D */
		#region DepartmentSelectListItems

		public IEnumerable<SelectListItem> GetDepartmentSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Department>>(CacheEnum.DepartmentList.ToString()) ?? _cacheStoreService.AddOrUpdate<Department>(CacheEnum.DepartmentList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region DesignationSelectListItems

		public IEnumerable<SelectListItem> GetDesignationSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Designation>>(CacheEnum.DesignationList.ToString()) ?? _cacheStoreService.AddOrUpdate<Designation>(CacheEnum.DesignationList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region DiscountSelectListItems

		public IEnumerable<SelectListItem> GetDiscountSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Percent", Value = "P" },
				new SelectListItem { Text = "Amount", Value = "A" },
			};
			return items;
		}

		#endregion

		#region DistrictSelectListItems

		public IEnumerable<SelectListItem> GetDistrictSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<SetDistrict>>(CacheEnum.DistrictList.ToString()) ?? _cacheStoreService.AddOrUpdate<SetDistrict>(CacheEnum.DistrictList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		/* E */
		#region EmpLeaveTypeByGender

		public dynamic GetEmpLeaveTypeByGender(string gender)
		{
			var dataList = CacheStore.Get<List<LeaveType>>(CacheEnum.EmpLeaveType.ToString()) ?? _cacheStoreService.AddOrUpdate<LeaveType>(CacheEnum.EmpLeaveType.ToString());
			dataList = !string.IsNullOrEmpty(gender) ? dataList.Where(c => c.Gender == gender || c.Gender == "A").ToList() : dataList;
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.TypeName });
			return dynamicData;
		}

		#endregion

		#region EmpLeaveTypeSelectListItems

		public IEnumerable<SelectListItem> GetEmpLeaveTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<LeaveType>>(CacheEnum.EmpLeaveType.ToString()) ?? _cacheStoreService.AddOrUpdate<LeaveType>(CacheEnum.EmpLeaveType.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.TypeName }));
			return items;
		}

		#endregion

		#region EmployeeActionTypeSelectListItems

		public IEnumerable<SelectListItem> GetEmployeeActionTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = EmpDisciplinaryEnum.SowCause.GetDescription(), Value = EmpDisciplinaryEnum.SowCause.ToInt32ToString() },
				new SelectListItem { Text = EmpDisciplinaryEnum.Warning.GetDescription(), Value = EmpDisciplinaryEnum.Warning.ToInt32ToString() },
				new SelectListItem { Text = EmpDisciplinaryEnum.Suspend.GetDescription(), Value = EmpDisciplinaryEnum.Suspend.ToInt32ToString() },
				new SelectListItem { Text = EmpDisciplinaryEnum.Others.GetDescription(), Value = EmpDisciplinaryEnum.Others.ToInt32ToString() },

			});

			return items;
		}

		#endregion

		#region EmployeeLeaveApplicationStatus

		public IEnumerable<SelectListItem> EmployeeLeaveApplicationStatus(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem {Value="0", Text = "Pending" },
				new SelectListItem {Value="1", Text = "Approve"},
				new SelectListItem {Value="2", Text = "Reject"},
				new SelectListItem {Value="3", Text = "Cancel"},
			};
			return items;
		}

		#endregion

		#region EmployeeSelectListItems

		public IEnumerable<SelectListItem> GetEmployeeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Employee>>(CacheEnum.EmployeeList.ToString()) ?? _cacheStoreService.AddOrUpdate<Employee>(CacheEnum.EmployeeList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region EmployeeSelectListItems

		public IEnumerable<SelectListItem> GetDptWiseEmployeeSelectListItems(long dptId, bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Employee>>(CacheEnum.EmployeeList.ToString()) ?? _cacheStoreService.AddOrUpdate<Employee>(CacheEnum.EmployeeList.ToString());
			dataList = dataList.Where(x => x.DepartmentId == dptId).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region EmployeeStatusSelectListItems

		public IEnumerable<SelectListItem> GetEmployeeStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = EmployeeStatusEnum.Permanent.GetDescription(), Value = EmployeeStatusEnum.Permanent.ToInt32ToString() },
				new SelectListItem { Text = EmployeeStatusEnum.Contractual.GetDescription(), Value = EmployeeStatusEnum.Contractual.ToInt32ToString() },
				new SelectListItem { Text = EmployeeStatusEnum.Adhoc.GetDescription(), Value = EmployeeStatusEnum.Adhoc.ToInt32ToString() },
				new SelectListItem { Text = EmployeeStatusEnum.Guest.GetDescription(), Value = EmployeeStatusEnum.Guest.ToInt32ToString() },
			});

			return items;
		}

		#endregion

		#region EmpRefTypeSelectListItems

		public IEnumerable<SelectListItem> GetEmpRefTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = EmpRefTypeEnum.REFERENCE.GetDescription(), Value = EmpRefTypeEnum.REFERENCE.ToInt32ToString() },
				new SelectListItem { Text = EmpRefTypeEnum.NOMINEE.GetDescription(), Value = EmpRefTypeEnum.NOMINEE.ToInt32ToString() },
				new SelectListItem { Text = EmpRefTypeEnum.EMERGENCY_CONTACT.GetDescription(), Value = EmpRefTypeEnum.EMERGENCY_CONTACT.ToInt32ToString() },
			});

			return items;
		}

		#endregion

		#region EmployeeStatus

		public IEnumerable<SelectListItem> GetEmployeeIsEnableSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			//var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = EmployeeIsEnableEnum.Disable.GetDescription(), Value = EmployeeIsEnableEnum.Disable.ToInt32ToString() },
				new SelectListItem { Text = EmployeeIsEnableEnum.Current.GetDescription(), Value = EmployeeIsEnableEnum.Current.ToInt32ToString() },
			});

			return items;
		}

		#endregion

		#region EmployeeLeaveReviewerSLNoListItems

		public IEnumerable<SelectListItem> GetEmployeeLeaveReviewerSLNoListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem {Text  = ReviewerSlNoEnum.FirstApprover.GetDescription(), Value = ReviewerSlNoEnum.FirstApprover.ToInt32ToString()},
				new SelectListItem {Text  = ReviewerSlNoEnum.SecondApprover.GetDescription(), Value = ReviewerSlNoEnum.SecondApprover.ToInt32ToString()},
				new SelectListItem {Text  = ReviewerSlNoEnum.ThirdApprover.GetDescription(), Value = ReviewerSlNoEnum.ThirdApprover.ToInt32ToString()},
				new SelectListItem {Text  = ReviewerSlNoEnum.FourthApprover.GetDescription(), Value = ReviewerSlNoEnum.FourthApprover.ToInt32ToString()},
				new SelectListItem {Text  = ReviewerSlNoEnum.FifthApprover.GetDescription(), Value = ReviewerSlNoEnum.FifthApprover.ToInt32ToString()},
				new SelectListItem {Text  = ReviewerSlNoEnum.SixthApprover.GetDescription(), Value = ReviewerSlNoEnum.SixthApprover.ToInt32ToString()},
				new SelectListItem {Text  = ReviewerSlNoEnum.FINALAPPROVER.GetDescription(), Value = ReviewerSlNoEnum.FINALAPPROVER.ToInt32ToString()}
			};

			return items;
		}

		#endregion

		/* F */
		#region FeePaymentTypeSelectListItems

		public IEnumerable<SelectListItem> GetFeePaymentTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = FeePaymentTypeEnum.OneTime.GetDescription(), Value = FeePaymentTypeEnum.OneTime.ToInt32ToString() },
				new SelectListItem { Text = FeePaymentTypeEnum.Monthly.GetDescription(), Value = FeePaymentTypeEnum.Monthly.ToInt32ToString() },
				new SelectListItem { Text = FeePaymentTypeEnum.StudentWise.GetDescription(), Value = FeePaymentTypeEnum.StudentWise.ToInt32ToString() },
				new SelectListItem { Text = FeePaymentTypeEnum.Event.GetDescription(), Value = FeePaymentTypeEnum.Event.ToInt32ToString() },
			});

			return items;
		}

		#endregion

		#region FineTypeSelectListItems

		public IEnumerable<SelectListItem> GetFineTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem { Text = "Percent", Value = "P" },
				new SelectListItem { Text = "Amount", Value = "A" },
			};
			return items;
		}

		#endregion

		#region FinYearSelectListItems

		public IEnumerable<SelectListItem> GetFinYearSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<SetFincYear>>(CacheEnum.FinYearList.ToString()) ?? _cacheStoreService.AddOrUpdate<SetFincYear>(CacheEnum.FinYearList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.YearName }));
			return items;
		}

		#endregion

		/* G */
		#region GenderSelectListItems

		public IEnumerable<SelectListItem> GetGenderSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Male", Value = "M" },
				new SelectListItem { Text = "Female", Value = "F" },
			};
			return items;
		}

		#endregion

		#region GenderSelectListItemsForLeaveTypes

		public IEnumerable<SelectListItem> GetGenderSelectListItemsForLeaveTypes(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem {Value="F", Text = "Female" },
				new SelectListItem {Value="A", Text = "All"}
			};
			return items;
		}

		#endregion

		#region GuestEmployeeSelectListItems

		public IEnumerable<SelectListItem> GetGuestEmployeeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Employee>>(CacheEnum.EmployeeList.ToString()) ?? _cacheStoreService.AddOrUpdate<Employee>(CacheEnum.EmployeeList.ToString());
			dataList = dataList.Where(c => c.EmployeeStatus == (short)EmployeeStatusEnum.Guest).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		/* H */

		#region HolidaySelectListItems

		public IEnumerable<SelectListItem> GetHolidaySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "FULL DAY", Value = "F" },
				new SelectListItem { Text = "HALF DAY", Value = "H" },
			};
			return items;
		}

		#endregion

		/* I */

		#region IdentityTypeSelectListItems

		public IEnumerable<SelectListItem> GetIdentityTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = IdentityTypeEnum.BirthCertificate.GetDescription(), Value = IdentityTypeEnum.BirthCertificate.ToInt32ToString() },
				new SelectListItem { Text = IdentityTypeEnum.Nid.GetDescription(), Value = IdentityTypeEnum.Nid.ToInt32ToString()},
				new SelectListItem { Text = IdentityTypeEnum.Passport.GetDescription(), Value = IdentityTypeEnum.Passport.ToInt32ToString()}
			});
			return items;
		}

		#endregion

		/* L */
		#region LeaveLimitTypeSelectListItems

		public IEnumerable<SelectListItem> GetLeaveLimitTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Year", Value = "Y" },
				new SelectListItem { Text = "Job Life", Value = "L" },
			};
			return items;
		}

		#endregion

		/* M */
		#region MaritalStatusSelectListItems

		public IEnumerable<SelectListItem> GetMaritalStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Unmarried", Value = "U" },
				new SelectListItem { Text = "Married", Value = "M" },
			};
			return items;
		}

		#endregion

		#region MonthSelectListItems

		public IEnumerable<SelectListItem> GetMonthSelectListItems(bool isDefaultSelectAdd = true)
		{
			//var items = new List<SelectListItem>();
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = MonthEnum.January.GetDescription(), Value = MonthEnum.January.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.February.GetDescription(), Value = MonthEnum.February.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.March.GetDescription(), Value = MonthEnum.March.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.April.GetDescription(), Value = MonthEnum.April.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.May.GetDescription(), Value = MonthEnum.May.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.June.GetDescription(), Value = MonthEnum.June.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.July.GetDescription(), Value = MonthEnum.July.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.August.GetDescription(), Value = MonthEnum.August.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.September.GetDescription(), Value = MonthEnum.September.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.October.GetDescription(), Value = MonthEnum.October.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.November.GetDescription(), Value = MonthEnum.November.ToInt32ToString() },
				new SelectListItem { Text = MonthEnum.December.GetDescription(), Value = MonthEnum.December.ToInt32ToString() }
			});

			return items;
		}

		#endregion

		/* N */

		#region NonAcademicDepartmentSelectListItems

		public IEnumerable<SelectListItem> GetNonAcademicDepartmentSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Department>>(CacheEnum.DepartmentList.ToString()) ?? _cacheStoreService.AddOrUpdate<Department>(CacheEnum.DepartmentList.ToString()).Where(c => !c.IsAcademic && !c.IsDeleted);
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		/* O */

		#region OrderSelectListItems

		public IEnumerable<SelectListItem> GetOrderSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<OrderMst>>(CacheEnum.OrderList.ToString()) ?? _cacheStoreService.AddOrUpdate<OrderMst>(CacheEnum.OrderList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.OrderDate)}-({c.OrderNo})" }));
			return items;
		}

		public dynamic GetOrderDynamicData(long reqId)
		{
			var dataList = CacheStore.Get<List<OrderMst>>(CacheEnum.OrderList.ToString()) ?? _cacheStoreService.AddOrUpdate<OrderMst>(CacheEnum.OrderList.ToString());
			dataList = reqId > 0 ? dataList.Where(c => c.ReqId == reqId).ToList() : dataList;
			var dynamicData = dataList.Select(c => new { c.Id, Name = $"{DU.Utility.GetDate(c.OrderDate)}-({c.OrderNo})" });
			return dynamicData;
		}


		public IEnumerable<SelectListItem> GetOrderForReceiveSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<OrderMst>>(CacheEnum.OrderList.ToString()) ?? _cacheStoreService.AddOrUpdate<OrderMst>(CacheEnum.OrderList.ToString());
			dataList = dataList.Where(x => x.Status == (short)OrderStatusEnum.APPROVED).ToList();
			dataList = dataList.Where(x => (x.ReceiveStatus == (short)OrderReceiveStatusEnum.PARTIAL || x.ReceiveStatus == (short)OrderReceiveStatusEnum.NOT)).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.OrderDate)}-({c.OrderNo})" }));
			return items;
		}

		public IEnumerable<SelectListItem> GetReceiveOrderSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<OrderMst>>(CacheEnum.OrderList.ToString()) ?? _cacheStoreService.AddOrUpdate<OrderMst>(CacheEnum.OrderList.ToString());
			dataList = dataList.Where(x => x.ReceiveStatus != (short)OrderReceiveStatusEnum.NOT).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.OrderDate)}-({c.OrderNo})" }));
			return items;
		}

		#endregion

		#region OfficialDepartmentSelectListItems

		public IEnumerable<SelectListItem> GetOfficialDepartmentSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Department>>(CacheEnum.DepartmentList.ToString()) ?? _cacheStoreService.AddOrUpdate<Department>(CacheEnum.DepartmentList.ToString()).Where(c => !c.IsAcademic && !c.IsDeleted);
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		/* P */
		#region PaymentTypeSelectListItems

		public IEnumerable<SelectListItem> GetPaymentTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Bank", Value = "1" },
				new SelectListItem { Text = "Cash", Value = "2" },
				new SelectListItem { Text = "Online", Value = "3" }
			};
			return items;
		}

		#endregion

		#region PayTypeSelectListItems

		public IEnumerable<SelectListItem> PayTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Monthly", Value = "M" },
				new SelectListItem { Text = "Shift Wise", Value = "S" },
			};
			return items;
		}

		#endregion

		#region PfSettlementStatus

		public IEnumerable<SelectListItem> PfSettlementStatus(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "TRANSFER" },
				new SelectListItem {Value="L", Text = "LEFT" },
                //new SelectListItem {Value="T", Text = "TRANSFER"},
                new SelectListItem {Value="C", Text = "CANCEL"},
			};
			return items;
		}

		#endregion

		#region PfSourceSelectListItems

		public IEnumerable<SelectListItem> GetPfSourceSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Gross", Value = "G" },
				new SelectListItem { Text = "Basic", Value = "B" },
			};
			return items;
		}

		#endregion

		#region PostingTypeSelectListItems

		public IEnumerable<SelectListItem> GetPostingTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem {Value="R", Text = "REGULAR" },
				new SelectListItem {Value="S", Text = "OSD"},
				new SelectListItem {Value="P", Text = "PROMOTION"},
				new SelectListItem {Value="I", Text = "INCREMENT"},
				new SelectListItem {Value="B", Text = "PROMOTION & INCREMENT"},
				new SelectListItem {Value="O", Text = "OTHERS"}
			};
			return items;
		}

		#endregion

		#region PrEmployeeSelectListItems

		public IEnumerable<SelectListItem> GetPrEmployeeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var prEmpParts = CacheStore.Get<List<PrEmpSalaryPart>>(CacheEnum.PrEmpSalaryPartList.ToString()) ?? _cacheStoreService.AddOrUpdate<PrEmpSalaryPart>(CacheEnum.PrEmpSalaryPartList.ToString());
			var prEmpIds = prEmpParts.Select(c => c.EmployeeId).Distinct();
			var dataList = CacheStore.Get<List<Employee>>(CacheEnum.EmployeeList.ToString()) ?? _cacheStoreService.AddOrUpdate<Employee>(CacheEnum.EmployeeList.ToString());
			dataList = dataList.Where(c => !prEmpIds.Contains(c.Id)).ToList();

			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		/* R */
		#region ReligionSelectListItems

		public IEnumerable<SelectListItem> GetReligionSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = ReligionEnum.Islam.GetDescription(), Value = ReligionEnum.Islam.ToInt32ToString() },
				new SelectListItem { Text = ReligionEnum.Hindu.GetDescription(), Value = ReligionEnum.Hindu.ToInt32ToString() },
				new SelectListItem { Text = ReligionEnum.Christian.GetDescription(), Value = ReligionEnum.Christian.ToInt32ToString() },
				new SelectListItem { Text = ReligionEnum.Buddist.GetDescription(), Value = ReligionEnum.Buddist.ToInt32ToString() },
				new SelectListItem { Text = ReligionEnum.Other.GetDescription(), Value = ReligionEnum.Other.ToInt32ToString() },
			});
			return items;
		}

		#endregion

		#region RoleSelectListItems

		public IEnumerable<SelectListItem> GetRoleSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = _cacheStoreService.GetSession<ApplicationRole>(CacheEnum.RoleList.ToString());
			dataList = dataList.Where(c => c.Id != RoleEnum.Administrator.ToInt64()).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region ReceiverTypeItems
		public IEnumerable<SelectListItem> GetReceiverTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = ReceiverTypeEnum.ALL.GetDescription(), Value = ReceiverTypeEnum.ALL.ToInt32ToString() },
				new SelectListItem { Text = ReceiverTypeEnum.Employee.GetDescription(), Value = ReceiverTypeEnum.Employee.ToInt32ToString() },
				new SelectListItem { Text = ReceiverTypeEnum.Teacher.GetDescription(), Value = ReceiverTypeEnum.Teacher.ToInt32ToString() },
				new SelectListItem { Text = ReceiverTypeEnum.Student.GetDescription(), Value = ReceiverTypeEnum.Student.ToInt32ToString() },
			});

			return items;
		}
		#endregion

		#region ReceiveStatus
		public IEnumerable<SelectListItem> GetReceiveStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);

			items.AddRange(new List<SelectListItem>()
			{
				new SelectListItem(){Text = "Partial", Value = "1"},
				new SelectListItem(){Text = "Full", Value = "2"},
				new SelectListItem(){Text = "Force Full", Value = "3"}
			});
			return items;
		}
		#endregion

		public IEnumerable<SelectListItem> GetRequisitionSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RequsitionInfo>>(CacheEnum.RequsitionInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<RequsitionInfo>(CacheEnum.RequsitionInfoList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.ReqDate)}-({c.ReqNo})" }));
			return items;
		}

		/* S */
		#region SalaryPartTypeSelectListItems

		public IEnumerable<SelectListItem> GetSalaryPartTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = ""},
				new SelectListItem { Text = "Addition", Value = "A" },
				new SelectListItem { Text = "Deduction", Value = "D" }
			};
			return items;
		}

		#endregion

		#region SalaryTypeSelectListItems

		public IEnumerable<SelectListItem> GetSalaryTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = SalaryTypeEnum.Monthly.GetDescription(), Value = SalaryTypeEnum.Monthly.ToInt32ToString() },
				new SelectListItem { Text = SalaryTypeEnum.ClassWise.GetDescription(), Value = SalaryTypeEnum.ClassWise.ToInt32ToString() },
			});

			return items;
		}

		#endregion

		#region SalutationTypeSelectListtems
		public IEnumerable<SelectListItem> GetSalutationSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Mr", Value = "Mr" },
				new SelectListItem { Text = "Ms", Value = "Ms" },
				new SelectListItem { Text = "Mrs", Value = "Mrs" },
				new SelectListItem { Text = "NGO", Value = "NGO" },
			};
			return items;
		}
		#endregion

		#region ScheduleTypeSelectListItems

		public IEnumerable<SelectListItem> GetScheduleTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = ScheduleTypeEnum.Class.GetDescription(), Value = ScheduleTypeEnum.Class.ToInt32ToString() },
				new SelectListItem { Text = ScheduleTypeEnum.Exam.GetDescription(), Value = ScheduleTypeEnum.Exam.ToInt32ToString() },
				new SelectListItem { Text = ScheduleTypeEnum.Assignment.GetDescription(), Value = ScheduleTypeEnum.Assignment.ToInt32ToString() },
				new SelectListItem { Text = ScheduleTypeEnum.ClassTest.GetDescription(), Value = ScheduleTypeEnum.ClassTest.ToInt32ToString() },
			});

			return items;
		}

		#endregion

		#region ShiftTypeSelectListItems

		public IEnumerable<SelectListItem> ShiftTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = "",  },
				new SelectListItem { Text = "Permanent", Value = "P" },
				new SelectListItem { Text = "Duty", Value = "D" },
			};
			return items;
		}

		#endregion

		#region SubAccHeadSelectListItems

		public IEnumerable<SelectListItem> GetSubAccHeadSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<AccHead>>(CacheEnum.SubAccHeadList.ToString()) ?? _cacheStoreService.AddOrUpdate<AccHead>(CacheEnum.SubAccHeadList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.HeadName }));
			return items;
		}

		#endregion

		#region SupplierSelectListItems

		public IEnumerable<SelectListItem> GetSupplierSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<SupplierInfo>>(CacheEnum.SupplierInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<SupplierInfo>(CacheEnum.SupplierInfoList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.SupplierName }));
			return items;
		}

		#endregion

		/* T */
		#region TeacherDesignationSelectListItems

		public IEnumerable<SelectListItem> GetTeacherDesignationSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<Designation>>(CacheEnum.DesignationList.ToString()) ?? _cacheStoreService.AddOrUpdate<Designation>(CacheEnum.DesignationList.ToString());
			dataList = dataList.Where(c => c.IsTeacher).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		#region TypeSelectListItems

		public IEnumerable<SelectListItem> GetTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = ""},
				new SelectListItem { Text = "Mandatory", Value = "M" },
				new SelectListItem { Text = "Optional", Value = "O" },
			};
			return items;
		}

		#endregion

		#region TaskTypeSelectListItems

		public IEnumerable<SelectListItem> GetTaskTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HkTaskType>>(CacheEnum.TaskTypeList.ToString()) ?? _cacheStoreService.AddOrUpdate<HkTaskType>(CacheEnum.TaskTypeList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.TypeName}" }));
			return items;
		}

		#endregion

		#region TaskTypeSelectListItems

		public IEnumerable<SelectListItem> GetTaskNameSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HkTaskName>>(CacheEnum.TaskNameList.ToString()) ?? _cacheStoreService.AddOrUpdate<HkTaskName>(CacheEnum.TaskNameList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Name}" }));
			return items;
		}

		#endregion

		/* V */
		#region ValueTypeSelectListItems

		public IEnumerable<SelectListItem> GetValueTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "---Select---", Value = ""},
				new SelectListItem { Text = "Percent", Value = "P" },
				new SelectListItem { Text = "Amount", Value = "A" }
			};
			return items;
		}

		#endregion

		#region VcNoteTypeSelectListItem

		public IEnumerable<SelectListItem> GetVcNoteTypeSelectListItem(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem {Value=$"{VcNoteType.Create}", Text = $"{VcNoteType.Create}" },
				new SelectListItem {Value=$"{VcNoteType.Approve}", Text = $"{VcNoteType.Approve}" },
				new SelectListItem {Value=$"{VcNoteType.Audit}", Text = $"{VcNoteType.Audit}" },
				new SelectListItem {Value=$"{VcNoteType.BankClear}", Text = $"{VcNoteType.BankClear}" },
				new SelectListItem {Value=$"{VcNoteType.FinalAttach}", Text = $"{VcNoteType.FinalAttach}" },
				new SelectListItem {Value=$"{VcNoteType.Note}", Text = $"{VcNoteType.Note}" },
				new SelectListItem {Value=$"{VcNoteType.AutoCreate}", Text = $"{VcNoteType.AutoCreate}" },
			};
			return items;
		}

		#endregion

		#region VcTypeSelectListItems

		public IEnumerable<SelectListItem> GetVcTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Value = "", Text = "---Select---" },
				new SelectListItem { Text = "Journal Voucher", Value = $"{VoucherType.JournalVoucher}" },
				new SelectListItem { Text = "Bank Debit Voucher", Value = $"{VoucherType.BankDebitVoucher}" },
				new SelectListItem { Text = "Bank Credit Voucher", Value = $"{VoucherType.BankCreditVoucher}" },
				new SelectListItem { Text = "Cash Debit Voucher", Value = $"{VoucherType.CashDebitVoucher}" },
				new SelectListItem { Text = "Cash Credit Voucher", Value = $"{VoucherType.CashCreditVoucher}" },
			};
			return items;
		}

		#endregion

		/* Y */
		#region YearSelectListItems

		public IEnumerable<SelectListItem> GetYearSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>();
			int startYear = 2000;
			var yearData = Enumerable.Range(startYear, DateTime.Now.Year - startYear + 5);

			foreach (var item in yearData)
			{
				items.Add(new SelectListItem { Text = item.ToString(), Value = item.ToString() });
			}
			items = items.OrderByDescending(c => c.Value).ToList();
			return items;
		}

		#endregion

		public IEnumerable<SelectListItem> GetItemSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<ItemInfo>>(CacheEnum.ItemInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<ItemInfo>(CacheEnum.ItemInfoList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.ItemName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetUnitSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<UnitInfo>>(CacheEnum.UnitInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<UnitInfo>(CacheEnum.UnitInfoList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.UnitName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetReqPrioritySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);

			items.AddRange(new List<SelectListItem>()
			{
				new SelectListItem(){Text = "Normal", Value = "N"},
				new SelectListItem(){Text = "High", Value = "H"},
				new SelectListItem(){Text = "Argent", Value = "A"},
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetRoomFacilityCategorySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtRoomFacilityCategory>>(CacheEnum.RoomFacilityCategoryList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtRoomFacilityCategory>(CacheEnum.RoomFacilityCategoryList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.CategoryName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetRoomSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtRoomInfo>>(CacheEnum.RoomList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtRoomInfo>(CacheEnum.RoomList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.RoomNo }));
			return items;
		}

		public IEnumerable<SelectListItem> GetRoomCategorySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtRoomCategory>>(CacheEnum.RoomCategoryList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtRoomCategory>(CacheEnum.RoomCategoryList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.CategoryName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetFloorSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtFloorInfo>>(CacheEnum.FloorList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtFloorInfo>(CacheEnum.FloorList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.FloorName }));
			return items;
		}

		public dynamic GetRoomCategoryDynamicData()
		{
			var dataList = CacheStore.Get<List<HtRoomCategory>>(CacheEnum.RoomCategoryList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtRoomCategory>(CacheEnum.RoomCategoryList.ToString());
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.CategoryName });
			return dynamicData;
		}

		public dynamic GetRoomDynamicData()
		{
			var dataList = CacheStore.Get<List<HtRoomInfo>>(CacheEnum.RoomList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtRoomInfo>(CacheEnum.RoomList.ToString());
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.RoomNo });
			return dynamicData;
		}

		public dynamic GetGuestDynamicData()
		{
			var dataList = CacheStore.Get<List<HtGuestInfo>>(CacheEnum.GuestList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtGuestInfo>(CacheEnum.GuestList.ToString());
			var dynamicData = dataList.Select(c => new { c.Id, Name = $@"{c.Salutation} {c.FirstName} {c.LastName} ({c.Mobile}) {(c.CompanyId > 0 ? $"- {c.Company?.Name}" : "")}" });
			return dynamicData;
		}

		public IEnumerable<SelectListItem> GetGuestSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtGuestInfo>>(CacheEnum.GuestList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtGuestInfo>(CacheEnum.GuestList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Salutation} {c.FirstName} {c.LastName} ({c.Mobile})" }));
			return items;
		}

		public IEnumerable<SelectListItem> GetRoomFacilitySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtRoomCategory>>(CacheEnum.RoomFacilityList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtRoomCategory>(CacheEnum.RoomCategoryList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.CategoryName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetCategoryListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<CategoryInfo>>(CacheEnum.CategoryInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<CategoryInfo>(CacheEnum.CategoryInfoList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.CategoryName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetComplementaryListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtComplementary>>(CacheEnum.ComplementaryList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtComplementary>(CacheEnum.ComplementaryList.ToString());
			dataList = dataList.Where(x => x.IsActive).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Title }));
			return items;
		}

		public dynamic GetComplementaryDynamicData()
		{
			var dataList = CacheStore.Get<List<HtComplementary>>(CacheEnum.ComplementaryList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtComplementary>(CacheEnum.ComplementaryList.ToString());
			dataList = dataList.Where(x => x.IsActive).ToList();
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.Title });
			return dynamicData;
		}

		public IEnumerable<SelectListItem> GetBookingServiceStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = BookingServiceStatusEnum.Booked.GetDescription(), Value = BookingServiceStatusEnum.Booked.ToInt32ToString() },
				new SelectListItem { Text = BookingServiceStatusEnum.CheckIn.GetDescription(), Value = BookingServiceStatusEnum.CheckIn.ToInt32ToString() },
				new SelectListItem { Text = BookingServiceStatusEnum.CheckOut.GetDescription(), Value = BookingServiceStatusEnum.CheckOut.ToInt32ToString() },
				new SelectListItem { Text = BookingServiceStatusEnum.Canceled.GetDescription(), Value = BookingServiceStatusEnum.Canceled.ToInt32ToString() },
				new SelectListItem { Text = BookingServiceStatusEnum.NoShow.GetDescription(), Value = BookingServiceStatusEnum.NoShow.ToInt32ToString() },
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetBookingReportStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = BookingReportStatusEnum.Pending.GetDescription(), Value = BookingReportStatusEnum.Pending.ToInt32ToString() },
				new SelectListItem { Text = BookingReportStatusEnum.Approved.GetDescription(), Value = BookingReportStatusEnum.Approved.ToInt32ToString() },
				new SelectListItem { Text = BookingReportStatusEnum.CheckIn.GetDescription(), Value = BookingReportStatusEnum.CheckIn.ToInt32ToString() },
				new SelectListItem { Text = BookingReportStatusEnum.CheckOut.GetDescription(), Value = BookingReportStatusEnum.CheckOut.ToInt32ToString() },
				new SelectListItem { Text = BookingReportStatusEnum.Canceled.GetDescription(), Value = BookingReportStatusEnum.Canceled.ToInt32ToString() },
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetPayemntStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = "All", Value = "10" },
				new SelectListItem { Text = PaymentStatusEnum.Pending.GetDescription(), Value = PaymentStatusEnum.Pending.ToInt32ToString() },
				new SelectListItem { Text = PaymentStatusEnum.PartialPayment.GetDescription(), Value = PaymentStatusEnum.PartialPayment.ToInt32ToString() },
				new SelectListItem { Text = PaymentStatusEnum.FullPayment.GetDescription(), Value = PaymentStatusEnum.FullPayment.ToInt32ToString() }
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetPayModeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = PayModeEnum.Cash.GetDescription(), Value = PayModeEnum.Cash.ToInt32ToString() },
				new SelectListItem { Text = PayModeEnum.Bank.GetDescription(), Value = PayModeEnum.Bank.ToInt32ToString() },
				new SelectListItem { Text = PayModeEnum.Bkash.GetDescription(), Value = PayModeEnum.Bkash.ToInt32ToString() },
				new SelectListItem { Text = PayModeEnum.Card.GetDescription(), Value = PayModeEnum.Card.ToInt32ToString() }
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetCategoryTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "Item Type", Value = "I" },
				new SelectListItem { Text = "Service Type", Value = "S" },
			};
			return items;
		}

		public IEnumerable<SelectListItem> GetItemCategorySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<CategoryInfo>>(CacheEnum.ItemCategoryList.ToString()) ?? _cacheStoreService.AddOrUpdate<CategoryInfo>(CacheEnum.ItemCategoryList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.CategoryName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetApprovedOnlineBookingSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtOnlineBooking>>(CacheEnum.OnlineBookingList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtOnlineBooking>(CacheEnum.OnlineBookingList.ToString());
			dataList = dataList.Where(x => x.Status == OnlineBookingStatusEnum.Approved).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.OnlineBookingNumber }));
			return items;
		}

		public IEnumerable<SelectListItem> GetRoomBookingStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = BookingStatusEnum.Available.GetDescription(), Value = BookingStatusEnum.Available.ToInt32ToString() },
				new SelectListItem { Text = BookingStatusEnum.Booked.GetDescription(), Value = BookingStatusEnum.Booked.ToInt32ToString() },
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetRoomAvailabilityStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = AvailabilityStatusEnum.Available.GetDescription(), Value = AvailabilityStatusEnum.Available.ToInt32ToString() },
				new SelectListItem { Text = AvailabilityStatusEnum.Occupied.GetDescription(), Value = AvailabilityStatusEnum.Occupied.ToInt32ToString() },
				new SelectListItem { Text = AvailabilityStatusEnum.OutOfOrder.GetDescription(), Value = AvailabilityStatusEnum.OutOfOrder.ToInt32ToString() },
			});
			return items;
		}
		public IEnumerable<SelectListItem> GetRoomAvailabilityStatusForHKSelectListItems(bool isDefaultSelectAdd = false)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = "---Select---", Value = "-1" },
				new SelectListItem { Text = "Available", Value = "0" },
				new SelectListItem { Text = "Occupied", Value = "1" },
				new SelectListItem { Text = "OutOfOrder", Value = "2" },
				new SelectListItem { Text = "Today's C/In", Value = "3" },
				new SelectListItem { Text = "Expected C/Out", Value = "4" },
				new SelectListItem { Text = "Reserved", Value = "5" },
				new SelectListItem { Text = "Checked Out", Value = "6" },
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetCleanStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = CleaningStatusEnum.VC.GetDescription(), Value = CleaningStatusEnum.VC.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.VD.GetDescription(), Value = CleaningStatusEnum.VD.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.O.GetDescription(), Value = CleaningStatusEnum.O.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.CO.GetDescription(), Value = CleaningStatusEnum.CO.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.OOO.GetDescription(), Value = CleaningStatusEnum.OOO.ToInt32ToString() },
			});
			return items;
		}

		//Used for HK DashBoard
		public IEnumerable<SelectListItem> GetHKCleanStatusSelectListItems(bool isDefaultSelectAdd = false)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = "---Select---", Value = "-1" },
				new SelectListItem { Text = CleaningStatusEnum.VC.GetDescription(), Value = CleaningStatusEnum.VC.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.VD.GetDescription(), Value = CleaningStatusEnum.VD.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.O.GetDescription(), Value = CleaningStatusEnum.O.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.CO.GetDescription(), Value = CleaningStatusEnum.CO.ToInt32ToString() },
				new SelectListItem { Text = CleaningStatusEnum.OOO.GetDescription(), Value = CleaningStatusEnum.OOO.ToInt32ToString() },
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetFoodCategorySelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RsFoodCategory>>(CacheEnum.FoodCategoryList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsFoodCategory>(CacheEnum.FoodCategoryList.ToString());
			dataList = dataList.Where(x => x.IsActive).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.CategoryName }));
			return items;
		}

		public dynamic GetFoodCategoryDynamicData()
		{
			var dataList = CacheStore.Get<List<RsFoodCategory>>(CacheEnum.FoodCategoryList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsFoodCategory>(CacheEnum.FoodCategoryList.ToString());
			dataList = dataList.Where(x => x.IsActive).ToList();
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.CategoryName });
			return dynamicData;
		}

		public dynamic GetRsCustomerDynamicData(string customerTypeCode)
		{
			var dataList = CacheStore.Get<List<RsCustomer>>(CacheEnum.RsCustomerList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsCustomer>(CacheEnum.RsCustomerList.ToString());
			if (customerTypeCode == RsCustomerTypeCode.Employee)
				dataList = dataList.Where(x => x.EmployeeId > 0).ToList();
			else if (customerTypeCode == RsCustomerTypeCode.Hotel)
				dataList = dataList.Where(x => x.GuestId > 0).ToList();
			else
				dataList = dataList.Where(x => !x.IsDeleted).ToList();

			var dynamicData = dataList.Select(c => new { c.Id, Name = $"{c.Salutation} {c.FirstName} {c.LastName} ({c.Mobile})" });
			return dynamicData;
		}

		public IEnumerable<SelectListItem> GetRsCustomerSelectListItems(bool isDefaultSelectAdd = true, string customerTypeCode = "")
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RsCustomer>>(CacheEnum.RsCustomerList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsCustomer>(CacheEnum.RsCustomerList.ToString());

			if (customerTypeCode == RsCustomerTypeCode.Employee)
				dataList = dataList.Where(x => x.EmployeeId > 0).ToList();
			else if (customerTypeCode == RsCustomerTypeCode.Hotel)
				dataList = dataList.Where(x => x.GuestId > 0).ToList();
			else
				dataList = dataList.Where(x => !x.IsDeleted).ToList();

			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Salutation} {c.FirstName} {c.LastName} ({c.Mobile})" }));
			return items;
		}

		public IEnumerable<SelectListItem> GetCustomerTypeSelectListItems(bool isDefaultSelectAdd = true, bool withoutHotel = false)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RsCustomerType>>(CacheEnum.CustomerTypeList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsCustomerType>(CacheEnum.CustomerTypeList.ToString());

			if (withoutHotel)
			{
				dataList = dataList.Where(x => x.TypeCode != RsCustomerTypeCode.Hotel).ToList();
			}

			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.TypeName }));
			return items;
		}

		public dynamic GetCustomerTypeDynamicData()
		{
			var dataList = CacheStore.Get<List<RsCustomerType>>(CacheEnum.CustomerTypeList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsCustomerType>(CacheEnum.CustomerTypeList.ToString());
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.TypeName, Code = c.TypeCode, Discount = c.DiscountPercent });
			return dynamicData;
		}

		public IEnumerable<SelectListItem> GetTableSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RsTable>>(CacheEnum.TableList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsTable>(CacheEnum.TableList.ToString());
			dataList = dataList.Where(x => x.IsActive).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.TableNo }));
			return items;
		}

		public IEnumerable<SelectListItem> GetFoodOrderStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = RsOrderStatusEnum.Pending.GetDescription(), Value = RsOrderStatusEnum.Pending.ToInt32ToString() },
				new SelectListItem { Text = RsOrderStatusEnum.Served.GetDescription(), Value = RsOrderStatusEnum.Served.ToInt32ToString() },
				new SelectListItem { Text = RsOrderStatusEnum.Canceled.GetDescription(), Value = RsOrderStatusEnum.Canceled.ToInt32ToString() },
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetFoodOrderPaymentStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = RsOrderPaymentStatusEnum.Pending.GetDescription(), Value = RsOrderPaymentStatusEnum.Pending.ToInt32ToString() },
				new SelectListItem { Text = RsOrderPaymentStatusEnum.PartialPayment.GetDescription(), Value = RsOrderPaymentStatusEnum.PartialPayment.ToInt32ToString() },
				new SelectListItem { Text = RsOrderPaymentStatusEnum.FullPayment.GetDescription(), Value = RsOrderPaymentStatusEnum.FullPayment.ToInt32ToString() },
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetPermanentShiftSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<DutyShift>>(CacheEnum.DutyShiftList.ToString()) ?? _cacheStoreService.AddOrUpdate<DutyShift>(CacheEnum.DutyShiftList.ToString());
			dataList = dataList.Where(x => x.ShiftType == "P").ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.ShiftName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetDutyShiftSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<DutyShift>>(CacheEnum.DutyShiftList.ToString()) ?? _cacheStoreService.AddOrUpdate<DutyShift>(CacheEnum.DutyShiftList.ToString());
			dataList = dataList.Where(x => x.ShiftType == "D").ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.ShiftName }));
			return items;
		}

		public IEnumerable<SelectListItem> GetServiceSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtService>>(CacheEnum.HtServiceList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtService>(CacheEnum.HtServiceList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"({c.ServiceCode}){c.ServiceName}" }));
			return items;
		}

		public IEnumerable<SelectListItem> GetExtraServiceSelectListItems(bool isDefaultSelectAdd = true, bool ignoreExtraBed = false)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtService>>(CacheEnum.HtServiceList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtService>(CacheEnum.HtServiceList.ToString());
			
			if (ignoreExtraBed)
			{
				dataList = dataList.Where(x => x.ServiceCode != HtServiceCode.ExtraBed).ToList();
			}

			items.AddRange(dataList.Where(x => x.IsExtra).Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"({c.ServiceCode}){c.ServiceName}" }));
			return items;
		}

		public IEnumerable<SelectListItem> GetRefundCancelBookingSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtBookingService>>(CacheEnum.RefundCancelBookingList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtBookingService>(CacheEnum.RefundCancelBookingList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{DU.Utility.GetDate(c.BookingDate)}-({c.BookingNo})" }));
			return items;
		}

		public IEnumerable<SelectListItem> GetFoodItemSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RsFoodItem>>(CacheEnum.FoodItemList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsFoodItem>(CacheEnum.FoodItemList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.ItemCode}_{c.ItemName}" }));
			return items;
		}

		#region WaiterSelectListItems

		public IEnumerable<SelectListItem> GetWaiterSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<RsWaiter>>(CacheEnum.WaiterList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsWaiter>(CacheEnum.WaiterList.ToString());
			dataList = dataList.Where(x => x.IsActive && !x.IsDeleted).ToList();
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name }));
			return items;
		}

		#endregion

		public IEnumerable<SelectListItem> GetHallShiftSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = HallBookingShiftEnum.DayShift.GetDescription(), Value = HallBookingShiftEnum.DayShift.ToInt32ToString() },
				new SelectListItem { Text = HallBookingShiftEnum.NightShift.GetDescription(), Value = HallBookingShiftEnum.NightShift.ToInt32ToString() },
				new SelectListItem { Text = HallBookingShiftEnum.Both.GetDescription(), Value = HallBookingShiftEnum.Both.ToInt32ToString() }
			});
			return items;
		}

		public dynamic GetHallShiftDynamicData()
		{
			var items = new List<SelectListItem>();
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = HallBookingShiftEnum.DayShift.GetDescription(), Value = HallBookingShiftEnum.DayShift.ToInt32ToString() },
				new SelectListItem { Text = HallBookingShiftEnum.NightShift.GetDescription(), Value = HallBookingShiftEnum.NightShift.ToInt32ToString() },
				new SelectListItem { Text = HallBookingShiftEnum.Both.GetDescription(), Value = HallBookingShiftEnum.Both.ToInt32ToString() }
			});
			var dynamicData = items.Select(c => new { Id = c.Value, Name = c.Text });
			return dynamicData;
		}

		public IEnumerable<SelectListItem> GetHallSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<HtHallInfo>>(CacheEnum.HallList.ToString()) ?? _cacheStoreService.AddOrUpdate<HtHallInfo>(CacheEnum.HallList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.HallName}" }));
			return items;
		}

		public dynamic GetItemDynamicData()
		{
			var dataList = CacheStore.Get<List<ItemInfo>>(CacheEnum.ItemInfoList.ToString()) ?? _cacheStoreService.AddOrUpdate<ItemInfo>(CacheEnum.ItemInfoList.ToString());
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.ItemName });
			return dynamicData;
		}
		public dynamic GetFoodItemDynamicData()
		{
			var dataList = CacheStore.Get<List<RsFoodItem>>(CacheEnum.FoodItemList.ToString()) ?? _cacheStoreService.AddOrUpdate<RsFoodItem>(CacheEnum.FoodItemList.ToString());
			var dynamicData = dataList.Select(c => new { c.Id, Name = c.ItemName });
			return dynamicData;
		}

		public IEnumerable<SelectListItem> GetDrCrSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
                //new SelectListItem { Text = "Debit", Value = "C" },
                //new SelectListItem { Text = "Credit", Value = "D" },

                 new SelectListItem { Text = "Decrease", Value = "C" },
				 new SelectListItem { Text = "Increase", Value = "D" },
			};
			return items;
		}

		public IEnumerable<SelectListItem> GetNtfEventSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			var dataList = CacheStore.Get<List<NtfEventInfo>>(CacheEnum.NtfEventList.ToString()) ?? _cacheStoreService.AddOrUpdate<NtfEventInfo>(CacheEnum.NtfEventList.ToString());
			items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.EventName}" }));
			return items;
		}

		public IEnumerable<SelectListItem> GetDueReportGroupByList(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "Order Wise", Value = "O" },
				new SelectListItem { Text = "Client Wise", Value = "C" },
			};
			return items;
		}
		public IEnumerable<SelectListItem> GetFoodOrderReportGroupByList(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "Item Wise", Value = "I" },
				new SelectListItem { Text = "Order Wise", Value = "O" }
			};
			return items;
		}

		public IEnumerable<SelectListItem> GetAdvanceTypeSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);

			items.AddRange(new List<SelectListItem>()
			{
				new SelectListItem(){Text = "Room Reservation", Value = "RR"},
				new SelectListItem(){Text = "Room Advance", Value = "RA"},
				new SelectListItem(){Text = "Hall Reservation", Value = "HR"},
				new SelectListItem(){Text = "Hall Advance", Value = "HA"},
			});
			return items;
		}

		public IEnumerable<SelectListItem> GetGuestDueReportGroupByList(bool isDefaultSelectAdd = true)
		{
			var items = new List<SelectListItem>()
			{
				new SelectListItem { Text = "Booking Wise", Value = "G" },
				new SelectListItem { Text = "Group Wise", Value = "C" },
			};
			return items;
		}

		public IEnumerable<SelectListItem> GetApprovalStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = "All", Value = "All" },
				new SelectListItem { Text = OnlineBookingStatusEnum.Pending.GetDescription(), Value = OnlineBookingStatusEnum.Pending.ToInt32ToString() },
				new SelectListItem { Text = OnlineBookingStatusEnum.Approved.GetDescription(), Value = OnlineBookingStatusEnum.Approved.ToInt32ToString() },
				new SelectListItem { Text = OnlineBookingStatusEnum.Canceled.GetDescription(), Value = OnlineBookingStatusEnum.Canceled.ToInt32ToString() }
			});


			return items;
		}

		public IEnumerable<SelectListItem> GetHKRoomAssignStatusSelectListItems(bool isDefaultSelectAdd = true)
		{
			var items = GetDefaultSelectListItem(isDefaultSelectAdd);
			items.AddRange(new List<SelectListItem>
			{
				new SelectListItem { Text = RoomAssignEnum.Assigned.GetDescription(), Value = RoomAssignEnum.Assigned.ToInt32ToString() },
				new SelectListItem { Text = RoomAssignEnum.Running.GetDescription(), Value = RoomAssignEnum.Running.ToInt32ToString() },
				new SelectListItem { Text = RoomAssignEnum.Completed.GetDescription(), Value = RoomAssignEnum.Completed.ToInt32ToString() }
			});


			return items;
		}

        public IEnumerable<SelectListItem> GetStockStatusSelectListItems(bool isDefaultSelectAdd = true)
        {
            var items = GetDefaultSelectListItem(isDefaultSelectAdd);
            items.AddRange(new List<SelectListItem>
            {
				new SelectListItem { Text = StockStatusEnum.InStock.GetDescription(), Value = StockStatusEnum.InStock.ToInt32ToString() },
				new SelectListItem { Text = StockStatusEnum.OutOfStock.GetDescription(), Value = StockStatusEnum.OutOfStock.ToInt32ToString() }
			});


            return items;
        }
    }
}
