$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        CustomerId: $("#CustomerId").val(),
        CustomerTypeId: $("#CustomerTypeId").val(),

    }

    $("#ltReportFilter").html("From Date: " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "FoodOrder/RsPaymentTransectionReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});

$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        CustomerId: $("#CustomerId").val(),
        CustomerTypeId: $("#CustomerTypeId").val(),
    }

    const url = `${API}FoodOrder/RsPaymentTransectionReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&customerId=${model.CustomerId}&customerTypeId=${model.CustomerTypeId}`;
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