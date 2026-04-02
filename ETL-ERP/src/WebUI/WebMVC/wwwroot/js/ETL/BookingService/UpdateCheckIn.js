let slNo = 0;
let roomList = [];
let guestList = [];

const extraBedCharge = 500;

const CleaningStatus = {
    VACANT_CLEAN: 0,
    VACANT_DIRTY: 1,
    OCCUPIED: 2,
    CHECK_OUT: 3,
    OUT_OF_ORDER: 4,
};

$(document).ready(function () {
    loadRoomData();
    loadBookingGuestData();
    renderInitRoomTableBody();
    renderInitGuestTableBody();
    $("#clearDiv").hide();
});

$(document.body).on("change", "#date-time-picker-check-from", function () {
    const actualCheckInTime = $("#date-time-picker-check-from").datetimepicker('getValue');

    $(".actual-in-picker").datetimepicker({
        value: actualCheckInTime,
        startDate: new Date(actualCheckInTime)
    });
    $("#clearDiv").show();
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
                            roomRent: v.roomRent,
                            roomServiceCharge: v.roomServiceCharge,
                            roomDiscount: v.roomDiscount,
                            complementaryId: v.complementaryId,
                            complementaryName: v.complementaryName,
                            rent: v.rent,
                            serviceCharge: v.serviceCharge,
                            discount: v.discount,
                            totalRent: v.netRent,
                            adult: v.adult,
                            child: v.child,
                            extraBed: v.extraBed,
                            extraBedCharge: v.extraBedCharge,
                            checkInTime: checkInDate,
                            checkOutTime: checkOutDate,
                            rawCheckInTime: v.checkInTime,
                            rawCheckOutTime: v.checkOutTime,
                            days: days,
                            actualCheckInTime: actualCheckInDate,
                            actualCheckOutTime: actualCheckOutDate,
                            rawActualCheckInTime: v.actualCheckInTime,
                            rawActualCheckOutTime: v.actualCheckOutTime,
                            bookingDayStatus: v.bookingDayStatus,
                            cleaningStatus: v.cleaningStatus,
                            cleaningStatusText: v.cleaningStatusText,
                            isCharged: v.isCharged
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

$(document.body).on("change", "#CategoryId", function () {

    const categoryId = $(this).val();

    if (categoryId > 0) {
        categoryWiseRoomData();
    }
});

function categoryWiseRoomData() {
    const categoryId = $("#CategoryId").val();

    const checkInTime = $("#date-time-picker-in").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-out").datetimepicker('getValue');

    if (checkInTime == null || checkOutTime == null) {
        return errorMsg("Please Select Booked Check-In & Check-Out Date..!!");
    }

    const fromDateStr = convertJsToStrDate(new Date(checkInTime));
    const toDateStr = convertJsToStrDate(new Date(checkOutTime));

    if (categoryId > 0 && fromDateStr != "" && toDateStr != "") {
        _dropdownManager.getRoomByCategoryId(categoryId, fromDateStr, toDateStr, "#RoomId", null, null);

        const url = `${API}RoomCategory/GetRoomCatgoryInfoById/${categoryId}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("category-info: ", rData);
                $("#rent").val(rData.rent);
                $("#service-charge").val(rData.serviceCharge);
                $("#total-rent").val(rData.totalRent);
            } else {
                console.log("No Room Category Info Found...!");
            }
        })
    }
}

function loadRoomSelectData() {
    const categoryId = $("#CategoryId").val();
    const fromDateStr = $("#check-in-time").val();
    const toDateStr = $("#check-out-time").val();

    if (categoryId > 0 && fromDateStr != "" && toDateStr != "") {
        _dropdownManager.getRoomByCategoryId(categoryId, fromDateStr, toDateStr, "#RoomId", null, null);

        const url = `${API}RoomCategory/GetRoomCatgoryInfoById/${categoryId}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("category-info: ", rData);
                $("#rent").val(rData.rent);
                $("#service-charge").val(rData.serviceCharge);
                $("#total-rent").val(rData.totalRent);
            } else {
                console.log("No Room Category Info Found...!");
            }
        })
    }
}

$(document.body).on("change", "#RoomId", function () {

    const roomId = $(this).val();

    if (roomId > 0) {

        const url = `${API}RoomInfo/GetRoomInfoById/${roomId}`;
        $.get(url, function (rData) {
            if (rData) {
                $("#room-rent").val(rData.rent);
                $("#room-service-charge").val(rData.serviceCharge);
                $("#cleaningStatus").val(rData.cleaningStatus);
                const days = roomDateDifference();
                if (days > 0) {
                    let rentAmount = rData.rent * days;
                    let serviceChargeAmount = rData.serviceCharge * days;

                    $("#rent").val(rentAmount);
                    $("#service-charge").val(serviceChargeAmount);

                    let totalRent = parseFloat(rentAmount) + parseFloat(serviceChargeAmount);
                    $("#total-rent").val(totalRent);
                }

            } else {
                console.log("No Room Info Found...!");
            }
        })
    }
});

