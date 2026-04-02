﻿let itemList = [];

$(document).ready(function () {
    initialRow();
});

$(document.body).on("change", "#itemId", function () {
    const itemId = $(this).val();

    if (itemId > 0) {
        const url = API + "ItemInfo/GetUnitByItemId";

        const params = {
            id: itemId
        };

        $.post(url, params, function (rData) {
            if (rData != null) {
                console.log("item-info:", rData);
                $("#qty").val(1);
                $("#qty").focus();
                $("#unitId").val(rData.unitId);
                $("#unitName").val(rData.unitName);
                $("#categoryId").val(rData.categoryId);
                $("#ledgerId").val(rData.ledgerId);
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
                $("#stock").val(rData);
            }
        });
    }
});

$(document.body).on("click", "#itemAddBtn", function () {
    const itemId = $("#itemId").val();
    const itemName = $("#itemId option:selected").text();
    const stock = $("#stock").val();

    const unitId = $("#unitId").val();
    const unitName = $("#unitName").val();

    const qty = parseFloat($("#qty").val());
    const rate = parseFloat($("#rate").val());
    const total = parseFloat($("#total").val());
    const categoryId = $("#categoryId").val();
    const ledgerId = $("#ledgerId").val();

    const model = {
        itemId: itemId,
        itemName: itemName,
        stock: stock,
        unitId: unitId,
        unitName: unitName,
        qty: qty,
        rate: rate,
        total: total,
        categoryId: categoryId,
        ledgerId: ledgerId
    }

    if (!(model.qty > 0)) {
        failedMsg("Please Mention Quantity Properly, It should be greater than Zero");
        return false;
    }

    if (!(model.rate > 0)) {
        failedMsg("Please Mention Item Rate Properly, It should be greater than Zero");
        return false;
    }

    if (model.itemId > 0) {
        const exist = itemList.find(x => x.itemId == itemId);

        if (exist != null) {
            return failedMsg("Item Already Exists..!!");
        } else {
            itemList.push(model);
        }
    }

    renderTBody();
});

$(document.body).on("change", "#qty", function () {
    calculateTotal();
});

$(document.body).on("change", "#rate", function () {
    calculateTotal();
});


$(document.body).on("change", ".qty-value", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        calculateTotal(index);

        updateTotalAmount();
    }
});

$(document.body).on("change", ".rate-value", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        calculateTotal(index);

        updateTotalAmount();
    }
});

function updateTotalAmount() {
    const totalAmount = _.sumBy(itemList, "total");

    $("#totalAmount").val(totalAmount);
}

$(document.body).on("click", ".rmvItem", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        itemList.splice(index, 1);
    }

    renderTBody();
});

$(document.body).on("click", "#DirectReceiveSubmitBtn", function () {
    console.log('ItemList', itemList)
    if (itemList.length > 0) {
        swal({
            title: "Submit Confirmation",
            text: "Are you sure you want to submit the form?",
            icon: "warning",
            buttons: {
                cancel: {
                    text: "Cancel",
                    value: false,
                    visible: true,
                    className: "btn btn-secondary",
                },
                confirm: {
                    text: "Yes, Submit",
                    value: true,
                    visible: true,
                    className: "btn btn-primary",
                }
            },
            dangerMode: true,
        }).then((willSubmit) => {
            if (willSubmit) {
                let isValid = true;
                let msg = "";

                if (isValid) {
                    console.log("Submitting form...");
                    startFormPosting("#CreateDirectReceiveForm");
                } else {
                    failedMsg(msg);
                }
            } else {
                failedMsg("Submission Cancelled!");
            }
        });
    } else {
        failedMsg("You did not make any receive..!!");
    }
});

