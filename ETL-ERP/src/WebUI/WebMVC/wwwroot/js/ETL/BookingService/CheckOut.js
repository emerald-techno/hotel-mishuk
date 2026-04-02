let roomList = [];

const extraBedCharge = 500;

$(document).ready(function () {
    loadRoomData();
    loadBookingGuestData();
});

$(document.body).on("change", "#date-time-picker-check-to", function () {
    const actualCheckOutTime = $("#date-time-picker-check-to").datetimepicker('getValue');

    if (actualCheckOutTime == null) {
        return;
    }

    $(".date-time-picker").datetimepicker({
        value: actualCheckOutTime,
        startDate: new Date(actualCheckOutTime)
    });

    globalChangeDate();
})

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
                        const checkOutDate = convertJsonFullDate(new Date(v.checkOutTime));

                        const actualCheckInDate = convertJsonFullDate(new Date(v.actualCheckInTime));
                        const actualCheckOutDate = convertJsonFullDate(new Date(v.actualCheckOutTime));

                        let days = dateDifference(v.checkInTime, v.checkOutTime);
                        console.log("Total-Days", days);

                        if (v.bookingDayStatus > 0) {
                            if (v.bookingDayStatus == 1) {
                                days = days + 0.5;
                            } else if (v.bookingDayStatus == 2) {
                                days = days + 1;
                            }
                        }

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
                            totalRent: v.netRent,
                            checkInTime: checkInDate,
                            checkOutTime: checkOutDate,
                            adult: v.adult,
                            child: v.child,
                            rawCheckInTime: new Date(v.checkInTime),
                            rawCheckOutTime: new Date(v.checkOutTime),
                            days: days,
                            roomRent: v.roomRent,
                            roomDiscount: v.roomDiscount,
                            extraBed: v.extraBed,
                            extraBedCharge: v.extraBedCharge,
                            extraBedAsServiceCharge: v.extraBedAsServiceCharge,
                            actualCheckInTime: actualCheckInDate,
                            actualCheckOutTime: actualCheckOutDate,
                            rawActualCheckInTime: v.actualCheckInTime,
                            rawActualCheckOutTime: v.actualCheckOutTime,
                            bookingDayStatus: v.bookingDayStatus
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
            console.log('extra', roomList)
            const slNoCell = `<td>${i + 1}
                                 <input type='hidden' name='BookingRoomVms[${i}].Id' value='${v.id}'/>   
                              </td>`;

            const roomInfoCell = `<td> 
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomCategoryId' value='${v.categoryId}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomId' value='${v.roomId}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].Adult' value='${v.adult}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].Child' value='${v.child}'/>
                                    <div>
                                        <b>${v.roomName}</b>
                                        <p>${v.categoryName}</p>
                                        <small>Adult: ${v.adult}</small>
                                        <small>Child: ${v.child}</small>
                                    </div>
                                  </td>`;

            let discountInput = v.rawActualCheckOutTime == null
                ? `<input type='number' class='form-control p-1 mt-1 room-discount' id='room_per_discount_${i}' data-index='${i}' value='${v.roomDiscount}' />`
                : `<input type='number' class='form-control p-1 mt-1' id='room_per_discount_${i}' value='${v.roomDiscount}' readonly/>`;

            const checkInCell = `<td> 
                                    <input type='hidden' name='BookingRoomVms[${i}].CheckInTimeStr' value='${v.checkInTime}'/>
                                    <b> Check In</b> <br/>
                                    <span>${v.checkInTime}</span></br>
                                    <small>Discount Per Room<small></br>
                                    ${discountInput}
                                </td>`;

            let dateInput = v.rawActualCheckOutTime == null
                ? `<input class='form-control date-time-picker' id='date_time_${i}' name='BookingRoomVms[${i}].ActualCheckOutTimeStr' data-index='${i}' type="text" readonly>`
                : `<input class='form-control' name='BookingRoomVms[${i}].ActualCheckOutTimeStr' value='${v.actualCheckOutTime}' type="text" readonly>`;

            const checkOutCell = `<td> 
                                    <input type='hidden' name='BookingRoomVms[${i}].CheckOutTimeStr' value='${v.checkOutTime}'/>
                                    <b> Check Out</b> <br/>
                                    <span>${v.checkOutTime}</span> <br/>
                                    <b>Actual Check Out</b> <br/>
                                    ${dateInput}
                                </td>`;

            const halfDayCheckBox = `<div class="form-check">
                                      <input class="form-check-input half_day_check" type="checkbox" value='true' name='BookingRoomVms[${i}].IsHalfDay' id="half_day_check_${i}" data-index='${i}'>
                                      <input type='hidden' name='BookingRoomVms[${i}].IsHalfDay' value='false'>
                                      <label class="form-check-label">
                                        Is Half Day
                                      </label>
                                    </div>`;


            const dayUseCheckBox = `<div class="form-check">
                                      <input class="form-check-input day_use_check" type="checkbox" value='true' name='BookingRoomVms[${i}].IsDayUse' id="day_use_check_${i}" data-index='${i}'>
                                      <input type='hidden' name='BookingRoomVms[${i}].IsDayUse' value='false'>
                                      <label class="form-check-label">
                                        Is Day Use
                                      </label>
                                    </div>`;

            const exraBedHtml = `<b>Extra Bed Charge:</b> <br/>
                                <input type='number' class='bed-charge' data-index='${i}' id='extra_bed_charge_${i}' name='BookingRoomVms[${i}].ExtraBedCharge' value='${v.extraBedCharge}' disabled/>`;

            let dayCheck = "";

            if (!(v.bookingDayStatus > 0) && v.rawActualCheckOutTime == null) {
                dayCheck = `${halfDayCheckBox}${dayUseCheckBox}`;
            } else if (v.bookingDayStatus == 1) {
                dayCheck = `<b>Half Day</b>`;
            } else if (v.bookingDayStatus == 2) {
                dayCheck = `<b>Full Day/Day Use</b>`;
            }

            const noOfDaysCell = `<td>
                                    <input class='form-control' type='number' id='days_${i}' value='${v.days}' readonly/> days <br>
                                    ${dayCheck}
                                </td>`;

            const hiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].Rent' value='${v.rent}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].ServiceCharge' value='${v.serviceCharge}'/>`;

            const priceCell = `<td>
                                <b>Rent:</b> ${v.roomRent} <br/>
                                <b>Service Charge:</b> ${v.serviceCharge} <br/>
                                <b>Discount:</b> <br/>
                                <input type='number' id='room_discount_${i}' name='BookingRoomVms[${i}].Discount' value='${v.discount}' readonly/> <br/>
                                <b>Extra Bed:</b> ${v.extraBedAsServiceCharge} <br/>
                                
                                ${hiddenInput}
                              </td>`;

            const totalCell = `<td style='padding:5px;'>
                                    <input class='form-control text-end' name='BookingRoomVms[${i}].NetRent' id='total_rent_${i}' value='${v.totalRent}' readonly/>
                               </td>`;

            const row = `<tr style='font-size: smaller;'>${slNoCell}${roomInfoCell}${checkInCell}${checkOutCell}${noOfDaysCell}${priceCell}${totalCell}</tr>`;

            $("#RoomTableTbody").append(row);

        });

        $(".date-time-picker").datetimepicker('option', 'disabled', true);

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#sub-total").val(sumResult);

        const totalExtraBedChargeSum = calculateSum(roomList.map(x => x.extraBedAsServiceCharge));
        $("#Extra_Bed_Charge").val(totalExtraBedChargeSum);

        calculateNetTotal();
    }
}

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

        let bedCharge = (room.extraBed * extraBedCharge) * days;

        let rentAmount = roomRent * days;
        let totalRent = parseFloat(rentAmount) + parseFloat(serviceCharge) + parseFloat(bedCharge);

        roomList[index].days = days;
        roomList[index].totalRent = totalRent;

        $(`#days_${index}`).val(days);
        $(`#extra_bed_charge_${index}`).val(bedCharge);
        $(`#total_rent_${index}`).val(totalRent);

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#sub-total").val(sumResult);

        calculateNetTotal();
    }
});

