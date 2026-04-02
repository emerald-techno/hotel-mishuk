//$(document).ready(function () {
//    var businessDateStr = $("#BusinessDateStr").val();
//    var maxDateObj = parseDDMMMYYYY(businessDateStr);

//    var dp = $("#AuditDateStr").datepicker({
//        dateFormat: "dd/mm/yyyy",
//        autoClose: true,
//        defaultDate: maxDateObj,
//        maxDate: maxDateObj
//    }).data('datepicker');

//    dp.selectDate(new Date(businessDateStr));
//});

$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        BusinessDateStr: $("#BusinessDateStr").val()
    }

    const dateStr = $("#BusinessDateStr").val();

    $("#ltReportFilter").html("Date : " + model.BusinessDateStr);
    const url = API + "NightAudit/NightAuditSummaryReport";
    const params = { businessDateStr: dateStr };
    $.post(url, params, function (rData) {
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});

$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        BusinessDateStr: $("#BusinessDateStr").val()
    }

    const url = `${API}NightAudit/NightAuditSummaryReportPrint?businessDateStr=${model.BusinessDateStr}`;
    window.open(url, "_blank");
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<b>No Data Found</b>");
    }
}