namespace Domain.ConfigurationModel;

public class QueryResult<T>
{
    public int TotalItems { get; set; }
    public int TotalEmployement { get; set; }
    public decimal TotalInvestment { get; set; }
    public double TotalLoanAmount { get; set; }
    public IList<T> Items { get; set; }
}