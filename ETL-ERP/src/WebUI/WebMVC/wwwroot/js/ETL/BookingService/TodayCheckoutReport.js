//$(document).ready(function () {
//    const d = new Date();
//    let text = convertJsonFullDateForView(d);

//    $("#ltReportFilter").html("Date: " + text);
//    const url = API + "BookingService/TodayCheckOutReport";

//    /*const params = { model: model };*/
//    $.post(url, function (rData) {
//        console.log(rData);
//        if (rData != null) {
//            generateReportTable(rData);
//        }
//    });
//})


////$(document.body).on("click", "#ShowReportBtn", function () {
////    const d = new Date();
////    let text = d.toDateString();

////    $("#ltReportFilter").html("Date: " + text);
////    const url = API + "BookingService/TodayArrivalReport";

////    const params = { model: model };
////    $.post(url, params, function (rData) {
////        console.log(rData);
////        if (rData != null) {
////            generateReportTable(rData);
////        }
////    });
////});

//$(document.body).on("click", "#ReportPrintBtn", function () {
//    //const model = {
//    //    StrFromDate: $("#StrFromDate").val(),
//    //    StrToDate: $("#StrToDate").val(),
//    //    RoomCategoryId: $("#RoomCategoryId").val(),
//    //    RoomId: $("#RoomId").val(),
//    //    FloorId: $("#FloorId").val(),
//    //    BookingStatus: $("#BookingStatus").val()
//    //}

//    //if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
//    //    const url = `${API}BookingService/BookingServiceReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&categoryId=${model.RoomCategoryId}&roomId=${model.RoomId}&floorId=${model.FloorId}`;
//    //    window.open(url, "_blank");
//    //}

//    const url = `${API}BookingService/TodayCheckOutReportPrint`;
//    window.open(url, "_blank");
//});

//function generateReportTable(data) {
//    if (data.length > 0) {
//        $("#ReportContainer").empty();
//        $("#ReportContainer").append(data);

//    } else {
//        $("#ReportContainer").html("<b>No Data Found</b>");
//    }

//}


$(document).ready(function () {
    const fromDate = $(".from_dt").val();
    const toDate = $(".to_dt").val();

    loadReport();
});

function loadReport() {
    const fromDate = $(".from_dt").val();
    const toDate = $(".to_dt").val();

    $("#ltReportFilter").html("Report From: <b>" + fromDate + "</b> To: <b>" + toDate + "</b>");

    const url = API + "BookingService/TodayCheckOutReport";

    $.post(url, { strFromDate: fromDate, strToDate: toDate }, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}


$(document.body).on("click", "#ShowReportBtn", function () {
    loadReport();
});

$(document.body).on("click", "#ReportPrintBtn", function () {
    const fromDate = $(".from_dt").val();
    const toDate = $(".to_dt").val();

    const url = `${API}BookingService/TodayCheckOutReportPrint?strFromDate=${fromDate}&strToDate=${toDate}`;
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