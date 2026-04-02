$(document).ready(function () {
    var businessDateStr = $("#BusinessDateStr").val();
    var maxDateObj = parseDDMMMYYYY(businessDateStr);

    var dp = $("#AuditDateStr").datepicker({
        dateFormat: "dd/mm/yyyy",
        autoClose: true,
        defaultDate: maxDateObj,
        maxDate: maxDateObj
    }).data('datepicker');

    dp.selectDate(new Date(businessDateStr));
});


$(document.body).on("click", "#GanerateNightAuditSummaryBtn", function () {
    loadReportPartial();
});

function loadReportPartial() {
    const auditDateStr = $("#AuditDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const url = API + "NightAudit/GetNightAuditSummaryPartial";
        const params = { businessDateStr: auditDateStr };
        loadRoomAuditPartialWithParams(url, params, "#NightAuditSummaryPartialDiv", null);
    }
}

function loadRoomAuditPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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
            } else {
                failedMsg("No Room Audit Info Found");
            }
        });

    }
}

$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        businessDateStr: $("#AuditDateStr").val()
    }
    //const model = { }
    console.log(model);
    const url = `${API}NightAudit/RoomAuditPrint?businessDateStr=${model.businessDateStr}`;

    window.open(url, "_blank");
});


function parseDDMMMYYYY(s) {
    // Example: "14-Feb-2025"
    var parts = s.split('-');
    var day = parseInt(parts[0], 10);
    var mon = parts[1].toLowerCase().substring(0, 3);
    var year = parseInt(parts[2], 10);

    var months = {
        jan: 0, feb: 1, mar: 2, apr: 3, may: 4, jun: 5,
        jul: 6, aug: 7, sep: 8, oct: 9, nov: 10, dec: 11
    };

    return new Date(year, months[mon], day);
}

$(document.body).on("click", "#DayCloseBtn", function () {

    var businessDate = new Date($("#BusinessDateStr").val());
    var actualDate = new Date();

    var businessDateAt12PM = moment(businessDate.setHours(23,59)).format("DD-MMM-yyyy HH:mm:ss");
    var actualDateTime = moment(actualDate).format("DD-MMM-yyyy HH:mm:ss");
    var isSameDate = moment(actualDateTime).isSame(businessDateAt12PM, 'day');

    if (isSameDate && actualDateTime < businessDateAt12PM) {
        swal({
            title: "Can't close night audit before 11.59 PM of BusinessDate..!!",
            text: "If close before 11.59 PM, there some issue might happen..!!",
            icon: "warning",
            buttons: false,
            dangerMode: true,
        }).then((result) => {
                if (result) {
                    closeBusinessDay();
                }
            })
    }
    else {
        closeBusinessDay();
    }
})


function closeBusinessDay() {
    const auditDateStr = $("#AuditDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const url = API + "NightAudit/CloseBusinessDay";
        const params = { businessDateStr: auditDateStr };

        $("#DayCloseBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Close Business Day Successfully..!!");
                setTimeout(() => {
                    loadReportPartial();
                }, 1500);
            } else {
                failedMsg("Business Day Closer Failed");
            }

            $("#DayCloseBtn").prop("disabled", false).text("Close The Day");
        }).fail(function (e) {
            console.log(e.responseText);
            failedMsg(`Business Day Closer Failed.${e.responseText}`);

            $("#DayCloseBtn").prop("disabled", false).text("Close The Day");
        })
    } else {
        failedMsg("Business Day Closer Failed");
    }
}

