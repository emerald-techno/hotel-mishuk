let hallList = [];

$(document).ready(function () {
    loadGuestSelectList();
    renderInitHallTableBody();
});

//#region GuestSection

function loadGuestSelectList(guestId) {

    const url = API + "GuestInfo/GetGuestJsonData";

    createSelectList(url, null, "#GuestId", null, function () {
        if (guestId > 0) {
            $("#GuestId").val(guestId).trigger("change");
        }
    });
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
}

//#endregion

//#region HallTable
function loadShiftSelectList() {

    const url = API + "BookingService/GetHallStatusJsonData";

    createSelectList(url, null, "#HallShift", null, null);
}

$(document.body).on("change", "#HallShift", function () {
    const bookingFromDateInputValue = $("#booking-date-time-picker").val();
    const bookingToDateInputValue = $("#booking-date-time-picker-to").val();

    if (bookingFromDateInputValue === "" || bookingToDateInputValue === "") {
        return failedMsg("Hall Booking Date Not Found... Please Select It First...!!");
    }

    const bookingFromDate = $("#booking-date-time-picker").datetimepicker('getValue');   
    const bookingToDate = $("#booking-date-time-picker-to").datetimepicker('getValue');   

    if (bookingFromDate == null || bookingToDate == null) {
        return failedMsg("Hall Booking Date Not Found... Please Select It First...!!");
    }

    const bookingFromDateStr = convertJsonFullDate(new Date(bookingFromDate));
    const bookingToDateStr = convertJsonFullDate(new Date(bookingToDate));

    const hallShift = $(this).val();

    $("#rent").val("");
    $("#service-charge").val("");
    $("#total-rent").val("");

    if (bookingFromDate != null && bookingToDate != null && hallShift > 0) {

        const url = `${API}BookingService/GetHallByDateRange?fromDateStr=${bookingFromDateStr}&toDateStr=${bookingToDateStr}&shift=${hallShift}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("hall-list: ", rData);

                bindDropdownList(rData, "#HallId", null);
            } else {
                console.log("No Hall Info Found...!");
            }
        })
    }
});

$(document.body).on("change", "#HallId", function () {

    const hallId = $(this).val();
    const hallShift = $("#HallShift option:selected").val();

    if (hallId > 0) {

        const url = `${API}HallInfo/GetHallInfoById/${hallId}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("hall-info: ", rData);
                $("#hall-rent").val(rData.rent);
                $("#hall-service-charge").val(rData.serviceCharge);

                var shiftCount = hallShift == 3 ? 2 : 1;

                let rentAmount = rData.rent * shiftCount;
                let serviceChargeAmount = rData.serviceCharge * shiftCount;

                $("#rent").val(rentAmount);
                $("#service-charge").val(serviceChargeAmount);

                let totalRent = parseFloat(rentAmount) + parseFloat(serviceChargeAmount);
                $("#total-rent").val(totalRent);

            } else {
                console.log("No Room Info Found...!");
            }
        })
    }
});

$(document.body).on("click", "#add-hall", function () {

    const hallShift = $("#HallShift option:selected").val();
    const hallShiftName = $("#HallShift option:selected").text();
    const hallId = $("#HallId option:selected").val();
    const hallName = $("#HallId option:selected").text();
    const rent = $("#rent").val();
    const sc = $("#service-charge").val();
    const totalRent = $("#total-rent").val();
    const remarks = $("#dtl_remarks").val();

    const bookingFromDate = $("#booking-date-time-picker").datetimepicker('getValue');
    const bookingToDate = $("#booking-date-time-picker-to").datetimepicker('getValue');    

    var datesInRange = getDateList(bookingFromDate, bookingToDate);

    console.log("dates:", datesInRange);

    if (datesInRange.length > 0) {
        datesInRange.forEach((v, i) => {

            const hallBookingDateStr = convertJsonFullDate(new Date(v));

            const model = {
                id: 0,
                bookingDate: v,
                bookingDateStr: hallBookingDateStr,
                hallShift: hallShift,
                hallShiftName: hallShiftName,
                hallId: hallId,
                hallName: hallName,
                rent: rent,
                serviceCharge: sc,
                totalRent: totalRent,
                remarks: remarks
            };

            console.log("booking-hall-info: ", model);

            if (model.bookingDate != null && model.hallShift > 0 && model.hallId > 0) {

                const existHall = hallList.find(x => x.bookingDateStr === model.bookingDateStr && x.hallShift === model.hallShift && x.hallId === model.hallId);

                /*const existHallIndex = hallList.findIndex(x => x.bookingDateStr === model.bookingDateStr && x.hallShift === model.hallShift && x.hallId === model.hallId);*/

                if (existHall != null) {
                    errorMsg("Hall Already Added For Same Date & Same Shift...!!");
                } else {
                    hallList.push(model);
                }
            }
        })

        renderHallTableBody();
    }
});

function calculateSum(array) {
    var sum = 0;

    $.each(array, function (index, value) {
        sum += parseFloat(value);
    });

    return sum;
}

