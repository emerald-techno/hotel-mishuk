$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        CategoryId: $("#CategoryId").val(),
        ItemId: $("#ItemId").val(),
        DepartmentId: $("#DepartmentId").val(),
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
    }
    const url = API + "InventoryReport/IssueReport";
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
        DepartmentId: $("#DepartmentId").val(),
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
    }

    const url = `${API}InventoryReport/IssueReportPrint?categoryId=${model.CategoryId}&itemId=${model.ItemId}&departmentId=${model.DepartmentId}&strFromDate=${model.StrFromDate}&strToDate=${model.StrToDate}`;
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