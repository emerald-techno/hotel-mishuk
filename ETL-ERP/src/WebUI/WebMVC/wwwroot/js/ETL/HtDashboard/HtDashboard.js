$(document).ready(function () {
    getRoomData();
    getPendingBooking();

    // Set an interval to call the function every 10 seconds (10000 ms)
    setInterval(getPendingBooking, 600000); // run after 10 min interval
});

$(document.body).on("click", "#SearchBtn", function () {
    getRoomData();
});

function getRoomData() {
    const queryDate = $("#StrQueryDate").val();

    if (queryDate != null && queryDate != "") {
        const url = `${API}HtDashboard/GetRoomStatusByDate?selectDate=${queryDate}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("room-list: ", rData);
                renderStatusCount(rData);
                renderRoomBox(rData);
            } else {
                console.log("No Room list Found...!");
            }
        })
    }
}
function getRoomDataByStatus(status, cleanStatus) {
    const queryDate = $("#StrQueryDate").val();

    if (queryDate != null && queryDate != "") {
        const url = `${API}HtDashboard/GetRoomStatusByDate?selectDate=${queryDate}&roomStatus=${status}&cleanStatus=${cleanStatus}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("room-list: ", rData);
                renderRoomBox(rData);
            } else {
                console.log("No Room list Found...!");
            }
        })
    }
}

function renderStatusCount(rData) {
    if (rData != null && rData.length > 0) {
        const bookedList = rData.filter(x => x.status == 1);
        const bookedCount = bookedList.length;
        $("#booked-count").html(`(${bookedCount})`);

        const occupiedList = rData.filter(x => x.status == 2);
        const occupiedCount = occupiedList.length;
        $("#occupied-count").html(`(${occupiedCount})`);

        const availableList = rData.filter(x => x.status == 3);
        const availableCount = availableList.length;
        $("#available-count").html(`(${availableCount})`);

        const oooList = rData.filter(x => x.status == 4);
        const oooVdList = rData.filter(x => x.status == 1 && x.cleaningStatus == 4);
        const oooCount = oooList.length + parseInt(oooVdList.length);
        $("#ooo-count").html(`(${oooCount})`);

        const vdList = rData.filter(x => x.status == 5);
        const bookedVdList = rData.filter(x => x.status == 1 && x.cleaningStatus == 1);
        const vdCount = vdList.length + parseInt(bookedVdList.length);
        $("#vd-count").html(`(${vdCount})`);
    }
}

const cleaningStatusRenderSection = {
    DashBoard: 1,
    Modal: 2
};
function renderCleaningStatus(v, section) {

    if (section == cleaningStatusRenderSection.DashBoard && v.status != 1) {
        return { roomClass: "", statusClass: "", label: "" };
    }
    //if (section == cleaningStatusRenderSection.Modal && v.status == 2) {
    //    return { roomClass: "", statusClass: "", label: "" };
    //}

    switch (v.cleaningStatus) {
        case 0: return { roomClass: "booked-room", statusClass: "status-clean", label: "V & C" };
        case 1: return { roomClass: "booked-room", statusClass: "status-dirty", label: "V & D" };
        case 2: return { roomClass: "booked-room", statusClass: "status-o", label: "O" };
        case 4: return { roomClass: "booked-room", statusClass: "status-ooo", label: "O.O.O" };
        default: return { roomClass: "booked-room", statusClass: "status-unknown", label: "Unknown" };
    };

}

function renderRoomBox(rData) {
    $("#room-section").empty();

    if (rData != null && rData.length > 0) {
        rData.forEach((v, i) => {

            let btnColor = v.status == 1 ? "btn-info" : v.status == 2 ? "btn-secondary" : v.status == 3 ? "btn-primary"
                : v.status == 4 ? "btn-danger" : v.status == 5 ? "btn-warning" : "btn-primary";

            let todayCheckoutClass = '';

            if (v.isTodayCheckout) {
                todayCheckoutClass = "today-checkout";
            } else {
                todayCheckoutClass = '';
            }

            var cleaningData = renderCleaningStatus(v, cleaningStatusRenderSection.DashBoard)

            let html = `<button class="btn room_btn ${btnColor} ${todayCheckoutClass} ${cleaningData.roomClass} ${cleaningData.statusClass}"
                                data-label="${cleaningData.label}"
                                data-room-id='${v.roomId}' 
                                style="max-width: 120px" 
                                data-bs-toggle="modal" 
                                data-bs-target="#roomModalCenter">
                            <span>Room: ${v.roomNo}</span>
                        </button>`;

            $("#room-section").append(html);
        })
        $(".room-status").html(`Room Status (${rData.length})`);
    } else {
        $("#room-section").html(`<h4 class='text-center'>No Room Found</h4>`);
        $(".room-status").html(`Room Status (0)`);
    }
}

