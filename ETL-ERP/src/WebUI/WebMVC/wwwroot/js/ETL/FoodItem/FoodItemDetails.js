$(document).ready(function () {
    $("#Price").val("");
    $("#Quantity").val("");
});

$('#itemEntryModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#itemEntryModal') // Ensure dropdown appears within modal
    });
});

$('#setItemEntryModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#setItemEntryModal') // Ensure dropdown appears within modal
    });
});

$(document.body).on("change", "#ItemId", function () {
    const itemId = $(this).val();

    if (itemId > 0) {
        const url = API + "FoodItem/GetItemAverageAmountJsonData";

        const params = {
            id: itemId
        };

        $.post(url, params, function (rData) {
            if (rData != null) {
                console.log("item-stock-info:", rData);
                $("#Price").val(rData.averageAmount);
                $("#Quantity").focus();
                $("#UnitId").val(rData.unitId).trigger(update);
            } else {
                failedMsg("Item Unit Info Not Found...!!");
            }
        }).fail(function (xhr) {
            failedMsg(xhr.responseText);
        });
    }
});

$(document.body).on("click", "#ItemEntryBtn", function (e) {
    e.preventDefault();
    const foodItemId = $("#Id").val();
    const itemId = $("#ItemId").val();
    const unitId = $("#UnitId").val();
    const price = parseFloat($("#Price").val());
    const qty = parseFloat($("#Quantity").val());
    const amount = parseFloat($("#Amount").val());

    if (foodItemId > 0 && itemId > 0 && unitId > 0 && qty > 0) {
        const url = API + "FoodIngredient/Create";

        const params = {
            foodItemId: foodItemId,
            itemId: itemId,
            unitId: unitId,
            price: price,
            quantity: qty,
            amount: amount,
            isAjaxPost: true
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Food Ingredients Add Successful");
                $("#itemEntryModal").modal('hide');
                clearFoodIngredientForm();

                setTimeout(() => {
                    window.location.href = API + "FoodItem/Details/" + foodItemId;
                }, 1500);
            } else {
                failedMsg("Food Ingredients Entry Failed");
            }
        }).fail(function () {
            failedMsg("Food Ingredients Entry Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
})

function calculateAmount() {
    const price = parseFloat($("#Price").val());
    const quantity = parseFloat($("#Quantity").val());
    const amount = price * quantity;

    $("#Amount").val(amount.toFixed(2));
}

$(document).on("input", "#Price, #Quantity", function () {
    calculateAmount();
});

$(document).on("click", ".close", function () {
    clearFoodIngredientForm();
});

function clearFoodIngredientForm() {
    $("#ItemId").val("").trigger("change");
    $("#UnitId").val("").trigger("change");
    $("#Price").val("");
    $("#Quantity").val("");
    $("#Amount").val("");
}

$(document.body).on("click", "#SetItemEntryBtn", function () {

    const foodItemId = $("#Id").val();
    const setFoodItemId = $("#SetFoodItemId").val();
    const qty = parseFloat($("#SetItemQuantity").val());

    if (foodItemId > 0 && setFoodItemId > 0) {
        const url = API + "FoodItem/FoodSetItemAdd";

        const params = {
            foodItemId: foodItemId,
            setFoodItemId: setFoodItemId,
            quantity: qty,
            isEnable: true
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Set Menu Food Item Add Successful");
                $("#setItemEntryModal").modal('hide');
                clearFoodSetItemForm();

                setTimeout(() => {
                    window.location.href = API + "FoodItem/Details/" + foodItemId;
                }, 1500);
            } else {
                failedMsg("Set Food Item Entry Failed");
            }
        }).fail(function () {
            failedMsg("Set Food Item Entry Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
})

function clearFoodSetItemForm() {
    $("#SetFoodItemId").val("").trigger("change");
    $("#SetItemQuantity").val("");
}

$(document.body).on("change", ".SetIsEnable", function () {
    const index = $(this).attr("data-index");
    const setItemId = $(this).attr("data-id");

    if (index > -1) {
        const isEnable = $(`#SetIsEnable_${index}`).is(":checked");

        console.log("is-enable", isEnable);

        if (setItemId > 0) {

            const url = `${API}FoodItem/SetMenuItemStatus?id=${setItemId}&status=${isEnable}`;

            $.get(url, function (rData) {
                if (rData) {
                    console.log("status changed success", rData);
                } else {
                    console.log("status changed failed..!!");
                }
            })
        }
    }
})

function removeSetItem(id) {
    const foodItemId = $("#Id").val();

    if (id > 0) {
        const url = `${API}FoodItem/RemoveMenuItem?id=${id}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("remove item success", rData);

                setTimeout(() => {
                    window.location.href = API + "FoodItem/Details/" + foodItemId;
                }, 1000);
            } else {
                console.log("remove item failed..!!");
            }
        })
    }
}

$(document.body).on("change", ".IsActive", function () {
    const index = $(this).attr("data-index");
    const ingredientId = $(this).attr("data-id");

    if (index > -1) {
        const isActive = $(`#IsActive_${index}`).is(":checked");

        if (ingredientId > 0) {

            const url = `${API}FoodIngredient/ActiveStatus?id=${ingredientId}&status=${isActive}`;

            $.get(url, function (rData) {
                if (rData) {
                    console.log("Food ingredient status changed success", rData);
                } else {
                    console.log("Food ingredient status changed failed..!!");
                }
            })
        }
    }
})

function removeIngredient(id) {
    const foodItemId = $("#Id").val();

    if (id > 0) {
        const url = `${API}FoodIngredient/RemoveIngredient?id=${id}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("remove ingredient item success", rData);

                setTimeout(() => {
                    window.location.href = API + "FoodItem/Details/" + foodItemId;
                }, 1000);
            } else {
                console.log("remove ingredient item failed..!!");
            }
        })
    }
}