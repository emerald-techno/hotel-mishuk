$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        FromDateStr: $("#FromDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
        GuestFilter: $("#GuestFilter").val(),
        BookingFilter: $("#BookingFilter").val()

    };

    let hasError = false;

    if (hasAnyError(model.FromDateStr)) {
        failedMsg("Please check From Date");
        hasError = true;
    }

    if (hasAnyError(model.ToDateStr)) {
        failedMsg("Please check To Date");
        hasError = true;
    }

    if (hasError) return;

    $("#ltReportFilter").html("From Date: " + model.FromDateStr + " To Date: " + model.ToDateStr);

    const url = API + "BookingService/CancelReport";
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
        FromDateStr: $("#FromDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
        GuestFilter: $("#GuestFilter").val(),
        BookingFilter: $("#BookingFilter").val()
    }
    //const model = { }
    console.log(model);
    const url = `${API}BookingService/CancelReportPrint?fromDateStr=${model.FromDateStr}&toDateStr=${model.ToDateStr}&bookingNo=${model.BookingFilter}&guestName=${model.GuestFilter}`;

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