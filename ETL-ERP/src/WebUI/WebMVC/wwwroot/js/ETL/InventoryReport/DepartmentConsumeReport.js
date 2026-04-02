$(document.body).on("click", "#ShowReportBtn", function () {
    const model = getSearchObject();
    const url = API + "InventoryReport/DepartmentConsumeReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});

$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = getSearchObject();

    const url = `${API}InventoryReport/DepartmentConsumeReportPrint?strFromDate=${model.StrFromDate}&strToDate=${model.StrToDate}&departmentId=${model.DepartmentId}&categoryId=${model.CategoryId}&itemId=${model.ItemId}`;
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
function getSearchObject() {
    const model = {
        DepartmentId: $("#DepartmentId").val(),
        ItemId: $("#ItemId").val(),
        CategoryId: $("#CategoryId").val(),
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        DepartmentText: $("#DepartmentId option:selected").text()
    };

    return model;
}