let foodOrder = null;
let foodItemList = [];

$(document).ready(function () {
    loadFoodItem();
});

function loadFoodItem() {
    foodItemList = [];

    const orderId = $("#Id").val();

    if (orderId > 0) {
        const url = `${API}FoodOrder/GetFoodOrderById/${orderId}`;
        $.get(url, function (rData) {
            if (rData != null) {

                console.log("item-list: ", rData);

                if (rData.rsFoodOrderItems != null && rData.rsFoodOrderItems.length > 0) {
                    rData.rsFoodOrderItems.forEach(v => {
                        const model = {
                            id: v.id,
                            orderId: v.orderId,
                            foodId: v.foodId,
                            foodName: v.foodName,
                            foodDescription: v.foodDescription,
                            quantity: v.quantity,
                            rate: v.rate,
                            totalAmount: v.totalAmount
                        };

                        foodItemList.push(model);
                    })
                }

                renderFoodItems();

            } else {
                console.log("No Food Items Found...!");
            }
        })
    }
}

$(document.body).on("change", "#ItemId", function () {
    const itemId = $(this).val();

    if (itemId > 0) {
        const url = `${API}FoodItem/GetFoodInfoById/${itemId}`;

        $.get(url, function (rData) {
            if (rData != null) {
                console.log("item-info: ", rData);

                $("#food-description").html(`<small>${rData.description}</small>`);
                $("#food-description-input").val(rData.description);
                $("#Rate").val(rData.netRate);

            } else {
                console.log("No Food Item Found...!");
            }
        })
    }
})

$(document.body).on("click", "#AddItemBtn", function () {
    const orderId = $("#Id").val();
    const itemName = $("#ItemId option:selected").text();
    const itemId = $("#ItemId").val();

    const qty = parseFloat($("#Qty").val());
    const rate = parseFloat($("#Rate").val());

    const foodDescription = $("#food-description-input").val();

    const model = {
        id: 0,
        orderId: orderId,
        foodId: itemId,
        foodName: itemName,
        quantity: qty,
        rate: rate,
        totalAmount: qty * rate,
        foodDescription: foodDescription,
    };

    if (!(model.foodId > 0)) {
        failedMsg("Please Select Food item to add..");
        return false;
    }

    if (!(model.quantity > 0)) {
        failedMsg("Please Mention Quantity Properly, It should be greater than Zero");
        return false;
    }

    if (foodItemList.length > 0) {
        var item = foodItemList.find(x => x.foodId == model.foodId);

        if (item != null || item != undefined) {
            failedMsg("Item Already Added");
        } else {
            foodItemList.push(model);
        }
    } else {
        foodItemList.push(model);
    }

    renderFoodItems();
    clearDetail();
});


function renderFoodItems() {
    $("#OrderTableTbody").empty();

    if (foodItemList.length > 0) {

        foodItemList.forEach((modelObject, index) => {

            const slNo = `<td>${index + 1}<input type='hidden' name='RsFoodOrderItems[${index}].Id' value='${modelObject.id}'/></td>`;
            const itemCell = `<td><input type='hidden' name='RsFoodOrderItems[${index}].FoodId' value='${modelObject.foodId}'/>${modelObject.foodName}<br/>
            <small>${modelObject.foodDescription}<small>
            </td>`;
            const rateCell = `<td><input type='number' class='form-control item-rate' name='RsFoodOrderItems[${index}].Rate' data-index='${index}' value='${modelObject.rate}' id='item_rate_${index}'/></td>`;
            const qtyCell = `<td><input type='number' class='form-control item-qty' name='RsFoodOrderItems[${index}].Quantity' data-index='${index}' value='${modelObject.quantity}' id='item_qty_${index}'/></td>`;
            const totalCell = `<td><input type='number' class='form-control' name='RsFoodOrderItems[${index}].TotalAmount' value='${modelObject.totalAmount}' id='item_total_${index}' readonly/></td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index},${modelObject.foodId})'><i class='fa fa-trash'></i></button></td>`;

            const row = "<tr id=item" + index + ">" + slNo + itemCell + rateCell + qtyCell + totalCell + actionCell + "</tr>";
            $("#OrderTableTbody").append(row);
        });

        const sumResult = calculateSum(foodItemList.map(x => x.totalAmount));
        $("#OrderAmount").val(sumResult);

        calculateNetTotal();
    } else {
        addNoDataFoundFooterWithMsg("#ItemTableTbody", "No Item Added For Reservation...!!");
    }
}

function deleteItemRow(index, itemId) {
    if (index > -1) {
        foodItemList.splice(index, 1);
    }
    renderFoodItems();
}

function clearDetail() {
    $("#ItemId").val("").trigger("change");
    $("#Qty").val(0);
    $("#Rate").val(0);
    $("#food-description").html('');
    $("#food-description-input").val('');
}

function calculateSum(array) {
    var sum = 0;

    $.each(array, function (index, value) {
        sum += parseFloat(value);
    });

    return sum;
}

$(document.body).on("change", ".item-rate", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        calculateItemTotalByIndex(index);
    }
})

$(document.body).on("change", ".item-qty", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        calculateItemTotalByIndex(index);
    }
})

function calculateItemTotalByIndex(index) {
    if (index > -1) {
        const itemRate = $(`#item_rate_${index}`).val();
        const itemQty = $(`#item_qty_${index}`).val();

        foodItemList[index].rate = itemRate;
        foodItemList[index].quantity = itemQty;

        const total = itemRate * itemQty;
        $(`#item_total_${index}`).val(total);

        foodItemList[index].totalAmount = total;

        const sumResult = calculateSum(foodItemList.map(x => x.totalAmount));
        $("#OrderAmount").val(sumResult);

        calculateNetTotal();
    }
}

$(document.body).on("change", "#ServiceCharge", function () {
    calculateNetTotal();
});

$(document.body).on("change", "#Discount", function () {
    calculateNetTotal();
});

function calculateNetTotal() {
    const subTotal = parseFloat($("#OrderAmount").val());
    const vat = parseFloat($("#VAT").val());
    const serviceCharge = parseFloat($("#ServiceCharge").val());
    const discount = parseFloat($("#Discount").val());
    const paidAmount = parseFloat($("#PaidAmount").val());
    //const netAmount = $("#NetAmount").val();

    const netTotal = (subTotal + vat + serviceCharge) - (discount);

    console.log("Net Total: ", netTotal);
    $("#NetAmount").val(netTotal);

    let dueAmount = netTotal - paidAmount;
    dueAmount = dueAmount > 0 ? dueAmount : 0;
    $("#DueAmount").val(dueAmount);
}

$(document.body).on("click", "#UpdateBtn", function () {

    if (foodItemList.length > 0) {

        let isValid = true;
        let msg = "";

        if (isValid) {
            startFormPosting("#RsOrderUpdateForm");
        } else {
            failedMsg(msg);
        }
    } else {
        failedMsg("You did not set any item..!!");
    }
});