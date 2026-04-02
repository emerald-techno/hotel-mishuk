let openingItemList = [];

$(document).ready(function () {
    getInventoryOpenignData();
})

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
    }
});

$(document.body).on("click", "#AddItemBtn", function () {
    const itemName = $("#ItemId option:selected").text();
    const itemId = $("#ItemId").val();
    const unitName = $("#UnitId option:selected").text();
    const unitId = $("#UnitId").val();
    const qty = parseFloat($("#Qty").val());
    const unitPrice = parseFloat($("#Rate").val());
    const remarks = $("#ItemRemarks").val();

    const model = {
        id: 0,
        itemId: itemId,
        itemName: itemName,
        itemUnitId: unitId,
        itemUnitName: unitName,
        itemQty: qty,
        unitPrice: unitPrice,
        remarks: remarks
    }

    if (!(model.itemId > 0)) {
        failedMsg("Please Select item to add Detail");
        return false;
    }

    if (!(model.itemUnitId > 0)) {
        failedMsg("Please Select unit to add Detail");
        return false;
    }

    if (!(model.itemQty > 0)) {
        failedMsg("Please Mention Quantity Properly, It should be greater than Zero");
        return false;
    }

    if (openingItemList.length > 0) {
        var item = openingItemList.find(x => x.itemId == model.itemId);

        if (item != null || item != undefined) {
            failedMsg("Item Already Added");
        } else {
            openingItemList.push(model);
        }
    } else {
        openingItemList.push(model);
    }

    createDetailTable();
    clearDetail();
});

function getInventoryOpenignData() {

    openingItemList = [];

    const dptId = $("#IssueDeptId").val();

    if (dptId > 0) {

        const url = `${API}InventoryOpening/GetInventoryOpenignDataByDeptId?dptId=${dptId}`;

        $.get(url, function (rData) {
            if (rData != null) {
                console.log("Inventory Opening Data", rData);

                $("#Id").val(rData.id);
                $("#IsOpenignUpdate").val(true);

                if (rData.tranDtls != null && rData.tranDtls.length > 0) {

                    rData.tranDtls.forEach(v => {
                        const model = {
                            id: v.id,
                            itemId: v.itemId,
                            itemName: v.itemName,
                            itemUnitId: v.itemUnitId,
                            itemUnitName: v.itemUnitName,
                            itemQty: v.itemQty,
                            unitPrice: v.unitPrice,
                            remarks: v.remarks
                        }

                        openingItemList.push(model);
                    });

                    createDetailTable();
                }

            } else {
                $("#Id").val(0);
                $("#IsOpenignUpdate").val(false);

                createDetailTable();
            }
        })
    } else {
        return failedMsg("Department Information Not Found...!");
    }
}

function createDetailTable() {
    $("#ItemTableTbody").empty();

    if (openingItemList.length > 0) {

        openingItemList.forEach((modelObject, index) => {

            const slNo = `<td>${index + 1}</td>`;
            const itemCell = `<td><input type='hidden' name='TranDtls[${index}].ItemId' value='${modelObject.itemId}'/>${modelObject.itemName}</td>`;
            const unitCell = `<td><input type='hidden' name='TranDtls[${index}].ItemUnitId' value='${modelObject.itemUnitId}' />${modelObject.itemUnitName}</td>`;
            const qtyCell = `<td><input type='hidden' name='TranDtls[${index}].ItemQty' value='${modelObject.itemQty}'"/>${modelObject.itemQty}</td>`;
            const unitPriceCell = `<td><input type='hidden' name='TranDtls[${index}].UnitPrice' value='${modelObject.unitPrice}' />${modelObject.unitPrice}</td>`;
            const remarksCell = `<td><input type='hidden' name='TranDtls[${index}].Remarks' value='${modelObject.remarks}' />${modelObject.remarks}</td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index},${modelObject.itemId})'><i class='fa fa-trash'></i></button></td>`;


            const row = "<tr id=item" + index + ">" + slNo + itemCell + unitCell + qtyCell + unitPriceCell + remarksCell + actionCell + "</tr>";
            $("#ItemTableTbody").append(row);
        });
    } else {
        addNoDataFoundFooterWithMsg("#ItemTableTbody", "No Item Added For Opening...!!");
    }
}

function deleteItemRow(index, itemId) {
    if (index > -1) {
        openingItemList.splice(index, 1);
    }
    createDetailTable();
}

function clearDetail() {
    $("#ItemId").val("").trigger("change");
    $("#UnitId").val("").trigger("change");
    $("#Qty").val(0);
    $("#Rate").val(0);
    $("#ItemRemarks").val("");
}

$(document.body).on("click", "#OpeningSubmitBtn", function () {

    if (openingItemList.length > 0) {

        let isValid = true;
        let msg = "";

        if (isValid) {
            startFormPosting("#InevntoryOpeningForm");
        } else {
            failedMsg(msg);
        }
    } else {
        failedMsg("You did not make any opening..!!");
    }
});