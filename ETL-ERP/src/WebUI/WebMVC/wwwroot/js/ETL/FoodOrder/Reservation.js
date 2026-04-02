let foodItemList = [];
let selectedItemList = [];
let selectedCategoryId = 0;
let customerTypeList = [];
let discountPercent = 0;

function CustomerType() {
    const model = {
        Hotel: "Hotel",
        WalkIn: "Walk-In",
        Employee: "Employee",
        Online: "Online"
    }
    return model;
}

$(document).ready(function () {
    loadCustomerTypeSelectList();
    loadCustomerSelectList();
    getCategoryData();
    loadFoodItem();

    $("#discount-info").html(`${discountPercent}%`);
});

//#region CustomerType
function loadCustomerTypeSelectList() {
    const url = API + "FoodOrder/GetCustomerTypeJsonData";

    /*createSelectList(url, null, "#CustomerTypeId", null, null);*/
    $.post(url, function (rData) {
        if (!hasAnyError(rData) && rData.length > 0) {
            console.log("Customer-Type: ", rData);
            customerTypeList = rData;
            bindDropdownList(rData, "#CustomerTypeId", null);
        }
    });
}

$(document.body).on("change", "#CustomerTypeId", function () {
    discountPercent = 0;
    const customerTypeId = $(this).val();

    if (customerTypeId > 0) {
        const customerType = customerTypeList.find(x => x.id == customerTypeId);

        if (customerType != null && customerType != undefined) {
            discountPercent = customerType.discount;
            /*$("#discount-info").html(`${discountPercent}%`);*/
            $("#discount-info").val(discountPercent);
        }

        calculateDiscount();
    }
})

//#endregion

//#region Category Section
function initAllCategory() {
    const model = {
        id: 0,
        name: "All Items"
    }

    foodCategoryList.push(model);
}

function getCategoryData() {
    foodCategoryList = [];

    initAllCategory();

    const url = `${API}FoodCategory/GetFoodCategoryJsonData`;
    $.get(url, function (rData) {
        if (rData) {
            console.log("category-list: ", rData);

            if (rData.length > 0) {
                rData.forEach(v => {
                    const model = {
                        id: v.id,
                        name: v.name
                    };

                    foodCategoryList.push(model);
                })
            }

            renderCategory();

        } else {
            console.log("No Food Category Info Found...!");
        }
    })
}

function renderCategory() {
    if (foodCategoryList.length > 0) {
        $("#category-section").empty();

        foodCategoryList.forEach((v, i) => {

            let isActive = v.id == selectedCategoryId ? true : false;

            let color = isActive ? "bg-primary" : "bg-info";

            /*const category = `<button type='button' class='btn btn-pill ${color} btn-sm category_btn' style='margin-right:5px;' value='${v.id}'>${v.name}</button>`;*/

            const category = `<div class='${color} category_btn' style='margin-right:5px;' data-value='${v.id}'>${v.name}</div>`;

            $("#category-section").append(category);
        })
    }
}

$(document.body).on("click", ".category_btn", function () {
    const value = $(this).attr("data-value");

    if (value >= 0) {
        console.log("value get successful:", value);

        const category = foodCategoryList.find(x => x.id == value);

        if (category != null && category != undefined) {
            selectedCategoryId = category.id;
        }

        getCategoryData();

        loadFoodItem();
    }
})

//#endregion

//#region Food Section
//function loadFoodItem() {
//    foodItemList = [];

//    const url = `${API}FoodItem/GetFoodItems`;
//    $.get(url, function (rData) {
//        if (rData) {

//            console.log("item-list: ", rData);

//            if (rData.length > 0) {
//                rData.forEach(v => {
//                    foodItemList.push(v);
//                })
//            }

//            renderItems();

//        } else {
//            console.log("No Food Items Found...!");
//        }
//    })
//}


function loadFoodItem() {
    foodItemList = [];

    if (selectedCategoryId >= 0) {

        const url = `${API}FoodItem/GetFoodByCategoryId?categoryId=${selectedCategoryId}`;
        $.get(url, function (rData) {
            if (rData) {

                console.log("item-list: ", rData);

                if (rData.length > 0) {
                    rData.forEach(v => {
                        foodItemList.push(v);
                    })
                }

                renderItems();

            } else {
                console.log("No Food Items Found...!");
            }
        })
    };
}