$(document.body).on("click", "#add-room", function (e) {
    e.preventDefault();
    const categoryId = $("#CategoryId option:selected").val();
    const categoryName = $("#CategoryId option:selected").text();
    const roomId = $("#RoomId option:selected").val();
    const roomName = $("#RoomId option:selected").text();
    const complementaryId = $("#ComplementaryId option:selected").val();
    const complementaryName = $("#ComplementaryId option:selected").text();
    const rent = $("#rent").val();
    const sc = $("#service-charge").val();
    const discount = $("#room-discount").val();
    const totalRent = $("#total-rent").val();

    const roomRent = $("#room-rent").val();
    const roomServiceCharge = $("#room-service-charge").val();
    const roomDiscount = $("#room-per-discount").val();

    const roomInTime = $("#date-time-picker-in").datetimepicker('getValue');
    const roomOutTime = $("#date-time-picker-out").datetimepicker('getValue');

    const roomInTimeStr = convertJsonFullDate(new Date(roomInTime));
    const roomOutTimeStr = convertJsonFullDate(new Date(roomOutTime));

    const cleaningStatus = $("#cleaningStatus").val();


    if (cleaningStatus != CleaningStatus.VACANT_CLEAN) {
        return errorMsg("Room Is Not Vacant and Clean.Please Add Vacant and Clean Room...!!");
    }

    const model = {
        id: 0,
        categoryId: categoryId,
        categoryName: categoryName,
        roomId: roomId,
        roomName: roomName,
        roomRent: roomRent,
        roomServiceCharge: roomServiceCharge,
        roomDiscount: roomDiscount,
        complementaryId: complementaryId,
        complementaryName: complementaryName,
        rent: rent,
        serviceCharge: sc,
        discount: discount,
        totalRent: totalRent,
        adult: 0,
        child: 0,
        extraBed: 0,
        extraBedCharge: 0,
        checkInTime: roomInTimeStr,
        checkOutTime: roomOutTimeStr,
        rawCheckInTime: roomInTime,
        rawCheckOutTime: roomOutTime,
        actualCheckInTime: roomInTimeStr,
        actualCheckOutTime: '',
        rawActualCheckInTime: roomInTime,
        rawActualCheckOutTime: '',
        cleaningStatus: cleaningStatus,
        cleaningStatusText: cleaningStatus == CleaningStatus.VACANT_CLEAN ? 'Vacant & Clean' : 'N/A'
    };

    if (model.discount == "" || model.discount == null) {
        model.discount = 0;
    }

    console.log("room-info: ", model);

    if (model.categoryId > 0 && model.roomId > 0) {

        const existRoom = roomList.find(x => x.roomId == model.roomId);

        //if (existRoom != null) {
        //    errorMsg("Room Already Added...!!");
        //} else {
        //    roomList.push(model);
        //}

        roomList.push(model);
    }

    renderRoomTableBody();
});

function loadCategorySelectList() {

    const url = API + "RoomCategory/GetRoomCategoryJsonData";

    createSelectList(url, null, "#CategoryId", null, null);
}

function loadComplementarySelectList() {

    const url = API + "Complementary/GetComplementaryJsonData";

    createSelectList(url, null, "#ComplementaryId", null, null);
}

function renderInitRoomTableBody() {

    const slNoCell = `<td>#</td>`;

    const checkInOutCell = `<td> 
                                <input class='form-control' id='date-time-picker-in' type="text">
                                <input class='form-control mt-2' id='date-time-picker-out' type="text">
                                <div class='d-flex mt-2'>
                                    <span style='align-self:center;margin-right:5px;'> Days </span>
                                    <input class='form-control' id='days' type="number" value='0' readonly> 
                                </div>
                            </td>`;

    const categoryCell = `<td> <select class='form-control dd-type' id='CategoryId'></select> </td>`;

    const roomCell = `<td>
                        <select class='form-control dd-type' id='RoomId'></select> 
                        <input id="room-rent" class="form-control btn-square" type="hidden">
                        <input id="room-service-charge" class="form-control btn-square" type="hidden">
                        <small>Discount Per Room<small></br>
                        <input type='number' class='form-control p-1 mt-1' id='room-per-discount' value='0' />
                        <input id="cleaningStatus" type="hidden">
                    </td>`;

    const rentGroup = `<div class='input-group input-group-sm mb-1'>
                            <span class='input-group-text'>Rent</span>
                            <input id="rent" class="form-control btn-square" type="number" readonly>
                       </div>`;

    const serviceChargeGroup = `<div class='input-group input-group-sm mb-1'>
                                    <span class='input-group-text'>S.C.</span>
                                    <input id="service-charge" class="form-control btn-square" type="number" readonly>
                               </div>`;

    const discountGroup = `<div class='input-group input-group-sm mb-1'>
                                    <span class='input-group-text'>Discount</span>
                                    <input id="room-discount" class="form-control btn-square" type="number" readonly>
                               </div>`;

    const totalRentGroup = `<div class='input-group input-group-sm mb-1'>
                                <span class='input-group-text'>Total</span>
                                <input id="total-rent" class="form-control btn-square" type="number" readonly>
                           </div>`;

    const costCell = `<td>${rentGroup}${serviceChargeGroup}${discountGroup}${totalRentGroup}</td>`;

    const complementaryCell = `<td> <select class='form-control dd-type' id='ComplementaryId'></select> </td>`;

    const actionCell = `<td class='text-center'> <a class='mr-2' href='#' id='add-room' title='Add'><i class="fa fa-plus fa-2x"></i></a> </td>`;

    const row = `<tr>${slNoCell}${checkInOutCell}${categoryCell}${roomCell}${costCell}${complementaryCell}${actionCell}</tr>`;

    $("#RoomTableTbody").append(row);

    $(".dd-type").select2({ width: "100%" }).on("change", function (e) {
        $(this).valid();
    });

    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

    if (checkInTime != null) {
        $("#date-time-picker-in").datetimepicker({
            value: checkInTime,
            startDate: new Date(checkInTime),
            defaultTime: '12:00'
        });
    } else {
        $("#date-time-picker-in").datetimepicker({
            defaultTime: '12:00'
        });
    }

    if (checkOutTime != null) {
        $("#date-time-picker-out").datetimepicker({
            value: checkOutTime,
            startDate: new Date(checkOutTime),
            defaultTime: '12:00'
        });
    } else {
        $("#date-time-picker-out").datetimepicker({
            defaultTime: '12:00'
        });
    }

    loadCategorySelectList();
    loadComplementarySelectList();
}

