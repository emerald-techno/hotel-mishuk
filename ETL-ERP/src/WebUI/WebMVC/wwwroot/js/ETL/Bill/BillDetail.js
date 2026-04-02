$(document).ready(function () {
    ShowBillDetail();
})
function ShowBillDetail() {
    const billId = $("#BillId").val();
    const url = API + "Bill/GetBillingDetailById";
    const params = { id: billId };
    $.post(url, params, function (rData) {
        //console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}
function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<b>No Data Found</b>");
    }

}
