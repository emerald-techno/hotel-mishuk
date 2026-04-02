using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Inventory.ItemConversion
{
    public class ItemConvertionSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public double Quantity { get; set; } = 1;
        public double ConvertedQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime? DisableDate { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? ActionById { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsAjaxPost { get; set; }

        //-----FK----------

        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public long UnitId { get; set; }
        public string UnitName { get; set; }
        public long? ConvertedUnitId { get; set; }
        public string ConvertedUnitName { get; set; }

        public IEnumerable<SelectListItem> ItemLookUp { get; set; }
        public IEnumerable<SelectListItem> UnitLookUp { get; set; }



        // --- Datatable ---
        public long UserId { get ; set ; }
        public int SerialNo { get ; set ; }
        public bool CanCreate { get ; set ; }
        public bool CanUpdate { get ; set ; }
        public bool CanView { get ; set ; }
        public bool CanDelete { get ; set ; }
    }
}
