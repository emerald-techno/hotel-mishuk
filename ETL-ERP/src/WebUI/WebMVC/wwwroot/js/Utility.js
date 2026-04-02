

$(document).ready(function () {
    //successMsg();
    //#region DataTable-------------
    //$("#searchTable, .search-table").DataTable({
    //    "aLengthMenu": DataTable.lengthMenu,
    //    "iDisplayLength": DataTable.displayLength,
    //});

    //$(".search-table").DataTable({
    //    "aLengthMenu": DataTable.lengthMenu,
    //    "iDisplayLength": DataTable.displayLength,
    //});
    //#endregion


    //Dropdown Select Input Type Js-------------
    $(".dd-type").select2({ width: "100%" }).on("change", function (e) {
        $(this).valid();
    });




    //#region Alert Message

    //const msgObj = $("#BaseMsgHidden").data();
    //if (!hasAnyError(msgObj)) {
    //    if (msgObj.key == MessageEnum.SuccessStr || msgObj.key == MessageEnum.Success) {
    //        successMsg(msgObj.value);
    //        window.setTimeout(function () { $("#BaseMsgModal").modal("hide"); }, 4000);
    //    } else if (msgObj.key == MessageEnum.ErrorStr || msgObj.key == MessageEnum.Error) {
    //        failedMsg(msgObj.value);
    //    }
    //}

    //$("#BaseMsgHidden").removeData("key");
    //$("#BaseMsgHidden").removeData("value");

    const msgObj = $("#BaseMsgHidden").data();
    if (!hasAnyError(msgObj)) {
        if (msgObj.key == MessageEnum.SuccessStr || msgObj.key == MessageEnum.Success) {
            swal("Success!", `${msgObj.value}`, "success");
        } else if (msgObj.key == MessageEnum.ErrorStr || msgObj.key == MessageEnum.Error) {
            swal("Error!", `${msgObj.value}`, "error");
        } else if (msgObj.key == MessageEnum.WarningStr || msgObj.key == MessageEnum.Warning) {
            swal({
                title: "Information Successfully Saved!",
                icon: "success",
                content: {
                    element: "div",
                    attributes: {
                        innerHTML: msgObj.value,
                        style: "background-color:#dc3545;color:white;font-weight:600;"
                    }
                }
            });
        }
    }

    $("#BaseMsgHidden").removeData("key");
    $("#BaseMsgHidden").removeData("value");

    //#endregion

    $(".disable-control").attr("disabled", "disabled");


    const prevUrl = $("#BasePrevUrlHidden").val();
    if (!hasAnyError(prevUrl)) {
        redirectToUrl(prevUrl);
    }
});




//var dragTitle = "Need Scroll? Just Drag <<< OR >>>";
var dragTitle = "";
const API = "/../../";
//const API_URL = "/../../";



const MessageEnum = {
    Success: 1,
    SuccessStr: "Success",
    Error: 2,
    ErrorStr: "Error",
    Warning: 3,
    WarningStr: "warning"
};

const DataTable = {
    lengthMenu: [[50, 75, 100, 125, 150, 175, 200], [50, 75, 100, 125, 150, 175, 200]],
    displayLength: 50,
    processing: true,
    serverSide: true
};



//#region DateTime
//Date Picker Js Start-----------------

var week = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

$("#date-picker").datetimepicker({ format: 'DD/MM/YYYY' });
$(".date-picker-current").datetimepicker({ format: 'DD/MM/YYYY', defaultDate: moment() });
$(".date-picker").datetimepicker({
    format: 'DD/MM/YYYY',
    icons: {
        time: "fa fa-clock-o",
        date: "fa fa-calendar",
        up: "fa fa-arrow-up",
        down: "fa fa-arrow-down",
        previous: "fa fa-chevron-left",
        next: "fa fa-chevron-right",
        today: "fa fa-clock-o",
        clear: "fa fa-trash-o"
    }
});

/*$(".datepicker-here").datepicker({ dateFormat: "dd/mm/yyyy" }).attr("placeholder", "dd/mm/yyyy");*/

$(".datepicker-here").datepicker({
    dateFormat: "dd/mm/yyyy",
    autoClose: true,
    onSelect: function (d, i) {
        if (d !== i.lastVal) {
            $(this).change();
        }
    }
});

//$(".date-picker").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");
//$(".date-time-picker").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");

//$(".date-picker-future").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", changeMonth: true, changeYear: true, yearRange: "-100:+50" }).attr("placeholder", "dd/mm/yyyy");
//$(".date-time-picker-future").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", changeMonth: true, changeYear: true, yearRange: "-100:+50" }).attr("placeholder", "dd/mm/yyyy");

//$(".date-picker-now").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).datepicker("setDate", "0").attr("placeholder", "dd/mm/yyyy");
//$(".date-time-picker-now").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).datepicker("setDate", "0").attr("placeholder", "dd/mm/yyyy");

//$(".date-picker-dob").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");
//$(".date-time-picker-dob").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");

//$(".date-picker-dob").attr("title", "** Note: Date must be 18 Years from To Date");
//$(".date-time-picker-dob").attr("title", "** Note: Date must be 18 Years from To Date");

//$(".date-picker-future").attr("placeholder", "dd/ mm/ yyyy");

//----------------Check Fiscal Year Range End------------------



$(document.body).on("change", ".date-picker", function () {
    const date = convertStrToDate($(this).val());
    if (!date) $(this).val("");

    const isFutureDate = checkItsFutureDate(date);
    if (isFutureDate == true) {
        failedMsg("Sorry, Input Date Can't Be Greater Than To date");
        $(this).val("");
    }
});


$(document.body).on("change", ".date-picker-dob", function () {
    const givenDate = $(this).val();
    const date = convertStrToDate(givenDate);
    if (!date) $(this).val("");

    const isFutureDate = checkItsFutureDate(date);
    if (isFutureDate == true) {
        failedMsg("Sorry, Input Date Can't Be Greater Than To date");
        $(this).val("");
        return false;
    }


    const days = getDayDiffBetweenTwoDates(givenDate, getCurrentDate());
    const isValid = days > 0 ? days / 365 - 18 > 0 ? true : false : false;
    if (!isValid) {
        failedMsg("Sorry! Age is Under 18! ( " + givenDate + " )");
        $(this).val("");
    }

});


$(document.body).on("change", ".date-picker-future", function () {
    const date = convertStrToDate($(this).val());
    if (!date) $(this).val("");
});

$(document.body).on("change", ".date-picker-now", function () {
    const date = convertStrToDate($(this).val());
    if (!date) $(this).val("");
    const isFutureDate = checkItsFutureDate(date);
    if (isFutureDate == true) {
        failedMsg("Sorry, Input Date Can't Be Greater Than To date");
        $(this).datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0" }).datepicker("setDate", "0");
    }
});



