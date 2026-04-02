//$(document).ready(function () {
//    const d = new Date();
//    let text = convertJsonFullDateForView(d);

//    $("#ltReportFilter").html("Date: " + text);
//    const url = API + "BookingService/TodayArrivalReport";

//    /*const params = { model: model };*/
//    $.post(url, function (rData) {
//        console.log(rData);
//        if (rData != null) {
//            generateReportTable(rData);
//        }
//    });
//})


//$(document.body).on("click", "#ReportPrintBtn", function () {
//    const url = `${API}BookingService/TodayArrivalReportPrint`;
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

$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrQueryDate: $("#StrQueryDate").val()
    }

    $("#ltReportFilter").html("Date : " + model.StrQueryDate);
    const url = API + "BookingService/TodayArrivalReport";
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
        StrQueryDate: $("#StrQueryDate").val()
    }

    const url = `${API}BookingService/TodayArrivalReportPrint?queryDate=${model.StrQueryDate}`;
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