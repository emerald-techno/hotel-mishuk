using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;

namespace Domain.ViewModel.Hr.EmpDisciplinary
{
    public class EmpDisciplinaryVm
    {
        public long Id { get; set; }
        public short ActionType { get; set; } // 1=Sow Cause, 2 = Warning, 3= Suspend , 4= Others
        public string ActionTypeText => ActionType switch { 1 => "Sow Cause", 2 => "Warning", 3 => "Suspend", 4 => "Others", _ => "--" };
        public DateTime DisciplineDate { get; set; }
        public string ActionReason { get; set; }
        public string ActionDesc { get; set; }
        public string Committee { get; set; }
        public string ActionFileUrl { get; set; }
        public short SlNo { get; set; } = 1;
        public string Remarks { get; set; }
        public long EmployeeId { get; set; }
        public bool IsAjaxPost { get; set; }

        #region File Upload

        public IFormFile ActionFile { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\EmpDisciplinary";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = ActionFile?.IsNullOrEmpty() == true ? null : ActionFile,
                FolderPath = FolderPath,
                FileNamePrefix = "ACTION"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (ActionFile != null) ActionFileUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion
    }
}
