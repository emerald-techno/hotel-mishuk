$(document).ready(function () {
    search();
})

$(document.body).on("click", "#OnlineBookingSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#OnlineBookingSearchTable")) {
        const table = $("#OnlineBookingSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#OnlineBookingSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "OnlineBooking/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    let bookingDate = convertJsonFullDateForView(new Date(item.onlineBookingDate));
                    const bookingInfo = `<div>
                                    <h5>${item.onlineBookingNumber}</h5>
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
                    let checkInTime = convertJsonFullDateForView(new Date(item.arrivalDate));
                    return checkInTime;

                }
            },
            {
                "render": function (data, type, item) {
                    let checkOutTime = convertJsonFullDateForView(new Date(item.departureDate));
                    return checkOutTime;

                }
            },
            { "data": "categoryList" },
            {
                "render": function (data, type, item) {

                    let bookingStatus = "";

                    if (item.status == 0) {
                        bookingStatus = `<div style='font-size: 12px' class='badge badge-warning'>Pending</div>`;
                    } else if (item.status == 1) {
                        bookingStatus = `<div style='font-size: 12px' class='badge badge-primary'>Approved</div>`;
                    } else if (item.status == 2) {
                        bookingStatus = "<div style='font-size: 12px' class='badge badge-danger'>Canceled</div>";
                    }

                    let div = `<div>${bookingStatus}</div>`;

                    return div;
                }
            },
            { "data": "totalRent" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("OnlineBookingSearchTable", oTable);

                    let showHtml = "";

                    let approveButton = `<a class='mr-2 approveconfirm' data-id='${item.id}' href='#' title='Approve'><i class="fa fa-check-square-o fa-2x"></i></a>`;
                    let rejectButton = `<a class='ml-2 rejectconfirm' data-id='${item.id}' href='#' title='Reject'><i class="fa fa-times fa-2x"></i></i></a>`;
                    let detailsButton = `<a class='ml-2 details' data-id='${item.id}' href='#' title='Details'><i class="fa fa-eye fa-2x"></i></i></a>`;

                    /*let checkBtn = item.bookingStatus == 1 ? checkInBtn : checkOutBtn;*/

                    if (item.status == 0) {
                        showHtml = approveButton + rejectButton + detailsButton;
                    } else {
                        showHtml = `<div class='text-center'>${detailsButton}</div>`;
                    }


                    return `<div>${showHtml}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("OnlineBookingSearchTable");
}


function getSearchObject() {
    const status = $("#Status").val();
    let onlyPending = true;

    if (status != null && status != undefined && status != "" && status=="All") {
        onlyPending = false;
    } 

    const model = {
        OnlineBookingNumber: $("#OnlineBookingNumber").val(),
        GuestName: $("#GuestName").val(),
        GuestMobile: $("#GuestMobile").val(),
        Status: $("#Status").val(),
        IsPendingOnly: onlyPending
    };

    return model;
}

$(document.body).on("click", ".approveconfirm", function () {
    swal({
        title: "Approve Confirmation",
        text: "Are you sure to approve this booking?",
        icon: "success",
        buttons: true,
        dangerMode: false,
    })
        .then((result) => {
            if (result) {
                var id = $(this).attr("data-id");
                const url = `${API}OnlineBooking/Approve/${id}`;

                $.get(url, function (rData) {
                    if (rData) {
                        successMsg("Approve Successfully");
                    } else {
                        failedMsg("Approve Failed...!")
                    }
                    search();
                })
            }
        })
})


$(document.body).on("click", ".rejectconfirm", function () {
    swal({
        title: "Reject Confirmation",
        text: "Are you sure to reject this booking?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                var id = $(this).attr("data-id");
                const url = `${API}OnlineBooking/Reject/${id}`;

                $.get(url, function (rData) {
                    if (rData) {
                        successMsg("Reject Successfully");
                    } else {
                        failedMsg("Reject Failed...!")
                    }
                    search();
                })
            }
        })
})


$(document.body).on("click", ".details", function () {
    var id = $(this).attr("data-id");
    const url = `${API}OnlineBooking/Details/${id}`;
    window.location.href = url;
    
});