function globalChangeDate() {
    if (roomList.length > 0) {
        roomList.forEach((v, index) => {
            /*const checkOutTime = $(`#date_time_${index}`).datetimepicker('getValue');*/

            let checkOutTime = v.rawActualCheckOutTime == null ? $(`#date_time_${index}`).datetimepicker('getValue') : v.rawActualCheckOutTime;

            console.log("test-time:", checkOutTime);

            /*var room = roomList[index];*/
            let room = v;

            let days = dateDifference(room.rawCheckInTime, checkOutTime);

            if (v.bookingDayStatus > 0) {
                if (v.bookingDayStatus == 1) {
                    days = days + 0.5;
                } else if (v.bookingDayStatus == 2) {
                    days = days + 1;
                }
            }

            const roomRent = room.roomRent;
            const roomDiscount = room.roomDiscount;
            const serviceCharge = room.serviceCharge;

            let bedCharge = (room.extraBed * extraBedCharge) * days;

            let rentAmount = roomRent * days;
            let discountAmount = roomDiscount * days;
            let totalRent = (parseFloat(rentAmount) + parseFloat(serviceCharge) + parseFloat(bedCharge)) - (parseFloat(discountAmount));

            roomList[index].days = days;
            roomList[index].totalRent = totalRent;

            $(`#days_${index}`).val(days);
            $(`#extra_bed_charge_${index}`).val(bedCharge);
            $(`#room_discount_${index}`).val(discountAmount);
            $(`#total_rent_${index}`).val(totalRent);

            var isHalfDay = $(`#half_day_check_${index}`).prop("checked");

            if (isHalfDay) {
                calculateHalfDay(index, true);
            }

            var isDayUse = $(`#day_use_check_${index}`).prop("checked");

            if (isDayUse) {
                calculateDayUse(index, true);
            }
        })

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#sub-total").val(sumResult);

        calculateNetTotal();
    }
}

