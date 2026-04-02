using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.RequsitionInfo;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using System.Data;
using DU = Domain.Utility;

namespace Repository.Inventory
{
    public class RequsitionInfoRepository : BaseRepository<RequsitionInfo>, IRequsitionInfoRepository, IDisposable
    {
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public RequsitionInfoRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public async Task<RequsitionInfoVm> GetRequsitionInfoByIdAsync(long id)
        {
            var dataModel = await Context.RequsitionInfos
                .Include(c => c.RequsitionInfoDtls)
                    .ThenInclude(c => c.Item)
                .Include(c => c.RequsitionInfoDtls)
                    .ThenInclude(c => c.ItemUnit)
                .Include(c => c.SubmitBy)
                .Include(c => c.Dept)
                .Include(c => c.ReqBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            var model = _iMapper.Map<RequsitionInfoVm>(dataModel);

            model.Departement = dataModel.Dept?.Name;
            model.RequsitionFor = dataModel.ReqBy?.Name;
            model.SubmitByName = dataModel.SubmitBy?.FullName;

            foreach (var data in model.RequsitionInfoDtls)
            {
                var reqDtl = dataModel.RequsitionInfoDtls.FirstOrDefault(c => c.Id == data.Id);
                if (reqDtl == null) continue;

                data.ReqNo = reqDtl?.Req.ReqNo;
                data.ItemName = reqDtl.Item?.ItemName;
                data.ItemUnitName = reqDtl.ItemUnit?.UnitName;
            }

            return model;
        }

        public async Task<DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm>>
            SearchAsync(DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm> vm)
        {
            var searchResult = Context.RequsitionInfos
                .Include(c => c.Dept)
                .Include(c => c.ReqBy)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Requsition not found");

            if (!string.IsNullOrEmpty(model.ReqNo))
            {
                searchResult = searchResult.Where(c => c.ReqNo.ToLower() == model.ReqNo.ToLower());
            }

            if (model.SFromDate != null)
            {
                var fromDate = DU.Utility.ConvertStrToDate(model.SFromDate);
                searchResult = searchResult.Where(c => c.ReqDate >= fromDate);
            }
            if (model.SToDate != null)
            {
                var toDate = DU.Utility.ConvertStrToDate(model.SToDate);
                searchResult = searchResult.Where(c => c.ReqDate <= toDate);
            }

            if (model.DeptId > 0)
            {
                searchResult = searchResult.Where(c => c.DeptId == model.DeptId);
            }

            if (model.ReqById > 0)
            {
                searchResult = searchResult.Where(c => c.ReqById == model.ReqById);
            }

            if (model.Priority != null)
            {
                searchResult = searchResult.Where(c => c.Priority == model.Priority);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.ReqNo.ToLower().Contains(value));
            }


            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderByDescending(c => c.ActionDate)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<RequsitionInfoSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.DeptName = filterData?.Dept?.Name;
                    searchDto.ReqByName = filterData?.ReqBy?.Name;
                }
            }
            return vm;
        }

    }
}
