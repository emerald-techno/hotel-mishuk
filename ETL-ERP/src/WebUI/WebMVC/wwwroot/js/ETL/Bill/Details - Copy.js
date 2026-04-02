$(document.body).on("click", "#PayBillBtn", function () {
    const billId = $("#BillId").val();
    const payAmount = $("#PaidAmount").val();

    if (billId > 0 && payAmount > 0) {
        $("#PayBillForm").submit();
    }
});