function calculateSum(array) {
    var sum = 0;

    $.each(array, function (index, value) {
        sum += parseFloat(value);
    });

    return sum;
}


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
    const extraBedCharge = parseFloat($("#Extra_Bed_Charge").val());
    const paidAmount = parseFloat($("#PaidAmount").val());
    //const vat = parseFloat($("#Vat").val());
    //const tax = parseFloat($("#Tax").val());
    const discount = parseFloat($("#Discount").val());
    const due = parseFloat($("#due-amount").val());

    if (discount > due) {
        failedMsg("Discount can't be graeter than Due amount...Please check and try again..!!");
        $("#Discount").val(0);
        calculateNetTotal();
        return;
    }

    const netTotal = (subTotal + extraBedCharge) - (discount);

    console.log("Net Total: ", netTotal);
    $("#net-total").val(netTotal);

    let dueAmount = netTotal > paidAmount ? netTotal - paidAmount : 0;
    $("#due-amount").val(dueAmount);

    let refundAmount = paidAmount > netTotal ? paidAmount - netTotal : 0;
    $("#refund-amount").val(refundAmount);

    calculateBillWithFood(netTotal);
}

function calculateBillWithFood(netTotal) {
    const foodBill = parseFloat($("#food-total").val());
    if (foodBill > 0) {
        const totalBill = foodBill + netTotal;
        $("#room-net-total").val(netTotal);
        $("#food-grand-total").val(totalBill);
    }
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

//#endregion

$(document.body).on("click", "#CheckOutSubmitBtn", function () {

    swal({
        title: "Are you sure to check-out this booking?",
        text: "Once check-out then the booking can't be revert again..!!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const bookingId = $("#bookingId").val();
                const actualCheckOutTime = $("#date-time-picker-check-to").datetimepicker('getValue');

                if (actualCheckOutTime == null || actualCheckOutTime == undefined) {
                    return errorMsg("Please Select Actual CheckOut Time..!!");
                }

                if (bookingId > 0 && actualCheckOutTime != "") {
                    $("#CheckOutForm").submit();
                }
            }
        })
});

$(document.body).on("change", "#discount_percent", function () {
    calculateDiscount();
});

function calculateDiscount() {
    const subTotal = parseFloat($("#sub-total").val());
    const discountPercent = parseFloat($("#discount_percent").val());

    if (discountPercent > 0) {
        const discountAmount = calculatePercentage(subTotal, discountPercent);
        $("#Discount").val(discountAmount);
    }

    calculateNetTotal();
}

function calculatePercentage(value, percentage) {
    return (value * percentage) / 100;
}

//#region IsHalf Day

$(document.body).on("click", ".half_day_check", function () {

    var isChecked = $(this).is(':checked');
    var index = $(this).attr("data-index");

    if (index > -1) {
        if (isChecked) {
            console.log('Checkbox is checked.');
            $(`#half_day_check_${index}`).prop('checked', true);
            calculateHalfDay(index, true);
        } else {
            console.log('Checkbox is not checked.');
            $(`#half_day_check_${index}`).prop('checked', false);
            calculateHalfDay(index, false);
        }
    }
});

function calculateHalfDay(index, check) {
    const room = roomList[index];

    let days = 0;
    if (check) {
        days = room.days + 0.5;
    } else {
        days = room.days - 0.5;
    }

    const roomRent = room.roomRent;
    const roomDiscount = room.roomDiscount;
    const serviceCharge = room.serviceCharge;
    //const extraBedCharge = room.extraBedAsServiceCharge;

    /*let bedCharge = (room.extraBed * extraBedCharge) * days;*/

    //let bedDays = 0;
    //if (check) {
    //    bedDays = days - 0.5;
    //} else {
    //    bedDays = days;
    //}

    /*let bedCharge = (room.extraBed * extraBedCharge) * bedDays;*/

    //let bedCharge = $(`#extra_bed_charge_${index}`).val();
    //let bedCharge = extraBedCharge;

    let rentAmount = roomRent * days;
    let discountAmount = roomDiscount * days;
    let totalRent = parseFloat(rentAmount) + parseFloat(serviceCharge) - parseFloat(discountAmount);

    roomList[index].days = days;
    roomList[index].totalRent = totalRent;

    $(`#days_${index}`).val(days);
    //$(`#extra_bed_charge_${index}`).val(bedCharge);
    $(`#room_discount_${index}`).val(discountAmount);
    $(`#total_rent_${index}`).val(totalRent);

    const sumResult = calculateSum(roomList.map(x => x.totalRent));
    $("#sub-total").val(sumResult);

    calculateNetTotal();
}

