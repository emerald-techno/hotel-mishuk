$(document).ready(function () {
    $(".form-control").attr("autocomplete", "off");
})
$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        MishukLedgerId: $("#MishukLedgerId").val(),
    }
    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "AccTranMst/ProfitOrLossStatement";
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
        MishukLedgerId: $("#MishukLedgerId").val(),
    }

    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
        const url = `${API}AccTranMst/ProfitOrLossStatementPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&mishukLedgerId=${model.MishukLedgerId}`;
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