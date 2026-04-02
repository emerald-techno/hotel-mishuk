using Domain.Entities.HR;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Domain.ViewModel.HotelManagement.RoomInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.HotelManagement.Booking
{
    public class RoomBookingScheduleVm
    {
        public long BookingId { get; set; }
        public long RoomCategoryId { get; set; }
        public string RoomCategoryName { get; set; }
        public long? RoomId { get; set; }
        public string RoomNo { get; set; }
        public long? FloorId { get; set; }
        public string FloorName { get; set; }
        public long? ComplementaryId { get; set; }
        public string ComplementaryName { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string CheckInTimeStr { get; set; }
        public string CheckOutTimeStr { get; set; }
        public string ActualCheckInTimeStr { get; set; }
        public string ActualCheckOutTimeStr { get; set; }
        public double Rent { get; set; }
        public double ServiceCharge { get; set; }
        public double Vat { get; set; }
        public double Tax { get; set; }
        public double Discount { get; set; }
        public double NetRent { get; set; }
        public double? Adult { get; set; } = 0;
        public double? Child { get; set; } = 0;
        public double RoomRent { get; set; } = 0;

        public string BookingNo { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingDateStr { get; set; }
        public short BookingStatus { get; set; }
        public short PaymetnStatus { get; set; }
        public string BookingStatusName { get; set; }
        public string PaymentStatusName { get; set; }
        public decimal PaidAdmount { get; set; } = 0;


        //search options
        public virtual ICollection<HtRoomCategoryVm> RoomCategoryList { get; set; }
        public virtual ICollection<HtRoomInfoVm> RoomList { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }   


    }
}
