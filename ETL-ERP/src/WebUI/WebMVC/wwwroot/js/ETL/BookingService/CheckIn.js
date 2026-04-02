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

const IdentityTypeEnum = {
    Nid: 1,
    BirthCertificate: 2,
    Passport: 3
}
const CountryCodeEnum = {
    Bangladesh: 'bd'
}
$(document).ready(function () {
    loadRoomData();
    loadBookingGuestData();
    renderInitRoomTableBody();
    renderInitGuestTableBody();
    $("#clearDiv").hide();

    //#region for_GuestUpdateModal
    $('input[name="companyOption"]').on('change', function () {

        if ($('#E_existingCompany').is(':checked')) {
            $('#E_existingCompanySection').show();
            $('#E_newCompanySection').hide();
        }

        if ($('#E_newCompany').is(':checked')) {
            $('#E_existingCompanySection').hide();
            $('#E_newCompanySection').show();
        }

    });

    //#endregion

    //#region for_GuestEntryModal
    $('input[name="companyOption"]').on('change', function () {
        if ($('#existingCompany').is(':checked')) {
            $('#existingCompanySection').show();
            $('#newCompanySection').hide();
        } else if ($('#newCompany').is(':checked')) {
            $('#existingCompanySection').hide();
            $('#newCompanySection').show();
        }
    });
    //#endregion

    toggleIdentityField();

});

