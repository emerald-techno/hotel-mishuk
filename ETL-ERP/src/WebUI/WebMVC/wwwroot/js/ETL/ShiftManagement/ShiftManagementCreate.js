var shiftList = [];

$(document.body).on("change", "#SelectMonth", function () {
    

    const year = $("#SelectYear").val();
    const month = $(this).val();

    if (year > 0 && month > 0) {
        getExistingData(year, month);
    } else {
        shiftList = [];
        createShiftTable();
    }
});

function getExistingData(year, month) {
    if (year > 0 && month > 0) {
        shiftList = [];

        const url = `${API}ShiftManagement/GetExistShift?year=${year}&month=${month}`;

        $.get(url, function (rData) {
            if (rData != null && rData.length > 0) {
                rData.forEach(v => {
                    const model = {
                        id: v.id,
                        startDateStr: v.startDateStr,
                        endDateStr: v.endDateStr,
                        permanentShiftId: v.permanentShiftId,
                        permanentShiftName: v.permanentShiftName,
                        dutyShiftId: v.dutyShiftId,
                        dutyShiftName: v.dutyShiftName
                    }

                    shiftList.push(model);
                });

                createShiftTable();
            } else {
                createShiftTable();
            }
        })
    }
}

$(document.body).on("click", "#AddShiftBtn", function () {
    const startDate = $("#start-date").val();
    const endDate = $("#end-date").val();
    const permanentShiftName = $("#PermanentShiftId option:selected").text();
    const permanentShiftId = $("#PermanentShiftId").val();
    const dutyShiftName = $("#DutyShiftId option:selected").text();
    const dutyShiftId = $("#DutyShiftId").val();

    const model = {
        id: 0,
        startDateStr: startDate,
        endDateStr: endDate,
        permanentShiftId: permanentShiftId,
        permanentShiftName: permanentShiftName,
        dutyShiftId: dutyShiftId,
        dutyShiftName: dutyShiftName
    }

    if (!(model.permanentShiftId > 0)) {
        failedMsg("Please Select Permanent Shift to add Shift");
        return false;
    }

    if (!(model.dutyShiftId > 0)) {
        failedMsg("Please Select Duty Shift to add Shift");
        return false;
    }

    if (!(isInMonth(model.startDateStr))) {
        return failedMsg("Please Select Start Date In Month");
    }

    if (!(isInMonth(model.endDateStr))) {
        return failedMsg("Please Select End Date In Month");
    }

    if ((isToDateSmaller(model.startDateStr, model.endDateStr))) {
        return failedMsg("Please Select End Date Higher Than Start Date");
    }

    if (shiftList.length > 0) {
        var p_shift = shiftList.find(x => x.permanentShiftId == model.permanentShiftId);

        //let startDateExists = false;
        //let endDateExists = false;

        //for (let x of shiftList) {
        //    startDateExists = isDateInRange(model.startDateStr, x.startDateStr, x.endDateStr);
        //    if (startDateExists) {
        //        break;
        //    }

        //    endDateExists = isDateInRange(model.endDateStr, x.startDateStr, x.endDateStr);
        //    if (endDateExists) {
        //        break;
        //    }
        //}

        //if (startDateExists) {
        //    return failedMsg("Start Date Already Added");
        //}

        //if (endDateExists) {
        //    return failedMsg("End Date Already Added");
        //}

        if (p_shift != null || p_shift != undefined) {
            failedMsg("P-Shift Already Added");
        } else {
            shiftList.push(model);
        }
    } else {
        shiftList.push(model);
    }

    createShiftTable();
});

function createShiftTable() {
    $("#ShiftTableTbody").empty();

    if (shiftList.length > 0) {

        shiftList.forEach((modelObject, index) => {

            const hiddenInput = `<input type='hidden' name='ShiftManagementVms[${index}].Id' value='${modelObject.id}'/>`;

            const slNo = `<td>${index + 1}${hiddenInput}</td>`;
            const startDateCell = `<td><input type='hidden' name='ShiftManagementVms[${index}].StartDateStr' value='${modelObject.startDateStr}'/>${modelObject.startDateStr}</td>`;
            const endDateCell = `<td><input type='hidden' name='ShiftManagementVms[${index}].EndDateStr' value='${modelObject.endDateStr}' />${modelObject.endDateStr}</td>`;
            const pShiftCell = `<td><input type='hidden' name='ShiftManagementVms[${index}].PermanentShiftId' value='${modelObject.permanentShiftId}'"/>${modelObject.permanentShiftName}</td>`;
            const dShiftCell = `<td><input type='hidden' name='ShiftManagementVms[${index}].DutyShiftId' value='${modelObject.dutyShiftId}' />${modelObject.dutyShiftName}</td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index})'><i class='fa fa-trash'></i></button></td>`;


            const row = "<tr id=item" + index + ">" + slNo + startDateCell + endDateCell + pShiftCell + dShiftCell + actionCell + "</tr>";
            $("#ShiftTableTbody").append(row);
        });
    } else {
        addNoDataFoundFooterWithMsg("#ShiftTableTbody", "No Shift Added For Setup...!!");
    }
}

function deleteItemRow(index) {
    if (index > -1) {
        shiftList.splice(index, 1);
    }
    createShiftTable();
}

$(document.body).on("click", "#SetupSubmitBtn", function () {

    if (shiftList.length > 0) {

        let isValid = true;
        let msg = "";

        if (isValid) {
            startFormPosting("#ShiftSetupForm");
        } else {
            failedMsg(msg);
        }
    } else {
        failedMsg("You did not make any shift setup..!!");
    }
});

function isDateInRange(checkDate, fromDate, toDate) {
    // Convert input strings to Date objects
    checkDate = parseCustomDateString(checkDate);
    fromDate = parseCustomDateString(fromDate);
    toDate = parseCustomDateString(toDate);

    // Check if the date is within the range
    return checkDate >= fromDate && checkDate <= toDate;
}

function isInMonth(checkDate) {
    checkDate = parseCustomDateString(checkDate);
    const month = $("#SelectMonth").val();

    if (!(month > 0)) {
        failedMsg("Please Select Month First...!!");
        return false;
    }

    const checkMonth = checkDate.getMonth() + 1;
    if (checkMonth != month) {
        return false;
    }

    return true;
}

function isToDateSmaller(fromDateStr, toDateStr) {
    // Convert date strings to Date objects
    var fromDate = new Date(fromDateStr);
    var toDate = new Date(toDateStr);

    // Compare the dates
    return toDate < fromDate;
}


function parseCustomDateString(dateString) {
    // Split the date string into day, month, and year
    var dateParts = dateString.split('/');
    var day = parseInt(dateParts[0], 10);
    var month = parseInt(dateParts[1], 10) - 1; // Adjust month to zero-based index
    var year = parseInt(dateParts[2], 10);

    // Create a new Date object
    var parsedDate = new Date(year, month, day);

    return parsedDate;
}