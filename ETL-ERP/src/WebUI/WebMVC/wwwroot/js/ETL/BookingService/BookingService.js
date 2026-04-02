let slNo = 0;
let roomList = [];
let guestList = [];
let roomCategoryList = [];

const IdentityTypeEnum = {
    Nid: 1,
    BirthCertificate: 2,
    Passport: 3
}
$(document).ready(function () {
    renderInitRoomTableBody();
    renderInitGuestTableBody();

    $('input[name="companyOption"]').on('change', function () {
        if ($('#existingCompany').is(':checked')) {
            $('#existingCompanySection').show();
            $('#newCompanySection').hide();
        } else if ($('#newCompany').is(':checked')) {
            $('#existingCompanySection').hide();
            $('#newCompanySection').show();
        }
    });

    toggleIdentityField();
});

$(document.body).on("change", "#date-time-picker-from", function () {
    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

    //if (checkInTime != null && checkInTime != undefined && checkOutTime != null && checkOutTime != undefined) {
    //    if (checkOutTime < checkInTime) {
    //        return errorMsg("C/In time is higher than C/Out time...!!");
    //    }
    //}

    if (checkInTime != null) {
        $("#date-time-picker-in").datetimepicker({
            value: checkInTime,
            startDate: new Date(checkInTime)
        });

        serviceDateDifference();
        loadRoomData();
    }
})

$(document.body).on("change", "#date-time-picker-to", function () {
    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

    if (checkInTime != null && checkInTime != undefined && checkOutTime != null && checkOutTime != undefined) {
        if (checkOutTime < checkInTime) {
            return errorMsg("C/In time is higher than C/Out time...!!");
        }
    }

    if (checkOutTime != null) {
        $("#date-time-picker-out").datetimepicker({
            value: checkOutTime,
            startDate: new Date(checkOutTime)
        });

        serviceDateDifference();
        loadRoomData();
    }
})

//function serviceDateDifference() {
//    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
//    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

//    if (checkInTime == null || checkOutTime == null) {
//        return;
//    }

//    const fromDate = new Date(checkInTime);
//    const toDate = new Date(checkOutTime);

//    const timeDifference = toDate - fromDate;

//    const seconds = Math.floor(timeDifference / 1000);
//    const minutes = Math.floor(seconds / 60);
//    const hours = Math.floor(minutes / 60);
//    const days = Math.floor(hours / 24);

//    console.log("Time difference in days:", days);

//    $("#days").val(days);
//}


function serviceDateDifference() {
    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

    if (checkInTime == null || checkOutTime == null) {
        return;
    }

    const fromDate = new Date(checkInTime);
    fromDate.setHours(0, 0, 0, 0);

    const toDate = new Date(checkOutTime);
    toDate.setHours(0, 0, 0, 0);

    // Calculating difference in milliseconds
    const timeDifference = toDate - fromDate;

    // Converting milliseconds to days
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    console.log("Time difference in days:", days);

    $("#days").val(days);
}

//#region Room Table Section

$(document.body).on("change", "#CategoryId", function () {
    //loadRoomData();
    categoryWiseRoomData();
});

