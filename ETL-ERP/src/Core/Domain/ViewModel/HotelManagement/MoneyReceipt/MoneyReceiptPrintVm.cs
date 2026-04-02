namespace Domain.ViewModel.HotelManagement.MoneyReceipt;

public class MoneyReceiptPrintVm
{
    public int Id { get; set; }
    public string TransactionNo { get; set; }
    public string BookingNo { get; set; }
    public string BillNumber { get; set; }
    public DateTime PaidDate { get; set; }
    public string GuestName { get; set; }
    public string RoomNoList { get; set; }
    public string Mobile { get; set; }
    public string GuestAddress { get; set; }
    public double PaidAmount { get; set; }
    public string AmountInWords { get; set; }
    public int PayMode { get; set; }
    public string Description { get; set; }
   
}
