namespace Domain.ModelInterface;

public interface IDataTableSearch
{
    long Id { get; set; }
    long UserId { get; set; }
    int SerialNo { get; set; }
    bool CanCreate { get; set; }
    bool CanUpdate { get; set; }
    bool CanView { get; set; }
    bool CanDelete { get; set; }

}