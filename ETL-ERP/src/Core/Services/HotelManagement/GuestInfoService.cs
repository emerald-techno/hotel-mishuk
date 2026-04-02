using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.GuestInfo;
using Interface.Repository.Admin;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class GuestInfoService : BaseService<HtGuestInfo>, IGuestInfoService
{
    #region Config
    private IGuestInfoRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly ISetCountryRepository _iCountryRepository;
    private readonly IClientCompanyRepository _iClientCompanyRepository;

    public GuestInfoService(IGuestInfoRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork,
        IAutoCodeRepository iAutoCodeRepository,
        ISetCountryRepository iCountryRepository,
        IClientCompanyRepository iClientCompanyRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iCountryRepository = iCountryRepository;
        _iClientCompanyRepository = iClientCompanyRepository;
    }

    #endregion

    #region Create
    public async Task<(bool, long)> GuestEntry(HtGuestInfoVm vm)
    {
        if (vm == null)
            throw new Exception("Guest info not found...!");

        var existingGuestInfo = _iRepository.GetSingleOrDefault(d => d.Mobile == vm.Mobile);
        if (existingGuestInfo != null)
            throw new Exception("Guest mobile number already exists");

        var guestModel = _iMapper.Map<HtGuestInfo>(vm);

        guestModel.GuestCode = await GetGuestCode();
        guestModel.ActionById = CurrentUserId;
        guestModel.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
        guestModel.Dob = DU.Utility.ConvertStrToDate(vm.DobString);

        var bd = _iCountryRepository.GetSingleOrDefault(x => x.Code == "BD");
        if (bd == null)
            throw new Exception("Can't find bangladesh country...!");

        guestModel.CountryId = guestModel.CountryId > 0 ? guestModel.CountryId : bd.Id;
        guestModel.DistrictId = vm.DistrictId ?? null;

        #region Company

        ClientCompany company = null;

        if (vm.CompanyId > 0)
        {
            guestModel.CompanyId = vm.CompanyId;
        }
        else
        {
            if (!string.IsNullOrEmpty(vm.CompanyName) || !string.IsNullOrEmpty(vm.CompanyMobile))
            {
                var existingCompanyInfo = _iClientCompanyRepository.GetSingleOrDefault(d => d.Mobile == vm.CompanyMobile);
                if (existingCompanyInfo != null)
                    throw new Exception("Company mobile number already exists");

                company = new ClientCompany();
                company.Name = vm.CompanyName;
                company.Mobile = vm.CompanyMobile;
                company.ActionById = CurrentUserId;
                company.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
            }
        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (company != null)
        {
            await _iClientCompanyRepository.AddAsync(company);
            await _iUnitOfWork.CompleteAsync();

            guestModel.CompanyId = company.Id;
        }

        await _iRepository.AddAsync(guestModel);

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) return (false, 0);
        ts.Complete();
        return (true, guestModel.Id);
    }

    #endregion

    #region Search
    public async Task<DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm>> SearchAsync(DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion

    #region GetGuestCode

    public async Task<string> GetGuestCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.HtGuestInfos.ToString(), "GuestCode", "GST", 6);
        return data;
    }

    #endregion

    #region GuestPrint
    public async Task<string> GuestPrintHtml(long id)
    {
        var model = await _iRepository.GetGuestByIdAsync(id);

        var fullHtml = "";

        // Basic information
        fullHtml += "<div style=''>";
        fullHtml += "<p style='padding-bottom:10px;'>";
        fullHtml += $@"<b style='font-size:22px;'>{model.Salutation} {model.FirstName} {model.LastName}</b>";
        fullHtml += "</p>";

        fullHtml += $@"<table class='cv-info-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:30%;padding:3px 0px;'>Guest Code</td>
                                        <td style='width:70%;padding:3px 0px;'>: {model.GuestCode}</td>
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

        fullHtml += "<tbody>";

        fullHtml += "<tr>";
        fullHtml += $"<td style='width:15%;'>Gender</td><td style='width:35%;'>{model.GenderText}</td>";
        fullHtml += $"<td style='width:15%;'>Occupation</td><td style='width:35%;'>{model.Occupation}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $"<td>District</td><td>{model.DistrictName}</td>";
        fullHtml += $"<td>Address</td><td>{model.Address}</td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";
        fullHtml += "</div>";

        return fullHtml;
    }

    #endregion

    #region Guest Details
    public async Task<HtGuestDetailsVm> GetGuestDetails(long id)
    {
        var model = await _iRepository.GetGuestDetailsByIdAsync(id);
        return model;
    }
    #endregion

    #region Update
    public async Task<(bool, long)> GuestUpdateAsync(HtGuestInfoVm vm)
    {
        if (vm == null)
            throw new Exception("Guest info not found...!");

        var existingGuestInfo = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id);
        if (existingGuestInfo == null)
            throw new Exception("No Guest Found");

        //var preservedGuestCode = existingGuestInfo.GuestCode;

        //_iMapper.Map(vm, existingGuestInfo);


        existingGuestInfo.Salutation = vm.Salutation;
        existingGuestInfo.FirstName = vm.FirstName;
        existingGuestInfo.LastName = vm.LastName;
        existingGuestInfo.Dob = DU.Utility.ConvertStrToDate(vm.DobString); ;
        existingGuestInfo.Gender = vm.Gender;
        existingGuestInfo.Mobile = vm.Mobile;
        existingGuestInfo.Email = vm.Email;
        existingGuestInfo.Address = vm.Address;
        existingGuestInfo.IdentityType = vm.IdentityType;
        existingGuestInfo.IdentityNo = vm.IdentityNo;
        existingGuestInfo.DistrictId = vm.DistrictId;
        existingGuestInfo.IsVip = vm.IsVip;

        existingGuestInfo.UpdatedById = CurrentUserId;
        existingGuestInfo.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();

        var country = vm.CountryId > 0
            ? await _iCountryRepository.GetFirstOrDefaultAsync(x => x.Id == vm.CountryId)
            : await _iCountryRepository.GetFirstOrDefaultAsync(x => x.Code == "BD");

        if (country == null)
            throw new Exception("Can't find country...!");

        existingGuestInfo.CountryId = country.Id;

        #region Company

        ClientCompany company = null;

        if (vm.CompanyId > 0)
        {
            company = _iClientCompanyRepository.GetById((long)vm.CompanyId);
            existingGuestInfo.CompanyId = company.Id;
        }
        else if (!string.IsNullOrEmpty(vm.CompanyName) || !string.IsNullOrEmpty(vm.CompanyMobile))
        {
            var existingCompanyInfo = _iClientCompanyRepository.GetSingleOrDefault(d => d.Mobile == vm.CompanyMobile);
            if (existingCompanyInfo != null)
                throw new Exception("Company mobile number already exists");

            company = new ClientCompany
            {
                Name = vm.CompanyName,
                Mobile = vm.CompanyMobile,
                ActionById = CurrentUserId,
                ActionDate = Domain.Utility.Utility.GetBdDateTimeNow()
            };
        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (company != null && company.Id == 0)
        {
            await _iClientCompanyRepository.AddAsync(company);
            await _iUnitOfWork.CompleteAsync();
            existingGuestInfo.CompanyId = company.Id;
        }

        //await _iRepository.UpdateAsync(existingGuestInfo);

        var isUpdated = await _iUnitOfWork.CompleteAsync();
        if (!isUpdated)
            throw new Exception("Guest update failed");

        ts.Complete();

        return (true, existingGuestInfo.Id);
    }

    #endregion
}