$(document.body).on("change", "#date-time-picker-check-from", function () {
    const actualCheckInTime = $("#date-time-picker-check-from").datetimepicker('getValue');

    $(".date-time-picker").datetimepicker({
        value: actualCheckInTime,
        startDate: new Date(actualCheckInTime)
    });
    $("#clearDiv").show();
    /*toggleIdentityField();*/
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
                            checkInTime: checkInDate,
                            checkOutTime: checkOutDate,
                            adult: v.adult,
                            child: v.child,
                            extraBed: v.extraBed,
                            extraBedCharge: v.extraBedCharge,
                            rawCheckInTime: v.checkInTime,
                            rawCheckOutTime: v.checkOutTime,
                            cleaningStatus: v.cleaningStatus,
                            cleaningStatusText: v.cleaningStatusText
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
        //loadRoomSelectData();
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
                console.log("room-info: ", rData);
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
        cleaningStatus: cleaningStatus,
        cleaningStatusText: cleaningStatus == CleaningStatus.VACANT_CLEAN ? 'Vacant & Clean' : 'N/A'
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

function renderInitRoomTableBody() {
    const CheckBox = `<td></td>`;
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
                            <small>Discount Per Room</small></br>
                            <input type='number' class='form-control p-1 mt-1' id='room-per-discount' value='0'>
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

    const row = `<tr>${CheckBox}${slNoCell}${checkInOutCell}${categoryCell}${roomCell}${costCell}${complementaryCell}${actionCell}</tr>`;

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

            let checkInTimeInputHtml = cleaningStatus == CleaningStatus.VACANT_CLEAN ? `<input class='form-control date-time-picker actual-check-in' name='BookingRoomVms[${i}].ActualCheckInTimeStr' type="text">`
                : `<input class='form-control' name='BookingRoomVms[${i}].ActualCheckInTimeStr' type="text" disabled>`;

            let extraBedHtml = ` <small>Extra Bed<small></br>
                                 <input type='number' class='form-control p-1 mt-1 extra-bed' name='BookingRoomVms[${i}].ExtraBed' value='${v.extraBed}' data-index='${i}' placeholder='ExtraBed'/>`;
            const CheckBox = `<td><input type="checkbox" class="select-row" data-index="${i}"></td>`;

            const slNoCell = `<td>${i + 1}
                                 <input type='hidden' name='BookingRoomVms[${i}].Id' value='${v.id}'/>   
                              </td>`;

            const checkInOutCell = `<td> 
                                <input type='hidden' name='BookingRoomVms[${i}].CheckInTimeStr' value='${v.checkInTime}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].CheckOutTimeStr' value='${v.checkOutTime}'/>
                                <b> Check In</b> <br/>
                                <span>${v.checkInTime}</span> <br/>
                                <b> Check Out</b> <br/>
                                <span>${v.checkOutTime}</span> <br/>
                                <b>Actual Check In</b> <br/>
                                ${checkInTimeInputHtml}
                            </td>`;

            const categoryCell = `<td> <input type='hidden' name='BookingRoomVms[${i}].RoomCategoryId' value='${v.categoryId}'/> ${v.categoryName} <br/>
                                    <div class='mt-2'>
                                        <input type='number' class='form-control p-1' name='BookingRoomVms[${i}].Adult' placeholder='Adult'/>
                                        <input type='number' class='form-control p-1 mt-1' name='BookingRoomVms[${i}].Child' placeholder='Children'/>
                                    </div>
                                  </td>`;

            const roomhiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].RoomRent' value='${v.roomRent}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomServiceCharge' value='${v.roomServiceCharge}'/>`;

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
                                    ${roomhiddenInput}
                                </div>
                            </td>`;

            const hiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].Rent' value='${v.rent}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].ServiceCharge' value='${v.serviceCharge}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].Discount' value='${v.discount}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].NetRent' value='${v.totalRent}'/>`;

            const costCell = `<td>
                                <b>Rent:</b> ${v.rent} <br/>
                                <b>Service Charge:</b> ${v.serviceCharge} <br/>
                                <b>Discount:</b> ${v.discount} <br/>
                                <b>Bed Charge:</b> <input type='number' id='bed_charge_${i}' name='BookingRoomVms[${i}].ExtraBedCharge' value='${v.extraBedCharge}' readonly/> <br/>
                                <b>Total Rent:</b> <input type='number' id='total_rent_${i}' name='BookingRoomVms[${i}].NetRent' value='${v.totalRent}' readonly/> <br/>
                                ${hiddenInput}
                              </td>`;

            const noComplement = `<td>No Complement</td>`;

            const withComplement = `<td> <input type='hidden' name='BookingRoomVms[${i}].ComplementaryId' value='${v.complementaryId}'/> ${v.complementaryName} </td>`;

            const complementaryCell = v.complementaryId > 0 ? withComplement : noComplement;

            const actionCell = `<td class='text-center'> <a class='mr-2 remove-room' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x text-danger"></i></a> </td>`;

            const row = `<tr style='font-size: smaller;'>${CheckBox}${slNoCell}${checkInOutCell}${categoryCell}${roomCell}${costCell}${complementaryCell}${actionCell}</tr>`;

            $("#RoomTableTbody").append(row);

        });

        $(".date-time-picker").datetimepicker({
            defaultTime: '12:00'
        });

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
                            address: v.address,
                            companyId: v.companyId,
                            companyName: v.companyName,
                            country: v.country,
                            countryId: v.countryId,
                            districtId: v.districtId,
                            dob: v.dob,
                            email: v.email,
                            gender: v.gender,
                            guestMobile: v.guestMobile,
                            identityNo: v.identityNo,
                            identityType: v.identityType,
                            isMain: v.isMain,
                            isVip: v.isVip
                        };

                        guestList.push(model);
                        console.log('guestList', rData)
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

//#region Guest Edit Section
function toggleIdentityField(code = '') {
    $('.passport').hide();
    if (!hasAnyError(code)) {
        /*code == CountryCodeEnum.Bangladesh ? $('#nid').show() : $('#passport').show();*/
        if (!(code == CountryCodeEnum.Bangladesh)) {
            $('.nid').hide()
            $('.passport').show()
        }
        else {
            $('.nid').show()
            $('.passport').hide()
        }
    }
}
$(document.body).on("change", "#CountryId, #E_CountryId", function () {
    var selectedCode = $(this).find('option:selected').data('code').toLowerCase();

    toggleIdentityField(selectedCode);
});

