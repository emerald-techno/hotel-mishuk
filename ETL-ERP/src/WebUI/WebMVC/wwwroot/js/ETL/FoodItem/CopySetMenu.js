let FoodSetItemVms = [];
const foodId = $("#PreItemId").val();

$(document).ready(function () {
    $("#Price").val("");
    $("#Quantity").val("");
    getSetMenuByFoodId(foodId)
    
});

//#Region getSetMenuByFoodId
function getSetMenuByFoodId() {
    
    if (foodId > 0) {
        const url = API + "FoodItem/GetSetItemsByFoodId/" + foodId;
        $.get(url, function (rData) {
            console.log("set-menu-items", rData);
            if (rData != undefined) {
                if (rData.length > 0) {
                    rData.forEach(v => {
                        console.log("set-items",v)
                        const model = {
                            setFoodItemId: v.setFoodItemId,
                            setFoodItemName: v.setFoodItemName,
                            quantity: v.quantity,
                            setFoodItemPrice: v.setFoodItemPrice,
                            //total: quantity * setFoodItemPrice
                        }

                        console.log(model);
                        FoodSetItemVms.push(model);
                    });
                    
                    createDetailTable();
                }
            }
            initialRow();
        });
    }
}
//#End-Region getSetMenuByFoodId


//# region Initial Row
function initialRow() {

    const html = `<tr>
                    <td style='width:30%'><select class='dd-type form-control' id='foodItemId'></select></td>
                    <td style='width:10%'><input class='form-control text-center' type='number' id='price' readonly /></td>
                    <td style='width:10%'><input class='form-control text-center' type='number' id='qty'/></td>
                    <td style='width:15%'><input class='form-control text-center' type='number' id='total' readonly/></td>
                    <td class='text-center' style='width:5%'><button type='button' class='btn btn-outline-primary mb-1' id='itemAddBtn' title="Add-Item"><i class='fa fa-plus'></i></button></td>
                </tr>`;

    $("#setMenuTableBody").append(html);

    $(".dd-type").select2({ width: "100%" }).on("change", function (e) {
        $(this).valid();
    });

    _dropdownManager.getFoodItemSelectListItems("#foodItemId", null, null);

    $('#foodItemId').select2('focus');
}
//#End-Region initialRow


//#Region createDetailTable
function createDetailTable() {
    $("#setMenuTableBody").empty();

    if (FoodSetItemVms.length > 0) {
        FoodSetItemVms.forEach((modelObject, index) => {

            const itemCell = `
                <td>
                    <input type='hidden' name='FoodSetItemVms[${index}].SetFoodItemId' value='${modelObject.setFoodItemId}' />
                    ${modelObject.setFoodItemName}
                </td>`;

            const priceCell = `
                <td class="text-center">
                    <input type='hidden' name='FoodSetItemVms[${index}].SetFoodItemPrice' data-index='${index}' id="Price_${index}" value='${modelObject.setFoodItemPrice}' />
                    ${modelObject.setFoodItemPrice}
                </td>`;

            const quantityCell = `
                <td>
                    <input type='number' 
                           id='Quantity_${index}' 
                           data-index='${index}' 
                           name='FoodSetItemVms[${index}].Quantity' 
                           value='${modelObject.quantity}' 
                           class='form-control text-center Quantity' />
                </td>`;

            const totalCell = `
                <td class="text-center">
                    <span id="Total_${index}" data-index='${index}'>${(modelObject.setFoodItemPrice * modelObject.quantity).toFixed(2)}</span>
                </td>`;

            const actionCell = `
                <td class="text-center">
                    <button type='button' class='btn btn-icon btn-outline-danger rmvItem' data-index='${index}'>
                        <i class='fa fa-trash'></i>
                    </button>
                </td>`;

            const row = `<tr id='item${index}'>${itemCell}${priceCell}${quantityCell}${totalCell}${actionCell}</tr>`;
            $("#setMenuTableBody").append(row);
        });

       
        $(".Quantity").on("input", function () {
            //const index = $(this).data("index");
            //const price = parseFloat($(`#Price_${index}`).val());
            //const quantity = parseFloat($(this).val()) || 0;
            //const total = (price * quantity).toFixed(2);
            //$(`#Total_${index}`).text(total);
            calculateAmount()
        });
    } else {
        addNoDataFoundFooterWithMsg("#setMenuTableBody", "No Items Found for this Set Menu!");
    }
}
//#End-Region createDetailTable


//#Region clearFoodSetItemForm
function clearFoodSetItemForm() {
    $("#SetFoodItemId").val("").trigger("change");
    $("#SetItemQuantity").val("");
}
//#End-Region clearFoodSetItemForm

//#Region Get Price of food-Item
$(document.body).on("change", "#foodItemId", function () {
    const id = $(this).val(); 
    const url = API + "FoodItem/GetFoodItemRateById/"+id; 


    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        success: function (response) {
            console.log("res",response)
            if (response) {
                $("#price").val(response);
                $('#qty').focus();
            } else {
                console.log("No data found.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
        }
    });
});

function calculateAmount() {
    const price = parseFloat($("#price").val());
    const quantity = parseFloat($("#qty").val());
    const amount = price * quantity;

    $("#total").val(amount.toFixed(2));
}

$(document).on("input", "#price, #qty", function () {
    calculateAmount();
});
//#End Region Get Price of food-Item

//function removeSetItem(id) {
//    const foodItemId = $("#Id").val();

//    if (id > 0) {
//        const url = `${API}FoodItem/RemoveMenuItem?id=${id}`;
//        $.get(url, function (rData) {
//            if (rData) {
//                console.log("remove item success", rData);

//                setTimeout(() => {
//                    window.location.href = API + "FoodItem/Details/" + foodItemId;
//                }, 1000);
//            } else {
//                console.log("remove item failed..!!");
//            }
//        })
//    }
//}

$(document.body).on("click", "#itemAddBtn", function () {
    const foodItemId = $("#foodItemId").val();
    const itemName = $("#foodItemId option:selected").text();

    const qty = parseFloat($("#qty").val());
    const price = parseFloat($("#price").val());
    const total = parseFloat($("#total").val());

    const model = {
        //foodItemId: foodItemId,
        //itemName: itemName,
        //qty: qty,
        //price: price,
        //total: total

        setFoodItemId: foodItemId,
        setFoodItemName: itemName,
        quantity: qty,
        setFoodItemPrice: price
    }

    if (!(model.quantity > 0)) {
        failedMsg("Please Mention Quantity Properly, It should be greater than Zero");
        return false;
    }

    if (!(model.setFoodItemPrice > 0)) {
        failedMsg("Please Mention Item Rate Properly, It should be greater than Zero");
        return false;
    }

    if (model.setFoodItemId > 0) {
        const exist = FoodSetItemVms.find(x => x.setFoodItemId == foodItemId);

        if (exist != null) {
            return failedMsg("Item Already Exists..!!");
        } else {
            FoodSetItemVms.push(model);
        }
    }

    createDetailTable();
    initialRow();
});

function deleteItemRow(index) {
    $(`#item${index}`).closest("tr").remove();
}

$(document.body).on("click", ".rmvItem", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        FoodSetItemVms.splice(index, 1);
    }

    createDetailTable();
    initialRow();
});