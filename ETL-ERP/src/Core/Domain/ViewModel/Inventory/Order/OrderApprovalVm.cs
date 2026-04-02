namespace Domain.ViewModel.Inventory.Order
{
    public class OrderApprovalDtlVm
    {
        public long Id { get; set; }
        public double AprOrderQty { get; set; }
    }
    public class OrderApprovalVm
    {
        public long Id { get; set; }
        public short Status { get; set; }
        public virtual ICollection<OrderApprovalDtlVm> ApprovalDtls { get; set; }
    }
}
