$(document).ready(function () {
    const d = new Date();
    let text = convertJsonFullDateForView(d);

    $("#ltReportFilter").html("Date: " + text);
    const url = API + "BookingService/InHouseGuestReport";

    /*const params = { model: model };*/
    $.post(url, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
})


//$(document.body).on("click", "#ShowReportBtn", function () {
//    const d = new Date();
//    let text = d.toDateString();

//    $("#ltReportFilter").html("Date: " + text);
//    const url = API + "BookingService/TodayArrivalReport";

//    const params = { model: model };
//    $.post(url, params, function (rData) {
//        console.log(rData);
//        if (rData != null) {
//            generateReportTable(rData);
//        }
//    });
//});

$(document.body).on("click", "#ReportPrintBtn", function () {
    //const model = {
    //    StrFromDate: $("#StrFromDate").val(),
    //    StrToDate: $("#StrToDate").val(),
    //    RoomCategoryId: $("#RoomCategoryId").val(),
    //    RoomId: $("#RoomId").val(),
    //    FloorId: $("#FloorId").val(),
    //    BookingStatus: $("#BookingStatus").val()
    //}

    //if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
    //    const url = `${API}BookingService/BookingServiceReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&categoryId=${model.RoomCategoryId}&roomId=${model.RoomId}&floorId=${model.FloorId}`;
    //    window.open(url, "_blank");
    //}

    const url = `${API}BookingService/InHouseGuestReportPrint`;
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