$(document.body).on("click", ".img-popup-image", function () {
    $("#BaseImgModal").modal("show");
    const imgTag = "<img style='width:100%; height:100%' src='" + this.src + "' alt='" + this.alt + "'/>";
    $("#BaseImgModalBody").html(imgTag);
});

function closeModal(selector) {
    $(selector).modal("hide");
}

function isDateValid(date) {
    if (!hasAnyError(date)) {
        const dateFormat = /(^((((0[1-9])|([1-2][0-9])|(3[0-1]))|([1-9]))\x2F(((0[1-9])|(1[0-2]))|([1-9]))\x2F(([0-9]{2})|(((19)|([2]([0]{1})))([0-9]{2}))))$)/g;
        return dateFormat.test(date) ? true : false;
    }
    failedMsg("Sorry!! Date Not Found");
    return false;
}


function convertStrToDate(dateParam) {
    if (isDateValid(dateParam)) {
        const date = dateParam.split("/");
        const newDate = new Date(date[2], (date[1] - 1), date[0]);
        return newDate;
    }
    failedMsg("Sorry!! Date Not Valid");
    return false;

}

function checkItsFutureDate(dateTime) {
    const now = new Date();
    var future = false;
    if (Date.parse(now) < Date.parse(dateTime)) {
        future = true;
    }
    return future;
}


function convertJsonDate(date) {
    if (!hasAnyError(date) && $.type(date) === "string" && date.includes("/Date")) {
        return $.datepicker.formatDate("dd/mm/yy", eval(`new ${date.slice(1, -1)}`));
    } else if (!hasAnyError(date) && $.type(date) === "string" && !date.includes("/Date")) {
        return date;
    }
    return "";
}

function convertJsonFullDate(date) {
    if (!hasAnyError(date)) {
        const dt = moment(date).format("DD/MM/YY h:mm:ss a");
        return dt;

    } else if (date === null) {
        return "";
    }
    return "";
}


function convertStrTimeToHtmlTime(strTime) {
    if (!hasAnyError(strTime)) {
        const time = strTime.substr(0, 5);
        return time;
    }
    return "";
}



function convertJsonTime(date) {
    if (!hasAnyError(date)) {
        const time = moment(date).format("h:mm:ss A");

        return time;

    } else if (date === null) {
        return "";
    }
    failedMsg("Sorry! Date Not Found");
    return false;
}

function convertJsonTimeInHrMin(date) {
    if (!hasAnyError(date)) {
        const time = moment(date).format("h:mm A");

        return time;

    } else if (date === null) {
        return "";
    }
    failedMsg("Sorry! Date Not Found");
    return false;
}


function convertJsonToCSharpDate(date) {
    if (!hasAnyError(date) && $.type(date) === "string" && date.includes("/Date")) {
        return $.datepicker.formatDate("mm/dd/yy", eval("new " + date.slice(1, -1)));
    } else if (!hasAnyError(date) && $.type(date) === "string" && !date.includes("/Date")) {
        return date;
    }
    failedMsg("Sorry! Date Not Found");
    return false;
}

function convertStrDDMMYYtoMMDDYY(date) {
    if (!hasAnyError(date)) {
        const value = date.split(/\//);
        const result = [value[1], value[0], value[2]].join('/');
        return result;
    }
}

function convertJsonDateForView(date) {
    if (!hasAnyError(date) && $.type(date) === "string" && date.includes("/Date")) {
        return $.datepicker.formatDate("dd-M-yy", eval("new " + date.slice(1, -1)));
    } else if (!hasAnyError(date) && $.type(date) === "string" && !date.includes("/Date")) {
        return date;
    } else if (date === null) {
        const result = convertNullToNAData(null);
        return result;
    }
    failedMsg("Sorry! Date Not Found");
    return false;
}

function convertJsonFullDateForView(date) {
    if (!hasAnyError(date)) {
        const dt = moment(date).format("Do-MMM-YY");
        return dt;
    } else if (date === null) {
        return "";
    }
    failedMsg("Sorry! Date Not Found");
    return false;
}

function convertJsonDateForViewWithTime(date) {

    if (!hasAnyError(date)) {
        const dt = moment(date).format("Do-MMM-YY hh:mm A");
        return dt;
    } else if (date === null) {
        return "";
    }
    failedMsg("Sorry! Date Not Found");
    return false;
}


function getDayDiffBetweenTwoDates(fromDate, toDate) {
    if (!hasAnyError(fromDate) && !hasAnyError(toDate)) {
        const fDate = new Date(convertStrDDMMYYtoMMDDYY(fromDate));
        const tDate = new Date(convertStrDDMMYYtoMMDDYY(toDate));

        if (fDate <= tDate) {
            const timeDiff = Math.abs(tDate.getTime() - fDate.getTime());
            const diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24)) + 1;
            return diffDays;
        }


    }
    return null;
}



function getCurrentDate(isForCSharp = false, isForJsFormat = false) {
    let today = new Date();

    if (isForJsFormat) {
        return today;
    }

    let dd = today.getDate();
    let mm = today.getMonth() + 1; //January is 0!
    const yyyy = today.getFullYear();

    if (dd < 10) {
        dd = `0${dd}`;
    }

    if (mm < 10) {
        mm = `0${mm}`;
    }

    today = dd + "/" + mm + "/" + yyyy;

    if (isForCSharp) {
        today = mm + "/" + dd + "/" + yyyy;
    }

    return today;
}


function getYYMMDDObjByDays(days) {
    let stringResult = "";
    let numberResult = "";
    let values = "";
    let numberValues = "";
    if (!hasAnyError(days) && $.isNumeric(days) && days > 0) {

        // The string we're working with to create the representation

        // Map lengths of `diff` to different time periods
        values = [[' year', 365], [' month', 30], [' day', 1]];
        numberValues = [['', 365], ['', 30], ['', 1]];
    }


    // Iterate over the values...
    for (let i = 0; i < values.length; i++) {
        const amount = Math.floor(days / values[i][1]);

        // ... and find the largest time value that fits into the diff
        //if (amount >= 1) {
        // If we match, add to the string ('s' is for pluralization)
        stringResult += amount + values[i][0] + (amount > 1 ? 's' : '') + ' ';
        numberResult += amount + numberValues[i][0] + ' ';

        // and subtract from the diff
        days -= amount * values[i][1];
        //}
    }


    numberResult = numberResult.split("");
    if (numberResult[0] == 0 && numberResult[1] == 0 && numberResult[2] == 0) {
        stringResult = "N/ A";
    }
    if (!hasAnyError(numberResult) && !hasAnyError(stringResult)) {


        const model = {
            year: numberResult[0],
            month: numberResult[1],
            days: numberResult[2],
            fullYYDDMMForView: stringResult
        };
        return model;
    }


    return null;
}


