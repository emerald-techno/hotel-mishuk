$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        FromDateStr: $("#FromDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
    }

    const url = API + "BookingService/RoomOccupancyReport";
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
    }

    const url = `${API}BookingService/RoomOccupancyReportPrint?fromDateStr=${model.FromDateStr}&toDateStr=${model.ToDateStr}`;
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