let bookingInfo = null;

$(document).ready(function () {
    $("#booking-sec").hide();
    //addNoDataFoundFooterWithMsg("#ReceiveTBody", "Please Select Order First..!");
})

$(document.body).on("change", "#BookingId", function () {
    $("#booking-sec").html('');
    $("#booking-sec").hide();

    const bookingId = $(this).val();

    if (bookingId > 0) {
        const url = `${API}Refund/GetCancelBookingInfoBookingId?bookingId=${bookingId}`;

        $.get(url, function (rData) {
            if (rData !== null && rData !== undefined) {
                console.log("Booking Info:", rData);
                bookingInfo = rData;

                renderBookingInfo();
            }
        });
    }
});

function renderBookingInfo() {

    if (bookingInfo != null && bookingInfo != undefined) {

        let bookingDate = convertJsonFullDateForView(new Date(bookingInfo.bookingDate));
        let checkInDate = convertJsonFullDateForView(new Date(bookingInfo.bookingCheckInDate));
        let checkOutDate = convertJsonFullDateForView(new Date(bookingInfo.bookingCheckOutDate));

        const html = `<table class="table table-bordered" style="width:100%;">
                            <thead>
                                <tr class="text-start">
                                    <th colspan="2"><h4>Booking Info</h4></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Booking No</b></td>
                                    <td style='width:70%;'>${bookingInfo.bookingNo}</td>
                                </tr>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Booking Date</b></td>
                                    <td style='width:70%;'>${bookingDate}</td>
                                </tr>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Check In Date</b></td>
                                    <td style='width:70%;'>${checkInDate}</td>
                                </tr>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Check Out Date</b></td>
                                    <td style='width:70%;'>${checkOutDate}</td>
                                </tr>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Net Amount</b></td>
                                    <td style='width:70%;'>
                                        <input id='net-amount' class='form-control' value='${bookingInfo.bookingNetAmount}' readonly/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Advance Amount</b></td>
                                    <td style='width:70%;'>
                                        <input id='advance-amount' class='form-control' value='${bookingInfo.paidAmount}' readonly/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Already Refunded Amount</b></td>
                                    <td style='width:70%;'>
                                        <input id='already-refund-amount' class='form-control' value='${bookingInfo.alreadyRefundAmount}' readonly/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class='text-start' style='width:30%;'><b>Refund Amount</b></td>
                                    <td style='width:70%;'>
                                        <input id='refund-amount' name='RefundAmount' class='form-control' value='0'/>
                                    </td>
                                </tr>
                            </tbody>
                        </table>`;

        $("#booking-sec").html(html);
        $("#booking-sec").show();
    }
}

$(document.body).on("click", "#RefundSubmitBtn", function () {
    const bookingId = $("#BookingId").val();
    const refundAmount = parseFloat($("#refund-amount").val());
    const advanceAmount = parseFloat($("#advance-amount").val());
    const alreadyRefundAmount = parseFloat($("#already-refund-amount").val());

    let totalRefundAmount = refundAmount + alreadyRefundAmount;

    if (totalRefundAmount > advanceAmount) {
        return failedMsg("Refund Amount Is Higher Than Given Advance Amount..!");
    }

    if (bookingId > 0 && refundAmount > 0) {
        $("#RefundForm").submit();
    }
});