$('#guestEditModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#guestEditModal')
    });
});
$(document).on('click', '.edit-guest', function (e) {
    e.preventDefault();

    selectedGuestIndex = $(this).data('index');
    const guest = guestList[selectedGuestIndex];

    bindGuestToEditModal(guest);
});
function bindGuestToEditModal(guest) {
    let salutation = guest.guestName?.split(' ')[0] || '';
    let firstName = guest.guestName?.split(' ')[1] || '';
    let lastName = guest.guestName?.split(' ')[2] || '';

    let dob = guest.dob ? moment(guest.dob.split('T')[0]).format("DD/MM/YYYY") : null;
    $('#E_salutation').val(salutation);
    $('#E_first-name').val(firstName);
    $('#E_last-name').val(lastName);

    $('#E_address').val(guest.address || '');
    $('#E_Dob').val(dob || '');
    $('#E_mobile').val(guest.guestMobile || '');
    $('#E_Email').val(guest.email || '');
    $('#E_Gender').val(guest.gender).trigger(update);

    $('#E_DistrictId').val(guest.districtId).trigger(update);
    $('#E_CountryId').val(guest.countryId).trigger(update);
    toggleIdentityField($('#E_CountryId').find('option:selected').data('code').toLowerCase());

    //#region Identity Handling
    if (guest.identityType == IdentityTypeEnum.Nid) {
        $('#E_nid').val(guest.identityNo);
        $('#E_passport').val('');
    } else {
        $('#E_passport').val(guest.identityNo);
        $('#E_nid').val('');
    }
    //#endregion

    $('#E_vip').prop('checked', guest.isVip);

    $('#E_existingCompany').prop('checked', true).trigger('change');
    guest.companyId > 0 ? $('#E_GuestCompanyId').val(guest.companyId).trigger(update) : $('#E_GuestCompanyId').val('').trigger(update);

}
$(document.body).on("click", "#GuestEditBtn", function () {

    if (selectedGuestIndex === -1) return;

    const guest = guestList[selectedGuestIndex];

    var selectedCode = $('#E_CountryId option:selected').data('code').toLowerCase();

    const identityType = selectedCode == CountryCodeEnum.Bangladesh
        ? IdentityTypeEnum.Nid
        : IdentityTypeEnum.Passport;

    const identityNo = selectedCode == CountryCodeEnum.Bangladesh
        ? $("#E_nid").val()
        : $("#E_passport").val();

    const params = {
        id: guest.guestId,
        salutation: $("#E_salutation").val(),
        firstName: $("#E_first-name").val(),
        lastName: $("#E_last-name").val(),
        dobString: $("#E_Dob").val(),
        mobile: $("#E_mobile").val(),
        email: $("#E_Email").val(),
        gender: $("#E_Gender").val(),
        countryId: $("#E_CountryId").val(),
        districtId: $("#E_DistrictId").val(),
        identityType: identityType,
        identityNo: identityNo,
        address: $("#E_address").val(),
        companyId: $("#E_existingCompany").is(":checked") ? $("#E_GuestCompanyId").val() : null,
        companyName: $("#E_newCompany").is(":checked") ? $("#E_CompanyName").val() : null,
        companyMobile: $("#E_newCompany").is(":checked") ? $("#E_CompanyMobile").val() : null,
        isVip: $('#E_vip').is(':checked')
    };
    if (params.salutation != "" && params.firstName != "" && params.mobile != "") {
        $.post(API + "GuestInfo/GuestUpdate", params, function (isUpdated) {

            if (isUpdated) {
                successMsg("Guest Updated Successfully");
                $("#guestEditModal").modal('hide');

                loadBookingGuestData();
                //guestList[selectedGuestIndex] = { ...guestList[selectedGuestIndex], ...params };
            } else {
                failedMsg("Guest Update Failed");
            }

        }).fail(function (err) {
            failedMsg(err.responseText);
        });
    } else {
        failedMsg("Salutation, First Name, Mobile Number is required");
    }
});
//#endregion

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

            const actionCell = `<td class='text-center'> 
                                    <a class='mr-2 remove-guest' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x text-danger"></i></a> 
                                    <a class='mr-2 edit-guest' data-bs-toggle='modal' data-bs-target='#guestEditModal' href='#' data-index='${i}' title='Edit'><i class="fa fa-pencil fa-2x"></i></a> 
                                </td>`;

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

