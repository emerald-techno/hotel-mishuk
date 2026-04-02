var detailItemArray = [];

$(document).ready(function () {
    $("#Stock").val("0");
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

$(document.body).on("click", "#AddItemBtn", function () {
    const itemName = $("#ItemId option:selected").text();
    const itemId = $("#ItemId").val();
    const unitName = $("#UnitId option:selected").text();
    const unitId = $("#UnitId").val();
    const qty = $("#Qty").val();
    const stock = $("#Stock").val();
    const remarks = $("#Dtl_Remarks").val();

    const model = {
        itemId: itemId,
        itemName: itemName,
        unitId: unitId,
        unitName: unitName,
        reqQty: qty,
        stock: stock,
        remarks: remarks
    }

    if (!(model.itemId > 0)) {
        failedMsg("Please Select item to add Detail");
        return false;
    }

    if (!(model.reqQty > 0)) {
        failedMsg("Please Mention Quantity Properly, It should be greater than Zero");
        return false;
    }

    if (detailItemArray.length > 0) {
        var item = detailItemArray.find(x => x.itemId == model.itemId);

        if (item != null || item != undefined) {
            failedMsg("Item Already Added");
        } else {
            detailItemArray.push(model);
        }
    } else {
        detailItemArray.push(model);
    }

    $('#ItemId').select2('focus');

    createDetailTable();
    clearDetail();
});


$(document.body).on("click", "#CreateRequsitionSubmitBtn", function () {

    const deptId = $("#DeptId").val();

    if (!(deptId > 0)) {
        return failedMsg("Please Add Department..!!");
    }

    if (detailItemArray.length > 0) {

        let isValid = true;
        let msg = "";

        if (isValid) {
            startFormPosting("#CreateRequsitionForm");
        } else {
            failedMsg(msg);
        }
    } else {
        failedMsg("You did not make any requsition !!");
    }
});


function createDetailTable() {

    $("#ItemTableTbody").empty();

    if (detailItemArray.length > 0) {

        detailItemArray.forEach((modelObject, index) => {

            const slNo = `<td>${index + 1}</td>`;
            const itemCell = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].ItemId' value='${modelObject.itemId}'/>${modelObject.itemName}</td>`;
            const unitCell = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].ItemUnitId' value='${modelObject.unitId}' />${modelObject.unitName}</td>`;
            const reqQtyCell = `<td><input type='number' class='form-control text-center' name='RequsitionInfoDtls[${index}].ReqQty' id='reqQty_${index}' value='${modelObject.reqQty}' data-index='${index}'"/></td>`;
            const stockCell = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].Stock' value='${modelObject.stock}' />${modelObject.stock}</td>`;
            const remarksCell = `<td><input type='hidden' name='RequsitionInfoDtls[${index}].Remarks' value='${modelObject.remarks}' />${modelObject.remarks}</td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index},${modelObject.itemId})'><i class='fa fa-trash'></i></button></td>`;


            const row = "<tr id=item" + index + ">" + slNo + itemCell + unitCell + reqQtyCell + stockCell + remarksCell + actionCell + "</tr>";
            $("#ItemTableTbody").append(row);
        });
    } else {
        addNoDataFoundFooterWithMsg("#ItemTableTbody", "No Item Added For Requsition !");
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
}