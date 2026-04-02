let roomList = [];


$(document).ready(function () {
    loadRoomData();
    loadBookingGuestData();

    /*$('.ckeckinbtn').hide();*/
});

$(document.body).on("change", "#date-time-picker-check-to", function () {
    const actualCheckOutTime = $("#date-time-picker-check-to").datetimepicker('getValue');

    $(".date-time-picker").datetimepicker({
        value: actualCheckOutTime,
        startDate: new Date(actualCheckOutTime)
    });
})

$(".dd-type").select2({ width: "100%" }).on("change", function (e) {
    $(this).valid();
});
$('#paymentEntryModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#paymentEntryModal')
    });
});
$('#serviceAddModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#serviceAddModal')
    });
});

//#region Room Table Section

function loadRoomData() {
    const bookingId = $("#bookingId").val();

    if (bookingId > 0) {

        const url = `${API}BookingService/GetBookedRoomByBookingId/${bookingId}`;

        $.get(url, function (rData) {
            if (rData) {
                console.log("room list: ", rData);

                if (rData.length > 0) {
                    roomList = [];

                    rData.forEach(v => {
                        const checkInDate = convertJsonFullDate(new Date(v.checkInTime));
                        let checkOutDate = convertJsonFullDate(new Date(v.checkOutTime));

                        const actualCheckInDate = v.actualCheckInTime != null ? convertJsonFullDate(new Date(v.actualCheckInTime)) : null;
                        const actualCheckOutDate = v.actualCheckOutTime != null ? convertJsonFullDate(new Date(v.actualCheckOutTime)) : null;

                        if (v.actualCheckOutTime != null) {
                            checkOutDate = convertJsonFullDate(new Date(v.actualCheckOutTime));
                        }

                        let days = dateDifference(v.checkInTime, v.checkOutTime);

                        if (v.actualCheckOutTime != null) {
                            days = dateDifference(v.checkInTime, v.actualCheckOutTime);
                        }

                        /*days = days == 0 ? 1 : days;*/

                        if (v.bookingDayStatus == 1) {
                            days = (days + 0.5);
                        } else if (v.bookingDayStatus == 2) {
                            days = (days + 1);
                        }

                        console.log("Total-Days", days);

                        const model = {
                            id: v.id,
                            categoryId: v.roomCategoryId,
                            categoryName: v.roomCategoryName,
                            roomId: v.roomId,
                            roomName: v.roomNo,
                            complementaryId: v.complementaryId,
                            complementaryName: v.complementaryName,
                            rent: v.rent,
                            serviceCharge: v.serviceCharge,
                            discount: v.discount,
                            extraBed: v.extraBed,
                            extraBedCharge: v.extraBedCharge,
                            totalRent: v.netRent,
                            checkInTime: checkInDate,
                            checkOutTime: checkOutDate,
                            adult: v.adult,
                            child: v.child,
                            rawCheckInTime: new Date(v.checkInTime),
                            rawCheckOutTime: new Date(v.checkOutTime),
                            days: days,
                            roomRent: v.roomRent,
                            roomServiceCharge: v.roomServiceCharge,
                            isHalfDay: v.isHalfDay,
                            actualCheckInTime: actualCheckInDate,
                            actualCheckOutTime: actualCheckOutDate,
                            bookingDayStatus: v.bookingDayStatus,
                            cleaningStatus: v.cleaningStatus,
                            cleaningStatusText: v.cleaningStatusText,
                            roomDiscount: v.roomDiscount
                        };

                        roomList.push(model);
                    });

                    renderRoomTableBody();
                }

            } else {
                console.log("No Room Found..!!");
            }
        })
    }
}