function loadRoomData() {
    const categoryId = $("#CategoryId").val();

    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

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

$(document.body).on("change", "#RoomId", function () {

    const roomId = $(this).val();

    if (roomId > 0) {

        const url = `${API}RoomInfo/GetRoomInfoById/${roomId}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("room-info: ", rData);
                $("#room-rent").val(rData.rent);
                $("#room-service-charge").val(rData.serviceCharge);

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

$(document.body).on("change", "#date-time-picker-in", function () {
    const days = roomDateDifference();
    const roomRent = $("#room-rent").val();
    const serviceCharge = $("#room-service-charge").val();

    let rentAmount = roomRent * days;
    let serviceChargeAmount = serviceCharge * days;

    $("#rent").val(rentAmount);
    $("#service-charge").val(serviceChargeAmount);

    let totalRent = parseFloat(rentAmount) + parseFloat(serviceChargeAmount);
    $("#total-rent").val(totalRent);
})

$(document.body).on("change", "#date-time-picker-out", function () {
    const days = roomDateDifference();
    const roomRent = $("#room-rent").val();
    const serviceCharge = $("#room-service-charge").val();

    let rentAmount = roomRent * days;
    let serviceChargeAmount = serviceCharge * days;

    $("#rent").val(rentAmount);
    $("#service-charge").val(serviceChargeAmount);

    let totalRent = parseFloat(rentAmount) + parseFloat(serviceChargeAmount);
    $("#total-rent").val(totalRent);
})

$(document.body).on("change", "#room-per-discount", function () {
    const roomPerDiscount = $(this).val();

    if (roomPerDiscount == "" || roomPerDiscount == null) {
        $(this).val(0);
    }

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

    //const roomInTimeStr = convertJsonDateForViewWithTime(new Date(roomInTime));
    //const roomOutTimeStr = convertJsonDateForViewWithTime(new Date(roomOutTime));

    const roomInTimeStr = convertJsonFullDate(new Date(roomInTime));
    const roomOutTimeStr = convertJsonFullDate(new Date(roomOutTime));

    const model = {
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
        checkInTime: roomInTimeStr,
        checkOutTime: roomOutTimeStr
    };

    if (model.discount == "" || model.discount == null) {
        model.discount = 0;
    }

    console.log("room-info: ", model);

    const existedRoom = roomList.find(x => x.roomId == model.roomId);

    if (model.categoryId > 0 && model.roomId > 0) {

        if (existedRoom != null) {
            return failedMsg("Room Already Added..!");
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

    const actionCell = `<td class='text-center'> <a class='btn btn-square btn-primary btn-xs' href='#' id='add-room' title='Add'><i class="fa fa-plus"></i> Add </a> </td>`;

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
            startDate: new Date(checkInTime)
        });
    } else {
        $("#date-time-picker-in").datetimepicker();
    }

    if (checkOutTime != null) {
        $("#date-time-picker-out").datetimepicker({
            value: checkOutTime,
            startDate: new Date(checkOutTime)
        });
    } else {
        $("#date-time-picker-out").datetimepicker();
    }

    loadCategorySelectList();
    loadComplementarySelectList();
}

function renderRoomTableBody() {

    $("#RoomTableTbody").empty();

    if (roomList.length > 0) {

        roomList.forEach((v, i) => {

            const slNoCell = `<td>${i + 1}</td>`;

            const checkInOutCell = `<td> 
                                <input type='hidden' name='BookingRoomVms[${i}].CheckInTimeStr' value='${v.checkInTime}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].CheckOutTimeStr' value='${v.checkOutTime}'/>
                                <b>Booking Check In</b><br/>
                                ${v.checkInTime}<br/>
                                <b>Booking Check Out</b><br/>
                                ${v.checkOutTime}
                            </td>`;

            const categoryCell = `<td> <input type='hidden' name='BookingRoomVms[${i}].RoomCategoryId' value='${v.categoryId}'/> ${v.categoryName} </td>`;

            const roomhiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].RoomRent' value='${v.roomRent}'/>
                                    <input type='hidden' name='BookingRoomVms[${i}].RoomServiceCharge' value='${v.roomServiceCharge}'/>`;

            const roomCell = `<td> <input type='hidden' name='BookingRoomVms[${i}].RoomId' value='${v.roomId}'/> ${v.roomName}
                                ${roomhiddenInput}
                              </td>`;

            const hiddenInput = `<input type='hidden' name='BookingRoomVms[${i}].Rent' value='${v.rent}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].ServiceCharge' value='${v.serviceCharge}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].Discount' value='${v.discount}'/>
                                <input type='hidden' name='BookingRoomVms[${i}].NetRent' value='${v.totalRent}'/>`;

            const costCell = `<td>
                                <b>Rent:</b> ${v.rent} <br/>
                                <b>Service Charge:</b> ${v.serviceCharge} <br/>
                                <b>Discount:</b> ${v.discount} <br/>
                                <b>Total Rent:</b> ${v.totalRent} <br/>
                                ${hiddenInput}
                              </td>`;

            const complementaryCell = `<td> <input type='hidden' name='BookingRoomVms[${i}].ComplementaryId' value='${v.complementaryId}'/> ${v.complementaryName} </td>`;

            const actionCell = `<td class='text-center'> <a class='btn btn-square btn-primary btn-xs mr-2 remove-room' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x"></i> Remove </a> </td>`;

            const row = `<tr>${slNoCell}${checkInOutCell}${categoryCell}${roomCell}${costCell}${complementaryCell}${actionCell}</tr>`;

            $("#RoomTableTbody").append(row);

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

$(document.body).on("click", ".remove-room", function (e) {
    e.preventDefault();
    const roomIndex = $(this).attr("data-index");

    if (roomIndex > -1) {
        roomList.splice(roomIndex, 1);
    };

    renderRoomTableBody();
});

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

    // Calculating difference in milliseconds
    const timeDifference = toDate - fromDate;

    // Converting milliseconds to days
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    $("#days").val(days);

    return days;
}

//#endregion

//#region Guest Table Section
$('#guestEntryModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#guestEntryModal')
    });
});