function renderRoomTableBody() {

    $("#RoomTableTbody").empty();

    if (roomList.length > 0) {

        roomList.forEach((v, i) => {

            let cleaningStatus = v.cleaningStatus != null ? v.cleaningStatus : 'N/A';
            let cleaningStatusText = v.cleaningStatusText != null ? v.cleaningStatusText : 'N/A';
            let statusBadge = cleaningStatus == CleaningStatus.VACANT_CLEAN ? "badge badge-success" : cleaningStatus == CleaningStatus.VACANT_DIRTY ? "badge badge-secondary" : CleaningStatus.OCCUPIED ? "badge badge-info" : "";

            const checkBox = `<input type="checkbox" class="select-row" data-index="${i}">`;

            const slNoCell = `<td>
                                  ${i + 1}
                                 <input type='hidden' name='BookingRoomVms[${i}].Id' value='${v.id}'/>   
                              </td>`;

            let checkInActual = `<input type='hidden' name='BookingRoomVms[${i}].ActualCheckInTimeStr' value='${v.actualCheckInTime}'/><span>${v.actualCheckInTime}</span>`;

            if (v.rawActualCheckInTime == null) {
                checkInActual = cleaningStatus == 0 ? `<input class='form-control actual-in-picker' data-index='${i}' name='BookingRoomVms[${i}].ActualCheckInTimeStr' id='actual_room_check_in_${i}' type='text' autocomplete='off'>`
                    : `<input class='form-control' data-index='${i}' name='BookingRoomVms[${i}].ActualCheckInTimeStr' id='actual_room_check_in_${i}' type='text' autocomplete='off' disabled>`;;
            }

            let bookedCheckOut = `<input class='form-control booked-date-time-picker' data-index='${i}' name='BookingRoomVms[${i}].CheckOutTimeStr' id='booked_room_check_out_${i}' type='text' autocomplete='off'>`;
            let checkOutActual = `<input class='form-control date-time-picker' data-index='${i}' name='BookingRoomVms[${i}].ActualCheckOutTimeStr' id='room_check_out_${i}' type='text' autocomplete='off'>`;

            let extraBedhtml = `<small>Extra Bed<small></br>
                                <input type='number' class='form-control p-1 mt-1 extra-bed' name='BookingRoomVms[${i}].ExtraBed' value='${v.extraBed}' data-index='${i}' placeholder='ExtraBed'/></br>`;
            let paxHtml = `<div class='mt-2'>
                                <input type='number' class='form-control p-1' name='BookingRoomVms[${i}].Adult' placeholder='Adult'/>
                                <input type='number' class='form-control p-1 mt-1' name='BookingRoomVms[${i}].Child' placeholder='Children'/>
                            </div>`;
            let dayStatusText = ``;

            if (v.rawActualCheckOutTime != null && v.rawActualCheckOutTime != "") {
                bookedCheckOut = `<input type='hidden' name='BookingRoomVms[${i}].CheckOutTimeStr' value='${v.checkOutTime}'/><span>${v.checkOutTime}</span>`;
                checkOutActual = `<input type='hidden' name='BookingRoomVms[${i}].ActualCheckOutTimeStr' value='${v.actualCheckOutTime}'/><span>${v.actualCheckOutTime}</span>`;

                dayStatusText = v.bookingDayStatus == 1 ? `<b>Half Day</b>` : v.bookingDayStatus == 2 ? `<b>Day Use/ Full Day</b>` : ``;
                paxHtml = `<div class='mt-2'>
                             <input type='number' class='form-control p-1' name='BookingRoomVms[${i}].Adult' placeholder='Adult' readonly/>
                             <input type='number' class='form-control p-1 mt-1' name='BookingRoomVms[${i}].Child' placeholder='Children' readonly/>
                          </div>`;
            }

            const checkInOutCell = `<td> 
                                        <input type='hidden' name='BookingRoomVms[${i}].CheckInTimeStr' value='${v.checkInTime}'/>                                       
                                        <b>Booked Check In</b> <br/>
                                        <span>${v.checkInTime}</span> <br/>
                                        <b>Booked Check Out</b> <br/>
                                        ${bookedCheckOut} <br/>
                                        <b>Actual Check In</b> <br/>
                                        ${checkInActual} <br/>
                                        <b>Actual Check Out</b> <br/>
                                        ${checkOutActual}
                                    </td>`;

            const categoryCell = `<td> <input type='hidden' name='BookingRoomVms[${i}].RoomCategoryId' value='${v.categoryId}'/> ${v.categoryName}
                                    ${paxHtml}
                                </td>`;

            const roomhiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].RoomRent' value='${v.roomRent}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomServiceCharge' value='${v.roomServiceCharge}'/>`;

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

            let discountInput = v.rawActualCheckOutTime == null
                ? `<input type='number' class='form-control p-1 mt-1 room-discount' id='room_per_discount_${i}' data-index='${i}' value='${v.roomDiscount}' />`
                : `<input type='number' class='form-control p-1 mt-1' id='room_per_discount_${i}' value='${v.roomDiscount}' readonly/>`;

            const roomCell = `<td> 
                                <input type='hidden' name='BookingRoomVms[${i}].RoomId' value='${v.roomId}'/>
                                <strong>${v.roomName}</strong>
                                <span class="${statusBadge} px-1 py-1" style="border-radius:12px;font-size:9px;">
                                    ${cleaningStatusText}
                                </span>
                                <br/>
                                <div class='mt-2'>
                                    <b>Rent:</b> ${v.roomRent} <br/>
                                    <b>Service Charge:</b> ${v.roomServiceCharge} <br/>
                                    <b>Discount:</b> ${v.roomDiscount} <br/>
                                    <small>Discount Per Room<small></br>
                                    ${discountInput}
                                    ${roomhiddenInput}
                                    
                                    <div class='day_check' id='day_check_${i}'>
                                        ${halfDayCheckBox}
                                        ${dayUseCheckBox}
                                    </div>
                                    ${dayStatusText}
                                </div>
                            </td>`;

            const costCell = `<td>
                                <b>Rent:</b> <input type='number' id='rent_${i}' name='BookingRoomVms[${i}].Rent' value='${v.rent}' readonly/> <br/>
                                <b>Service Charge:</b> <input type='number' id='service_charge_${i}' name='BookingRoomVms[${i}].ServiceCharge' value='${v.serviceCharge}' readonly/> <br/>
                                <b>Discount:</b> <input type='number' id='room_discount_${i}' name='BookingRoomVms[${i}].Discount' value='${v.discount}' readonly/> <br/>
                                <b>Bed Charge:</b> <input type='number' id='bed_charge_${i}' name='BookingRoomVms[${i}].ExtraBedCharge' value='${v.extraBedCharge}' readonly/> <br/>
                                <b>Total Rent:</b> <input type='number' id='total_rent_${i}' name='BookingRoomVms[${i}].NetRent' value='${v.totalRent}' readonly/> <br/>
                              </td>`;

            const noComplement = `<td>No Complement</td>`;

            const withComplement = `<td> <input type='hidden' name='BookingRoomVms[${i}].ComplementaryId' value='${v.complementaryId}'/> ${v.complementaryName} </td>`;

            const complementaryCell = v.complementaryId > 0 ? withComplement : noComplement;

            let deleteBtnHtml = v.isCharged ? `<b>Audited</b>` : `<a class='mr-2 remove-room' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x text-danger"></i></a>`;

            const actionCell = `<td class='text-center'> ${deleteBtnHtml} </td>`;

            const row = `<tr style='font-size: smaller;'>${slNoCell}${checkInOutCell}${categoryCell}${roomCell}${costCell}${complementaryCell}${actionCell}</tr>`;

            $("#RoomTableTbody").append(row);

            if (v.rawCheckOutTime != null) {
                $(`#booked_room_check_out_${i}`).datetimepicker({
                    value: v.rawCheckOutTime,
                    startDate: new Date(v.rawCheckOutTime),
                    /*defaultTime: '12:00'*/
                });
            } else {
                $(`#booked_room_check_out_${i}`).datetimepicker({
                    /*defaultTime: '12:00'*/
                });
            }

            if (v.rawActualCheckOutTime != null) {
                $(`#room_check_out_${i}`).datetimepicker({
                    value: v.rawActualCheckOutTime,
                    startDate: new Date(v.rawActualCheckOutTime),
                    //defaultTime: '12:00'
                });
            } else {
                $(`#room_check_out_${i}`).datetimepicker({
                    //defaultTime: '12:00'
                });
            }

            if (v.rawActualCheckInTime == null) {
                $(`.actual-in-picker`).datetimepicker({
                    //defaultTime: '12:00'
                });
            }

        });

        $(".day_check").hide();
        /*$(".date-time-picker").datetimepicker();*/

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#net-total").val(sumResult)

        renderInitRoomTableBody();

    } else {
        renderInitRoomTableBody();
    }
}

