namespace Domain.ViewModel.Inventory.Order
{
    public class OrderDtlVm
    {
        public long Id { get; set; }
        public double OrderQty { get; set; }
        public double AprOrderQty { get; set; }
        public double ActualAmount { get; set; }
        public double Stock { get; set; }
        public double Rate { get; set; }
        public long SlNo { get; set; }
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        //--------------FK-------------------

        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public long ItemUnitId { get; set; }
        public string ItemUnitName { get; set; }
        public long ActionById { get; set; }
        public long? UpdatedById { get; set; }
    }
}
