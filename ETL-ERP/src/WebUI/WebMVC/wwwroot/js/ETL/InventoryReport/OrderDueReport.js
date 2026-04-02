$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        OrderId: $("#OrderId").val(),
        SupplierId: $("#SupplierId").val()
    }
    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "InventoryReport/OrderDueReport";
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
        OrderId: $("#OrderId").val(),
        SupplierId: $("#SupplierId").val()
    }

    const url = `${API}InventoryReport/OrderDueReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}
                    &orderId=${model.OrderId}&supplierId=${model.SupplierId}`;
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