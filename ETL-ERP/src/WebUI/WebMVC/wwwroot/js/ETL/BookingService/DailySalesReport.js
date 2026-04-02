$(document).ready(function () {
    const d = new Date();
    let text = convertJsonFullDateForView(d);

    $("#ltReportFilter").html("Date: " + text);
    const url = API + "BookingService/RoomDailySalesReport";

    $.post(url, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
})

$(document.body).on("click", "#ReportPrintBtn", function () {
    const url = `${API}BookingService/RoomDailySalesReportPrint`;
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