using Domain.ConfigurationModel;

namespace Domain.ModelInterface;

public interface IFile
{
    long Id { get; set; }
    int FileTypeId { get; set; }
    string FileSource { get; set; }
    string FileName { get; set; }
    byte[] File { get; set; }
    FileType FileType { get; set; }
}