function renderInitHallTableBody() {

    const slNoCell = `<td>#</td>`;

    const bookingDateCell = `<td> 
                                <b>Booked From</b> <br/>
                                <input class='form-control' id='booking-date-time-picker' type="text"> <br/>
                                <b>Booked To</b> <br/>
                                <input class='form-control' id='booking-date-time-picker-to' type="text"> <br/>
                            </td>`;

    const shiftCell = `<td> <select class='form-control dd-type' id='HallShift'></select> </td>`;

    const hallCell = `<td> 
                        <select class='form-control dd-type' id='HallId'></select>
                        <input type='hidden' id='hall-rent'/>
                        <input type='hidden' id='hall-serviece-charge'/>
                      </td>`;

    const rentGroup = `<div class='input-group input-group-sm mb-1'>
                            <span class='input-group-text'>Rent</span>
                            <input id="rent" class="form-control btn-square" type="number">
                       </div>`;

    const serviceChargeGroup = `<div class='input-group input-group-sm mb-1'>
                                    <span class='input-group-text'>S.C.</span>
                                    <input id="service-charge" class="form-control btn-square" type="number" readonly>
                               </div>`;

    const totalRentGroup = `<div class='input-group input-group-sm mb-1'>
                                <span class='input-group-text'>Total</span>
                                <input id="total-rent" class="form-control btn-square" type="number" readonly>
                           </div>`;

    const costCell = `<td>${rentGroup}${serviceChargeGroup}${totalRentGroup}</td>`;

    const remarksCell = `<td> <input id="dtl_remarks" class="form-control" type="text"> </td>`;

    const actionCell = `<td class='text-center'> <a class='btn btn-square btn-primary btn-xs' href='#' id='add-hall' title='Add'><i class="fa fa-plus"></i> Add </a> </td>`;

    const row = `<tr>${slNoCell}${bookingDateCell}${shiftCell}${hallCell}${costCell}${remarksCell}${actionCell}</tr>`;

    $("#HallTableTbody").append(row);

    $(".dd-type").select2({ width: "100%" }).on("change", function (e) {
        $(this).valid();
    });

    $("#booking-date-time-picker").datetimepicker({
        timepicker: false,
        format: 'd/m/Y',
    });

    $("#booking-date-time-picker-to").datetimepicker({
        timepicker: false,
        format: 'd/m/Y',
    });

    loadShiftSelectList();
}

function renderHallTableBody() {

    $("#HallTableTbody").empty();

    if (hallList.length > 0) {

        hallList.forEach((v, i) => {

            const slNoCell = `<td>${i + 1}</td>`;           

            const bookingDateCell = `<td> 
                                        <input type='hidden' name='BookingHallVms[${i}].BookingDateStr' value='${v.bookingDateStr}'/>
                                        <b>Hall Booking Date</b><br/>
                                        ${v.bookingDateStr}<br/>
                                    </td>`;

            const shiftCell = `<td> <input type='hidden' name='BookingHallVms[${i}].HallShift' value='${v.hallShift}'/> ${v.hallShiftName} </td>`;

            const hallCell = `<td> <input type='hidden' name='BookingHallVms[${i}].HallId' value='${v.hallId}'/> ${v.hallName} </td>`;

            const hiddenInput = `<input type='hidden' name='BookingHallVms[${i}].Rent' value='${v.rent}'/>
                                <input type='hidden' name='BookingHallVms[${i}].ServiceCharge' value='${v.serviceCharge}'/>
                                <input type='hidden' name='BookingHallVms[${i}].NetRent' value='${v.totalRent}'/>`;

            const costCell = `<td>
                                <b>Rent:</b> ${v.rent} <br/>
                                <b>Service Charge:</b> ${v.serviceCharge} <br/>
                                <b>Total Rent:</b> ${v.totalRent} <br/>
                                ${hiddenInput}
                              </td>`;

            const remarksCell = `<td> <input type='hidden' name='BookingHallVms[${i}].Remarks' value='${v.remarks}'/> ${v.remarks} </td>`;

            const actionCell = `<td class='text-center'> <a class='btn btn-square btn-primary btn-xs mr-2 remove-hall' href='#' data-index='${i}' title='Delete'><i class="fa fa-times fa-2x"></i> Remove </a> </td>`;

            const row = `<tr>${slNoCell}${bookingDateCell}${shiftCell}${hallCell}${costCell}${remarksCell}${actionCell}</tr>`;

            $("#HallTableTbody").append(row);

        });

        const sumResult = calculateSum(hallList.map(x => x.totalRent));
        $("#net-total").val(sumResult);

        renderInitHallTableBody();

    } else {
        renderInitHallTableBody();
    }
}

$(document.body).on("change", "#rent", function () {
    const rent = parseFloat($(this).val());
    const sc = parseFloat($("#service-charge").val());

    if (rent > -1) {
        const totalRent = rent + sc;
        $("#total-rent").val(totalRent);
    }
});

$(document.body).on("click", ".remove-hall", function () {
    const hallIndex = $(this).attr("data-index");

    if (hallIndex > -1) {
        hallList.splice(hallIndex, 1);
    };

    renderHallTableBody();
});

//#endregion

$(document.body).on("click", "#BookingSubmitBtn", function () {
    const bookingDate = $("#BookingDateStr").val();
    const guestId = $("#GuestId").val();

    if (!(hallList.length > 0)) {
        return failedMsg("No Hall Found..!!");
    }

    if (!(guestId > 0)) {
        return failedMsg("No Guest Found..!!");
    }

    if (bookingDate != "" && guestId > 0) {
        $("#HallBookingForm").submit();
    }
});

//#region HallTable



//#endregion


function getDateList(startDate, endDate) {
    // Create an array to store the dates
    var dateList = [];

    // Copy the start date
    var currentDate = new Date(startDate);

    // Iterate through dates until reaching the end date
    while (currentDate <= endDate) {
        // Push the current date into the array
        dateList.push(new Date(currentDate));

        // Move to the next day
        currentDate.setDate(currentDate.getDate() + 1);
    }

    // Return the array of dates
    return dateList;
}