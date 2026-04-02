$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        CategoryId: $("#CategoryId").val(),
        ItemId: $("#ItemId").val(),
        GroupBy: $("#GroupBy").val(),
    }
    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "FoodOrder/RsSalesReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        //console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});


$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        CategoryId: $("#CategoryId").val(),
        ItemId: $("#ItemId").val(),
        GroupBy: $("#GroupBy").val(),
    }

    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
        const url = `${API}FoodOrder/RsSalesReportPrint?strFromDate=${model.StrFromDate}&strToDate=${model.StrToDate}&cateogryId=${model.CategoryId}&itemId=${model.ItemId}&groupBy=${model.GroupBy}`;
        window.open(url, "_blank");
    }
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<b>No Data Found</b>");
    }
}