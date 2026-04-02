$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        HallId: $("#HallId").val(),
        FromDateStr: $("#FromDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
    }

    const url = API + "BookingService/HallAvailableReport";
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
        HallId: $("#HallId").val(),
        FromDateStr: $("#FromDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
    }

    const url = `${API}BookingService/TodayArrivalReportPrint?hallId=${model.HallId}&fromDateStr=${model.FromDateStr}&toDateStr=${model.ToDateStr}`;
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