function renderItems() {
    if (foodItemList.length > 0) {

        $("#item-section").empty();

        foodItemList.forEach((v, i) => {

            const selectedItemIndex = selectedItemList.findIndex(x => x.foodId == v.id);

            let isSelected = selectedItemIndex > -1 ? true : false;

            let color = isSelected ? "btn-info" : "btn-primary";

            let img = '';

            if (!hasAnyError(v.photoUrl)) {
                img = `<img src ='${v.photoUrl}' style='width:100%;height:30%;'/>`;
            } else {
                img = `<img src ='${API}/img/No_Image_Available.jpg' style='width: 100%;height:30%;'/>`;
            }

            //const items = `<div class="btn btn-outline-primary-2x m-2 text-center" style="max-width:160px;">
            //                    ${img}
            //                    ${v.itemName}
            //                    <p>Price: ${v.netRate}</p>
            //                    <p>${v.itemCode}</p>
            //                    <a class='btn ${color} select_item_btn' type='button' data-index='${i}'>Add Item</a>
            //                </div>`;

            const items = `<div class='item-box'>
                                ${img}
                                
                                <div class='item-content'>
                                    <b>${v.itemName}</b>
                                    <p>Price: ${v.netRate}</p>
                                    <p>${v.itemCode}</p>
                                </div>

                                <a class='btn ${color} select_item_btn' type='button' data-index='${i}'>Add Item</a>
                            </div>`;

            $("#item-section").append(items);
        })
    }
}

$(document.body).on("click", ".select_item_btn", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {

        const item = foodItemList[index];

        if (!hasAnyError(item)) {

            const selectedItemIndex = selectedItemList.findIndex(x => x.foodId == item.id);

            if (selectedItemIndex > -1) {
                return failedMsg("Item Already Added...!!");
            }

            const model = {
                foodId: item.id,
                foodName: item.itemName,
                foodPhotoUrl: item.photoUrl,
                qty: 1,
                rate: item.netRate,
                totalAmount: item.netRate * 1,
                description: item.description
            }

            selectedItemList.push(model);
        };

        renderItems();

        renderCardItem();
    }
})

//#endregion

//#region Cart Section

function renderCardItem() {
    $("#cart-section").empty();
    if (selectedItemList.length > 0) {

        selectedItemList.forEach((v, i) => {
            let img = '';

            if (!hasAnyError(v.foodPhotoUrl)) {
                img = `<img src ='${v.foodPhotoUrl}' style='width:100%;height:100%;'/>`;
            } else {
                img = `<img src ='${API}/img/No_Image_Available.jpg' style='width: 100%;height:100%;'/>`;
            }

            const items = ` <tr>
                            <td style="width:40%;color:white">
                                <input type='hidden' name='RsFoodOrderItems[${i}].FoodId' value='${v.foodId}'/> <b>${v.foodName}</b><br/>
                                <small>${v.description}<small>
                            </td>
                            <td style="width:5%;color:white"><button class="btn-round minus_btn" data-index='${i}'>-</button></td>
                            <td style="width:20%;color:white;">
                                <input type='text' class='form-control text-center qty_input' id='qty_value_${i}' data-index='${i}' name='RsFoodOrderItems[${i}].Quantity' value='${v.qty}'/>
                            </td>
                            <td style="width:5%;color:white"><button class="btn-round plus_btn" data-index='${i}'>+</button></td>
                            <td style="width:20%;color:white">
                                <input type='hidden' name='RsFoodOrderItems[${i}].Rate' value='${v.rate}'/>
                                <input type='hidden' name='RsFoodOrderItems[${i}].TotalAmount' value='${v.totalAmount}'/>  
                                ${v.totalAmount}
                            </td>
                            <td style="width:10%"><a href='#' class='remove-item' style='color:#d43545;' data-index='${i}'><i class="fa fa-times fa-2x"></i></a></td>
                        </tr>`;

            $("#cart-section").append(items);
        })

        const sumResult = calculateSum(selectedItemList.map(x => x.totalAmount));
        $("#sub-total").val(sumResult);

        calculateDiscount();

        calculateNetTotal();
    } else {
        $("#sub-total").val(0);
        calculateDiscount();
        calculateNetTotal();
    }
}


$(document.body).on("change", ".qty_input", function () {
    const index = $(this).attr("data-index"); 

    if (index > -1) {
        const qtyValue = parseInt($(`#qty_value_${index}`).val());

        if (!(qtyValue > 0)) {
            return failedMsg("Quantity is have to be more than zero...!!");
        }

        const item = selectedItemList[index];

        if (!hasAnyError(item)) {
            selectedItemList[index].qty = qtyValue;
            selectedItemList[index].totalAmount = selectedItemList[index].rate * selectedItemList[index].qty;
        };

        renderCardItem();
    }
});


$(document.body).on("click", ".plus_btn", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {

        const item = selectedItemList[index];

        if (!hasAnyError(item)) {
            selectedItemList[index].qty = selectedItemList[index].qty + 1;
            selectedItemList[index].totalAmount = selectedItemList[index].rate * selectedItemList[index].qty;
        };

        renderCardItem();
    }
});

$(document.body).on("click", ".minus_btn", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {

        const item = selectedItemList[index];

        if (!hasAnyError(item)) {
            if (item.qty == 1) {
                return failedMsg("Item Quantity Can't Be Zero..!");
            }

            selectedItemList[index].qty = selectedItemList[index].qty - 1;
            selectedItemList[index].totalAmount = selectedItemList[index].rate * selectedItemList[index].qty;
        };

        renderCardItem();
    }
});

