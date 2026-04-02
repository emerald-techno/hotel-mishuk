
$(document).ready(function () {
    loadReport();
});

function loadReport() {
    const fromDate = $(".from_dt").val();
    const toDate = $(".to_dt").val();
    const serviceId = $("#ServiceId").val();

    $("#ltReportFilter").html("Report From: <b>" + fromDate + "</b> To: <b>" + toDate + "</b>");

    const url = API + "BookingService/ExtraServiceReport";

    $.post(url, { strFromDate: fromDate, strToDate: toDate, serviceId: serviceId }, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}


$(document.body).on("click", "#ShowReportBtn", function () {
    loadReport();
});

$(document.body).on("click", "#ReportPrintBtn", function (e) {
    e.preventDefault();
    const fromDate = $(".from_dt").val();
    const toDate = $(".to_dt").val();
    const serviceId = $("#ServiceId").val();

    const url = `${API}BookingService/ExtraServiceReportPrint?strFromDate=${fromDate}&strToDate=${toDate}&serviceId=${serviceId}`;
    window.open(url, "_blank");
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<h5 style='text-align:center'>No Data Found<h5>");
    }

}
