
using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.DutyShift;
using Interface.Repository.Attendance;
using Interface.Services.Attendance;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Attendance;

public class DutyShiftService : BaseService<DutyShift>, IDutyShiftService
{
    #region Config
    private IDutyShiftRepository _iRepository;
    private readonly IMapper _imapper;
    private readonly IUnitOfWork _unitOfWork;

    public DutyShiftService(IDutyShiftRepository repository, IMapper imapper, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _iRepository = repository;
        _imapper = imapper;
        _unitOfWork = unitOfWork;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm>> SearchAsync(DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion
}