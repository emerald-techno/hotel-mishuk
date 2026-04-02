using AutoMapper;
using Domain.Entities.Payroll;
using Interface.Repository.Payroll;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Payroll;

public class MonthlyAttSheetDtlRepository : BaseRepository<MonthlyAttendanceSheetDtl>, IMonthlyAttSheetDtlRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public MonthlyAttSheetDtlRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}