function getDateRangeBetweenTwoDates(fromDate, toDate, isDateWithName = false) {

    if (hasAnyError(fromDate) || hasAnyError(toDate)) {
        return null;
    }

    const dateObjectList = [];


    let fromDateArray = "";
    let toDateDateArray = "";

    if (fromDate.getDate) {
        fromDate = convertJsToStrDate(fromDate);
    }

    if (toDate.getDate) {
        toDate = convertJsToStrDate(toDate);
    }

    fromDateArray = convertStrDateToArray(fromDate);
    toDateDateArray = convertStrDateToArray(toDate);


    let currentDate = new Date(fromDateArray[2], fromDateArray[1] - 1, fromDateArray[0]);
    const endDate = new Date(toDateDateArray[2], toDateDateArray[1] - 1, toDateDateArray[0]);

    const addDays = function (days) {
        const date = new Date(this.valueOf());
        date.setDate(date.getDate() + days);
        return date;
    };

    while (currentDate <= endDate) {

        if (convertStringToBool(isDateWithName)) {
            const myDateObj = { DayName: "", Date: null };
            myDateObj.Date = convertJsToStrDate(currentDate);
            myDateObj.DayName = week[currentDate.getDay()];
            dateObjectList.push(myDateObj);
        } else {
            dateObjectList.push(convertJsToStrDate(currentDate));
        }

        currentDate = addDays.call(currentDate, 1);
    }


    return dateObjectList;

}


function getDateRangeBetweenAnyDateToPastAnyDate(fromDate, howManyDaysBack, isDateWithName) {

    const currentFromDate = getDateFromAnyDateToPastAnyJsDate(fromDate, howManyDaysBack);
    const dataList = getDateRangeBetweenTwoDates(currentFromDate, fromDate, isDateWithName);
    return dataList;
}

function getDateFromAnyDateToPastAnyJsDate(fromDate, howManyDaysBack) {
    if (hasAnyError(fromDate)) {
        fromDate = getCurrentDate(false, true);
    } else {
        if (!fromDate.getDay) {
            fromDate = convertStrToJsDate(fromDate);
        }
    }

    if (!(howManyDaysBack > 0)) {
        howManyDaysBack = 0;
    }

    const pastDate = new Date();
    pastDate.setDate(fromDate.getDate() - howManyDaysBack);

    return pastDate;
}

function getDateFromAnyDateToPastAnyStrDate(fromDate, howManyDaysBack) {
    const date = getDateFromAnyDateToPastAnyJsDate(fromDate, howManyDaysBack);
    const result = convertJsToStrDate(date);
    return result;
}



function getDateRangeBetweenAnyDateToFutureAnyDate(fromDate, howManyDaysFuture, isDateWithName) {

    const toDate = getDateFromAnyDateToFutureAnyJsDate(fromDate, howManyDaysFuture, isDateWithName);
    const dataList = getDateRangeBetweenTwoDates(fromDate, toDate);
    return dataList;
}

function getDateFromAnyDateToFutureAnyJsDate(fromDate, howManyDaysFuture) {
    if (hasAnyError(fromDate)) {
        fromDate = getCurrentDate(false, true);
    } else {
        if (!fromDate.getDay) {
            fromDate = convertStrToJsDate(fromDate);
        }
    }

    if (!(howManyDaysFuture > 0)) {
        howManyDaysFuture = 0;
    }

    const pastDate = new Date();
    pastDate.setDate(fromDate.getDate() + howManyDaysFuture);

    return pastDate;
}

function getDateFromAnyDateToFutureAnyStrDate(fromDate, howManyDaysFuture) {
    const date = getDateFromAnyDateToFutureAnyJsDate(fromDate, howManyDaysFuture);
    const result = convertJsToStrDate(date);
    return result;
}



function getDayName(date) {
    if (hasAnyError(date)) {
        return date;
    }

    const dateArray = convertStrDateToArray(date);

    const currentDate = new Date(dateArray[2], dateArray[1] - 1, dateArray[0]);
    const result = week[currentDate.getDay()];
    return result;
}


function getDateArray(date) {
    convertStrDateToArray(date);
}

function convertStrDateToArray(date) {
    if (hasAnyError(date)) {
        date = getCurrentDate();
    }

    const dateArray = date.split("/");
    const dateDay = eval(dateArray[0]);
    const dateMonth = eval(dateArray[1]);
    const dateYear = eval(dateArray[2]);

    return dateArray;
}




function convertStrToJsDate(date) {
    if (hasAnyError(date)) {
        date = getCurrentDate();
    }

    const dateArray = convertStrDateToArray(date);
    const result = new Date(dateArray[2], dateArray[1] - 1, dateArray[0]);
    return result;

}


function convertJsToStrDate(date) {
    if (hasAnyError(date)) {
        date = new Date();
    }
    const result = date.getDate() + "/" + ((date.getMonth() + 1) < 10 ? "0" : "") + (1 + date.getMonth()) + "/" + date.getFullYear();
    return result;

}





//#endregion





//#region Call Last Url--------
var returnObj = {
    "Url": "",
    "Params": ""
};
function clearReturnObj() {
    returnObj.Url = "";
    returnObj.Params = "";
}
function createReturnObj(url, params) {
    clearReturnObj();
    returnObj.Url = url;
    returnObj.Params = params;
}

function returnBackFunction(targetElementId) {
    if (returnObj.Url !== "" && returnObj.Params !== "") {

        $.ajax({
            type: "POST",
            url: returnObj.Url,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(returnObj.Params),
            success: function (responseHtml) {
                $(targetElementId).html(responseHtml);
            }
        });

    }
}

//#endregion


var gMsg = "";
//Function For Setup Success Message
function successMsg(msg) {
    if (hasAnyError(msg)) {
        msg = "Message : Save Successful !";
        swal("Success!", `${msg}`, "success");
    } else {
        msg = `Message : ${msg}`;
        swal("Success!", `${msg}`);
    }

    //$.uiAlert({
    //    textHead: msg, text: "", bgcolor: "#28B463", textcolor: "#fff",
    //    position: "top-right", // top And bottom ||  left / center / right
    //    icon: "checkmark box",
    //    time: 3
    //});
    //return false;

    //toastr.success(msg);
    $("#BaseMsgModal").modal("show");

    $("#BaseMsgCardHeader").removeClass("bg-danger");
    $("#BaseMsgCardHeader").addClass("bg-success");

    $("#BaseMsgCardTitle").text("Success");

    $("#BaseMsgTitleIcon").removeClass("ik-alert-triangle");
    $("#BaseMsgTitleIcon").addClass("ik-check-circle");

    $("#BaseMsgCardBody").removeClass("text-danger");
    $("#BaseMsgCardBody").addClass("text-success");

    $("#BaseMsgModalCloseButton").removeClass("btn-outline-danger");
    $("#BaseMsgModalCloseButton").addClass("btn-outline-success");

    if (hasAnyError(msg)) {
        $("#BaseMsgCardBody").text("Message : Save Successful !");
    } else {
        $("#BaseMsgCardBody").text(`Message : ${msg}`);
    }

}


