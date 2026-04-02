let orderItemList = [];
let supplierItems = [];
let total = 0;

$(document).ready(function () {
    $("#ItemTableFoot").hide();
    $("#Stock").val(0);
});

$(document.body).on("change", "#ItemId", function () {
    const itemId = $(this).val();

    if (itemId > 0) {
        const url = API + "ItemInfo/GetUnitByItemId";

        const params = {
            id: itemId
        };

        $.post(url, params, function (rData) {
            if (rData != null) {
                console.log("unit:", rData);
                $("#UnitId").val(rData.unitId).trigger("change");
            } else {
                failedMsg("Item Unit Info Not Found...!!");
            }
        }).fail(function () {
            failedMsg("Item Unit Info Not Found...!!");
        })

        const stockUrl = API + "ItemInfo/GetItemCurrentStockByItemId";

        $.post(stockUrl, params, function (rData) {
            console.log("stock", rData);
            if (!hasAnyError(rData)) {
                $("#Stock").val(rData);
            }
        });
    }
});

$(document.body).on("click", "#OrderCreateSubmitBtn", function () {
    if (orderItemList.length > 0) {
        let isValid = true;
        let msg = "";

        if (isValid) {
            startFormPosting("#OrderCreateForm");
        } else {
            failedMsg(msg);
        }

    } else {
        failedMsg("You did not make any Order For Items!!");
    }
})

$(document.body).on("change", "#ReqId", function () {
    requsitionItemLoad();
});

$(document.body).on("click", "#AddItemBtn", function () {

    total = 0;

    const itemName = $("#ItemId option:selected").text();
    const itemId = $("#ItemId").val();
    const unitName = $("#UnitId option:selected").text();
    const unitId = $("#UnitId").val();
    const stock = $("#Stock").val();
    const rate = $("#Rate").val();
    const orderQty = $("#Qty").val();

    const model = new Item(itemId, itemName, unitId, unitName, stock, 0, orderQty, rate);

    if (!(model.itemId > 0)) {
        failedMsg("Please Select item to add Detail");
        return false;
    }
    if (!(model.qty > 0)) {
        failedMsg("Please Mention some Order Quantity");
        return false;
    }

    if (!(model.rate > 0)) {
        failedMsg("Item rate should be more than Zero");
        return false;
    }

    if (orderItemList.length > 0) {
        var item = orderItemList.find(x => x.itemId == model.itemId);

        if (item != null || item != undefined) {
            failedMsg("Item Already Added");
        } else {
            orderItemList.push(model);
        }
    }
    else {
        orderItemList.push(model);
    }

    createDetailTableBody();
    calculateTotal();
    clearDetail();
});

$(document.body).on("change", ".rate", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        const qty = $(`#OrderQty_${index}`).val();
        const rate = $(`#OrderRate_${index}`).val();

        orderItemList[index].rate = rate;

        const subTotal = parseFloat(qty) * parseFloat(rate);

        $(`#OrderSubTotal_${index}`).val(subTotal);

        calculateTotal();
    }
});

$(document.body).on("change", ".qty", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        const qty = $(`#OrderQty_${index}`).val();
        const rate = $(`#OrderRate_${index}`).val();

        orderItemList[index].qty = qty;

        const subTotal = parseFloat(qty) * parseFloat(rate);

        $(`#OrderSubTotal_${index}`).val(subTotal);

        calculateTotal();
    }
});

function requsitionItemLoad() {
    const reqId = $("#ReqId").val();

    if (reqId > 0) {
        const url = API + "RequsitionInfo/GetById/" + reqId;

        $.get(url, function (rData) {
            if (rData.requsitionInfoDtls.length > 0) {

                orderItemList = [];

                rData.requsitionInfoDtls.forEach(v => {
                    const model = new Item(v.itemId, v.itemName, v.itemUnitId,
                        v.itemUnitName, v.stock, v.reqQty, v.reqQty, 0);

                    orderItemList.push(model);
                });

                createDetailTableBody();
                calculateTotal();
            }
        })
    }
}

function createDetailTableBody() {

    $("#ItemTableTbody").empty();

    if (hasDataInArray(orderItemList)) {

        $("#ItemTableFoot").show();

        orderItemList.forEach((modelObject, index) => {

            const slNo = `<td>${index + 1}</td>`;
            const itemCell = `<td><input type='hidden' name='OrderDtls[${index}].ItemId' value='${modelObject.itemId}' />${modelObject.itemName}</td>`;
            const unitCell = `<td><input type='hidden' name='OrderDtls[${index}].ItemUnitId' value='${modelObject.unitId}' />${modelObject.unitName}</td>`;
            const stockCell = `<td><input type='hidden' name='OrderDtls[${index}].Stock' value='${modelObject.stock}' readonly/>${modelObject.stock}</td>`;
            const reqQtyCell = `<td><input type='hidden' value='${modelObject.reqQty}' readonly/>${modelObject.reqQty}</td>`;
            const orderQty = `<td><input type='number' name='OrderDtls[${index}].OrderQty' value='${modelObject.qty}' id='OrderQty_${index}' class='form-control qty' data-index='${index}'/></td>`;
            const rateCell = `<td><input type='number' name='OrderDtls[${index}].Rate' value='${modelObject.rate}' id='OrderRate_${index}' class='form-control rate' data-index='${index}'/></td>`;
            const subTotalCell = `<td><input type='number' value='${modelObject.calcSum()}' id='OrderSubTotal_${index}' class='form-control subTotal' readonly/></td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index})'><i class='fa fa-trash'></i></button></td>`;


            const row = "<tr id=item" + index + ">" + slNo + itemCell + unitCell + stockCell + reqQtyCell + orderQty + rateCell + subTotalCell + actionCell + "</tr>";

            $("#ItemTableTbody").append(row);
        });
    } else {
        addNoDataFoundFooterWithMsg("#ItemTableTbody", "No Approved Item Found For Order !");
        $("#ItemTableFoot").hide();
    }

}

function calculateTotal() {
    let total = 0;
    if (orderItemList.length > 0) {

        orderItemList.forEach(v => {
            total += v.qty * v.rate;
        })

        $(`#total`).text(total);
    }
}

function deleteItemRow(index, itemId) {
    if (index > -1) {
        orderItemList.splice(index, 1);
    }

    createDetailTableBody();
    calculateTotal();
}

function clearDetail() {
    $("#ItemId").val("").trigger("change");
    $("#UnitId").val("").trigger("change");
    $("#Qty").val(0);
    $("#Rate").val(0);
    $("#Stock").val(0);
}

class Item {
    constructor(itemId, itemName, unitId, unitName, stock, reqQty, qty, rate) {
        this.itemId = itemId;
        this.itemName = itemName;
        this.unitId = unitId;
        this.unitName = unitName;
        this.stock = stock;
        this.reqQty = reqQty;
        this.qty = qty;
        this.rate = rate;
    }

    calcSum() {
        return this.qty * this.rate;
    }
}


