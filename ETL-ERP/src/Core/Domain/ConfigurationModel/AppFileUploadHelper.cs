using Microsoft.AspNetCore.Http;

namespace Domain.ConfigurationModel;

public class AppFileUploadHelper
{
    public List<IFormFile> FormFileList { get; set; }
    public IFormFile FormFile { get; set; }
    public string FolderPath { get; set; }
    public long? OperationId { get; set; }
    public int? OperationTypeId { get; set; }
    public bool IsFileUploadToRootDirectory { get; set; }

    public bool IsOfficialFile { get; set; } = true;
    //public bool IsMultipleFileUpload { get; set; }
    public int FileCount { get; set; } = 0;
    public string FileNamePrefix { get; set; }
}