//Function For Setup Error Message
function failedMsg(msg) {

    if (msg === "") {
        msg = "Message : Operation Failed !";
        swal("Error!", `${msg}`, "error");
    } else {
        msg = `Message : ${msg}`;
        swal("Error!", `${msg}`);
    }

    //$.uiAlert({
    //    textHead: msg, text: "", bgcolor: "#DB2828", textcolor: "#fff",
    //    position: "top-right", // top And bottom ||  left / center / right
    //    icon: "remove circle",
    //    time: 3
    //});
    //return false;


    //toastr.error(msg);

    $("#BaseMsgModal").modal("show");

    $("#BaseMsgCardHeader").removeClass("bg-success");
    $("#BaseMsgCardHeader").addClass("bg-danger");

    $("#BaseMsgCardTitle").text("Error");

    $("#BaseMsgTitleIcon").removeClass("ik-check-circle");
    $("#BaseMsgTitleIcon").addClass("ik-alert-triangle");


    $("#BaseMsgCardBody").removeClass("text-success");
    $("#BaseMsgCardBody").addClass("text-danger");

    $("#BaseMsgModalCloseButton").removeClass("btn-outline-success");
    $("#BaseMsgModalCloseButton").addClass("btn-outline-danger");

    //Don't Modified By IsAnyError() ==> SRABAN
    if (msg === false) {
        $("#BaseMsgCardBody").html("Sorry! Save Failed !");
    } else {
        $("#BaseMsgCardBody").html(msg);
    }

}


//Function For Error BaseMsgHidden
function errorMsg(msg) {
    failedMsg(msg);
    //$("#BaseFailedMessageAlertDiv").fadeTo(5000, 700);
}


function hasAnyError(e) {
    if (e !== undefined && e !== "" && e !== null && e !== true && e !== false && e.length !== undefined && e.length !== 0) {
        if ($.type(e) === "string" && e.indexOf("Error") !== -1) {
            failedMsg(e);
            return true;
        }
        return false;
    }

    if (e === "" || e === undefined || e === null || e.length === 0) {
        return true;
    }

    return false;
}

function hasDataInArray(data) {
    if (hasAnyError(data)) return false;
    if (Array.isArray(data) && data.length > 0) return true;
    return false;
}

function isArray(data) {
    if (hasAnyError(data)) return false;
    return Array.isArray(data);
}


//Modal Message And Data Handles
function OnItemAdd(obj) {
    if (!hasAnyError(obj)) {
        //successMsg("Information Successfully SAVED !");
        successMsg("সফল! তথ্য সঠিকভাবে সংরক্ষিত (SAVED) হয়েছে ।");
        $("#errorMsgModal").hide();
        $("#successMsgModal").fadeTo(3000, 500);
        window.setTimeout(function () { $(".modal").modal("hide") }, 1000);
        const option = "<option value='" + obj.Id + "'> " + obj.Name + "</option>";
        const targetEl = $("#DropdownId").val();
        if (targetEl !== "" && targetEl !== undefined) {
            $("#" + targetEl).append(option);
            $("#" + targetEl).val(obj.Id);
        }

        const isFrom = $("#IsFrom").val();
        if (isFrom !== "" && isFrom !== undefined) {
            //This function invoked from modal calling page
            objectDataBind(isFrom, obj);

        }
    } else {
        failedMsg(obj);
    }



}


function OnItemUpdate(obj) {
    successMsg("Information Successfully SAVED !");
    //successMsg("সফল! তথ্য সঠিকভাবে পরিবর্তিত (UPDATED) হয়েছে ।");
    $("#errorMsgModal").hide();
    $("#successMsgModal").fadeTo(3000, 500);
    window.setTimeout(function () { $(".modal").modal("hide") }, 1000);
    const option = "<option value='" + obj.Id + "'> " + obj.Name + "</option>";
    const targetEl = $("#DropdownId").val();
    if (targetEl != "" && targetEl != undefined) {
        $("#" + targetEl).append(option);
        $("#" + targetEl).val(obj.Id);
    }

    const isFrom = $("#IsFrom").val();
    if (isFrom != "" && isFrom != undefined) {
        //This function invoked from modal calling page
        //objectDataBind(isFrom, obj);

    }

}

function OnItemAddFail(e) {
    if (!hasAnyError(e)) {
        failedMsg("Sorry ! Save Failed !");
    }

}


function isDataUnique(url, params, hideSelector, callBackF) {
    if (!hasAnyError(url) && !hasAnyError(params) && !hasAnyError(hideSelector)) {
        startUiBlock();
        $.post(url, params, function (rData) {
            stopUiBlock();
            if (!hasAnyError(rData)) {
                checkUniqueStatus(rData, hideSelector);
                callNextFunction(callBackF);
            }

        });
    }


}


// Create Or Edit Existing Checking Data
function checkUniqueStatus(data, hideSelector) {
    if (!hasAnyError(data)) {
        data = convertStringToBool(data);
    }
    if (!data) {
        //failedMsg("দুঃখিত, এই আইটেমটি পূর্বে সংরক্ষিত হয়েছে, অনুগ্রহকরে তথ্য পরিবর্তন করে আবার চেষ্টা করুন।");
        failedMsg("Sorry! This Item Already Added, Please Try Another");
        $(hideSelector).hide();
        return false;
    } else if (data == true) {
        $(hideSelector).show();
        return true;
    }
}


// Create Or Edit Existing Checking Data
function checkExistStatus(data, hideSelector) {
    if (!hasAnyError(data)) {
        data = convertStringToBool(data);
    }
    if (data == true) {
        //failedMsg("দুঃখিত, এই আইটেমটি পূর্বে সংরক্ষিত হয়েছে, অনুগ্রহকরে তথ্য পরিবর্তন করে আবার চেষ্টা করুন।");
        failedMsg("Sorry! This Item Already Added, Please Try Another");

        $(hideSelector).hide();
        return true;
    } else {
        $(hideSelector).show();
        return false;
    }
}



//var index = 0;
//var serial = 0;
function tableSlNo(selector) {

    var sl = 0;
    $(selector).find("tr").each(function () {
        sl++;
        $(this).find("td").eq(1).html(sl);
    });

}



function isPhoneNoValid(phoneNo) {
    if (!hasAnyError(phoneNo)) {
        const phoneFormat = /(^(\d{11})$)/g;
        if (phoneFormat.test(phoneNo)) {
            return true;
        } else {
            return false;
        }
    }
    failedMsg("Sorry! Phone No Not Found");
    return false;
}




$.validator.unobtrusive.parse(document);