function renderRoomTableBody() {

    $("#RoomTableTbody").empty();

    if (roomList.length > 0) {

        roomList.forEach((v, i) => {

            console.log("room-info", v);

            let actualCheckInDate = v.actualCheckInTime != null ? v.actualCheckInTime : 'N/A';
            let actualCheckOutDate = v.actualCheckOutTime != null ? v.actualCheckOutTime : 'N/A';

            let cleaningStatus = v.cleaningStatus != null ? v.cleaningStatus : 'N/A';
            let cleaningStatusText = v.cleaningStatusText != null ? v.cleaningStatusText : 'N/A';
            let statusBadge = cleaningStatus == 0 ? "badge badge-success" : cleaningStatus == 1 ? "badge badge-secondary" : cleaningStatus == 2 ? "badge badge-info" : "";

            let complementaryHtml = '';
            let complementaryRemoveBtn = v.actualCheckOutTime == null ? `<a type="button" class="text-danger remove-complementary"  data-bookingRoomId="${v.id}"><i class="fa fa-close"></i></a>` : '';

            v.complementaryId > 0
                ? complementaryHtml = `<span class="badge-success px-1 py-1 text-white" style="border-radius:12px;font-size:9px;">${v.complementaryName}</span >
                                        ${complementaryRemoveBtn}`
                : complementaryHtml = `<a class="btn btn-primary complementaryModalOpenBtn" href="#" data-bs-toggle="modal" data-bookingRoomId="${v.id}" data-bs-target="#complementaryEntryModal" style="font-size:10px;padding:5px">Add Complementary</a>`;


            const slNoCell = `<td>${i + 1}
                                 <input type='hidden' name='BookingRoomVms[${i}].Id' value='${v.id}'/>   
                              </td>`;

            let statusBadgeHtml = `<span class="${statusBadge} px-1 py-1" style="border-radius:12px;font-size:9px;">${cleaningStatusText}</span >`;
            let showCleaningStatusHtml = v.actualCheckOutTime != null ? '<br/>' : statusBadgeHtml;

            const roomInfoCell = `<td> 
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomCategoryId' value='${v.categoryId}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomId' value='${v.roomId}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].Adult' value='${v.adult}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].Child' value='${v.child}'/>
                                    <div>
                                        <strong>${v.roomName}</strong>
                                        ${showCleaningStatusHtml}
                                        <p class='m-0'>${v.categoryName}</p><br/>
                                        <small>Extra Bed: ${v.extraBed}</small><br/>
                                        <small>Adult: ${v.adult}</small>
                                        <small>Child: ${v.child}</small>
                                    </div>
                                  </td>`;

            const checkInCell = `<td> 
                                    <b>Booked Check In</b> <br/>
                                    <span>${v.checkInTime}</span> <br/>
                                    <b>Actual Check In</b> <br/>
                                    <span>${actualCheckInDate}</span>
                                </td>`;

            const checkOutCell = `<td> 
                                    <b>Booked Check Out</b> <br/>
                                    <span>${v.checkOutTime}</span> <br/>
                                    <b>Actual Check Out</b> <br/>
                                    <span>${actualCheckOutDate}</span>
                                </td>`;

            let bookingDayStatusText = v.bookingDayStatus == 1 ? `<b>Half Day</b>` : v.bookingDayStatus == 2 ? `<b>Full Day/Day Use</b>` : ``;

            //let customDays = v.bookingDayStatus == 1 ? (parseFloat(v.days) + 0.5) : v.bookingDayStatus == 2 ? (parseFloat(v.days) + 1) : parseFloat(v.days);

            const noOfDaysCell = `<td>
                                    <input class='form-control' type='hidden' id='days_${i}' value='${v.days}' readonly/>
                                    <b style='font-size: larger;'>${v.days} Nights</b>
                                    <br/>
                                    ${complementaryHtml}
                                  </td>`;

            const hiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].Rent' value='${v.rent}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].ServiceCharge' value='${v.serviceCharge}'/>`;

            const netRentAmount = (parseFloat(v.roomRent) + parseFloat(v.roomServiceCharge)) - (parseFloat(v.roomDiscount));

            const priceCell = `<td>
                                <b>Rent:</b> ${v.roomRent} <br/>
                                <b>Service Charge:</b> ${v.roomServiceCharge} <br/>
                                <b>Discount:</b> ${v.roomDiscount} <br/>
                                <b>Net Rent:</b> ${netRentAmount} <br/>
                                ${hiddenInput}
                              </td>`;

            const totalCell = `<td style='padding:5px;'>
                                    <input class='form-control text-end' name='BookingRoomVms[${i}].NetRent' id='total_rent_${i}' value='${v.totalRent}' readonly/>
                               </td>`;

            const row = `<tr style='font-size: smaller;'>${slNoCell}${roomInfoCell}${checkInCell}${checkOutCell}${noOfDaysCell}${priceCell}${totalCell}</tr>`;

            $("#RoomTableTbody").append(row);

        });

        $(".date-time-picker").datetimepicker();

        const sumResult = calculateSum(roomList.map(x => x.totalRent));


        $("#sub-total").html(sumResult);

        const roomDiscount = calculateSum(roomList.map(x => x.discount));
        const totalDiscount = parseFloat($("#total-discount").val());
        const billDiscount = totalDiscount > roomDiscount ? totalDiscount - roomDiscount : 0;
        $("#bill-discount").html(billDiscount);

        calculateNetTotal();
    }
}

