using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ModelInterface;

public interface IConfirmable
{
    bool IsConfirmed { get; set; }
    long? ConfirmedById { get; set; }

    [DataType(DataType.Date)]
    [Column(TypeName = "DateTime2")]
    DateTime? ConfirmedOn { get; set; }

    bool IsRejected { get; set; }
    long? RejectedById { get; set; }

    [DataType(DataType.Date)]
    [Column(TypeName = "DateTime2")]
    DateTime? RejectedOn { get; set; }
    string RejectReason { get; set; }

    //AppIdentityUser ConfirmedBy { get; set; }
    //AppIdentityUser RejectedBy { get; set; }
}