//#endregion

//#region Day Use

$(document.body).on("click", ".day_use_check", function () {

    var isChecked = $(this).is(':checked');
    var index = $(this).attr("data-index");

    if (index > -1) {
        if (isChecked) {
            $(`#day_use_check_${index}`).prop('checked', true);
            calculateDayUse(index, true);
        } else {
            $(`#day_use_check_${index}`).prop('checked', false);
            calculateDayUse(index, false);
        }
    }
});

function calculateDayUse(index, check) {
    const room = roomList[index];

    let days = 0;

    if (check) {
        days = room.days + 1;
    } else {
        days = room.days - 1;
    }

    const roomRent = room.roomRent;
    const roomDiscount = room.roomDiscount;
    const serviceCharge = room.serviceCharge;

    //let bedCharge = (room.extraBed * extraBedCharge) * days;

    let rentAmount = roomRent * days;
    let discountAmount = roomDiscount * days;
    let totalRent = parseFloat(rentAmount) + parseFloat(serviceCharge) - parseFloat(discountAmount);

    roomList[index].days = days;
    roomList[index].totalRent = totalRent;

    $(`#days_${index}`).val(days);
    //$(`#extra_bed_charge_${index}`).val(bedCharge);
    $(`#room_discount_${index}`).val(discountAmount);
    $(`#total_rent_${index}`).val(totalRent);

    const sumResult = calculateSum(roomList.map(x => x.totalRent));
    $("#sub-total").val(sumResult);

    calculateNetTotal();
}

//#endregion

//#region Room Wise Discount

$(document.body).on("change", ".room-discount", function () {

    var index = $(this).attr("data-index");
    var roomDiscount = $(this).val();

    if (index > -1) {
        calculateRoomDiscount(index, roomDiscount);
    }
});

function calculateRoomDiscount(index, rmDiscount) {
    let roomDiscount = rmDiscount;

    const room = roomList[index];

    let days = room.days;

    const roomRent = room.roomRent;
    const serviceCharge = room.serviceCharge;

    let bedCharge = (room.extraBed * extraBedCharge) * days;

    let rentAmount = roomRent * days;
    let discountAmount = roomDiscount * days;
    let totalRent = parseFloat(rentAmount) + parseFloat(serviceCharge) + parseFloat(bedCharge) - parseFloat(discountAmount);

    roomList[index].days = days;
    roomList[index].roomDiscount = roomDiscount;
    roomList[index].totalRent = totalRent;

    $(`#days_${index}`).val(days);
    $(`#room_discount_${index}`).val(discountAmount);
    $(`#extra_bed_charge_${index}`).val(bedCharge);
    $(`#total_rent_${index}`).val(totalRent);

    const sumResult = calculateSum(roomList.map(x => x.totalRent));
    $("#sub-total").val(sumResult);

    calculateNetTotal();
}

//#endregion

//#region ExtraBed

$(document.body).on("change", ".bed-charge", function () {
    var index = $(this).attr("data-index");

    if (index > -1) {
        bedChargeCalculation(index);
    }
});

function bedChargeCalculation(index) {

    if (index > -1) {
        const room = roomList[index];

        const roomRent = room.roomRent;
        const roomDiscount = room.roomDiscount;
        const serviceCharge = room.serviceCharge;

        var bedCharge = parseFloat($(`#extra_bed_charge_${index}`).val());

        if (bedCharge >= 0) {
            let rentAmount = roomRent * room.days;
            let discountAmount = roomDiscount * room.days;
            let totalRent = parseFloat(rentAmount) + parseFloat(serviceCharge) + parseFloat(bedCharge) - parseFloat(discountAmount);

            roomList[index].extraBedCharge = bedCharge;
            roomList[index].totalRent = totalRent;

            $(`#total_rent_${index}`).val(totalRent);

            const sumResult = calculateSum(roomList.map(x => x.totalRent));
            $("#sub-total").val(sumResult);

            calculateNetTotal();
        }
    }

}

//#endregion