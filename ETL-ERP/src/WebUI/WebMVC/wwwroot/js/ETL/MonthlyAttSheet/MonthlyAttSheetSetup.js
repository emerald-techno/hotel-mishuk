$(document.body).on("click", "#GanerateReportBtn", function () {
    loadReportPartial();
});

function loadReportPartial() {

    const year = $("#Year").val();
    const month = $("#Month").val();


    if (year > 0 && month > 0) {
        const url = API + "MonthlyAttSheet/GetMonthlyAttSheetReportPartial";
        const params = { year: year, month: month };
        loadPartialWithParams(url, params, "#ReportPartialDiv", null);
    }

}

$(document.body).on("click", "#PrintReportBtn", function () {
    printReport();
});

function printReport() {
    const year = $("#Year").val();
    const month = $("#Month").val();

    if (year > 0 && month > 0) {
        const url = `${API}MonthlyAttSheet/PrintMonthlyAttSheet?year=${year}&month=${month}`;
        window.open(url, "_blank");
    }
}

$(document.body).on("click", "#ManualGanerateReportBtn", function () {
    loadManualReportPartial();
});

function loadManualReportPartial() {
    const year = $("#Year").val();
    const month = $("#Month").val();

    if (year > 0 && month > 0) {
        const url = API + "MonthlyAttSheet/GetMonthlyManualAttSheetReportPartial";
        const params = { year: year, month: month };
        loadPartialWithParams(url, params, "#ReportPartialDiv", null);
    }
}