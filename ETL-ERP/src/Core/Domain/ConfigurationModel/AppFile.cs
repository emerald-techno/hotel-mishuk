using Domain.ModelInterface;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.ConfigurationModel;

public class AppFile : IAppFile
{
    public long Id { get; set; }
    public string FileName { get; set; }
    public byte[] File { get; set; }
    public int? FileTypeId { get; set; }
    public string FileUrl { get; set; }
    public long OperationId { get; set; }
    public int OperationTypeId { get; set; }
    public FileType FileType { get; set; }

    public IFormFile FormFile { get; set; }
    [NotMapped]
    public IFormFile PostedFileBase { get; set; }
}