//#region Complmentary Entry
$(document.body).on("click", ".complementaryModalOpenBtn", function () {
    const bookingRoomId = $(this).attr("data-bookingRoomId");
    $("#C_BookingRoomId").val(bookingRoomId);
});
$(document.body).on("click", "#ComplementaryEntryBtn", function () {
    const complementaryId = $("#complementaryId").val();
    const bookingRoomId = $("#C_BookingRoomId").val();

    if (complementaryId > 0 && bookingRoomId > 0) {
        const url = API + "BookingService/ComplementaryEntry";

        $.post(url, { bookingRoomId, complementaryId }, function (rData) {
            if (rData == true) {
                successMsg("Complementary Entry Successful");

                $("#complementaryEntryModal").modal('hide');
                $("#complementaryId").val("").trigger(update);
                $("#C_BookingRoomId").val("");
                setTimeout(() => {
                    window.location.reload();
                }, 2000);
            } else {
                failedMsg("Complementary Entry Failed");
                $("#C_BookingRoomId").val("");
            }
        }).fail(function () {
            failedMsg("Complementary Entry Failed");
        })
    } else {
        failedMsg("Complementary is not selected");
        $("#C_BookingRoomId").val("");
    }
})

//#endregion

//#region RemoveComplementary
$(document.body).on("click", ".remove-complementary", function () {
    const bookingRoomId = $(this).attr("data-bookingRoomId");
    if (bookingRoomId > 0) {
        deleteComplementary(bookingRoomId);
    }
});