$(document.body).on("click", ".room_btn", function () {
    const roomId = $(this).attr("data-room-id");
    const queryDate = $("#StrQueryDate").val();

    console.log(`${roomId}-${queryDate}`);

    if (roomId > 0 && queryDate != "") {
        const url = `${API}RoomInfo/GetDayWiseRoomInfo?roomId=${roomId}&queryDateStr=${queryDate}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("room-info: ", rData);
                renderRoomInfo(rData);
            } else {
                console.log("No Room Info Found...!");
            }
        })
    }
});

function renderRoomInfo(rData) {
    let html = "";

    if (rData != null && rData != undefined) {

        console.log("room_info:", rData);
        let btnColor = rData.status == 1 ? "btn-info" : rData.status == 2 ? "btn-secondary" : rData.status == 3 ? "btn-primary"
            : rData.status == 4 ? "btn-danger" : rData.status == 5 ? "btn-warning" : "btn-primary";
        $("#rm_info_header").removeClass();
        $("#rm_info_header").addClass("modal-header " + btnColor);

        let acText = rData.isAc ? `<h6 class='f-w-600 txt-primary'>Yes</h6>` : `<h6 class='f-w-600 txt-secondary'>No</h6>`;
        let belconyText = rData.isBelcony ? `<h6 class='f-w-600 txt-primary'>Yes</h6>` : `<h6 class='f-w-600 txt-secondary'>No</h6>`;

        let bookedText = rData.bookingStatus == 1 ? `<h5 class='f-w-600 txt-primary'>Room ${rData.bookingStatusText}</h5>`
            : rData.bookingStatus == 2 ? `<h5 class='f-w-600 txt-info'>Room ${rData.bookingStatusText}</h5>`
                : rData.bookingStatus == 3 ? `<h5 class='f-w-600 txt-secondary'>Room ${rData.bookingStatusText}</h5>` : "";
        let checkoutLink = (rData.bookingStatus == 3) ? `<a class='da-checkout' href='BookingService/CheckOut/${rData.bookingId}'><h5 class='f-w-600 txt-secondary'>Check Out</h5></a>` : ``;
        checkoutLink = (rData.bookingStatus == 2) ? `<a class='da-checkout' href='BookingService/CheckIn/${rData.bookingId}'><h5 class='f-w-600 txt-info'>Check In</h5></a>` : checkoutLink;

        let detailLink = `<a class='bs-detail' href='BookingService/Details/${rData.bookingId}'><i class='fa fa-eye' style='font-size: 22px;' aria-hidden='true'></i></a>`;

        let guestNameText = rData.guestName != null ? `${rData.guestName}` : "N/A";
        let guestMobileText = rData.guestMobile != null ? `${rData.guestMobile}` : "N/A";

        let checkInTime = rData.checkInDate != null ? convertJsonFullDateForView(new Date(rData.checkInDate)) : "";
        let checkOutTime = rData.checkOutDate != null ? convertJsonFullDateForView(new Date(rData.checkOutDate)) : "";

        var cleaningData = renderCleaningStatus(rData, cleaningStatusRenderSection.Modal);
        let cleaningBadge = cleaningData.label
            ? `<span class="badge ${cleaningData.statusClass} ${cleaningData.roomClass} cleaningStatusModal" data-label="${cleaningData.label}" style="transform: rotate(10deg); top: 10px">
                    ${cleaningData.label}
               </span>`
            : "";
        //let btnColor = rData.status == 1 ? "btn-info" : rData.status == 2 ? "btn-secondary" : rData.status == 3 ? "btn-primary" :
        //    rData.status == 4 ? "btn-danger" : rData.status == 5 ? "btn-warning" : "btn-primary";
        //console.log(rData);
        //$('.room-title').addClass(btnColor);
        //$('.room-title').html(rData.status);

        var oooRemarksHtml = rData.oooRemarks?.length > 0 ? `<div class='col-md-3 mb-2'><b>OOO Remarks</b></div>
                                                            <div class='col-md-9 mb-2'>${rData.oooRemarks}</div>`
                                                           : `<div class="col-md-3 mb-2"></div>
                                                            <div class="col-md-2 mb-2"></div>`;

        var guestHtml = rData.guestName ? `
                    <div class='col-md-7 mb-3'>${bookedText}</div>
                    <div class='col-md-1 mb-3'>${detailLink}</div>
                    <div class='col-md-4 mb-3'>${checkoutLink}</div>
                    <div class="col-md-2 mb-2"><b>Guest Name</b></div>
                    <div class="col-md-5 mb-2">${guestNameText}</div>
                    <div class="col-md-3 mb-2"></div>
                    <div class="col-md-2 mb-2"></div>

                    <div class="col-md-2 mb-2"><b>Guest Mobile</b></div>
                    <div class="col-md-5 mb-2">${guestMobileText}</div>
                    <div class="col-md-3 mb-2"></div>
                    <div class="col-md-2 mb-2"></div>

                    <div class="col-md-2 mb-2"><b>Check In</b></div>
                    <div class="col-md-4 mb-2">${checkInTime}</div>
                    <div class="col-md-2 mb-2">Check Out</div>
                    <div class="col-md-4 mb-2">${checkOutTime}</div>` : "";

        html = `<div class="row">
                    <div class="col-md-2 mb-2"><b>Room No</b></div>
                    <div class="col-md-5 mb-2">${rData.roomNo}</div>
                    <div class="col-md-3 mb-2"><b>Rent</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.rent}</div>
                    <div class="col-md-2 mb-2"><b>Category</b></div>
                    <div class="col-md-5 mb-2">${rData.roomCategoryName}</div>
                    <div class="col-md-3 mb-2"><b>Service Charge</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.serviceCharge}</div>

                    <div class="col-md-2 mb-2"><b>Floor</b></div>
                    <div class="col-md-5 mb-2">${rData.floorName}</div>
                    <div class="col-md-3 mb-2"><b>VAT</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.vat}</div>

                    <div class="col-md-2 mb-2"><b>AC</b></div>
                    <div class="col-md-5 mb-2">${acText}</div>
                    <div class="col-md-3 mb-2"><b>Net Rent</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.totalRent}</div>

                    <div class="col-md-2 mb-2"><b>Belcony</b></div>
                    <div class="col-md-5 mb-2">${belconyText}</div>
                    <div class="col-md-3 mb-2"><b>Cleaning Status</b></div>
                    <div class="col-md-2 mb-2 text-end">${cleaningBadge}</div>

                    <div class="col-md-7 mb-2"><b>${rData.numberOfBed} Bed</b></div>
                    <div class="col-md-3 mb-2"></div>
                    <div class="col-md-2 mb-2"></div>

                    <div class="col-md-7 mb-2"><b>${rData.person} Person</b></div>
                    <div class="col-md-3 mb-2"></div>
                    <div class="col-md-2 mb-2"></div>

                    ${oooRemarksHtml}

                    <hr class=col-md-12/>
                    ${guestHtml}
                </div>`;
    }


    $("#room_info_sec").html(html);
}

function getPendingBooking() {
    $("#online-book-sec").empty();

    const url = `${API}Home/GetPendingOnlineBooking`;

    $.get(url, function (rData) {
        if (rData) {
            console.log("online pending booking list: ", rData);
            renderPendingBookingSection(rData);
        } else {
            console.log("No Pending Booking Found..!!");
        }
    })
}

function renderPendingBookingSection(data) {
    if (data.length > 0) {

        let headHtml = `<div class="alert alert-success" role="alert">
                            <div>
                                <span><i class="fa fa-info-circle"></i></span>
                                You Have <a href='${API}OnlineBooking/Search' style='color:lime;'><strong>( ${data.length} ) Pending</strong> </a> Online Booking To Attend..!!
                            </div>
                        </div>`;

        $("#online-book-sec").html(headHtml);
    } else {
        $("#online-book-sec").empty();
    }
}