$(document.body).on("click", "#ShowReportBtn", function () {
    const d = new Date();
    let text = convertJsonFullDateForView(d);

    const model = {
        Year: $("#Year").val(),
        Month: $("#Month").val(),
        DepartmentId: $("#DepartmentId").val()
    }

    $("#ltReportFilter").html("Date : " + text);
    const url = API + "PrSalaryMst/PayrollReport";
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
        Year: $("#Year").val(),
        Month: $("#Month").val(),
        DepartmentId: $("#DepartmentId").val()
    }

    const url = `${API}PrSalaryMst/PayrollReportPrint?year=${model.Year}&month=${model.Month}&departmentId=${model.DepartmentId}`;
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