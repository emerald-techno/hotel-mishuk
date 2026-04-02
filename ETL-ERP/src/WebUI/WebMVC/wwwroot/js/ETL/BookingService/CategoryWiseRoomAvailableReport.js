$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StartDateStr: $("#StartDateStr").val(),
        EndDateStr: $("#EndDateStr").val()
    };

    let hasError = false;

    if (hasAnyError(model.StartDateStr)) {
        failedMsg("Please check Report Date");
        hasError = true;
    }

    if (hasError) return;

    $("#ltReportFilter").html("From Date: " + model.StartDateStr + "To Date: " + model.EndDateStr);

    const url = API + "BookingService/CategoryWiseRoomAvailableReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});



$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        StartDateStr: $("#StartDateStr").val(),
        EndDateStr: $("#EndDateStr").val()
    }

    const url = `${API}BookingService/CategoryWiseRoomAvailableReportPrint?startDateStr=${model.StartDateStr}&endDateStr=${model.EndDateStr}`;

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