function renderTBody() {

    $("#directReceiveTbody").empty();

    if (itemList.length > 0) {


        itemList.forEach((v, i) => {

            const itemCell = `<td><input type='hidden' name='TranDtls[${i}].ItemId' value=${v.itemId} />${v.itemName}</td>`;
            const stockCell = `<td class='text-center'><input type='hidden' name='TranDtls[${i}].Stock' value=${v.stock} />${v.stock}</td>`;
            const unitCell = `<td><input type='hidden' name='TranDtls[${i}].LedgerId' value=${v.ledgerId} /><input type='hidden' name='TranDtls[${i}].CategoryId' value=${v.categoryId} /><input type='hidden' name='TranDtls[${i}].ItemUnitId' value=${v.unitId} />${v.unitName}</td>`;
            const qtyCell = `<td class='text-center'><input type='number' class='form-control text-center qty-value' name='TranDtls[${i}].ItemQty' id='qty_${i}' value='${v.qty}' data-index='${i}'/></td>`;
            const rateCell = `<td class='text-end'><input type='number' class='form-control text-end rate-value' name='TranDtls[${i}].UnitPrice' id='rate_${i}' value='${v.rate}' data-index='${i}'/></td>`;
            const totalCell = `<td class='text-end'><input type='number' class='form-control text-end' value='${v.total}' id='total_${i}' readonly/></td>`;
            const actionCell = `<td class='text-center'><i class='fa fa-times fa-2x rmvItem' data-index='${i}'></i></td>`;

            const row = `<tr>${itemCell}${stockCell}${unitCell}${qtyCell}${rateCell}${totalCell}${actionCell}</tr>`;

            $("#directReceiveTbody").append(row);
        })

        initialRow();
        footerRow();
    } else {
        initialRow();
    }
}


function initialRow() {

    const html = `<tr>
                    <td style='width:30%'><select class='dd-type form-control' id='itemId'></select></td>
                    <td style='width:10%'><input class='form-control' type='number' id='stock' readonly /></td>
                    <td style='width:15%'>
                        <input type='hidden' id='unitId' />
                        <input type='hidden' id='categoryId' />
                        <input type='hidden' id='ledgerId' />
                        <input class='form-control' type='text' id='unitName' readonly />
                    </td>
                    <td style='width:10%'><input class='form-control' type='number' id='qty'/></td>
                    <td style='width:15%'><input class='form-control' type='number' id='rate'/></td>
                    <td style='width:15%'><input class='form-control' type='number' id='total'/></td>
                    <td class='text-center' style='width:5%'><button type='button' class='btn btn-outline-primary mb-1 w-100' id='itemAddBtn' title="Add-Item"><i class='fa fa-plus'></i></button></td>
                </tr>`;

    $("#directReceiveTbody").append(html);

    $(".dd-type").select2({ width: "100%" }).on("change", function (e) {
        $(this).valid();
    });

    _dropdownManager.getItemSelectListItems("#itemId", null, null);

    $('#itemId').select2('focus');
}

function footerRow() {
    const total = _.sumBy(itemList, 'total');

    let html = `<tr>
                    <td class='text-end' colspan='5'><b>Total Amount</b> </td>
                    <td class='text-end'><input type='number' class='form-control' id='totalAmount' value='${total}' readonly/></td>
                </tr>`;

    $("#directReceiveTbody").append(html);
}

function calculateTotal(index = null) {
    if (index !== null) {
        const qty = parseFloat($(`#qty_${index}`).val());
        const rate = parseFloat($(`#rate_${index}`).val());

        itemList[index].qty = qty;
        itemList[index].rate = rate;

        const total = qty * rate;

        $(`#total_${index}`).val(total);

        itemList[index].total = total;
    } else {
        const qty = parseFloat($("#qty").val());
        const rate = parseFloat($("#rate").val());
        const total = qty * rate;
        $("#total").val(total);
    }
}

$(document.body).on("click", "#ItemEntryBtn", function () {
    const itemName = $("#EntryItemName").val();
    const unitId = $("#EntryUnitId").val();
    const categoryId = $("#EntryCategoryId").val();

    if (itemName != "" && unitId > 0 && categoryId > 0) {
        const url = API + "ItemInfo/ItemEntry";

        const params = {
            itemName: itemName,
            unitId: unitId,
            categoryId: categoryId
        };

        $.post(url, params, function (rData) {
            if (rData > 0) {
                successMsg("Item Entry Successful");
                $("#itemEntryModal").modal('hide');
                clearItemEntryForm();

                _dropdownManager.getItemSelectListItems("#itemId", null, null);
                $('#itemId').select2('focus');

            } else {
                failedMsg("Item Entry Failed");
            }
        }).fail(function (err) {
            failedMsg(err.responseText);
        })
    } else {
        failedMsg("Item Name, Catgeory, Unit is required");
    }
})

function clearItemEntryForm() {
    $("#EntryItemName").val("");
    $("#EntryUnitId").val('').trigger("change");
    $("#EntryCategoryId").val('').trigger("change");
}