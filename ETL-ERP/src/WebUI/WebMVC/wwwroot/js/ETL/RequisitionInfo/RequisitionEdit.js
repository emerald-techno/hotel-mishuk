var itemList = [];

$(document).ready(function () {
    getRequsitionInfo();
    $("#Stock").val("0");
});

function getRequsitionInfo() {
    const reqId = $("#Id").val();

    if (reqId > 0) {
        const url = API + "RequsitionInfo/GetById/" + reqId;

        $.get(url, function (rData) {
            if (rData !== undefined) {
                console.log("requsition-info", rData);

                if (rData.requsitionInfoDtls.length > 0) {

                    rData.requsitionInfoDtls.forEach(v => {
                        console.log("detail_value", v);

                        const model = {
                            id: v.id,
                            itemId: v.itemId,
                            itemName: v.itemName,
                            unitId: v.itemUnitId,
                            unitName: v.itemUnitName,
                            reqQty: v.reqQty,
                            stock: v.stock,
                        }

                        itemList.push(model);
                    });

                    createDetailTable();
                }
            }
        });
    }
}

$(document.body).on("click", "#AddItemBtn", function () {
    const itemName = $("#ItemId option:selected").text();
    const itemId = $("#ItemId").val();
    const unitName = $("#UnitId option:selected").text();
    const unitId = $("#UnitId").val();
    const qty = $("#Qty").val();
    const stock = $("#Stock").val();

    const model = {
        id: 0,
        itemId: itemId,
        itemName: itemName,
        unitId: unitId,
        unitName: unitName,
        reqQty: qty,
        stock: stock
    }

    if (!(model.itemId > 0)) {
        failedMsg("Please Select item to add Detail");
        return false;
    }

    if (!(model.reqQty > 0)) {
        failedMsg("Please Mention Quantity Properly, It should be greater than Zero");
        return false;
    }

    if (itemList.length > 0) {
        var item = itemList.find(x => x.itemId == model.itemId);

        if (item != null || item != undefined) {
            failedMsg("Item Already Added");
        } else {
            itemList.push(model);
        }
    } else {
        itemList.push(model);
    }

    createDetailTable();
    clearDetail();
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

function deleteItemRow(index, itemId) {
    if (index > -1) {
        itemList.splice(index, 1);
    }

    createDetailTable();
}

function clearDetail() {
    $("#ItemId").val("").trigger("change");
    $("#UnitId").val("").trigger("change");
    $("#Qty").val('');
    $("#Stock").val(0);
}

function createDetailTable() {

    if (itemList.length > 0) {

        $("#ItemTableTbody").empty();

        itemList.forEach((modelObject, index) => {

            const updateBtn = `<button type='button' class='btn btn-icon btn-outline-warning update-btn' data-item-id='${modelObject.itemId}'><i class='fa fa-edit'></i></button>`;

            const slNo = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].Id' value='${modelObject.id}'/>${index + 1}</td>`;
            const itemCell = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].ItemId' value='${modelObject.itemId}'/>${modelObject.itemName}</td>`;
            const unitCell = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].ItemUnitId' value='${modelObject.unitId}' />${modelObject.unitName}</td>`;
            const reqQtyCell = `<td><input type='number' class='form-control' name='RequsitionInfoDtls[${index}].ReqQty' value='${modelObject.reqQty}'"/></td>`;
            const stockCell = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].Stock' value='${modelObject.stock}' />${modelObject.stock}</td>`;
            const actionCell = `<td><button class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index}, ${modelObject.itemId})'><i class='fa fa-trash'></i></button></td>`;

            const row = "<tr id=item" + index + ">" + slNo + itemCell + unitCell + reqQtyCell + stockCell + actionCell + "</tr>";
            $("#ItemTableTbody").append(row);
        });
    }
}

$(document.body).on("click", "#UpdateReqBtn", function () {

    const deptId = $("#DeptId").val();

    if (!(deptId > 0)) {
        return failedMsg("Please Add Department..!!");
    }

    if (itemList.length > 0) {

        let isValid = true;
        let msg = "";

        if (isValid) {
            startFormPosting("#UpdateRequsitionForm");
        } else {
            failedMsg(msg);
        }
    } else {
        failedMsg("You did not make any changes in requsition !!");
    }
});
