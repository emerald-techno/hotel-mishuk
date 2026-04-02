using AutoMapper;
using Domain.Entities.HotelManagement;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class DateBreakfastService : BaseService<HtDateBreakfast>, IDateBreakfastService
{
    #region Config
    private IDateBreakfastRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;

    public DateBreakfastService(IDateBreakfastRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
    }

    #endregion

    public async Task<bool> TodayCbfAdded()
    {
        var todayCbf = await _iRepository.GetFirstOrDefaultAsync(x => x.BreakfastDate.Date == DateTime.Today.Date && !x.IsDeleted);

        if (todayCbf != null)
            return true;
        else
            return false;
    }

    public async Task<bool> CheckCbfByDate(DateTime cbfDate)
    {
        var cbf = await _iRepository.GetFirstOrDefaultAsync(x => x.BreakfastDate.Date == cbfDate.Date && !x.IsDeleted);

        if (cbf != null)
            return true;
        else
            return false;
    }
}