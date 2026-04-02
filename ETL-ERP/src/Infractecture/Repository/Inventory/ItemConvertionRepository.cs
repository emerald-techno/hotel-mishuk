using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemConversion;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Inventory
{
    public class ItemConvertionRepository : BaseRepository<ItemConvertion>, IItemConvertionRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public ItemConvertionRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search
       public async Task<DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm>> SearchAsync(DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm> vm)
        {
            var searchResult = Context.ItemConvertions
                .Include(x => x.Item)
                .Include(x=>x.Unit)
                .Include(x=>x.ConvertedUnit)
                .AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Food Convertion not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Item.ItemName.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.Item.ItemName)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<ItemConvertionSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.ItemName = filterData.Item.ItemName;
                    searchDto.UnitName = filterData.Unit.UnitName;
                    searchDto.ConvertedUnitName = filterData.ConvertedUnit.UnitName;
                }
            }
            return vm;
        }
        #endregion

        public void Dispose()
        {
            Context.Dispose();
        }

        
    }
}
