$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        CategoryId: $("#CategoryId").val(),
        ItemId: $("#ItemId").val(),
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        StockStatus: $("#StockStatus").val()
    }
    const url = API + "InventoryReport/StockRegisterReport";
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
        CategoryId: $("#CategoryId").val(),
        ItemId: $("#ItemId").val(),
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        StockStatus: $("#StockStatus").val()
    }

    const url = `${API}InventoryReport/StockRegisterReportPrint?categoryId=${model.CategoryId}&itemId=${model.ItemId}&stockStatus=${model.StockStatus}&strFromDate=${model.StrFromDate}&strToDate=${model.StrToDate}`;
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