function scrollDiv(divId) {
    const el = $(divId);
    if (!hasAnyError(divId) && el.length) {
        $("html, body").animate({
            scrollTop: $(divId).offset().top
        }, 2000);
    }

}


//setTimeout(function () { location.reload(); }, 5000);


$(document.body).on("click", "#ReportButton", function () {
    clearReportModal();
    const url = $(this).attr("data-url");
    var title = $(this).attr("data-title");
    if (!hasAnyError(url)) {
        $.get(url, function (rData) {
            $("#ReportModal").modal("show");
            $("#ReportTitle").html(title);
            $("#ReportModalBody").html(rData);
        });
    }
});



$(document.body).on("click", ".ReportButton", function () {
    clearReportModal();
    const url = $(this).attr("data-url");
    var title = $(this).attr("data-title");
    if (!hasAnyError(url)) {
        $.get(url, function (rData) {
            $("#ReportModal").modal("show");
            $("#ReportTitle").html(title);
            $("#ReportModalBody").html(rData);
        });
    }
});



$(document.body).on("click", ".DownloadButton", function (e) {
    e.preventDefault();

    //

    const url = getUrl(false, true);
    $("#BasePrevUrlHidden").val(url);

    if (confirmProceed("Do you want to download this?")) {
        window.location = $(this).attr("href");
    }

});

$(document.body).on("click", "#DownloadButton", function (e) {
    e.preventDefault();
    if (confirmProceed("Do you want to download this?")) {
        window.location = $(this).attr("href");
    }

});



function createReport(url, params, title) {
    clearReportModal();
    if (!hasAnyError(url)) {
        $.post(url, params, function (rData) {
            $("#ReportTitle").html(title);
            $("#ReportModalBody").html(rData);
        });
    }
}

function clearReportModal() {
    $("#ReportTitle").html("Please Wait...");
    $("#ReportModalBody").html("");
}


function startUiBlock() {
    $.blockUI({
        theme: true,
        baseZ: 999999,
        message: '<h4 style="border-radius: 4px;  background-color:white; color:#0098CB; font-weight: bolder;"><img src="' + API + 'images/ajax-loading.gif" /> ...Processing... Please Wait...</h4>'
    });
}


function startUiBlockForFiveSecond() {
    $.blockUI({
        theme: true,
        baseZ: 999999,
        message: '<h5 style="border-radius: 4px; background-color:white; color:green; font-weight: bolder;"><img src="' + API + 'images/ajax-loading.gif" />Processing...</h5>'
    });
}

function stopUiBlock() {
    $.unblockUI();
}

function getRound(value) {
    return Number(Math.round(value + "e" + 2) + "e-" + 2);
}



function getAmountAfterPercent(amount, percent) {

    amount = parseFloat(amount);
    percent = parseFloat(percent);

    if (!hasAnyError(amount) && !hasAnyError(percent)) {
        const data = ((amount * percent) / 100);
        return Math.round(data, 2);
    }
    return 0;
}




function getPercentFromValue(mainValue, achievedValue) {
    mainValue = parseFloat(mainValue);
    achievedValue = parseFloat(achievedValue);
    if (!hasAnyError(mainValue) && !hasAnyError(achievedValue)) {
        const data = ((mainValue * 100) / achievedValue);
        return Math.round(data, 2);
    }
    return 0;
}



function reloadPage(milSeconds = 1000) {
    window.setTimeout(function () { location.reload(); }, milSeconds);
}


$(document.body).on("click", ".base-view-btn", function () {
    $("#LgBaseModalBody").html("<h3 align='center' style='color:green; font-weight: bolder'>---Please Wait---</h3>");
    const url = $(this).attr("data-url");
    startUiBlock();
    $.get(url, function (rData) {
        stopUiBlock();
        if (!hasAnyError(rData)) {
            $("#LgBaseModal").modal("show");
            $("#LgBaseModalBody").html(rData);
        }
    });
});






//For Call Back Function

function callBack() {
    const isCallBackNeeded = $("#IsCBFNeeded").val();
    if (isCallBackNeeded == "true") {
        const cbfData = $("#IsCBFNeeded").attr("data-cbf");
        const cbf = eval(cbfData);
        if (typeof cbf == "function") {
            cbf();
        }
    }
}


//Select2 Trigger Change
var update = "change.select2";



$(document.body).on("submit", "form", function () {
    var buttons = $(this).find('[type="submit"]');
    if ($(this).valid()) {
        buttons.each(function (btn) {
            $(buttons[btn]).prop("disabled", true);
        });
        startUiBlock();
    } else {
        buttons.each(function (btn) {
            $(buttons[btn]).prop("disabled", false);
        });
    }
});


function enableAllInputType(formId) {
    $(`${formId} :input`).prop("disabled", false);
}




function createSelectList(url, params, targetEl, selectedData, callBackF) {
    clearDropDown(targetEl);
    /*startUiBlock();*/
    if (!hasAnyError(params)) {
        $.post(url, params, function (rData) {
            /*stopUiBlock();*/
            if (!hasAnyError(rData) && rData.length > 0) {
                bindDropdownList(rData, targetEl, selectedData);
                callNextFunction(callBackF);
            }
        });
    } else {
        $.post(url, function (rData) {
            /*stopUiBlock();*/
            if (!hasAnyError(rData) && rData.length > 0) {
                bindDropdownList(rData, targetEl, selectedData);
                callNextFunction(callBackF);
            }
            //else {
            //  failedMsg(rData + " Sorry! Data Not Found");
            //}
        });
    }

}


function clearDropDown(targetEl) {
    $(targetEl).empty();
    $(targetEl).append('<option value="" >---Select---</option>');
}

function bindDropdownList(dataList, targetEl, selectedData) {
    clearDropDown(targetEl);
    if (!hasAnyError(dataList) && dataList.length > 0) {
        dataList.forEach((e) => { $(targetEl).append("<option value='" + e.id + "'>" + e.name + "</option>"); });
    }

    if (!hasAnyError(selectedData) && selectedData > 0) {
        $(targetEl).val(selectedData).trigger(update);
    }
}



function getScrollButtons() {
    $(".scrollButtonDiv").html("");
    const lb = '<button type="button" title="Scroll Left" class="btn btn-mini glyphicon glyphicon-menu-left scrollButton scrollLeftButton"></button>';
    const rb = '<button type="button" title="Scroll Right" class="btn btn-mini glyphicon glyphicon-menu-right scrollButton scrollRightButton"></button>';
    return lb + rb;
}

$(".scrollButtonDiv").prepend(getScrollButtons());

//$(document.body).on("mouseover", ".table-div-size", function () {
//  var index = $(this).index();
//  var b = $(this).width();
//  var c = $(this).height();


//});



//$(".search-table").on("mouseover", "tbody tr", function () {
//  var index = $(this).index();

//});

