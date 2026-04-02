namespace Domain.ModelInterface;

public interface IAppFile
{
    long Id { get; set; }
    string FileName { get; set; }
    byte[] File { get; set; }
    int? FileTypeId { get; set; }
    string FileUrl { get; set; }
    long OperationId { get; set; }
    int OperationTypeId { get; set; }
}