function calculateSum(array) {
    var sum = 0;

    $.each(array, function (index, value) {
        sum += parseFloat(value);
    });

    return sum;
}

$(document.body).on("change", "#room-per-discount", function () {
    roomDiscountCalculation();
});

function roomDiscountCalculation() {
    const days = roomDateDifference();

    if (days > 0) {
        const roomRent = $("#room-rent").val();
        const serviceCharge = $("#room-service-charge").val();
        const discountPerRoom = $("#room-per-discount").val();

        let rentAmount = roomRent * days;
        let serviceChargeAmount = serviceCharge * days;
        let discount = parseFloat(discountPerRoom) * days;

        $("#rent").val(rentAmount);
        $("#service-charge").val(serviceChargeAmount);
        $("#room-discount").val(discount);

        let totalRent = ((parseFloat(rentAmount) + parseFloat(serviceChargeAmount)) - parseFloat(discount));
        $("#total-rent").val(totalRent);
    }
}

$(document.body).on("click", ".remove-room", function (e) {
    e.preventDefault();
    const roomIndex = $(this).attr("data-index");

    if (roomIndex > -1) {
        let roomData = roomList[roomIndex];

        if (roomData.rawActualCheckOutTime != null) {
            return failedMsg("This room is already check-out...!!");
        }
        roomList.splice(roomIndex, 1);
    };

    renderRoomTableBody();
});

$(document.body).on("change", "#date-time-picker-in", function () {
    const days = roomDateDifference();
    const roomRent = $("#room-rent").val();
    const roomServiceCharge = $("#room-service-charge").val();

    let rentAmount = roomRent * days;
    let serviceChargeAmount = roomServiceCharge * days;

    $("#rent").val(rentAmount);
    $("#service-charge").val(serviceChargeAmount);

    let totalRent = parseFloat(rentAmount) + parseFloat(serviceChargeAmount);
    $("#total-rent").val(totalRent);
})

$(document.body).on("change", "#date-time-picker-out", function () {
    const days = roomDateDifference();
    const roomRent = $("#room-rent").val();
    const roomServiceCharge = $("#room-service-charge").val();

    let rentAmount = roomRent * days;
    let serviceChargeAmount = roomServiceCharge * days;

    $("#rent").val(rentAmount);
    $("#service-charge").val(serviceChargeAmount);

    let totalRent = parseFloat(rentAmount) + parseFloat(serviceChargeAmount);
    $("#total-rent").val(totalRent);
})