$(document.body).on("click", ".scrollLeftButton", function () {
    const content = ".table-div-size";
    event.preventDefault();
    $(content).animate({
        scrollLeft: "-=200px"
    });
});

$(document.body).on("click", ".scrollRightButton", function () {
    const content = ".table-div-size";
    event.preventDefault();
    $(content).animate({
        scrollLeft: "+=200px"
    });
});


function convertNullToNAData(data) {
    if (hasAnyError(data)) {
        return "N/ A";
    }
    return data;
}

function convertNullToEmptyData(data) {
    if (data === undefined || data === "" || data === "null" || data === null) {
        return "";
    }
    return data;
}

function convertBoolToYesNo(data) {
    if (convertStringToBool(data)) {
        return "Yes";
    }
    return "No";
}


function isNumOrPercent(data) {
    if (convertStringToBool(data)) {
        return " %";
    }
    if (!convertStringToBool(data)) {
        return getCurrencyIcon();
    }
    return "N/ A";
}


function confirmProceed(msg = "") {
    if (msg === "") {
        msg = "Are You Sure To Proceed?";
    }
    var yesNo;
    if (confirm(msg)) {
        yesNo = true;
    }
    else {
        yesNo = false;
    }
    return yesNo;
}

function destroyDataTable(el) {
    if ($.fn.DataTable.isDataTable(`${el}`)) {
        const table = $(`${el}`).DataTable();
        table.destroy();
    }
}



function getSalarySheetMode(status) {
    status = convertStringToBool(status);
    if (status === true) {
        return "Addition";
    }
    if (status === false) {
        return "Deduction";
    }
    return "None";

}



function convertStringToBool(data) {
    if (hasAnyError(data)) return false;
    if (data === "true") {
        return true;
    }

    if (data === "false") {
        return false;
    }

    if (data === "True") {
        return true;
    }

    if (data === "False") {
        return false;
    }

    if (data === true) {
        return data;
    }

    if (data === false) {
        return data;
    }

    if (data === null) {
        return "";
    }

    return false;
}


function isTrue(data) {
    return convertStringToBool(data) === true;
}

function shake(divId) {
    $(divId).effect("shake");
}


function getSingleObjectFromArrayById(dataList, id) {
    const result = $.grep(dataList, function (e) {
        return e.Id === parseInt(id);
    });
    return result[0];
}

function getMultipleObjectFromArrayById(dataList, id) {
    const result = $.grep(dataList, function (e) {
        return e.Id === parseInt(id);
    });
    return result;
}


function clearAllTextBox(data) {
    $("input[type=text]").each(function () {
        if (!hasAnyError(data)) {
            $(this).val(data);
        } else {
            $(this).val("");
        }
    });
}


function getCurrencyIcon() {
    return " ৳";
}


function addCommasToNumber(number) {
    if (!hasAnyError(number)) {
        const rgx = /(\d+)(\d{3})/;
        while (rgx.test(number)) {
            number = number.replace(rgx, "$1" + "," + "$2");
        }
        return number;
    }
    return "";
}



function removeObjectFromArrayById(dataList, id) {
    const result = $.grep(dataList, function (e) {
        return e.Id !== parseInt(id);
    });
    return result;
}


function getSumFromArray(dataList) {
    var sum = 0;
    $.each(dataList, function (k, v) {
        sum += parseFloat(v.Amount);
    });

    return Math.round(sum, 2);
}



function clearAutoCode(value) {

    if (!hasAnyError(value) && value.includes("_")) {
        const dataArray = value.split("_");
        const finalResult = dataArray[0] + (dataArray.length > 1 ? dataArray[2] : "N/ A");
        return finalResult;

    }
    return value;
}





$(document.body).on("click", "#BaseConfirmationModalYesButton", function () {

});

$(document.body).on("click", ".g-confirm", function (e) {

    //e.preventDefault();
    //$("#BaseConfirmationModal").dialog("open");
    //$("#BaseConfirmationModal").dialog({
    //  autoOpen: false,
    //  modal: true,
    //  buttons: {
    //    "Confirm": function () {
    //      alert("You have confirmed!");
    //    },
    //    "Cancel": function () {
    //      $(this).dialog("close");
    //    }
    //  }
    //});


    if (confirmProceed("After Confirm, You Can't Edit This.")) {
        const url = $(this).attr("data-url");
        if (!hasAnyError(url)) {
            startUiBlock();
            $.post(url, function (rData) {
                stopUiBlock();
                if (!hasAnyError(rData) && rData === true) {
                    successMsg("Success! This Data Is Confirmed Successfully!");
                    reloadPage();
                } else {
                    failedMsg(rData);
                }
            });
        }
    }

});


$(document.body).on("click", ".g-delete", function () {
    if (confirmProceed()) {
        const url = $(this).attr("data-url");
        var redirectUrl = $(this).attr("data-redirectUrl");
        if (!hasAnyError(url)) {
            startUiBlock();
            $.post(url, function (rData) {
                stopUiBlock();
                if (!hasAnyError(rData) && rData === true) {
                    successMsg("Success! This Data Is Deleted Successfully!");

                    if (!hasAnyError(redirectUrl)) {
                        redirectToUrl(redirectUrl);
                    } else {
                        reloadPage();
                    }

                } else {
                    failedMsg("Sorry! Data Delete failed!");
                }
            });
        }
    }

});


function redirectToUrl(url) {
    window.location.replace(url);
}


$(document.body).on("click", ".g-load-partial", function () {
    const url = $(this).attr("data-url");
    const targetEl = $(this).attr("data-divId");
    if (!hasAnyError(url) && !hasAnyError(targetEl)) {
        loadPartial(url, "#" + targetEl);
    }

});


function loadPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
    $(targetEl).html("");
    if (!hasAnyError(url) && !hasAnyError(targetEl)) {
        startUiBlock();
        $.post(url, params, function (rData) {
            stopUiBlock();
            if (!hasAnyError(rData)) {
                $(targetEl).html(rData);
                if (!hasAnyError(scrollDiv) && convertStringToBool(scrollDiv) == true) {
                    scrollDiv(targetEl);
                }

                callNextFunction(callBackF);
            }
        });

    }
}




function loadPartial(url, targetEl, callBackF, scrollDiv) {
    $(targetEl).html("");
    if (!hasAnyError(url) && !hasAnyError(targetEl)) {
        startUiBlock();
        $.post(url, function (rData) {
            stopUiBlock();
            if (!hasAnyError(rData)) {
                $(targetEl).html(rData);
                if (!hasAnyError(scrollDiv) && convertStringToBool(scrollDiv) == true) {
                    scrollDiv(targetEl);
                }

                callNextFunction(callBackF);
            }
        });

    }
}

function callNextFunction(callBackF) {
    if ($.isFunction(callBackF)) {
        callBackF();
    }

}


