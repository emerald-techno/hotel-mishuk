namespace Domain.ModelInterface;

public interface IOperation
{
    long Id { get; set; }
    int OperationTypeId { get; set; }
    long OperationModelId { get; set; }
}