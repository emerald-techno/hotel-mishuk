let receiveItemList = [];

$(document).ready(function () {
    $("#Supplier-Sec").hide();
})

$(document.body).on("click", "#CreateReceiveSubmitBtn", function () {

    let receiveStatus = $("#ReceiveStatus").val();
    let orderId = $("#OrderId").val();
    let supplierId = $("#SupplierId").val();

    if (!(orderId > 0)) {
        return failedMsg("Please mention order information..!!");
    }

    if (!(supplierId > 0)) {
        return failedMsg("Please mention supplier information..!!");
    }

    if (receiveStatus == "") {
        $("#receiveStatusValidator").html("* Please Select Receive Status before submitting Receive");
    }

    if (receiveItemList.length > 0) {

        let isValid = true;
        let msg = "";

        receiveItemList.forEach(v => {
            if (!hasAnyError(v.qty) && !(v.qty > 0)) {
                isValid = false;
                msg = "Receive Item Quantity Have To More Than Zero";
            }

            if (v.qty > 0) {
                const totalRemainQty = parseInt(v.orderQty) - parseInt(v.actualRcvQty);

                if (totalRemainQty < v.qty) {
                    isValid = false;
                    msg = "Receive Item Quantity Higher Than Remain Quantity";
                }
            }
        });

        if (isValid) {
            startFormPosting("#CreateReceiveForm");
        } else {
            failedMsg(msg)
        }
    } else {
        failedMsg("No Items Found !!");
    }
});

$(document.body).on("change", "#OrderId", function () {

    const orderId = $(this).val();

    if (orderId > 0) {
        const orderUrl = `${API}Order/GetById/${orderId}`;

        $.get(orderUrl, function (rData) {
            if (rData !== undefined) {
                console.log("order-info", rData);

                $("#SupplierId").val(rData.supplierId);
                $("#Supplier-Sec").show();
                renderSupplier(rData.supplierName, rData.supplierMobile, rData.supplierAddress);
            } else {
                $("#SupplierId").val("");
                $("#Supplier-Sec").hide();
                renderSupplier("", "", "");
            }
        });
    } else {
        $("#SupplierId").val("");
        $("#Supplier-Sec").hide();
        renderSupplier("", "", "");
    }

    if (orderId > 0) {
        receiveItemList = [];

        const url = `${API}Order/OrderItemsForReceive?orderId=${orderId}`;

        $.get(url, function (rData) {
            if (rData !== undefined) {
                console.log("receive-item-list", rData);


                if (rData.length > 0) {
                    rData.forEach(v => {

                        const model = new ReceiveItem(
                            v.itemId,
                            v.itemName,
                            v.itemUnitId,
                            v.itemUnitName,
                            v.orderQty,
                            v.stock,
                            v.actualRcvQty,
                            v.itemQty,
                            v.unitPrice
                        );

                        receiveItemList.push(model);
                    });

                    createDetailTable();
                }
            }
        });
    } else {
        receiveItemList = [];
        createDetailTable();
    }
});

$(document.body).on("change", ".ItemQty", function () {
    const index = $(this).attr("data-index");
    const qtyValue = $(`#ItemQty_${index}`).val();

    if (index > -1) {
        receiveItemList[index].qty = qtyValue;
    }

    if (hasAnyError(qtyValue) || qtyValue <= 0) {
        $(`#ItemQty_Validate_${index}`).text("Have To Enter Value More Than Zero");
    } else {
        $(`#ItemQty_Validate_${index}`).text("");
    }
})

function createDetailTable() {
    $("#RcvItemTableTbody").empty();

    if (receiveItemList.length > 0) {

        receiveItemList.forEach((modelObject, index) => {

            const slNo = `<td>${index + 1}</td>`;
            const itemCell = `<td><input type='hidden' name='TranDtls[${index}].ItemId' value='${modelObject.itemId}' />${modelObject.itemName}</td>`;
            const unitCell = `<td><input type='hidden' name='TranDtls[${index}].ItemUnitId' id='unit_${index}' value='${modelObject.unitId}' />${modelObject.unitName}</td>`;
            const unitPriceCell = `<td><input type='number' name='TranDtls[${index}].UnitPrice' value='${modelObject.unitPrice}' class='form-control text-center' readonly/></td>`;
            const orderQtyCell = `<td><input type='hidden' name='TranDtls[${index}].OrderQty' value='${modelObject.orderQty}'"/>${modelObject.orderQty}</td>`;
            const alrdRcvCell = `<td><input type='hidden' name='' value='${modelObject.actualRcvQty}'"/>${modelObject.actualRcvQty}</td>`;
            const rcvQtyCell = `<td><input type='number' name='TranDtls[${index}].ItemQty' value='${modelObject.qty}' class='form-control ItemQty' id='ItemQty_${index}' data-index='${index}'/><span id='ItemQty_Validate_${index}' class="text-danger"></span></td>`;
            const stockCell = `<td><input type='number' name='TranDtls[${index}].Stock' value='${modelObject.stock}' class='form-control' readonly/></td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index})'><i class='fa fa-trash'></i></button></td>`;


            const row = "<tr id=item" + index + ">" + slNo + itemCell + unitCell + unitPriceCell + orderQtyCell + alrdRcvCell + rcvQtyCell + stockCell + actionCell + "</tr>";
            $("#RcvItemTableTbody").append(row);

        })
    } else {
        addNoDataFoundFooterWithMsg("#RcvItemTableTbody", "No Item Found For Receive !");
    }

}

function deleteItemRow(index) {

    if (index > -1) {
        receiveItemList.splice(index, 1);
    }

    createDetailTable();
}

function renderSupplier(name, mobile, address) {
    let html = `<h4>Supplier Info</h4>
                    <table class="table table-bordered">
                        <tbody>
                            <tr>
                                <td><b>Name</b></td>
                                <td>${name}</td>
                            </tr>
                            <tr>
                                <td><b>Mobile</b></td>
                                <td>${mobile}</td>
                            </tr>
                            <tr>
                                <td><b>Address</b></td>
                                <td>${address}</td>
                            </tr>
                        </tbody>
                    </table>`;

    $("#Supplier-Sec").html(html);
}

class ReceiveItem {
    constructor(itemId, itemName, unitId, unitName, orderQty, stock, actualRcvQty, qty, unitPrice) {
        this.itemId = itemId;
        this.itemName = itemName;
        this.unitId = unitId;
        this.unitName = unitName;
        this.orderQty = orderQty;
        this.stock = stock;
        this.actualRcvQty = actualRcvQty;
        this.qty = qty;
        this.unitPrice = unitPrice;
    }

}


