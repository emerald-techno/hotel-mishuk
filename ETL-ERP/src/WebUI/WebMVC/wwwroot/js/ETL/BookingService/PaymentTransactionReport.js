$(document).ready(function () {
    const fromDate = $(".from_dt").val();
    const toDate = $(".to_dt").val();

    loadReport();
});

function loadReport() {
    const isEnable = $("#ShowOnlyDue").is(":checked");

    const model = {
        StrFromDate: $(".from_dt").val(),
        StrToDate: $(".to_dt").val(),
        PayMode: $("#PayMode").val(),
        PayType: $("#PayType").val(),
        ShowOnlyDue: isEnable
    }

    const params = { model: model };

    const url = API + "BookingService/PaymentTransactionReport";

    $("#ltReportFilter").html("Report From: <b>" + model.StrFromDate + "</b> To: <b>" + model.StrToDate + "</b>");

    $.post(url, params, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}


$(document.body).on("click", "#ShowReportBtn", function () {
    loadReport();
});

$(document.body).on("click", "#ReportPrintBtn", function (e) {
    e.preventDefault();
    const fromDate = $(".from_dt").val();
    const toDate = $(".to_dt").val();
    const payMode = $("#PayMode").val();
    const isEnable = $("#ShowOnlyDue").is(":checked");

    const url = `${API}BookingService/PaymentTransactionReportPrint?strFromDate=${fromDate}&strToDate=${toDate}&payMode=${payMode}&showOnlyDue=${isEnable}`;
    window.open(url, "_blank");
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<h5 style='text-align:center'>No Data Found<h5>");
    }

}