function loadGuestSelectList(guestId) {

    const url = API + "GuestInfo/GetGuestJsonData";

    createSelectList(url, null, "#GuestId", null, function () {
        if (guestId > 0) {
            $("#GuestId").val(guestId).trigger("change");
        }
    });
}

const CountryCodeEnum = {
    Bangladesh: 'bd'
}

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
$(document.body).on("change", "#CountryId", function () {
    var selectedCode = $('#CountryId option:selected').data('code').toLowerCase();
    toggleIdentityField(selectedCode);
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

function clearGuestForm() {
    $("#salutation").val("");
    $("#first-name").val("");
    $("#last-name").val("");
    $("#mobile").val("");
    $("#CompanyName").val("");
    $("#CompanyMobile").val("");
    $("#GuestCompanyId").val("").trigger(update);
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

    const mainCell = `<td><input type='checkbox' class='text-center' id='IsMain'/></td>`;

    const actionCell = `<td class='text-center'> <a class='btn btn-square btn-primary btn-xs mr-2' href='#' id='add-guest' title='Add'><i class="fa fa-plus"></i> Add </a> </td>`;

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

            const slNoCell = `<td>${i + 1}</td>`;

            const guestCell = `<td> <input type='hidden' name='BookingGuestVms[${i}].GuestId' value='${v.guestId}'/> ${v.guestName} </td>`;

            const mainCell = `<td> <input type='hidden' name='BookingGuestVms[${i}].IsMain' value='${v.isMain}'/> ${v.isMain ? 'Yes' : 'No'}</td>`;

            const actionCell = `<td class='text-center'> <a class='mr-2 remove-guest' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x"></i></a> </td>`;

            const row = `<tr>${slNoCell}${guestCell}${mainCell}${actionCell}</tr>`;

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

$(document.body).on("click", "#BookingSubmitBtn", function () {
    const bookingDate = $("#BookingDateStr").val();
    const checkInTime = $("#CheckInTimeStr").val();
    const checkOutTime = $("#CheckOutTimeStr").val();

    if (!(roomList.length > 0)) {
        return failedMsg("No Room Found..!!");
    }

    if (!(guestList.length > 0)) {
        return failedMsg("No Guest Found..!!");
    }

    if (bookingDate != "" && checkInTime != "" && checkOutTime != "") {
        $("#BookingForm").submit();
    }
});

//#region Online Booking Section

$(document.body).on("change", "#OnlineBookingId", function () {

    $("#OnlineBookDataSec").html('');
    const onlineBookingId = $(this).val();

    if (onlineBookingId > 0) {

        const url = `${API}OnlineBooking/GetOnlineBookingById/${onlineBookingId}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("online-booking-info: ", rData);

                if (rData.onlineBookingDetails != null) {



                    rData.onlineBookingDetails.forEach(detail => {
                        const roomCategoryName = detail.roomCategoryName;
                        const model = {
                            categoryName: roomCategoryName
                        };
                        roomCategoryList.push(roomCategoryName);
                    });
                }

                console.log("Room Category List: ", roomCategoryList);
                renderBookingCard(rData);
            } else {
                console.log("No booking Info Found...!");
                $("#OnlineBookDataSec").html('');
            }
        })
    }
});

function renderBookingCard(bookingData) {
    if (bookingData != null) {

        let arrivalDate = convertJsonFullDateForView(new Date(bookingData.arrivalDate));
        let departureDate = convertJsonFullDateForView(new Date(bookingData.departureDate));

        let html = `<div class="row">
                        <ul>
                            <li><b>Guest Name:&nbsp;&nbsp;&nbsp;</b> ${bookingData.guestName}</li>
                            <li><b>Guest Mobile:&nbsp;&nbsp;&nbsp;</b> ${bookingData.guestMobile}</li>
                            <li><b>Room Type:&nbsp;&nbsp;&nbsp;</b> ${roomCategoryList.roomCategoryName}</li>
                            <li><b>Room Taken:&nbsp;&nbsp;&nbsp;</b> ${bookingData.roomCount}</li>
                            <li><b>Arrival:&nbsp;&nbsp;&nbsp;</b> ${arrivalDate}</li>
                            <li><b>Departure:&nbsp;&nbsp;&nbsp;</b> ${departureDate}</li>
                        </ul>
                    </div>`;

        $("#OnlineBookDataSec").html(html);
    } else {
        $("#OnlineBookDataSec").html('');
    }
}

//#endregion

//#region Multiple Room Selection

let modalRoomList = [];
let selectedRoomList = [];

$(document.body).on("click", "#group_book_btn", function () {
    modalRoomList = [];
    $("#room-section").empty();
    loadModalRoomData();
});

function loadModalRoomData() {

    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

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

            const roomInTime = $("#date-time-picker-in").datetimepicker('getValue');
            const roomOutTime = $("#date-time-picker-out").datetimepicker('getValue');

            const roomInTimeStr = convertJsonFullDate(new Date(roomInTime));
            const roomOutTimeStr = convertJsonFullDate(new Date(roomOutTime));

            const days = bookingDays();

            const model = {
                categoryId: v.categoryId,
                categoryName: v.categoryName,
                roomId: v.id,
                roomName: v.roomNo,
                roomRent: v.rent,
                roomServiceCharge: v.serviceCharge,
                complementaryId: complementaryId,
                complementaryName: complementaryName,
                rent: v.rent,
                serviceCharge: v.serviceCharge,
                discount: roomDiscount,
                totalRent: v.totalRent,
                checkInTime: roomInTimeStr,
                checkOutTime: roomOutTimeStr
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


//function bookingDays() {
//    const roomInTime = $("#date-time-picker-in").datetimepicker('getValue');
//    const roomOutTime = $("#date-time-picker-out").datetimepicker('getValue');

//    const fromDate = new Date(roomInTime);
//    const toDate = new Date(roomOutTime);

//    const timeDifference = toDate - fromDate;

//    const seconds = Math.floor(timeDifference / 1000);
//    const minutes = Math.floor(seconds / 60);
//    const hours = Math.floor(minutes / 60);
//    const days = Math.floor(hours / 24);

//    return days;
//}

function bookingDays() {
    const roomInTime = $("#date-time-picker-in").datetimepicker('getValue');
    const roomOutTime = $("#date-time-picker-out").datetimepicker('getValue');

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


//#region Forecast_Report
$(document.body).on("click", "#ShowReportBtn", function () {
    loadReport();
});
function loadReport() {
    const today = new Date();
    const sevenDaysLater = new Date();
    sevenDaysLater.setDate(today.getDate() + 30);

    const formatDate = (date) => {
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
    };

    const fromDate = formatDate(today);
    const toDate = formatDate(sevenDaysLater);


    const url = API + "BookingService/CategoryWiseRoomAvailableReport";

    $.post(url, {
        model: {
            startDateStr: fromDate,
            endDateStr: toDate
        }
    }, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}


function generateReportTable(data) {
    if (data.length > 0) {
        $("#report-section").empty();
        $("#report-section").append(data);

    } else {
        $("#report-section").html("<h5 style='text-align:center'>No Data Found<h5>");
    }

}

//#endregion