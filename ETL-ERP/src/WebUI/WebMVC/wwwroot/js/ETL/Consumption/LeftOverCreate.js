var detailItemArray = [];

$(document).ready(function () {
    $("#Stock").val("0");
});

$(document.body).on("change", "#ItemId", function () {
    const itemId = $(this).val();
    const dptId = $("#IssueDeptId").val();

    if (itemId > 0) {
        if (!(dptId > 0)) {
            $(this).val("").trigger("change");
            return failedMsg("Please Set Department First..!!");
        }

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

        const stockUrl = API + "InventoryReport/GetItemCurrentStockByDptId";

        const stockParams = {
            dptId: dptId,
            itemId: itemId
        };

        $.post(stockUrl, stockParams, function (rData) {
            if (rData != null) {
                console.log("stockData:", rData);
                $("#Stock").val(rData.stock);
                $("#UnitPrice").val(rData.unitPrice);
            } else {
                failedMsg("Item Stock Info Not Found...!!");
            }
        }).fail(function () {
            failedMsg("Item Stock Info Not Found...!!");
        })
    }
});

$(document.body).on("click", "#AddItemBtn", function () {
    const itemName = $("#ItemId option:selected").text();
    const itemId = $("#ItemId").val();
    const unitName = $("#UnitId option:selected").text();
    const unitId = $("#UnitId").val();
    const unitPrice = parseFloat($("#UnitPrice").val());
    const qty = parseFloat($("#Qty").val());
    const stock = parseFloat($("#Stock").val());
    const remarks = $("#ItemRemarks").val();

    const model = {
        itemId: itemId,
        itemName: itemName,
        unitId: unitId,
        unitName: unitName,
        qty: qty,
        stock: stock,
        remarks: remarks,
        unitPrice: unitPrice
    }

    detailItemArray.push(model);

    createDetailTable();
    clearDetail();
});



$(document.body).on("click", "#CreateConsumptionSubmitBtn", function () {

    if (detailItemArray.length > 0) {

        let isValid = true;
        let msg = "";

        if (isValid) {
            startFormPosting("#CreateConsumptionForm");
        } else {
            failedMsg(msg);
        }
    } else {
        failedMsg("You did not make any consumption..!!");
    }
});


function createDetailTable() {
    $("#ItemTableTbody").empty();

    if (detailItemArray.length > 0) {

        detailItemArray.forEach((modelObject, index) => {

            const unitPriceCell = `<input type='hidden' name='TranDtls[${index}].UnitPrice' value='${modelObject.unitPrice}'/>`;

            const slNo = `<td>${index + 1}</td>`;
            const itemCell = `<td><input type='hidden' name='TranDtls[${index}].ItemId' value='${modelObject.itemId}'/>${modelObject.itemName}</td>`;
            const unitCell = `<td><input type='hidden' name='TranDtls[${index}].ItemUnitId' value='${modelObject.unitId}' />${unitPriceCell}${modelObject.unitName}</td>`;
            const reqQtyCell = `<td><input type='hidden' name='TranDtls[${index}].ItemQty' value='${modelObject.qty}'"/>${modelObject.qty}</td>`;
            const stockCell = `<td><input type='hidden' name='TranDtls[${index}].Stock' value='${modelObject.stock}' />${modelObject.stock}</td>`;
            const remarksCell = `<td><input type='hidden' name='TranDtls[${index}].Remarks' value='${modelObject.remarks}' />${modelObject.remarks}</td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index},${modelObject.itemId})'><i class='fa fa-trash'></i></button></td>`;


            const row = "<tr id=item" + index + ">" + slNo + itemCell + unitCell + reqQtyCell + stockCell + remarksCell + actionCell + "</tr>";
            $("#ItemTableTbody").append(row);
        });
    } else {
        addNoDataFoundFooterWithMsg("#ItemTableTbody", "No Item Added For Left Over...!!");
    }
}

function deleteItemRow(index, itemId) {
    if (index > -1) {
        detailItemArray.splice(index, 1);
    }

    createDetailTable();
}

function clearDetail() {
    $("#ItemId").val("").trigger("change");
    $("#UnitId").val("").trigger("change");
    $("#Qty").val('');
    $("#Stock").val(0);
    $("#ItemRemarks").val('');
}