//function roomDateDifference() {
//    const roomInTime = $("#date-time-picker-in").datetimepicker('getValue');
//    const roomOutTime = $("#date-time-picker-out").datetimepicker('getValue');

//    const fromDate = new Date(roomInTime);
//    const toDate = new Date(roomOutTime);

//    const timeDifference = toDate - fromDate;

//    const seconds = Math.floor(timeDifference / 1000);
//    const minutes = Math.floor(seconds / 60);
//    const hours = Math.floor(minutes / 60);
//    const days = Math.floor(hours / 24);

//    console.log("Time difference in days for room:", days);

//    $("#days").val(days);

//    return days;
//}

function roomDateDifference() {
    const roomInTime = $("#date-time-picker-in").datetimepicker('getValue');
    const roomOutTime = $("#date-time-picker-out").datetimepicker('getValue');

    const fromDate = new Date(roomInTime);
    fromDate.setHours(0, 0, 0, 0);

    const toDate = new Date(roomOutTime);
    toDate.setHours(0, 0, 0, 0);

    const timeDifference = toDate - fromDate;

    // Converting milliseconds to days
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    console.log("Time difference in days for room:", days);

    $("#days").val(days);

    return days;
}

$(document.body).on("change", ".extra-bed", function () {
    const extraBed = $(this).val();
    const index = $(this).attr("data-index");

    if (index > -1) {
        let roomData = roomList[index];

        let days = dateDifference(roomData.rawCheckInTime, roomData.rawCheckOutTime);
        console.log("Total-Days", days);

        let bedCharge = (extraBed * extraBedCharge) * days;

        $(`#bed_charge_${index}`).val(bedCharge);

        roomList[index].extraBed = extraBed;
        roomList[index].extraBedCharge = parseFloat(bedCharge);

        const totalRent = parseFloat(roomData.rent) + parseFloat(roomData.serviceCharge) + parseFloat(bedCharge) - parseFloat(roomData.discount);
        roomList[index].totalRent = totalRent;

        $(`#total_rent_${index}`).val(totalRent);

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#net-total").val(sumResult);
    }

})

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

//#endregion

//#region Booked CheckOut Update

$(document.body).on("change", "#check-out-time", function () {
    const bookingCheckOutTime = $("#check-out-time").datetimepicker('getValue');
    $(".booked-date-time-picker").each(function () {
        const index = $(this).attr("data-index");

        $(this).datetimepicker({
            value: bookingCheckOutTime,
            startDate: new Date(bookingCheckOutTime)
        });

        calculateBookedDateWiseCost(index);

    });
});
$(document.body).on("change", ".actual-in-picker", function () {
    const index = $(this).attr("data-index");

    calculateBookedDateWiseCost(index);
});


$(document.body).on("change", ".booked-date-time-picker", function () {
    const index = $(this).attr("data-index");

    calculateBookedDateWiseCost(index);
});

function calculateBookedDateWiseCost(index) {
    if (index > -1) {
        const checkOutTime = $(`#booked_room_check_out_${index}`).datetimepicker('getValue');
        if (checkOutTime == null) {
            return;
        }
        const checkOutTimeStr = convertJsonFullDate(new Date(checkOutTime));

        let roomObj = roomList[index];
        //console.log(roomObj)
        let days = dateDifference(roomObj.rawActualCheckInTime, checkOutTime);

        //let days = 0;
        //var actualRoomCheckIn = $(`#actual_room_check_in_${index}`).val();

        //if (!hasAnyError(roomObj.rawActualCheckInTime)) {
        //    days = dateDifference(roomObj.rawActualCheckInTime, checkOutTime);
        //}
        //else if (!hasAnyError(actualRoomCheckIn)) {
        //    /*console.log('actualcheckIn', actualRoomCheckIn)*/
        //    days = dateDifference(actualRoomCheckIn, checkOutTime);
        //}
        //else {
        //    days = dateDifference(roomObj.rawCheckInTime, checkOutTime);
        //}

        let roomRent = roomObj.roomRent * days;
        let serviceCharge = roomObj.roomServiceCharge * days;
        let roomDiscount = roomObj.roomDiscount * days;

        let bedCharge = (roomObj.extraBed * extraBedCharge) * days;
        let netRent = roomRent + serviceCharge + bedCharge - roomDiscount;

        roomList[index].rent = roomRent;
        roomList[index].serviceCharge = serviceCharge;
        roomList[index].discount = roomDiscount;
        roomList[index].extraBedCharge = parseFloat(bedCharge);
        roomList[index].totalRent = netRent;

        roomList[index].checkOutTime = checkOutTimeStr;
        roomList[index].rawCheckOutTime = checkOutTime;

        $(`#rent_${index}`).val(roomRent);
        $(`#service_charge_${index}`).val(serviceCharge);
        $(`#room_discount_${index}`).val(roomDiscount);
        $(`#bed_charge_${index}`).val(bedCharge);
        $(`#total_rent_${index}`).val(netRent);

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#net-total").val(sumResult);
    }
}

//#endregion

//#region Actual Check Out

$(document.body).on("change", ".date-time-picker", function () {
    const index = $(this).attr("data-index");

    calculateDateWiseCost(index);
});

