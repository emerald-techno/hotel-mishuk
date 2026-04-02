using AutoMapper;
using Domain.Entities;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryMst;
using Interface.Repository.Payroll;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Payroll;

public class PrSalaryMstRepository : BaseRepository<PrSalaryMst>, IPrSalaryMstRepository, IDisposable
{
    #region Config

    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    public PrSalaryMstRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm>> SearchAsync(DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm> vm)
    {
        var searchResult = Context.PrSalaryMsts
            .Include(c => c.PrSalaryDtls)
            .AsQueryable().Where(c => !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null) throw new Exception("No Payroll Found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Year.ToString().ToLower().Contains(value));
        }
        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Year)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<PrSalaryMstSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
            }
        }
        return vm;
    }

    #endregion

    #region GetPrSalary

    public async Task<PrSalaryMst> GetPrSalaryByIdAsync(long id)
    {
        var data = await Context.PrSalaryMsts
            .Include(c => c.ApprovedBy)
            .Include(c => c.PrSalaryDtls)
                .ThenInclude(e => e.Employee)
                    .ThenInclude(d => d.Designation)
            .Include(c => c.PrSalaryDtls)
                .ThenInclude(e => e.Employee)
                    .ThenInclude(dp => dp.Department)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (data == null) throw new Exception("No Payroll Data Found");

        return data;
    }

    #endregion

    #region GetPrSalaryByMonth

    public async Task<PrSalaryMst> GetPrSalaryByMonthAsync(int year, int month)
    {
        var data = await Context.PrSalaryMsts
            .Include(c => c.ApprovedBy)
            .Include(c => c.PrSalaryDtls)
                .ThenInclude(e => e.Employee)
                    .ThenInclude(d => d.Designation)
            .Include(c => c.PrSalaryDtls)
                .ThenInclude(e => e.Employee)
                    .ThenInclude(dp => dp.Department)
            .FirstOrDefaultAsync(c => c.Year == year && c.Month == month && !c.IsDeleted);

        if (data == null) 
            throw new Exception("No Payroll Data Found");

        return data;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}