function deleteComplementary(bookingRoomId) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to remove this complementary?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url = `${API}BookingService/RemoveComplementary?bookingRoomId=${bookingRoomId}`;
                $.post(url, function (rData) {
                    if (rData == true) {
                        successMsg("Successfully Removed!");

                        setTimeout(() => {
                            window.location.reload()
                        }, 1000);

                    } else {
                        console.log("error")
                        failedMsg("Remove Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}
//#endregion

$(document.body).on("change", ".date-time-picker", function () {
    var index = $(this).attr("data-index");
    console.log("index: ", index);

    if (index > -1) {
        const checkOutTime = $(`#date_time_${index}`).datetimepicker('getValue');
        console.log("test-time:", checkOutTime);

        var room = roomList[index];

        const days = dateDifference(room.rawCheckInTime, checkOutTime);
        const roomRent = room.roomRent;
        const serviceCharge = room.serviceCharge;

        let rentAmount = roomRent * days;
        let totalRent = parseFloat(rentAmount) + parseFloat(serviceCharge);

        roomList[index].days = days;
        roomList[index].totalRent = totalRent;

        $(`#days_${index}`).val(days);
        $(`#total_rent_${index}`).val(totalRent);

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#sub-total").val(sumResult);

        calculateNetTotal();
    }
})

function calculateSum(array) {
    var sum = 0;

    $.each(array, function (index, value) {
        sum += parseFloat(value);
    });

    return sum;
}

//function dateDifference(startDate, endDate) {

//    const fromDate = new Date(startDate);
//    const toDate = new Date(endDate);

//    const timeDifference = toDate - fromDate;

//    const seconds = Math.floor(timeDifference / 1000);
//    const minutes = Math.floor(seconds / 60);
//    const hours = Math.floor(minutes / 60);
//    const days = Math.floor(hours / 24);

//    console.log("Time difference in days for room:", days);

//    return days;
//}


function dateDifference(startDate, endDate) {

    const fromDate = new Date(startDate);
    const toDate = new Date(endDate);

    fromDate.setHours(0, 0, 0, 0);
    toDate.setHours(0, 0, 0, 0);

    const timeDifference = toDate - fromDate;

    // Converting milliseconds to days
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    console.log("Time difference in days for room:", days);

    return days;
}

$(document.body).on("change", "#Vat", function () {
    calculateNetTotal();
});

$(document.body).on("change", "#Tax", function () {
    calculateNetTotal();
});

$(document.body).on("change", "#Discount", function () {
    calculateNetTotal();
});

function calculateNetTotal() {
    const subTotal = parseFloat($("#sub-total").val());
    const paidAmount = parseFloat($("#PaidAmount").val());
    const vat = parseFloat($("#Vat").val());
    const tax = parseFloat($("#Tax").val());
    const discount = parseFloat($("#Discount").val());

    const netTotal = (subTotal + vat + tax) - (paidAmount + discount);
    console.log(vat, tax, paidAmount)
    console.log("Net Total: ", netTotal);
    $("#net-total").val(netTotal);
}

//#endregion

//#region Guest Table Section

function loadBookingGuestData() {
    const bookingId = $("#bookingId").val();

    if (bookingId > 0) {

        const url = `${API}BookingService/GetBookingGuestByBookingId/${bookingId}`;

        $.get(url, function (rData) {
            if (rData) {
                console.log("guest list: ", rData);
                if (rData.length > 0) {
                    guestList = [];

                    rData.forEach(v => {
                        const model = {
                            id: v.id,
                            guestId: v.guestId,
                            guestName: v.guestName,
                            isMain: v.isMain
                        };

                        guestList.push(model);
                    });

                    renderGuestTableBody();
                }
            } else {
                console.log("No Guest Found..!!");
            }
        })
    }
}

function renderGuestTableBody() {

    $("#GuestTableTbody").empty();

    if (guestList.length > 0) {

        guestList.forEach((v, i) => {

            const slNoCell = `<td>${i + 1}</td>`;

            const guestCell = `<td> ${v.guestName}  ${v.isMain ? '(Contact Person)' : ''} </td>`;

            const row = `<tr style='font-size: smaller;'>${slNoCell}${guestCell}</tr>`;

            $("#GuestTableTbody").append(row);
        });
    }
}

$(document.body).on("click", "#PaymentEntryBtn", function () {
    const bookingId = $("#bookingId").val();
    const paymode = $("#PayMode").val();
    const paidAmount = $("#PaidAmount").val();
    const transactionNo = $("#TransactionNo").val();
    const accountNo = $("#AccountNo").val();
    const paidDateStr = $("#PaidDateStr").val();

    if (bookingId > 0 && paidAmount > 0) {
        const url = API + "BookingService/PaymentEntry";

        const params = {
            bookingId: bookingId,
            payMode: paymode,
            paidAmount: paidAmount,
            transactionNo: transactionNo,
            accountNo: accountNo,
            paidDateStr: paidDateStr
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Payment Entry Successful");
                $("#paymentEntryModal").modal('hide');
                clearPaymentForm();

                setTimeout(() => {
                    window.location.href = API + "BookingService/Details/" + bookingId;
                }, 2000);
            } else {
                failedMsg("Payment Entry Failed");
            }
        }).fail(function () {
            failedMsg("Payment Entry Failed");
        })
    } else {
        failedMsg("Booking info and payment amount is required");
    }
})

function clearPaymentForm() {
    $("#PayMode").val("");
    $("#PaidAmount").val("");
    $("#TransactionNo").val("");
    $("#AccountNo").val("");
}

//#endregion

//#region Remove_Payment

$(document.body).on("click", ".payment_delete_btn", function () {
    const paymentId = $(this).attr("data-id");
    if (paymentId > 0) {
        deletePayment(paymentId);
    }
});


function deletePayment(paymentId) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this Payment?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url = `${API}BookingService/DeletePayment?paymentId=${paymentId}`;
                $.get(url, function (rData) {
                    if (rData) {
                        console.log(rData)
                        successMsg("Successfully Deleted!");

                        setTimeout(() => {
                            window.location.reload()
                        }, 1000);

                    } else {
                        console.log("error")
                        failedMsg("Delete Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}

//#endregion

//#region Remove_Booking

$(document.body).on("click", ".booking_delete_btn", function () {
    const bookingId = $(this).attr("data-id");
    if (bookingId > 0) {
        deleteBooking(bookingId);
    }
});


function deleteBooking(id) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this Booking?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url = `${API}BookingService/BookingDelete?bookingId=${id}`;
                $.get(url, function (rData) {
                    if (rData) {
                        console.log(rData)
                        successMsg("Successfully Deleted!");

                        setTimeout(() => {
                            window.location.href = API + "BookingService/Search";
                        }, 1000);

                    } else {
                        console.log("error")
                        failedMsg("Delete Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}

//#endregion

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
                const bookingId = $("#bookingId").val();
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

