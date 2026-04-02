namespace Domain.ViewModel.Inventory.RequsitionInfo
{
    public class RequsitionApprovalDtlVm
    {
        public long Id { get; set; }
        public double? AprReqQty { get; set; }

    }

    public class RequsitionApprovalVm
    {
        public long Id { get; set; }
        public short Status { get; set; }

        public virtual ICollection<RequsitionApprovalDtlVm> ApprovalDtls { get; set; }

    }

}
