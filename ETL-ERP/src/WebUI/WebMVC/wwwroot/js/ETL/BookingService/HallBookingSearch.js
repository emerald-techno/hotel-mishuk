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
            url: API + "BookingService/HallSearch",
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

                    if (item.bookingStatus == 1) {
                        bookingStatus = `<div style='font-size: 12px' class='badge badge-primary'>Booked</div>`;
                    } else if (item.bookingStatus == 2) {
                        bookingStatus = `<div style='font-size: 12px' class='badge badge-secondary'>Check In</div>`;
                    } else if (item.bookingStatus == 3) {
                        bookingStatus = "<div style='font-size: 12px' class='badge badge-info'>Check Out</div>";
                    } else if (item.bookingStatus == 4) {
                        bookingStatus = "<div style='font-size: 12px' class='badge badge-dark'>No Show</div>";
                    } else if (item.bookingStatus == 9) {
                        bookingStatus = "<div style='font-size: 12px' class='badge badge-danger'>Canceled</div>";
                    }

                    let div = `<div>${bookingStatus}</div>`;

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

                    showTotalRowCountSpanInDataTable("BookingSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/HallDetails/${item.id}' title='View'><i class="fa fa-search"></i></a>`;

                    let checkBtn = ``;
                    let cancelBtn = ``;
                    let bookingEditBtn = ``;
                    let checkInEditBtn = ``;

                    if (item.bookingStatus == 1) {
                        bookingEditBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}BookingService/UpdateHallBooking/${item.id}' title='Update Hall Booking'><i class="fa fa-edit"></i></a>`;
                        /*cancelBtn = `<a class='cancel_confirm' style='margin-right: 3px; font-size:18px;' data-id='${item.id}' href='#' title='Cancel Booking'><i class="fa fa-ban text-danger"></i></a>`;*/
                    }


                    return `<div>${viewBtn}${bookingEditBtn}</div>`;
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
        GusetName: $("#GusetName").val(),
        BookingStatus: $("#BookingStatus").val(),
        PaymentStatus: $("#PaymentStatus").val(),
        OnlineBookingId: $("#OnlineBookingId").val(),
        OnlyUnPaid: onlyUnPaid
    };

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

$(document.body).on("click", ".cancel_confirm", function () {
    swal({
        title: "Cancel Confirmation",
        text: "Are you sure to cancel this booking?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                var id = $(this).attr("data-id");
                const url = `${API}BookingService/Cancel/${id}`;

                $.get(url, function (rData) {
                    if (rData) {
                        successMsg("Cancel Successfully");
                    } else {
                        failedMsg("Cancel Failed...!")
                    }
                    search();
                })
            }
        })
})