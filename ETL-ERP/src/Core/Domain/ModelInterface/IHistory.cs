namespace Domain.ModelInterface;

public interface IHistory
{
    long Id { get; set; }
    int HistoryStatusId { get; set; }
    long EntryBy { get; set; }
    DateTime EntryOn { get; set; }

}
