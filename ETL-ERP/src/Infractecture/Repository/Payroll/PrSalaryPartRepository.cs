using AutoMapper;
using Domain.Entities;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Repository.Payroll;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Payroll
{
    public class PrSalaryPartRepository : BaseRepository<PrSalaryPart>, IPrSalaryPartRepository, IDisposable
    {
        #region Config

        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public PrSalaryPartRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm>> SearchAsync(DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm> vm)
        {
            var searchResult = Context.PrSalaryParts.AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search payroll parts not found");

            if (model.IsEnable)
            {
                searchResult = searchResult.Where(p => p.IsEnable);
            }
            else
            {
                searchResult = searchResult.Where(p => !p.IsDeleted);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.PartName.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.SlNo)
                                             .ThenBy(c => c.IsEnable)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<PrSalaryPartSearchVm>>(data);

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

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}
