$(document).ready(function () {
    search();
})

$(document.body).on("click", "#BookingSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#BookingSearchTable")) {
        const table = $("#BookingSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#BookingSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "BookingService/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    let bookingDate = convertJsonFullDateForView(new Date(item.bookingDate));
                    const bookingInfo = `<div>
                                    <h5>${item.bookingNo}</h5>
                                    <b>${bookingDate}</b>
                                  </div>`;

                    return bookingInfo;
                },
            },
            {
                "render": function (data, type, item) {
                    const guestInfo = `<div>
                                    <h5>${item.guestName}</h5>
                                    <b>${item.guestMobile}</b>
                                  </div>`;

                    return guestInfo;

                }
            },
            {
                "render": function (data, type, item) {
                    let checkInTime = convertJsonFullDateForView(new Date(item.checkInTime));
                    return checkInTime;

                }
            },
            {
                "render": function (data, type, item) {
                    let checkOutTime = convertJsonFullDateForView(new Date(item.checkOutTime));
                    return checkOutTime;

                }
            },
            {
                "render": function (data, type, item) {

                    let bookingStatus = "";
                    let confirmStatus = item.bookingConfirmStatus == 1 ? `<i class='icofont icofont-checked' style='color: darkcyan;font-size: 18px;'></i>` : '';
                     
                    if (item.bookingStatus == 1) {
                        bookingStatus = `<div style='font-size: 12px' class='badge badge-primary'>Booked</div> ${confirmStatus}`;
                    } else if (item.bookingStatus == 2) {
                        bookingStatus = `<div style='font-size: 12px' class='badge badge-secondary'>Check In</div>`;
                    } else if (item.bookingStatus == 3) {
                        bookingStatus = "<div style='font-size: 12px' class='badge badge-info'>Check Out</div>";
                    } else if (item.bookingStatus == 4) {
                        bookingStatus = "<div style='font-size: 12px' class='badge badge-dark'>No Show</div>";
                    } else if (item.bookingStatus == 9) {
                        bookingStatus = "<div style='font-size: 12px' class='badge badge-danger'>Canceled</div>";
                    }

                    let div = `<div style='text-align: center;'>${bookingStatus}</div>`;

                    return div;
                }
            },
            { "data": "netRent" },
            {
                "render": function (data, type, item) {

                    let paymentStatus = "";

                    if (item.paymentStatus == 0) {
                        paymentStatus = `<div style='font-size: 12px' class='badge badge-info'>Pending</div>`;
                    } else if (item.paymentStatus == 1) {
                        paymentStatus = `<div style='font-size: 12px' class='badge badge-warning'>Partial Payment</div>`;
                    } else if (item.paymentStatus == 2) {
                        paymentStatus = "<div style='font-size: 12px' class='badge badge-success'>Full Paid</div>";
                    }

                    let div = `<div>${paymentStatus}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {
                    /*let bookingDate = convertJsonFullDateForView(new Date(item.bookingDate));*/

                    let bookingInfo = `<div>
                                    <h5>${item.onlineBookingNumber != "" && item.onlineBookingNumber != null ? item.onlineBookingNumber : "Not online booking"}</h5>
                                    <b>${item.onlineGuestMobile != "" && item.onlineGuestMobile != null ? item.onlineGuestMobile : ""}</b>
                                  </div>`;


                    let roomList = `<div><b>${item.roomList}</b></div>`;

                    return roomList;
                },
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("BookingSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let resCardPrintBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/ReservationCardPrint/?bookingId=${item.id}' target = '_blank' title='CardPrint'><i class="fa fa-solid fa-print"></i></a>`;

                    let checkInBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/CheckIn/${item.id}' title='Check In'><i class="fa fa-share"></i></a>`;
                    let checkOutBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/CheckOut/${item.id}' title='Check Out'><i class="fa fa-reply"></i></a>`;

                    /*let checkBtn = item.bookingStatus == 1 ? checkInBtn : checkOutBtn;*/

                    let checkBtn = ``;
                    let cancelBtn = ``;
                    let ReviceCancelBtn = ``;
                    let bookingEditBtn = ``;
                    let checkInEditBtn = ``;

                    var cancelDate = moment(item.cancelDate?.split("T")[0],'YYYY-MM-DD');
                    var today = moment();
                    var isInThreeMonthDuration = moment(cancelDate).add(90, 'days').isAfter(today);

                    var reviceBtn = !isInThreeMonthDuration
                        ? ``
                        : `<a href='../../BookingService/ReviceCancelBooking/${item.id}' target='_blank' style='color:#27ae60;text-decoration:none;font-size:17px;'  title='Revice Cancel'><i class="fa fa-undo" style="margin-right:5px;color:#27ae60;"></i></a>`;

                    console.log('data',item)
                    if (item.bookingStatus == 1) {
                        checkBtn = checkInBtn;
                        bookingEditBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/UpdateBooking/${item.id}' title='Update Booking'><i class="fa fa-edit"></i></a>`;
                        cancelBtn = `<a class='cancelModalBtn' data-id='${item.id}' style='margin-right: 3px; font-size:18px;' href='#' title='Cancel Booking' data-bs-toggle="modal" data-bs-target="#cancelModal"><i class="fa fa-ban text-danger" ></i></a>`;
                    } else if (item.bookingStatus == 2) {
                        checkInEditBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/UpdateCheckIn/${item.id}' title='Update Check In'><i class="fa fa-edit"></i></a>`;
                        checkBtn = checkOutBtn;
                    } else if (item.bookingStatus == 3) {
                        checkBtn = "";
                    } else if (item.bookingStatus == 4) {
                        checkBtn = "";
                    } else if (item.bookingStatus == 9) {
                        checkBtn = "";
                        ReviceCancelBtn = reviceBtn
                    }

                    return `<div>${viewBtn}${bookingEditBtn}${checkInEditBtn}${checkBtn}${cancelBtn}${resCardPrintBtn}${ReviceCancelBtn}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("BookingSearchTable");
}


function getSearchObject() {
    const paymentStatus = $("#PaymentStatus").val();
    let onlyUnPaid = true;

    if (paymentStatus == PaymentStatus().FullPayment) {
        onlyUnPaid = false;
    } else if (paymentStatus == PaymentStatus().All) {
        onlyUnPaid = false;
    }

    const model = {
        BookingNo: $("#BookingNo").val(),
        GuestName: $("#GuestName").val(),
        GuestMobile: $("#GuestMobile").val(),
        BookingStatus: $("#BookingStatus").val(),
        PaymentStatus: $("#PaymentStatus").val(),
        OnlineBookingId: $("#OnlineBookingId").val(),
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
        OnlyUnPaid: onlyUnPaid
    };

    if (model.BookingNo != "" && model.BookingNo != null && model.BookingNo != undefined) {
        model.OnlyUnPaid = false;
    }

    if (model.GuestName != "" && model.GuestName != null && model.GuestName != undefined) {
        model.OnlyUnPaid = false;
    }

    if (model.GuestMobile != "" && model.GuestMobile != null && model.GuestMobile != undefined) {
        model.OnlyUnPaid = false;
    }

    return model;
}

function PaymentStatus() {
    const model = {
        Pending: 0,
        PartialPayment: 1,
        FullPayment: 2,
        All: 10
    }
    return model;
}

//$(document.body).on("click", ".cancel_confirm", function () {
//    swal({
//        title: "Cancel Confirmation",
//        text: "Are you sure to cancel this booking?",
//        icon: "warning",
//        buttons: true,
//        dangerMode: true,
//    })
//        .then((result) => {
//            if (result) {
//                var id = $(this).attr("data-id");
//                const url = `${API}BookingService/Cancel/${id}`;

//                $.get(url, function (rData) {
//                    if (rData) {
//                        successMsg("Cancel Successfully");
//                    } else {
//                        failedMsg("Cancel Failed...!")
//                    }
//                    search();
//                })
//            }
//        })
//})

//#region CancelBooking
$(document.body).on("click", "#CancelBookingBtn", function () {
    swal({
        title: "Cancel Confirmation",
        text: "Are you sure to cancel this booking?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const bookingId = $(".cancelModalBtn").data("id");
                var cancelReason = $("#CancelReason").val();
                const url = `${API}BookingService/Cancel?id=${bookingId}&cancelReason=${cancelReason}`;

                $.get(url, function (rData) {
                    if (rData) {
                        successMsg("Cancel Successfully");
                    } else {
                        failedMsg("Cancel Failed...!")
                    }

                    setTimeout(() => {
                        window.location.href = API + "BookingService/Details/" + bookingId;
                    }, 500);
                })
            }
        })
});

//#endregion