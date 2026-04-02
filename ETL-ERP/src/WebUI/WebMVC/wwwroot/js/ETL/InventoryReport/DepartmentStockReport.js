$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        departmentId: $("#DepartmentId").val(),
        itemId: $("#ItemId").val(),
        categoryId: $("#CategoryId").val(),
        stockStatus: $("#StockStatus").val()
    }

    if (!(model.departmentId > 0)) {
        return failedMsg("Please Select Department..!!");
    }

    const url = API + "InventoryReport/DepartmentStockReport";
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
        departmentId: $("#DepartmentId").val(),
        itemId: $("#ItemId").val(),
        categoryId: $("#CategoryId").val(),
        stockStatus: $("#StockStatus").val()
    }

    if (!(model.departmentId > 0)) {
        return failedMsg("Please Select Department..!!");
    }

    const url = `${API}InventoryReport/DepartmentStockReportPrint?departmentId=${model.departmentId}&itemId=${model.itemId}&categoryId=${model.categoryId}&stockStatus=${model.stockStatus}`;
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