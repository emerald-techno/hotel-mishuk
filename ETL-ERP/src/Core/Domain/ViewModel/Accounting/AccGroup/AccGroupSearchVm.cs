using Domain.ModelInterface;

namespace Domain.ViewModel.Accounting.AccGroup
{
    public class AccGroupSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public string GroupShortCode { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }

        //-------------FK------------------------

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }




    }
}
