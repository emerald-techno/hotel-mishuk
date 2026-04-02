$(document).ready(function () {
    calculateNetTotal();
});

$(document.body).on("click", "#PaymentEntryBtn", function () {
    const bookingId = $("#bookingId").val();
    const paymode = $("#PayMode").val();
    const paidAmount = $("#PayAmount").val();
    const transactionNo = $("#TransactionNo").val();
    const accountNo = $("#AccountNo").val();
    const paidDateStr = $("#PaidDateStr").val();

    if (bookingId > 0 && paidAmount > 0) {
        const url = API + "BookingService/PaymentEntry";

        const params = {
            bookingId: bookingId,
            payMode: paymode,
            paidAmount: paidAmount,
            transactionNo: transactionNo,
            accountNo: accountNo,
            paidDateStr: paidDateStr
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Payment Entry Successful");
                $("#paymentEntryModal").modal('hide');
                clearPaymentForm();

                setTimeout(() => {
                    window.location.href = API + "BookingService/HallDetails/" + bookingId;
                }, 2000);
            } else {
                failedMsg("Payment Entry Failed");
            }
        }).fail(function () {
            failedMsg("Payment Entry Failed");
        })
    } else {
        failedMsg("Booking info and payment amount is required");
    }
})

function clearPaymentForm() {
    $("#PayMode").val("");
    $("#PaidAmount").val("");
    $("#TransactionNo").val("");
    $("#AccountNo").val("");
}

$(document.body).on("change", "#Vat", function () {
    calculateNetTotal();
});

$(document.body).on("change", "#Tax", function () {
    calculateNetTotal();
});

$(document.body).on("change", "#Discount", function () {
    calculateNetTotal();
});

function calculateNetTotal() {
    const netRent = parseFloat($("#NetRent").val());
    const paidAmount = parseFloat($("#PaidAmount").val());
    const vat = parseFloat($("#Vat").val());
    const tax = parseFloat($("#Tax").val());
    const discount = parseFloat($("#Discount").val());

    const netTotal = (netRent + vat + tax) - (discount);

    console.log("Net Total: ", netTotal);
    $("#net-total").val(netTotal);

    let dueAmount = netTotal > paidAmount ? netTotal - paidAmount : 0;
    $("#due-amount").val(dueAmount);
}

$(document.body).on("click", "#CompleteSubmitBtn", function () {
    const bookingId = $("#bookingId").val();

    if (bookingId > 0) {
        $("#CompleteBookingForm").submit();
    }
});