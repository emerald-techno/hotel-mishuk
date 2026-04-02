$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrQueryDate: $("#StrQueryDate").val()
    }

    $("#ltReportFilter").html("Date : " + model.StrQueryDate);
    const url = API + "FoodOrder/RsDailySalesSummary";
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
    console.log(model);
    const url = `${API}FoodOrder/RsDailySalesSummaryPrint?StrQueryDate=${model.StrQueryDate}`;
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