$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        RoomCategoryId: $("#RoomCategoryId").val(),
        RoomId: $("#RoomId").val(),
        FloorId: $("#FloorId").val(),
        BookingStatus: $("#BookingStatus").val()
    }
    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "BookingService/BookingServiceReport";
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
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        RoomCategoryId: $("#RoomCategoryId").val(),
        RoomId: $("#RoomId").val(),
        FloorId: $("#FloorId").val(),
        BookingStatus: $("#BookingStatus").val()
    }

    //if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
    //    const url = `${API}BookingService/BookingServiceReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&categoryId=${model.RoomCategoryId}&roomId=${model.RoomId}&floorId=${model.FloorId}`;
    //    window.open(url, "_blank");
    //}

    const url = `${API}BookingService/BookingServiceReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}
                    &categoryId=${model.RoomCategoryId}&roomId=${model.RoomId}&floorId=${model.FloorId}&bookingStatus=${model.BookingStatus}`;
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