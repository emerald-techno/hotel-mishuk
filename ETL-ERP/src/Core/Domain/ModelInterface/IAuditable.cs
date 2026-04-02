namespace Domain.ModelInterface;

public interface IAuditable
{
    DateTime ActionDate { get; set; }
    long ActionById { get; set; }
    DateTime? UpdateDate { get; set; }
    long? UpdatedById { get; set; }
    bool IsDeleted { get; set; }
}