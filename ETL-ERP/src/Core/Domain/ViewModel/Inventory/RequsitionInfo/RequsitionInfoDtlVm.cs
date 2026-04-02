namespace Domain.ViewModel.Inventory.RequsitionInfo
{
    public class RequsitionInfoDtlVm
    {
        public long Id { get; set; }
        public double ReqQty { get; set; }
        public double Stock { get; set; }
        public double? AprReqQty { get; set; }
        public double? IssueQty { get; set; }
        public long SlNo { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }

        //---- FK ----      

        public long ReqId { get; set; }
        public string ReqNo { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public long ItemUnitId { get; set; }
        public string ItemUnitName { get; set; }
    }
}
