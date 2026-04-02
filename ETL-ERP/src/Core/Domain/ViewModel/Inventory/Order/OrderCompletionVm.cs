namespace Domain.ViewModel.Inventory.Order
{
    public class OrderCompletionDtlVm
    {
        public long Id { get; set; }
        public double ActualAmount { get; set; }
    }


    public class OrderCompletionVm
    {
        public long Id { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string CompleteRemarks { get; set; }
        public virtual ICollection<OrderCompletionDtlVm> OrderCompletionDtls { get; set; }
    }
}
