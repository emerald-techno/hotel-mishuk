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

$(document.body).on("click", "#ItemEntryBtn", function () {

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

