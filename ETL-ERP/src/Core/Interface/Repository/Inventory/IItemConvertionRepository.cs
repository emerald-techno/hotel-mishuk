using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemConversion;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repository.Inventory
{
    public interface IItemConvertionRepository :IRepository<ItemConvertion>
    {
        Task<DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm>>
        SearchAsync(DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm> vm);
    }
}