//#region Guest Entry
$('#guestEntryModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#guestEntryModal')
    });
});

$(document.body).on("click", "#GuestEntryBtn", function () {
    const salutation = $("#salutation").val();
    const firstName = $("#first-name").val();
    const lastName = $("#last-name").val();
    const address = $("#address").val();
    const DateOfBirth = $("#Dob").val();
    const mobile = $("#mobile").val();
    const email = $("#Email").val();
    const gender = $("#Gender").val();
    const countryId = $("#CountryId").val();
    const districtId = $("#DistrictId").val();
    const companyId = $("#existingCompany").is(":checked") ? $("#GuestCompanyId").val() : null;
    const companyName = $("#newCompany").is(":checked") ? $("#CompanyName").val() : null;
    const companyMobile = $("#newCompany").is(":checked") ? $("#CompanyMobile").val() : null;

    var selectedCode = $('#CountryId option:selected').data('code').toLowerCase();

    const identityType = selectedCode == CountryCodeEnum.Bangladesh.toLowerCase()
        ? IdentityTypeEnum.Nid
        : IdentityTypeEnum.Passport;

    const identityNo = selectedCode == CountryCodeEnum.Bangladesh.toLowerCase()
        ? $("#nid").val()
        : $("#passport").val();

    //const identityType = IdentityTypeEnum.Nid;
    const isVip = $('#vip').is(':checked');


    if (salutation != "" && firstName != "" && mobile != "") {
        const url = API + "GuestInfo/GuestEntry";

        const params = {
            salutation: salutation,
            firstName: firstName,
            lastName: lastName,
            dobString: DateOfBirth,
            mobile: mobile,
            email: email,
            companyId: companyId,
            companyName: companyName,
            companyMobile: companyMobile,
            gender: gender,
            countryId: countryId,
            districtId: districtId,
            identityType: identityType,
            identityNo: identityNo,
            address: address,
            isVip: isVip

        };

        $.post(url, params, function (rData) {
            if (rData > 0) {
                successMsg("Guest Entry Successful");
                $("#guestEntryModal").modal('hide');
                clearGuestForm();

                loadGuestSelectList(rData);
            } else {
                failedMsg("Guest Entry Failed");
            }
        }).fail(function (err) {
            failedMsg(err.responseText);
        })
    } else {
        failedMsg("Salutation, First Name, Mobile Number is required");
    }
})

//#endregion


$(document.body).on("click", "#CheckInSubmitBtn", function () {
    const bookingDate = $("#BookingDateStr").val();
    const checkInTime = $("#CheckInTimeStr").val();
    const checkOutTime = $("#CheckOutTimeStr").val();
    const actualCheckInTime = $("#ActualCheckInTimeStr").val();

    if (bookingDate != "" && checkInTime != "" && checkOutTime != "" && actualCheckInTime != "") {
        $("#CheckInForm").submit();
    }
});


//#region Clear All Actual CheckIn Time

$(document.body).on("click", "#clearCheckOut", function () {
    $(".actual-check-in").val("");
});
//#endregion

//#region Clear Specific Actual CheckIn Time
$(document).on('click', '#selectAll', function () {
    $('.select-row').prop('checked', $(this).is(':checked'));
});

$(document.body).on("click", "#otherCheckOutTimeClear", function () {
    $(".select-row").each(function () {
        if (!$(this).is(':checked')) {
            const rowIndex = $(this).data('index');
            console.log("Selected Row", rowIndex)
            const checkInOutCell = $(`#RoomTableTbody tr:eq(${rowIndex}) `);
            checkInOutCell.find(".actual-check-in").val("");
            //checkInOutCell.find("input[name*='CheckOutTimeStr']").val('');
        }
    });
    $(".select-row").prop('checked', false);
    $(".selectAll").prop('checked', false);
});

//#endregion