function calculateDateWiseCost(index) {
    if (index > -1) {
        let actualRoomOutTime = $(`#room_check_out_${index}`).datetimepicker('getValue');

        const actualRoomOutTimeInput = $(`#room_check_out_${index}`).val();

        if (actualRoomOutTimeInput != "") {
            $(`#day_check_${index}`).show();
        } else {
            $(`#day_check_${index}`).hide();
        }

        let roomObj = roomList[index];

        let actualRoomOutTimeStr = convertJsonFullDate(new Date(actualRoomOutTime));

        if (actualRoomOutTimeInput == "") {
            actualRoomOutTime = roomObj.rawCheckOutTime;
        }

        let days = dateDifference(roomObj.rawActualCheckInTime, actualRoomOutTime);
        //let days = 0;
        //var actualRoomCheckIn = $(`#actual_room_check_in_${index}`).val();

        //if (!hasAnyError(roomObj.rawActualCheckInTime)) {
        //    days = dateDifference(roomObj.rawActualCheckInTime, actualRoomOutTime);
        //}
        //else if (!hasAnyError(actualRoomCheckIn)) {
        //    /*console.log('actualcheckIn', actualRoomCheckIn)*/
        //    days = dateDifference(actualRoomCheckIn, actualRoomOutTime);
        //}
        //else {
        //    days = dateDifference(roomObj.rawCheckInTime, actualRoomOutTime);
        //}

        let roomRent = roomObj.roomRent * days;
        let serviceCharge = roomObj.roomServiceCharge * days;
        let roomDiscount = roomObj.roomDiscount * days;

        let bedCharge = (roomObj.extraBed * extraBedCharge) * days;
        let netRent = roomRent + serviceCharge + bedCharge - roomDiscount;

        roomList[index].rent = roomRent;
        roomList[index].serviceCharge = serviceCharge;
        roomList[index].discount = roomDiscount;
        roomList[index].extraBedCharge = parseFloat(bedCharge);
        roomList[index].totalRent = netRent;

        roomList[index].actualCheckOutTime = actualRoomOutTimeStr;
        roomList[index].rawActualCheckOutTime = actualRoomOutTime;

        $(`#rent_${index}`).val(roomRent);
        $(`#service_charge_${index}`).val(serviceCharge);
        $(`#room_discount_${index}`).val(roomDiscount);
        $(`#bed_charge_${index}`).val(bedCharge);
        $(`#total_rent_${index}`).val(netRent);

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#net-total").val(sumResult);
    }
}

//#endregion

//#region IsHalf Day

$(document.body).on("click", ".half_day_check", function () {

    var isChecked = $(this).is(':checked');
    var index = $(this).attr("data-index");

    if (index > -1) {
        if (isChecked) {
            var isDayChecked = $(`#day_use_check_${index}`).is(':checked');
            if (isDayChecked) {
                $(`#half_day_check_${index}`).prop('checked', false);
                return failedMsg("Day Use/Full Day Is Already Cheaked...!!");
            } else {
                $(`#half_day_check_${index}`).prop('checked', true);
                calculateHalfDay(index, true);
            }
        } else {
            $(`#half_day_check_${index}`).prop('checked', false);
            calculateHalfDay(index, false);
        }
    }
});

function calculateHalfDay(index, check) {
    const room = roomList[index];

    let roomDays = dateDifference(room.rawActualCheckInTime, room.rawActualCheckOutTime);
    //var actualRoomCheckIn = $(`#actual_room_check_in_${index}`).val();

    //var actualRoomCheckOut = $(`#room_check_out_${index}`).val();

    //let roomDays = 0;
    //if (!hasAnyError(room.rawActualCheckInTime) && !hasAnyError(room.rawActualCheckOutTime)) {
    //    roomDays = dateDifference(room.rawActualCheckInTime, room.rawActualCheckOutTime);
    //}
    //else if (!hasAnyError(actualRoomCheckIn) && !hasAnyError(actualRoomCheckOut)) {
    //    roomDays = dateDifference(actualRoomCheckIn, actualRoomCheckOut);
    //} else {
    //    swal({
    //        title: "Attention..!!",
    //        text: "Please Mention Actual Check-In and Actual Check-Out Time",
    //        icon: "warning",
    //        buttons: false,
    //        dangerMode: true,
    //    })
    //}

    let days = 0;
    if (check) {
        days = roomDays + 0.5;
    } else {
        days = roomDays;
    }

    let roomRent = room.roomRent * days;
    let serviceCharge = room.roomServiceCharge * days;
    let roomDiscount = room.roomDiscount * days;

    let bedCharge = $(`#bed_charge_${index}`).val();
    let netRent = roomRent + serviceCharge + parseFloat(bedCharge) - roomDiscount;

    roomList[index].rent = roomRent;
    roomList[index].serviceCharge = serviceCharge;
    roomList[index].discount = roomDiscount;
    roomList[index].extraBedCharge = parseFloat(bedCharge);
    roomList[index].totalRent = netRent;

    $(`#rent_${index}`).val(roomRent);
    $(`#service_charge_${index}`).val(serviceCharge);
    $(`#room_discount_${index}`).val(roomDiscount);
    $(`#bed_charge_${index}`).val(bedCharge);
    $(`#total_rent_${index}`).val(netRent);

    const sumResult = calculateSum(roomList.map(x => x.totalRent));
    $("#net-total").val(sumResult);
}

//#endregion

//#region Day Use

$(document.body).on("click", ".day_use_check", function () {

    var isChecked = $(this).is(':checked');
    var index = $(this).attr("data-index");

    if (index > -1) {
        if (isChecked) {
            var isHalfChecked = $(`#half_day_check_${index}`).is(':checked');
            if (isHalfChecked) {
                $(`#day_use_check_${index}`).prop('checked', false);
                return failedMsg("Half Day Is Already Cheaked...!!");
            } else {
                $(`#day_use_check_${index}`).prop('checked', true);
                calculateDayUse(index, true);
            }
        } else {
            $(`#day_use_check_${index}`).prop('checked', false);
            calculateDayUse(index, false);
        }
    }
});