//$(document.body).on("hover", ".scroll-table", function (e) {

//  var mouseX = e.pageX;
//  var divWidth = $(this).width();
//  var divX = $(this).offset().left;
//  if (mouseX > divWidth - 50) {
//    $("html, div").animate({
//      scrollLeft: $(".end-scroll").offset().left
//    });
//  }

//  if (mouseX < divX + 30) {
//    $("html, div").animate({
//      scrollLeft: $(".start-scroll").offset({ scrollLeft: divX })
//    });
//  }

//});

function isFormValid(formId) {
    var isValid = $(formId).valid();
    return isValid;
}

function uploadFileByAjax() {
    window.addEventListener("submit", function (e) {
        var form = e.target;
        if (form.getAttribute("enctype") === "multipart/form-data") {
            if (form.dataset.ajax) {
                e.preventDefault();
                e.stopImmediatePropagation();
                var xhr = new XMLHttpRequest();
                xhr.open(form.method, form.action);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState == 4 && xhr.status == 200) {
                        if (form.dataset.ajaxUpdate) {
                            var updateTarget = document.querySelector(form.dataset.ajaxUpdate);
                            if (updateTarget) {
                                updateTarget.innerHTML = xhr.responseText;
                            }
                        }
                    }
                };
                xhr.send(new FormData(form));
            }
        }
    }, true);
}



function getDataFromServer(url, params, callback) {
    if (!hasAnyError(params)) {
        startUiBlock();
        $.post(url, params, function (rData) {
            stopUiBlock();
            if (!hasAnyError(rData) && !hasAnyError(callback)) {
                callback(rData);
            }
        });
    }
}



function getUrlQueryStrArray() {
    const params = {};
    window.location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (s, k, v) { params[k] = v; });
    return params;
}

function getUrlFirstWord() {
    return window.location.pathname.replace(/^\/([^\/]*).*$/, '$1');
}

function getUrlSecondWord() {
    const fullUrl = getUrl(true);
    if (!hasAnyError(fullUrl)) {
        const urlArray = fullUrl.split("/");
        const secondWord = urlArray.length > 1 ? urlArray[2] : null;
        return secondWord;
    }
}

function getUrl(isOnlyPageUrl = false, isPageUrlWithQueryStr = false) {

    if (isOnlyPageUrl) {
        return window.location.pathname;
    }

    if (isPageUrlWithQueryStr) {
        return window.location.pathname + window.location.search;
    }

    return window.location.href;
}

function setUrl(url, pageName, title) {
    changeUrl(url, pageName, title);
}

function changeUrl(url, pageName, title) {
    window.history.pushState({ page: title }, pageName, url);
}



function addNoDataFoundFooter(target) {
    if (!hasAnyError(target)) {
        removeNoDataFoundFooter(target);
        const footer = `<tr id='${target}_Footer'><td colspan='50' style='color: #072f4a; background-color: white; font-weight: bold; text-align:center'> No Record Found !</td></tr>`;
        $(target).append(footer);
    }
}

function addNoDataFoundFooterWithMsg(target, msg) {
    if (!hasAnyError(target)) {
        removeNoDataFoundFooter(target);
        const footer = `<tr id='${target}_Footer'><td colspan='50' style='color: #072f4a; background-color: white; font-weight: bold; text-align:center'> ${msg}</td></tr>`;
        $(target).append(footer);
    }
}

function removeNoDataFoundFooter(target) {
    if (target) {
        const tableFooter = $(target).children("tFooter");
        if (!hasAnyError(tableFooter)) {
            $(target).children("tFooter").remove();
        }

    }

}


function removeStyle(e) {
    if (hasAnyError(e)) return false;
    e.removeAttr("style");
}

function baseDelete(url, id, callBackF) {

    if (hasAnyError(url) && id > 0) {
        url = API + "AppFiles/DeleteFile/" + id;
    } else if (!hasAnyError(url) && id > 0) {
        url = url + "/" + id;
    }

    if (confirmProceed()) {
        startUiBlock();
        $.post(url, function (rData) {
            stopUiBlock();
            if (!hasAnyError(rData) && convertStringToBool(rData) == true) {
                successMsg("Information Deleted Successfully!");
                if (!$.isFunction(callBackF)) {
                    reloadPage();
                } else {
                    callNextFunction(callBackF);
                }
            } else {
                failedMsg(rData);
            }
        });
    }
}



function isObject(obj) {
    return obj === Object(obj);
}

function convertToObject(obj) {
    return obj === Object(obj) ? Object(obj) : obj;
}

function getObjProperties(obj) {
    if (isObject(obj)) {
        const result = Object.keys(obj);
        if (result.length > 0) {
            return result;
        }
    }
    return null;
}


function getObjValues(obj) {
    if (isObject(obj)) {
        const result = Object.values(obj);
        if (result.length > 0) {
            return result;
        }
    }
    return null;
}

function getEnumToInt32(obj, index) {
    if (isObject(obj) && Object.keys(obj).length > 0) {
        const result = Object.values(obj)[index - 1];
        return result;
    }
    return null;
}


function getEnumToCharToString(obj, index) {
    if (isObject(obj) && Object.keys(obj).length > 0) {
        const result = Object.keys(obj)[index - 1][0];
        return result;
    }
    return null;
}


function getEnumToString(obj, index) {
    if (isObject(obj) && Object.keys(obj).length > 0) {
        const result = getEnumFriendlyName(Object.keys(obj)[index - 1]);
        return result;
    }
    return null;
}

function getEnumFriendlyName(value) {
    let result = "";
    if (value.length > 0) {
        value.split("").forEach((c, i) => { if (c === c.toUpperCase() && i > 0) { result += ` ${c}`; } else { result += c; } });
        return result;
    }
    return value;
}




function stopFormPosting(e) {
    e.preventDefault();
}


function startFormPosting(formId) {
    $(formId).submit();
}



function convertDataTypeFromBase64String(strFile) {
    if (hasAnyError(strFile)) return strFile;
    return strFile.substr(22, strFile.length);
}


function getFileName(inputEl) {
    const files = document.getElementById(inputEl).files;
    if (!hasAnyError(files) && !hasAnyError(files[0])) {
        const name = files[0].name;
        return name;
    }
    return null;
}


function getFileExtension(inputEl) {
    const files = document.getElementById(inputEl).files;
    if (!hasAnyError(files) && !hasAnyError(files[0])) {
        const extension = files[0].name.split(".").pop();
        return extension.toLowerCase();
    }
    return null;
}

function isPdf(inputEl, wantReturn = true) {
    var data = false;
    if (isFileSizeValid(inputEl) == false) return false;
    if (getFileExtension(inputEl) == "pdf") { data = true; }
    if (wantReturn) return data;
    if (!data) {
        failedMsg("Sorry! Only Pdf is Acceptable!");
        $(`#${inputEl}`).val("");
    }
    return data;
}


