$(document.body).on("click", "#ShowReportBtn", function () {
    const d = new Date();
    let text = convertJsonFullDateForView(d);

    const model = {
        GuestId: $("#GuestId").val(),
        CompanyId: $("#CompanyId").val(),
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        ReporetGroupBy: $("#ReporetGroupBy").val()
    }

    $("#ltReportFilter").html("Date : " + text);
    const url = API + "BookingService/GuestDueReport";
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
        GuestId: $("#GuestId").val(),
        CompanyId: $("#CompanyId").val(),
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        ReporetGroupBy: $("#ReporetGroupBy").val()
    }

    const url = `${API}BookingService/GuestDueReportPrint?guestId=${model.GuestId}&companyId=${model.CompanyId}
    &strFromDate=${model.StrFromDate}&strToDate=${model.StrToDate}&groupBy=${model.ReporetGroupBy}`;
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