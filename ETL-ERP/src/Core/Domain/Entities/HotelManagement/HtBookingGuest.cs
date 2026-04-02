using Domain.Entities.Identity;

namespace Domain.Entities.HotelManagement;

public class HtBookingGuest
{
    public long Id { get; set; }
    public bool IsMain { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ActionDate { get; set; }

    // --- FK ---

    public long BookingId { get; set; }
    public HtBookingService Booking { get; set; }
    public long GuestId { get; set; }
    public HtGuestInfo Guest { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
