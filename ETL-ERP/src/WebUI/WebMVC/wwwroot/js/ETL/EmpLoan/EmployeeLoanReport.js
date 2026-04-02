$(document.body).on("click", "#ShowReportBtn", function () {
    const d = new Date();
    let text = convertJsonFullDateForView(d);

    const model = {
        employeeId: $("#EmployeeId").val(),
        strFromDate: $("#StrFromDate").val(),
        strToDate: $("#StrToDate").val()
    }

    $("#ltReportFilter").html("Date : " + text);
    const url = API + "EmpLoanMst/EmployeeLoanReport";
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
        emplyeeId: $("#EmployeeId").val(),
        strFromDate: $("#StrFromDate").val(),
        strToDate: $("#StrToDate").val()
    }

    const url = `${API}EmpLoanMst/EmployeeLoanReportPrint?strFromDate=${model.strFromDate}&strToDate=${model.strToDate}&employeeId=${model.emplyeeId}`;
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