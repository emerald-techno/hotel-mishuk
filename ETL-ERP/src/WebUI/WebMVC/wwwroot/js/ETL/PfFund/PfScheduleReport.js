$(document.body).on("click", "#ReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val()
    }
    $("#ReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "PfFundMst/PfScheduleReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        //console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});

$(document.body).on("click", "#PrintBtn", function () {
    const model = {
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
    }
    const url = `${API}PfFundMst/PfScheduleReportPrint?fromDate=${model.FormDateStr}&toDate=${model.ToDateStr}`;
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