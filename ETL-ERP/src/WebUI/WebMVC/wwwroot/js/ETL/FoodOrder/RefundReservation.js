$(document.body).on("click", "#RefundSubmitBtn", function () {
    const orderId = $("#OrderId").val();
    const refundAmount = parseFloat($("#RefundAmount").val());
    const refundDue = parseFloat($("#RefundDue").val());

    if (refundAmount > refundDue) {
        return failedMsg("Refund Amount Is Higher Than Due Refund...!!");
    }

    if (orderId > 0 && refundAmount > 0) {
        $("#RefundForm").submit();
    }
});