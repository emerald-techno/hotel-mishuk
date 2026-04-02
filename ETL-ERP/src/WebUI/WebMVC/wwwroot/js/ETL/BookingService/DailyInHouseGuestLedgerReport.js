$(document).ready(function () {
    ShowReportHtml();
})


$(document.body).on("click", "#ShowReportBtn", function () {
    ShowReportHtml();
});
function ShowReportHtml() {
    const model = {
        StrQueryDate: $("#StrQueryDate").val(),
    }
    const url = `${API}BookingService/DailyInHouseGuestLedgerReport`;
    const params = { vm: model };
    $.post(url, params, function (rData) {
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}

$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        StrReportDate: $("#StrQueryDate").val(),
    }

    if (!hasAnyError(model.StrReportDate) && !hasAnyError(model.StrReportDate)) {
        const url = `${API}BookingService/DailyInHouseGuestLedgerReportPrint?strReportDate=${model.StrReportDate}`;
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