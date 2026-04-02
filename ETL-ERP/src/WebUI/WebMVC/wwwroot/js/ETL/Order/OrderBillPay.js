let billInfo = null;

$(document).ready(function () {
    $("#order-sec").hide();
    addNoDataFoundFooterWithMsg("#ReceiveTBody", "Please Select Order First..!");

    $("#BillAmount").val("");
})

$(document.body).on("change", "#OrderMstId", function () {
    const orderId = $(this).val();

    if (orderId > 0) {
        //const url = API + "Order/OrderBill/" + orderId;

        const url = `${API}Order/OrderBill?orderId=${orderId}`;

        $.get(url, function (rData) {
            if (rData !== null && rData !== undefined) {
                console.log("Bill Info:", rData);
                billInfo = rData;

                renderSupplierSection();
                renderDetailTableBody();
            }
        });
    }
});

function renderSupplierSection() {

    if (billInfo != null && billInfo != undefined) {
        const html = `<table>
                        <thead>
                            <tr colspan='2'>
                                <th><h5>Supplier Info</h5></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Name</td>
                                <td>
                                    <input type='hidden' id='SupplierId' name='SupplierId' value='${billInfo.supplierId}'/>
                                    ${billInfo.supplierName}
                                </td>
                            </tr>
                            <tr>
                                <td>Mobile</td>
                                <td>${billInfo.supplierMobile}</td>
                            </tr>
                            <tr>
                                <td>Address</td>
                                <td>${billInfo.supplierAddress}</td>
                            </tr>
                        </tbody>
                    </table>`;

        $("#order-sec").html(html);
        $("#order-sec").show();
    }
}

function renderDetailTableBody() {

    $("#ReceiveTBody").empty();

    if (billInfo != null && hasDataInArray(billInfo.billSaveReceiveVms)) {

        billInfo.billSaveReceiveVms.forEach((modelObject, index) => {

            const slNo = `<td>${index + 1}</td>`;

            let rcvDate = convertJsonFullDateForView(new Date(modelObject.rcvTranDate));
            const rcvInfo = `<div>
                                    <h5>${modelObject.rcvTranNo}</h5>
                                    <b>${rcvDate}</b>
                                  </div>`;

            const rcvInfoCell = `<td>${rcvInfo}</td>`;
            const amountCell = `<td class='text-end'>${modelObject.rcvAmount}</td>`;

            const row = "<tr>" + slNo + rcvInfoCell + amountCell + "</tr>";

            $("#ReceiveTBody").append(row);
        });

        const totalRow = `<tr>
                            <td colspan='2' class='text-end'><b>Total</b></td>
                            <td class='text-end'><b>${billInfo.totalAmount}</b></td>
                          </tr>`;

        const paidRow = `<tr>
                            <td colspan='2' class='text-end'><b>Paid Amount</b></td>
                            <td class='text-end'><b>${billInfo.paidAmount}</b></td>
                          </tr>`;

        const dueRow = `<tr>
                            <td colspan='2' class='text-end'><b>Due</b></td>
                            <td class='text-end'><b>${billInfo.dueAmount}</b></td>
                          </tr>`;

        const footRow = `${totalRow}${paidRow}${dueRow}`;

        $("#ReceiveTBody").append(footRow);

    } else {
        addNoDataFoundFooterWithMsg("#ReceiveTBody", "No Receive Found For Order !");
    }
}

$(document.body).on("click", "#OrderPaySubmitBtn", function () {
    const orderId = $("#OrderMstId").val();
    const supplierId = $("#SupplierId").val();
    const billAmount = $("#BillAmount").val();

    if (billAmount > billInfo.dueAmount) {
        return failedMsg("Amount Is Higher Than Due Amount..!");
    }

    if (orderId > 0 && supplierId > 0 && billAmount > 0) {
        $("#OrderBillPaymentForm").submit();
    }
});