function calculateDayUse(index, check) {
    const room = roomList[index];

    let roomDays = dateDifference(room.rawActualCheckInTime, room.rawActualCheckOutTime);

    let days = 0;
    if (check) {
        days = roomDays + 1;
    } else {
        days = roomDays;
    }

    let roomRent = room.roomRent * days;
    let serviceCharge = room.roomServiceCharge * days;
    let roomDiscount = room.roomDiscount * days;

    let bedCharge = (room.extraBed * extraBedCharge) * days;

    let netRent = roomRent + serviceCharge + parseFloat(bedCharge) - roomDiscount;

    roomList[index].rent = roomRent;
    roomList[index].serviceCharge = serviceCharge;
    roomList[index].discount = roomDiscount;
    roomList[index].extraBedCharge = parseFloat(bedCharge);
    roomList[index].totalRent = netRent;

    $(`#rent_${index}`).val(roomRent);
    $(`#service_charge_${index}`).val(serviceCharge);
    $(`#room_discount_${index}`).val(roomDiscount);
    $(`#bed_charge_${index}`).val(bedCharge);
    $(`#total_rent_${index}`).val(netRent);

    const sumResult = calculateSum(roomList.map(x => x.totalRent));
    $("#net-total").val(sumResult);
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

function loadGuestSelectList() {

    const url = API + "GuestInfo/GetGuestJsonData";

    createSelectList(url, null, "#GuestId", null, null);
}

$(document.body).on("click", "#GuestEntryBtn", function () {
    const salutation = $("#salutation").val();
    const firstName = $("#first-name").val();
    const lastName = $("#last-name").val();
    const mobile = $("#mobile").val();

    if (salutation != "" && firstName != "" && mobile != "") {
        const url = API + "GuestInfo/GuestEntry";

        const params = {
            salutation: salutation,
            firstName: firstName,
            lastName: lastName,
            mobile: mobile
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Guest Entry Successful");
                $("#guestEntryModal").modal('hide');
                clearGuestForm();

                loadGuestSelectList();
            } else {
                failedMsg("Guest Entry Failed");
            }
        }).fail(function () {
            failedMsg("Guest Entry Failed");
        })
    } else {
        failedMsg("Salutation, First Name, Mobile Number is required");
    }
})

function clearGuestForm() {
    $("#salutation").val("");
    $("#first-name").val("");
    $("#last-name").val("");
    $("#mobile").val("");
}

$(document.body).on("click", "#add-guest", function () {
    const guestId = $("#GuestId option:selected").val();
    const guestName = $("#GuestId option:selected").text();
    let isMain = false;

    if ($("#IsMain").is(':checked')) {
        isMain = true;
    } else {
        isMain = false;
    }

    const existGuestIndex = guestList.findIndex(x => x.guestId == guestId);

    if (existGuestIndex > -1) {
        return errorMsg("Guest already added...!!");
    }

    const model = {
        id: 0,
        guestId: guestId,
        guestName: guestName,
        isMain: isMain
    };

    if (model.guestId > 0) {
        guestList.push(model);
    }

    renderGuestTableBody();
});

function renderInitGuestTableBody() {

    const slNoCell = `<td>#</td>`;

    const guestCell = `<td> <select class='form-control dd-type' id='GuestId'></select> </td>`;

    const mainCell = `<td><input type='checkbox' id='IsMain'/></td>`;

    const actionCell = `<td class='text-center'> <a class='mr-2' href='#' id='add-guest' title='Add'><i class="fa fa-plus fa-2x"></i></a> </td>`;

    const row = `<tr>${slNoCell}${guestCell}${mainCell}${actionCell}</tr>`;

    $("#GuestTableTbody").append(row);

    $(".dd-type").select2({ width: "100%" }).on("change", function (e) {
        $(this).valid();
    });

    loadGuestSelectList();
}

function renderGuestTableBody() {

    $("#GuestTableTbody").empty();

    if (guestList.length > 0) {

        guestList.forEach((v, i) => {

            const slNoCell = `<td>${i + 1} <input type='hidden' name='BookingGuestVms[${i}].Id' value='${v.id}'/> </td>`;

            const guestCell = `<td> <input type='hidden' name='BookingGuestVms[${i}].GuestId' value='${v.guestId}'/> ${v.guestName} </td>`;

            const mainCell = `<td> <input type='hidden' name='BookingGuestVms[${i}].IsMain' value='${v.isMain}'/> ${v.isMain ? 'Yes' : 'No'}</td>`;

            const actionCell = `<td class='text-center'> <a class='mr-2 remove-guest' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x text-danger"></i></a> </td>`;

            const row = `<tr style='font-size: smaller;'>${slNoCell}${guestCell}${mainCell}${actionCell}</tr>`;

            $("#GuestTableTbody").append(row);
        });

        renderInitGuestTableBody();

    } else {
        renderInitGuestTableBody();
    }
}

$(document.body).on("click", ".remove-guest", function () {
    const guestIndex = $(this).attr("data-index");

    if (guestIndex > -1) {
        guestList.splice(guestIndex, 1);
    };

    renderGuestTableBody();
});

//#endregion

$(document.body).on("click", "#UpdateCheckInSubmitBtn", function () {
    const bookingId = $("#bookingId").val();
    const bookingDate = $("#BookingDateStr").val();
    const checkInTime = $("#CheckInTimeStr").val();
    const checkOutTime = $("#CheckOutTimeStr").val();

    if (bookingId > 0 && bookingDate != "" && checkInTime != "" && checkOutTime != "") {
        $("#UpdateCheckInForm").submit();
    }
});


//#Region Clear All Actual CheckIn Time

$(document.body).on("click", "#clearCheckOut", function () {
    $(".actual-in-picker").val("");
});
//#End Region

//#Region Clear Specific Actual CheckIn Time
$(document).on('click', '#selectAll', function () {
    $('.select-row').prop('checked', $(this).is(':checked'));
});

