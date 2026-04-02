let slNo = 0;
let roomList = [];
let guestList = [];

const extraBedCharge = 500;

$(document).ready(function () {
    loadRoomData();
    loadBookingGuestData();
    renderInitRoomTableBody();
    renderInitGuestTableBody();
    
});

$(document.body).on("change", "#check-in-time", function () {
    const checkInTime = $("#check-in-time").datetimepicker('getValue');

    $(".room-in-time").datetimepicker({
        value: checkInTime,
        startDate: new Date(checkInTime),
        defaultTime: '12:00'
    });

    if (roomList.length > 0) {
        roomList.forEach((v, i) => {
            calculateDateWiseCost(i);
        });
    }
})

$(document.body).on("change", "#check-out-time", function () {
    const checkOutTime = $("#check-out-time").datetimepicker('getValue');

    $(".room-out-time").datetimepicker({
        value: checkOutTime,
        startDate: new Date(checkOutTime),
        defaultTime: '12:00'
    });

    if (roomList.length > 0) {
        roomList.forEach((v, i) => {
            calculateDateWiseCost(i);
        });
    }
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
                            rawCheckOutTime: v.checkOutTime
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
    categoryWiseRoomData();
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
    const checkInTime = $("#check-in-time").datetimepicker('getValue');
    const checkOutTime = $("#check-out-time").datetimepicker('getValue');
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

$(document.body).on("change", "#RoomId", function () {

    const roomId = $(this).val();

    if (roomId > 0) {

        const url = `${API}RoomInfo/GetRoomInfoById/${roomId}`;
        $.get(url, function (rData) {
            if (rData) {
                $("#room-rent").val(rData.rent);
                $("#room-service-charge").val(rData.serviceCharge);

                const days = roomDateDifference();
                if (days > 0) {
                    let rentAmount = rData.rent * days;
                    let serviceChargeAmount = rData.serviceCharge * days;

                    $("#rent").val(rentAmount);
                    $("#service-charge").val(serviceChargeAmount);

                    let totalRent = parseFloat(rentAmount) + parseFloat(rData.serviceCharge);
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
        rawCheckOutTime: roomOutTime
    };

    if (model.discount == "" || model.discount == null) {
        model.discount = 0;
    }

    console.log("room-info: ", model);

    if (model.categoryId > 0 && model.roomId > 0) {

        const existRoom = roomList.find(x => x.roomId == model.roomId);

        if (existRoom != null) {
            errorMsg("Room Already Added...!!");
        } else {
            roomList.push(model);
        }
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

$(document.body).on("change", ".room-in-time", function () {
    const index = $(this).attr("data-index");

    calculateDateWiseCost(index);
});

$(document.body).on("change", ".room-out-time", function () {
    const index = $(this).attr("data-index");

    calculateDateWiseCost(index);
});

function calculateDateWiseCost(index) {
    if (index > -1) {
        const roomInTime = $(`#date-time-picker-in_${index}`).datetimepicker('getValue');
        const roomOutTime = $(`#date-time-picker-out_${index}`).datetimepicker('getValue');

        const roomInTimeStr = convertJsonFullDate(new Date(roomInTime));
        const roomOutTimeStr = convertJsonFullDate(new Date(roomOutTime));

        let days = dateDifference(roomInTime, roomOutTime);
        $(`#days_${index}`).val(days);

        let roomObj = roomList[index];

        let roomRent = roomObj.roomRent * days;
        let serviceCharge = roomObj.roomServiceCharge * days;
        let discount = roomObj.roomDiscount * days;

        let netRent = roomRent + serviceCharge - discount;

        roomList[index].rent = roomRent;
        roomList[index].serviceCharge = serviceCharge;
        roomList[index].discount = discount;
        roomList[index].totalRent = netRent;

        roomList[index].checkInTime = roomInTimeStr;
        roomList[index].checkOutTime = roomOutTimeStr;
        roomList[index].rawCheckInTime = roomInTime;
        roomList[index].rawCheckOutTime = roomOutTime;

        $(`#rent_${index}`).val(roomRent);
        $(`#service-charge_${index}`).val(serviceCharge);
        $(`#room_discount_${index}`).val(discount);
        $(`#total-rent_${index}`).val(netRent);

        const sumResult = calculateSum(roomList.map(x => x.totalRent));
        $("#net-total").val(sumResult);
    }
}

function renderInitRoomTableBody() {

    const slNoCell = `<td>#</td>`;

    const categoryCell = `<td> <select class='form-control dd-type' id='CategoryId'></select> </td>`;

    const roomCell = `<td>
                        <select class='form-control dd-type' id='RoomId'></select>
                        <input id="room-rent" class="form-control btn-square" type="hidden">
                        <input id="room-service-charge" class="form-control btn-square" type="hidden">
                        <small>Discount Per Room<small></br>
                        <input type='number' class='form-control p-1 mt-1' id='room-per-discount' value='0' />
                      </td>`;

    const checkInOutCell = `<td> 
                                <input class='form-control' id='date-time-picker-in' type="text">
                                <input class='form-control mt-2' id='date-time-picker-out' type="text">
                                <div class='d-flex mt-2'>
                                    <span style='align-self:center;margin-right:5px;'> Days </span>
                                    <input class='form-control' id='days' type="number" value='0' readonly> 
                                </div>
                            </td>`;

    const rentGroup = `<div class='input-group input-group-sm mb-1'>
                            <span class='input-group-text'>Rent</span>
                            <input id="rent" class="form-control btn-square" type="number" readonly>
                       </div>`;

    const serviceChargeGroup = `<div class='input-group input-group-sm mb-1'>
                                    <span class='input-group-text'>S.C.</span>
                                    <input id="service-charge" class="form-control btn-square" type="number" readonly>
                               </div>`;

    const discountGroup = `<div class='input-group input-group-sm mb-1 mt-1'>
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

            const slNoCell = `<td>${i + 1}
                                 <input type='hidden' name='BookingRoomVms[${i}].Id' value='${v.id}'/>   
                              </td>`;

            const checkInOutCell = `<td> 
                                <input class='form-control room-in-time' name='BookingRoomVms[${i}].CheckInTimeStr' data-index='${i}' id='date-time-picker-in_${i}' type="text">
                                <input class='form-control room-out-time mt-2' name='BookingRoomVms[${i}].CheckOutTimeStr' data-index='${i}' id='date-time-picker-out_${i}' type="text">
                                <div class='d-flex mt-2'>
                                    <span style='align-self:center;margin-right:5px;'> Days </span>
                                    <input class='form-control' id='days_${i}' type="number" value='0' readonly>
                                </div>
                            </td>`;

            const categoryCell = `<td> <input type='hidden' name='BookingRoomVms[${i}].RoomCategoryId' value='${v.categoryId}'/> ${v.categoryName} </td>`;

            const roomhiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].RoomRent' value='${v.roomRent}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomServiceCharge' value='${v.roomServiceCharge}'/>`;

            const roomCell = `<td> 
                                <input type='hidden' name='BookingRoomVms[${i}].RoomId' value='${v.roomId}'/> <b> ${v.roomName} </b> <br/>
                                <b>Rent:</b> ${v.roomRent} <br/>
                                <b>Service Charge:</b> ${v.roomServiceCharge} <br/>
                                <b>Discount Per Room:</b> ${v.roomDiscount} <br/>
                                ${roomhiddenInput}
                            </td>`;

            const rentGroup = `<div class='input-group input-group-sm mb-1'>
                            <span class='input-group-text'>Rent</span>
                            <input id='rent_${i}' name='BookingRoomVms[${i}].Rent' value='${v.rent}' class="form-control btn-square" type="number" readonly>
                       </div>`;

            const serviceChargeGroup = `<div class='input-group input-group-sm mb-1'>
                                    <span class='input-group-text'>S.C.</span>
                                    <input id="service-charge_${i}" name='BookingRoomVms[${i}].ServiceCharge' value='${v.serviceCharge}' class="form-control btn-square" type="number" readonly>
                               </div>`;

            const discountGroup = `<div class='input-group input-group-sm mb-1'>
                                    <span class='input-group-text'>Discount</span>
                                    <input id="room_discount_${i}" name='BookingRoomVms[${i}].Discount' value='${v.discount}' class="form-control btn-square" type="number" readonly>
                               </div>`;

            const totalRentGroup = `<div class='input-group input-group-sm mb-1'>
                                <span class='input-group-text'>Total</span>
                                <input id="total-rent_${i}" name='BookingRoomVms[${i}].NetRent' value='${v.totalRent}' class="form-control btn-square" type="number" readonly>
                           </div>`;

            const costCell = `<td>${rentGroup}${serviceChargeGroup}${discountGroup}${totalRentGroup}</td>`;

            const noComplement = `<td>No Complement</td>`;

            const withComplement = `<td> <input type='hidden' name='BookingRoomVms[${i}].ComplementaryId' value='${v.complementaryId}'/> ${v.complementaryName} </td>`;

            const complementaryCell = v.complementaryId > 0 ? withComplement : noComplement;

            const actionCell = `<td class='text-center'> <a class='mr-2 remove-room' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x text-danger"></i></a> </td>`;

            const row = `<tr style='font-size: smaller;'>${slNoCell}${checkInOutCell}${categoryCell}${roomCell}${costCell}${complementaryCell}${actionCell}</tr>`;

            $("#RoomTableTbody").append(row);

            console.log("check-in-time", v.rawCheckInTime);
            console.log("check-out-time", v.rawCheckOutTime);

            if (v.rawCheckInTime != null) {
                $(`#date-time-picker-in_${i}`).datetimepicker({
                    value: v.rawCheckInTime,
                    startDate: new Date(v.rawCheckInTime),
                    defaultTime: '12:00'
                });
            } else {
                $(`#date-time-picker-in_${i}`).datetimepicker({
                    defaultTime: '12:00'
                });
            }

            if (v.rawCheckOutTime != null) {
                $(`#date-time-picker-out_${i}`).datetimepicker({
                    value: v.rawCheckOutTime,
                    startDate: new Date(v.rawCheckOutTime),
                    defaultTime: '12:00'
                });
            } else {
                $(`#date-time-picker-out_${i}`).datetimepicker({
                    defaultTime: '12:00'
                });
            }

            let days = dateDifference(v.rawCheckInTime, v.rawCheckOutTime);
            $(`#days_${i}`).val(days);
        });

        $(".date-time-picker").datetimepicker();

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

$(document.body).on("click", ".remove-room", function (e) {
    e.preventDefault();
    const roomIndex = $(this).attr("data-index");

    if (roomIndex > -1) {
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

        const totalRent = parseFloat(roomData.rent) + parseFloat(roomData.serviceCharge) + parseFloat(bedCharge);
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
    fromDate.setHours(0, 0, 0, 0);

    const toDate = new Date(endDate);
    toDate.setHours(0, 0, 0, 0);

    // Calculating difference in milliseconds
    const timeDifference = toDate - fromDate;

    // Converting milliseconds to days
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    console.log("Time difference in days for room:", days);

    return days;
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

$(document.body).on("click", "#UpdateSubmitBtn", function () {
    const bookingDate = $("#booking-date").val();
    const checkInTime = $("#check-in-time").val();
    const checkOutTime = $("#check-out-time").val();

    if (bookingDate != "" && checkInTime != "" && checkOutTime != "") {
        $("#UpdateBookingForm").submit();
    }
});

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
                rawActualCheckOutTime: ''
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

