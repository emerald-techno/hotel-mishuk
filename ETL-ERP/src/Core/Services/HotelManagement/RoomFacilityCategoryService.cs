using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomFacilityCategory;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class RoomFacilityCategoryService : BaseService<HtRoomFacilityCategory>, IRoomFacilityCategoryService
{
    #region Config
    private IRoomFacilityCategoryRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IRoomFacilityRepository _iRoomFacilityRepository;

    public RoomFacilityCategoryService(IRoomFacilityCategoryRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IRoomFacilityRepository iRoomFacilityRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iRoomFacilityRepository = iRoomFacilityRepository;
    }

    #endregion

    public async Task<DataTablePagination<HtRoomFacilityCategorySearchVm, HtRoomFacilityCategorySearchVm>> SearchAsync(DataTablePagination<HtRoomFacilityCategorySearchVm, HtRoomFacilityCategorySearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<List<SaveRoomFacilityCategoryVm>> GetCategoryWithFacility()
    {
        var categoryList = await _iRepository.GetAsync(x => x.IsActive && !x.IsDeleted);

        var dataList = new List<SaveRoomFacilityCategoryVm>();

        if (categoryList.Count > 0 is false)
            return dataList;

        foreach (var item in categoryList)
        {
            var model = new SaveRoomFacilityCategoryVm();
            model.FacilityCategoryId = item.Id;
            model.FacilityCategoryName = item.CategoryName;

            var facilityList = await _iRoomFacilityRepository.GetAsync(x => x.FacilityCategoryId == item.Id && x.IsActive && !x.IsDeleted);

            model.SaveRoomFacilities = facilityList.Select(x => new SaveRoomFacilityVm
            {
                FacilityId = x.Id,
                FacilityName = x.FacilityName
            }).ToList();

            dataList.Add(model);
        }

        return dataList;
    }
}