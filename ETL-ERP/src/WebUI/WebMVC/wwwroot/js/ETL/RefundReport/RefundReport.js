$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        GuestId: $("#GuestId").val(),
        CompanyId: $("#CompanyId").val(),

    }

    //$("#ltReportFilter").html("From Date: " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "Refund/HtRefundReport";
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
        GuestId: $("#GuestId").val(),
        CompanyId: $("#CompanyId").val(),
    }

    const url = `${API}Refund/HtRefundReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&guestId=${model.GuestId}&companyId=${model.CompanyId}`;
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