function isCsV(inputEl, wantReturn = true) {
    var data = false;
    if (isFileSizeValid(inputEl) == false) return false;
    if (getFileExtension(inputEl) == "csv") { data = true; }
    if (wantReturn) return data;
    if (!data) {
        failedMsg("Sorry! Only CSV is Acceptable!");
        $(`#${inputEl}`).val("");
    }
    return data;
}


function isExcel(inputEl, wantReturn = true) {
    var data = false;
    if (isFileSizeValid(inputEl) == false) return false;
    const ext = getFileExtension(inputEl);
    if (ext == "xls" || ext == "xlsx") { data = true; }
    if (wantReturn) return data;
    if (!data) {
        failedMsg("Sorry! Only Excel is Acceptable!");
        $(`#${inputEl}`).val("");
    }
    return data;
}


function isImage(inputEl, wantReturn = true) {
    var data = false;
    if (isFileSizeValid(inputEl) == false) return false;
    const extension = getFileExtension(inputEl);
    if (extension == "jpg" || extension == "png" || extension == "jpeg" || extension == "bmp") { data = true; }
    if (wantReturn) return data;
    if (!data) {
        failedMsg("Sorry! Only JPG/ PNG/ JPEG/ BMP is Acceptable!");
        $(`#${inputEl}`).val("");
    }
    return data;
}


function isFileSizeValid(inputEl) {
    let size = getFileSize(inputEl);
    if (size > 0) {
        size = parseFloat(size);
        if (size > 6114) {
            failedMsg("Sorry! File Size Can't Bigger Than 6 (Six) MB");
            $(`#${inputEl}`).val("");
            return false;
        }
    }
    return true;
}


function basePreviewPhoto(fromInputEl, imgTargetEl, imgShowDivEl, noPreviewDiv) {
    const files = document.getElementById(fromInputEl).files;
    //const files = $("#photos").files;
    $(imgTargetEl).empty();

    for (let i = 0; i < files.length; i++) {

        let extension = files[i].name.split('.').pop();
        extension = extension.toLowerCase();
        if (extension == "jpg" || extension == "png" || extension == "jpeg" || extension == "bmp") {
            const file = files[i];
            baseReadImage(file, fromInputEl, imgTargetEl);
            $(imgShowDivEl).show();
            $(noPreviewDiv).hide();
        }
        else {
            failedMsg("Only Image File Allowed");
            $(`#${fromInputEl}`).val("");
        }

    }
}

function baseReadImage(file, fromInputEl, imgTargetEl) {
    const reader = new FileReader;
    var image = new Image;
    reader.readAsDataURL(file);
    reader.onload = function (_file) {
        image.src = _file.target.result;
        image.onload = function () {
            const height = this.height;
            const width = this.width;
            const type = file.type;
            const size = (file.size / 1024);

            if (height <= 1080 && width <= 1960 && size <= 512 && type == "image/jpeg" || type == "image/jpg" || type == "image/png") {
                $(imgTargetEl).append(`<div class='col-md-12'><img src='${_file.target.result}' height='150' width='150' ></div>`);
            } else {
                failedMsg("Photo less or equal (1080 X 1960) pixel and maximum 512 kb </br> Photo format JPEG/PNG");
                $(`#${fromInputEl}`).val("");
            }
        };
    };
}


function getFileSize(el) {
    const files = document.getElementById(el).files;
    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        if (!hasAnyError(file)) {
            const size = (file.size / 1024);
            return size;
        }
    }
    return 0;
}


function isChecked(el) {
    return $(el).is(":checked");
}

function isCheckedBySelector(selector) {
    return $(selector).is(":checked");
}

function baseCheckedThisCheckBox(el) {
    if ($(el).is(":checked")) {
        $(el).prop("checked", true);
        $(el).val(true);
    } else {
        $(el).prop("checked", false);
        $(el).val(false);
    }
}

function getRadioButtonValueByName(elementName) {
    return $(`input[name=${elementName}]:checked`).val();
}

function getRadioButtonValueBySelector(selector) {
    return $(`${selector}:checked`).val();
}



function clearServerCache() {
    const url = API + "Home/ForceClearCache";
    stopUiBlock();
    $.post(url, function (rData) {
        stopUiBlock();
        if (!hasAnyError(rData)) {
            successMsg("Success! Cache Cleared Successful!");
        }
    });
}



function addTotalRowCountSpanInDataTable(idWithoutHash) {
    $(`#${idWithoutHash}_length`).append(`<span id='${idWithoutHash}_TotalRowCountSpan' style='font-weight:bolder;margin-left:5px;'></span>`);
}


function showTotalRowCountSpanInDataTable(idSelector, tableObject) {
    $(`#${idSelector}_TotalRowCountSpan`).text(` of total ${tableObject.page.info().recordsTotal} entries`);
}


//function customPost(url, params, callBackFunc, isAsync = true) {
//    if (!hasAnyError(url) && !hasAnyError(params)) {
//        $.ajax({
//            type: "POST", url: url, data: params, async: isAsync, success: function (rData) {
//                if (!hasAnyError(callBackFunc)) {
//                    callBackFunc(callBackFunc);
//                }
//                return rData;
//            }
//        });
//    } else if (!hasAnyError(url)) {
//        $.post(url, function (rData) {
//            if (!hasAnyError(callBackFunc)) {
//                callBackFunc(callBackFunc);
//            }
//            return rData;
//        });

//    }

//}



function getAutoGenCode(index = 0) {
    const date = new Date();
    const result = `${date.getYear()}${date.getMonth() + 1}${date.getDate()}${date.getHours()}${date.getMinutes()}${date.getSeconds()}${date.getMilliseconds() + index}`;
    //const uuid = ((new Date().getTime()).toString(36)) + '_' + (Date.now() + Math.random().toString()).split('.').join("_");
    return result;
}


function generateAutoGenCode(length, formatString) {
    if (!hasAnyError(formatString)) {
        let result = "";
        for (let i = 0; i < length; i++) {
            result += formatString.charAt(Math.floor(Math.random() * formatString.length));
        }
        return result;
    }
}

function getAutoGenNumber(length) {
    return generateAutoGenCode(length, "0123456789");
}

function getAutoGenAlpha(length) {
    return generateAutoGenCode(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
}

function getAutoGenSpecialChar(length) {
    return generateAutoGenCode(length, "!@#$%^&*()");
}

function getAutoGenAlphaNum(length) {
    return generateAutoGenCode(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
}

function getAutoGenAlphaSpecialChar(length) {
    return generateAutoGenCode(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()");
}

function getAutoGenAlphaNumSpecialChar(length) {
    return generateAutoGenCode(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()");
}



function setDelay(second, callbackFn) {
    setTimeout(() => {
        callNextFunction(callbackFn);
    }, second * 1000);
}