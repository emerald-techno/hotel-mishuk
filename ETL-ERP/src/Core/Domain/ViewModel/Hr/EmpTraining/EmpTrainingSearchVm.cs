using Domain.ModelInterface;

namespace Domain.ViewModel.Hr.EmpTraining
{
    public class EmpTrainingSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string CourseTitle { get; set; }
        public string InstituteName { get; set; }
        public string Location { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Result { get; set; }
        public string TrainingFile { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }
        //---------------------------------------------------------
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