function calculateSum(array) {
    var sum = 0;

    $.each(array, function (index, value) {
        sum += parseFloat(value);
    });

    return sum;
}

$(document.body).on("click", ".remove-item", function () {
    const itemIndex = $(this).attr("data-index");

    if (itemIndex > -1) {
        selectedItemList.splice(itemIndex, 1);
    };

    renderCardItem();

    renderItems();
});

$(document.body).on("change", "#VAT", function () {
    calculateNetTotal();
});

$(document.body).on("change", "#ServiceCharge", function () {
    calculateNetTotal();
});

$(document.body).on("change", "#discount-info", function () {
    const localDiscountPercent = $(this).val();
    if (localDiscountPercent > 0) {
        discountPercent = localDiscountPercent;

        calculateDiscount();
    }
});

$(document.body).on("change", "#Discount", function () {
    calculateNetTotal();
});

function calculateDiscount() {
    const subTotal = parseFloat($("#sub-total").val());
    if (discountPercent > 0) {
        const discountAmount = calculatePercentage(subTotal, discountPercent);
        $("#Discount").val(discountAmount);
    }

    calculateNetTotal();
}

function calculatePercentage(value, percentage) {
    return (value * percentage) / 100;
}

function calculateNetTotal() {
    const subTotal = parseFloat($("#sub-total").val());
    const vat = parseFloat($("#VAT").val());
    const serviceCharge = parseFloat($("#ServiceCharge").val());
    const discount = parseFloat($("#Discount").val());
    const paidAmount = parseFloat($("#PaidAmount").val());

    const netTotal = (subTotal + vat + serviceCharge) - (discount);

    console.log("Net Total: ", netTotal);
    $("#NetAmount").val(netTotal);


    //$("#PaidAmount").val(netTotal);

    //const dueAmount = netTotal - paidAmount;
    //$("#due-amount").val(dueAmount);
}

//#endregion

//#region Customer Section

function loadCustomerSelectList() {

    const url = API + "Customer/GetCustomerJsonData";

    createSelectList(url, null, "#CustomerId", null, null);
}

$(document.body).on("click", "#CustomerEntryBtn", function () {
    const salutation = $("#salutation").val();
    const firstName = $("#first-name").val();
    const lastName = $("#last-name").val();
    const mobile = $("#mobile").val();

    if (salutation != "" && firstName != "" && mobile != "") {
        const url = API + "Customer/CustomerEntry";

        const params = {
            salutation: salutation,
            firstName: firstName,
            lastName: lastName,
            mobile: mobile
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Customer Entry Successful");
                $("#customerEntryModal").modal('hide');
                clearCustomerForm();

                loadCustomerSelectList();
            } else {
                failedMsg("Customer Entry Failed");
            }
        }).fail(function () {
            failedMsg("Customer Entry Failed");
        })
    } else {
        failedMsg("Salutation, First Name, Mobile Number is required");
    }
})

function clearCustomerForm() {
    $("#salutation").val("");
    $("#first-name").val("");
    $("#last-name").val("");
    $("#mobile").val("");
}

//#endregion

$(document.body).on("click", "#OrderPaymentSubmitBtn", function () {
    const customerId = $("#CustomerId").val();
    const customerType = $("#CustomerTypeId").val();

    if (!(customerType > 0)) {
        return failedMsg("Please Select Customer Type..!");
    }

    if (!(customerId > 0)) {
        return failedMsg("Please Select Customer..!");
    }

    //if (!(selectedItemList.length > 0)) {
    //    return failedMsg("No Item Found To Order..!");
    //}

    if (customerId > 0 && customerType > 0) {
        $("#FoodOrderForm").submit();
    } else {
        failedMsg("Somthing Went Wrong...!");
    }
});

//#region debounce item search

//$(document.body).on("change", "#item-search", function () {
//    debouncedFunction();
//});

$(document.body).on("keyup", "#item-search", function () {
    debouncedFunction();
});


function debounce(func, delay) {
    let timeoutId;

    return function () {
        const context = this;
        const args = arguments;

        clearTimeout(timeoutId);

        timeoutId = setTimeout(function () {
            func.apply(context, args);
        }, delay);
    };
}

function loadItemWithName() {
    foodItemList = [];
    let searchValue = $("#item-search").val();

    if (searchValue != null) {

        const url = `${API}FoodItem/GetFoodByNameOrCode?value=${searchValue}`;
        $.get(url, function (rData) {
            if (rData) {

                console.log("item-list: ", rData);

                if (rData.length > 0) {
                    rData.forEach(v => {
                        foodItemList.push(v);
                    })
                }

                renderItems();

            } else {
                console.log("No Food Items Found...!");
            }
        })
    };
}

const debouncedFunction = debounce(loadItemWithName, 1000);

//#endregion