$(document.body).on("click", "#otherCheckOutTimeClear", function () {
    $(".select-row").each(function () {
        if (!$(this).is(':checked')) {
            const rowIndex = $(this).data('index');
            const checkInOutCell = $(`#RoomTableTbody tr:eq(${rowIndex}) `);
            checkInOutCell.find(".actual-in-picker").val("");
        }
    });
    $(".select-row").prop('checked', false);
    $(".selectAll").prop('checked', false);
});

//#End Region


//#region Multiple Room Selection

let modalRoomList = [];
let selectedRoomList = [];

$(document.body).on("click", "#group_book_btn", function () {
    modalRoomList = [];
    $("#room-section").empty();
    loadModalRoomData();
});

function loadModalRoomData() {

    const checkInTime = $("#group-book-check-in-picker").datetimepicker('getValue');
    const checkOutTime = $("#group-book-check-out-picker").datetimepicker('getValue');

    if (checkInTime == null || checkOutTime == null) {
        return failedMsg("Please Select Booked Check In & Out Date..!!");
    }

    const fromDateStr = convertJsToStrDate(new Date(checkInTime));
    const toDateStr = convertJsToStrDate(new Date(checkOutTime));

    if (fromDateStr != "" && toDateStr != "") {

        const url = `${API}RoomInfo/GetRoomByDateRange?checkInDate=${fromDateStr}&checkOutDate=${toDateStr}`;
        $.get(url, function (rData) {
            if (rData.length > 0) {
                console.log("modal-room-list: ", rData);
                modalRoomList = rData;
                renderModalRooms();
            } else {
                console.log("No Available Room Found...!");
                renderModalRooms();
            }
        })
    }
}

function renderModalRooms() {
    if (modalRoomList.length > 0) {

        $("#room-section").empty();

        modalRoomList.forEach((v, i) => {

            const roomIndex = selectedRoomList.findIndex(x => x.id == v.id);

            let color = 'btn-primary';

            if (roomIndex > -1) {
                color = 'btn-info';
            }

            const roomHtml = `<button class="btn ${color} mb-1" style="max-width:125px;margin-left:5px;" onclick='toggleRoom(${v.id})'>
                                    <h6>Room ${v.roomNo}</h6>
                                </button>`;

            $("#room-section").append(roomHtml);
        });
    } else {
        $("#room-section").html("<h3>No Available Room Found..!!</h3>");
    }
}

function toggleRoom(roomId) {
    const roomIndex = selectedRoomList.findIndex(x => x.id == roomId);

    if (roomIndex > -1) {
        selectedRoomList.splice(roomIndex, 1);
    } else {
        const roomObj = modalRoomList.find(x => x.id == roomId);

        if (roomObj == null || roomObj == undefined) {
            return errorMsg("Room Info Not Found...!");
        }

        selectedRoomList.push(roomObj);
    }

    renderModalRooms();
}

$(document.body).on("click", "#RoomSelectBtn", function () {

    $("#roomModal").modal('hide');

    submitModalRooms();
});

function submitModalRooms() {
    const complementaryId = $("#ModalComplementaryId option:selected").val();
    const complementaryName = $("#ModalComplementaryId option:selected").text();
    const roomDiscount = $("#group_room_discount").val();

    if (selectedRoomList.length > 0) {
        selectedRoomList.forEach((v, i) => {

            const roomInTime = $("#group-book-check-in-picker").datetimepicker('getValue');
            const roomOutTime = $("#group-book-check-out-picker").datetimepicker('getValue');

            const roomInTimeStr = convertJsonFullDate(new Date(roomInTime));
            const roomOutTimeStr = convertJsonFullDate(new Date(roomOutTime));

            const days = bookingDays();

            const model = {
                id: 0,
                categoryId: v.categoryId,
                categoryName: v.categoryName,
                roomId: v.id,
                roomName: v.roomNo,
                roomRent: v.rent,
                roomServiceCharge: v.serviceCharge,
                roomDiscount: roomDiscount,
                complementaryId: complementaryId,
                complementaryName: complementaryName,
                rent: v.rent,
                serviceCharge: v.serviceCharge,
                discount: roomDiscount,
                totalRent: v.totalRent,
                adult: 0,
                child: 0,
                extraBed: 0,
                extraBedCharge: 0,
                checkInTime: roomInTimeStr,
                checkOutTime: roomOutTimeStr,
                rawCheckInTime: roomInTime,
                rawCheckOutTime: roomOutTime,
                actualCheckInTime: roomInTimeStr,
                actualCheckOutTime: '',
                rawActualCheckInTime: roomInTime,
                rawActualCheckOutTime: null
            };

            model.rent = model.rent * days;
            model.serviceCharge = model.serviceCharge * days;
            model.discount = model.discount * days;
            model.totalRent = model.rent + model.serviceCharge - model.discount;

            const existedRoom = roomList.find(x => x.roomId == model.roomId);

            if (model.categoryId > 0 && model.roomId > 0) {

                if (existedRoom != null) {
                    return;
                } else {
                    roomList.push(model);
                }
            }
        })

        renderRoomTableBody();
    }
}

function bookingDays() {
    const roomInTime = $("#group-book-check-in-picker").datetimepicker('getValue');
    const roomOutTime = $("#group-book-check-out-picker").datetimepicker('getValue');

    const fromDate = new Date(roomInTime);
    fromDate.setHours(0, 0, 0, 0);

    const toDate = new Date(roomOutTime);
    toDate.setHours(0, 0, 0, 0);

    // Calculating difference in milliseconds
    const timeDifference = toDate - fromDate;

    // Converting milliseconds to days
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    return days;
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
    $("#net-total").val(sumResult);

    calculateNetTotal();
}

//#endregion