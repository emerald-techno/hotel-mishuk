let foodCategoryList = [];
let foodItemList = [];
let selectedItemList = [];
let selectedCategoryId = 0;
let customerTypeList = [];
let discountPercent = 0;
const roomScPercent = 10;

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
    loadCategorySelectList();
    loadCustomerSelectList();
    getCategoryData();
    loadFoodItem();

    $("#discount-info").html(`${discountPercent}%`);
    $("#tableSelect").on("change", function () {
        updateTitle();
        calculateServiceCharge();
    });

    const $nav = $('.main-nav');
    const $header = $('.page-main-header');
    const $toggleNavTop = $('#sidebar-toggle');

    $nav.addClass('close_icon');
    $header.addClass('close_icon');
    $toggleNavTop.attr('checked', false);

    $("#service-type-sec").hide();
});

function updateTitle() {
    const selectedText = $("#tableSelect option:selected").text();
    document.title = `${selectedText}`;
}

//#region CustomerType
function loadCustomerTypeSelectList() {
    const url = API + "FoodOrder/GetCustomerTypeJsonData";
    $.post(url, function (rData) {
        if (!hasAnyError(rData) && rData.length > 0) {
            console.log("Customer-Type: ", rData);
            customerTypeList = rData;
            bindDropdownList(rData, "#CustomerTypeId", 5);

            loadCustomerSelectList();
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
            $("#discount-info").val(discountPercent);

            if (customerType.code != CustomerType().Hotel) {
                $("#RoomId").val('').trigger(update);
                $("#service-type-sec").hide();
            }
        }

        calculateDiscount();
        loadCustomerSelectList();
    }
});

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
    });

}

function renderCategory() {
    if (foodCategoryList.length > 0) {
        $("#category-section").empty();

        foodCategoryList.forEach((v, i) => {

            let isActive = v.id == selectedCategoryId ? true : false;

            let color = isActive ? "bg-primary" : "bg-info";

            /*const category = `<button type='button' class='btn btn-pill ${color} btn-sm category_btn' style='margin-right:5px;' value='${v.id}'>${v.name}</button>`;*/

            const category = `<div class='badge bg-secondary slick_btn d-flex align-items-center justify-content-center' style='margin-right:5px;' data-value='${v.id}'>${v.name}</div>`;

            $("#category-section").append(category);
        });

        $("#category-section").slick({
            dots: false,
            infinite: true,
            slidesToShow: 10,
            slidesToScroll: 10,
        });
    }
}

$(document.body).on("click", ".slick_btn", function () {
    const value = $(this).attr("data-value");
    const slickIndex = $(this).attr("data-slick-index");

    if (value >= 0) {
        console.log("value get successful:", value);

        const category = foodCategoryList.find(x => x.id == value);

        if (category != null && category != undefined) {
            selectedCategoryId = category.id;
        }
        $(".slick-slide").removeClass("slick-current");
        $(`.slick-slide[data-slick-index="${slickIndex}"]`).addClass('slick-current');


        //getCategoryData();
        loadFoodItem();
    }
})

$('.slider').on('afterChange', function (event, slick, currentSlide) {

    $('.slick-slide').removeClass('slick-current');

});

//Category SelectList 
function loadCategorySelectList() {
    const url = `${API}FoodCategory/GetFoodCategoryJsonData`;

    $.post(url, function (rData) {
        if (!hasAnyError(rData) && rData.length > 0) {
            bindDropdownList(rData, "#CategoryId", 0);
        }
    });
}
$(document.body).on("change", "#CategoryId", function () {
    const value = $(this).val();

    if (value >= 0) {
        console.log("value get successful:", value);

        const category = foodCategoryList.find(x => x.id == value);

        if (category != null && category != undefined) {
            selectedCategoryId = category.id;
        }

        loadFoodItem();
    }
})


//#endregion

//#region Food Section
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

            let color = isSelected ? "btn-info" : "btn-outline-primary";

            let setMenuHtml = v.isSetMenuItem ? `<span class='badge badge-primary'>Set Menu</span>` : '';
            let img = '';

            if (!hasAnyError(v.photoUrl)) {
                img = `<img src ='${v.photoUrl}' style='width:100%;height:30%;'/>`;
            } else {
                img = `<img src ='${API}/img/No_Image_Available.jpg' style='width:100%;height:30%;'/>`;
            }

            const items = `<div class='item-box'>
                                ${img}
                                
                                <div class='item-content'>
                                    <b>${v.itemName}</b>
                                       ${setMenuHtml}

                                    <p>Price: ${v.netRate}</p>
                                    <p>${v.itemCode}</p>
                                </div>

                                <a class='btn ${color} select_item_btn' type='button' data-index='${i}'>Add Item</a>
                            </div>`;

            $("#item-section").append(items);
        })
    }
    else {
        $("#item-section").empty();
        $("#item-section").append(`<h3 style="text-align:center;">No Food Item Found in this Category</h3>`);
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
            let description = v.description ? v.description : "";
            //let des = '';

            //if (!hasAnyError(v.description)) {
            //    des = '<input type='hidden' name='RsFoodOrderItems[${i}].FoodId' value='${v.foodId}' /> ${ v.foodName }'
            //} else {
            //    des = '';
            //}

            if (!hasAnyError(v.foodPhotoUrl)) {
                img = `<img src ='${v.foodPhotoUrl}' style='width:100%;height:100%;'/>`;
            } else {
                img = `<img src ='${API}/img/No_Image_Available.jpg' style='width: 100%;height:100%;'/>`;
            }

            const items = ` <tr>
                                <td class="text-center d-flex align-items-center" style="width:10%">
                                    <a href='#' class='remove-item' style='color:#d43545;' data-index='${i}'><i class="fa fa-trash fa-2x"></i></a>
                                </td>

                                <td style="width:35%; color:black;">
                                    <div style="display: flex; flex-direction: row; align-items: center; height:100%;">
                                        <input type='hidden' name="RsFoodOrderItems[${i}].FoodId" value="${v.foodId}"  />
                                        <b>${v.foodName}</b>
                                    </div>
                                </td>

                                 <td style="width:25%;color:black" >
                                  <div style="display: flex; flex-direction: row; align-items: center; justify-content: center; height:100%">
                                    <button type='button' class="fa fa-solid fa fa-minus border-success minus_btn" style="border-radius:10px; border: 1px solid green;" data-index="${i}"></button>
                                    <input type="number" id="qty_${i}" class="form-control qty" name="RsFoodOrderItems[${i}].Quantity" value="${v.qty}" data-index="${i}" style="text-align: center; width: 50px; margin: 5px 0;" >
                                    <button type='button' class="fa fa-solid fa fa-plus border-success plus_btn" style="border-radius:10px; border: 1px solid green;" data-index="${i}"></button>
                                  </div>
                                </td>

                                <td style="width:15%;color:black">
                                    <div style="display: flex; flex-direction: row; align-items: center; justify-content: center; height:100%">
                                        <input type='hidden' name='RsFoodOrderItems[${i}].Rate' value='${v.rate}'/>
                                        <input type='hidden' name='RsFoodOrderItems[${i}].TotalAmount' value='${v.totalAmount}'/>  
                                        ${v.totalAmount}
                                    </div>
                                </td>
                            
                            </tr>`;

            $("#cart-section").append(items);
        })

        const sumResult = calculateSum(selectedItemList.map(x => x.totalAmount));
        $("#sub-total").val(sumResult);

        calculateDiscount();
        calculateServiceCharge();
        calculateNetTotal();
    } else {
        $("#sub-total").val(0);
        calculateDiscount();
        calculateServiceCharge();
        calculateNetTotal();
    }
}

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

$(document.body).on("change", ".qty", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        const item = selectedItemList[index];
        let itemQty = $(`#qty_${index}`).val();

        if (!hasAnyError(item)) {
            if (!(itemQty > 0)) {
                itemQty = 1;
            }
            selectedItemList[index].qty = itemQty;
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
    else {
        discountPercent = 0;

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
    else {
        discountAmount = 0;
        $("#Discount").val(discountAmount);
    }

    calculateNetTotal();
}

function calculateServiceCharge() {
    const roomId = $("#RoomId").val();
    const subTotal = parseFloat($("#sub-total").val());
    let serviceAmount = 0;
    const isRoomService = $(".roomService").prop("checked");

    if (roomScPercent > 0 && roomId > 0 && isRoomService) {
        serviceAmount = calculatePercentage(subTotal, roomScPercent);
        $("#ServiceCharge").val(serviceAmount);
        $("#ServiceCharge").prop("readonly", true);
    }
    else {
        serviceAmount = 0;
        $("#ServiceCharge").val(serviceAmount);
        $("#ServiceCharge").prop("readonly", false);
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
    const discount = $("#Discount").val() > 0 ? parseFloat($("#Discount").val()) : 0;
    const paidAmount = parseFloat($("#PaidAmount").val());

    const netTotal = (subTotal + vat + serviceCharge) - (discount);

    console.log("Net Total: ", netTotal);
    $("#NetAmount").val(netTotal);
    $("#PaidAmount").val(netTotal);

    //const dueAmount = netTotal - paidAmount;
    //$("#due-amount").val(dueAmount);
}
//$(document.body).on("change", "#tableSelect", function () {
//    calculateServiceCharge();
//});
//#endregion

//#region Customer Section

$(document.body).on("change", "#RoomId", function () {
    const roomId = $(this).val();
    $("#CustomerId").val('').trigger(update);

    const customerTypeId = $("#CustomerTypeId").val();
    const customerType = customerTypeList.find(x => x.id == customerTypeId);

    if (customerType != null && customerType != undefined) {
        if (customerType.code != CustomerType().Hotel) {
            $(this).val('').trigger(update);
            return failedMsg("Please Select Customer Type Hotel..!");
        } else {
            $("#service-type-sec").show();
        }
    }


    if (roomId > 0) {
        const url = `${API}Customer/GetCustomerInfoByRoomId?roomId=${roomId}`;
        $.get(url, function (rData) {
            if (rData != null) {
                console.log("customer-info: ", rData);
                $("#CustomerId").val(rData.id).trigger(update);
                calculateServiceCharge();
            } else {
                $("#CustomerId").val('').trigger(update);
                console.log("No Room Guest Found..!!");
            }
        });
    } else {
        $("#service-type-sec").hide();
        calculateServiceCharge();
    }
});

// Load Customer Section.

function loadCustomerSelectList() {
    const customerTypeId = $("#CustomerTypeId").val();

    if (customerTypeId > 0) {
        const customerType = customerTypeList.find(x => x.id == customerTypeId);

        if (customerType != null && customerType != undefined) {
            if (customerType.code == CustomerType().Employee || customerType.code == CustomerType().Hotel) {
                const url = API + `Customer/GetCustomerJsonData?code=${customerType.code}`;
                createSelectList(url, null, "#CustomerId", null, null);
            } else {
                const url = API + "Customer/GetCustomerJsonData";
                createSelectList(url, null, "#CustomerId", 6, null);
            }
        }
    }

    //const url = API + "Customer/GetCustomerJsonData";
    //createSelectList(url, null, "#CustomerId", 6, null);
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
    const customerTypeId = $("#CustomerTypeId").val();
    const roomId = $("#RoomId").val();

    if (!(customerTypeId > 0)) {
        return failedMsg("Please Select Customer Type..!");
    }

    if (!(customerId > 0)) {
        return failedMsg("Please Select Customer..!");
    }

    if (!(selectedItemList.length > 0)) {
        return failedMsg("No Item Found To Order..!");
    }

    if (customerTypeId > 0) {
        const customerType = customerTypeList.find(x => x.id == customerTypeId);
        if (customerType != null && customerType != undefined) {
            if (customerType.code == CustomerType().Hotel && !(roomId > 0)) {
                return failedMsg("If Customer Type Is Hotel Then Select Room Please...!");
            }
        }
    }

    if (roomId > 0 && customerTypeId > 0) {
        const customerType = customerTypeList.find(x => x.id == customerTypeId);
        if (customerType != null && customerType != undefined) {
            if (customerType.code != CustomerType().Hotel) {
                return failedMsg("If Room Selected Please Select Customer Type Hotel...!");
            }
        }
    }

    if (customerId > 0 && customerTypeId > 0) {
        $("#FoodOrderForm").submit();
    } else {
        failedMsg("Somthing Went Wrong...!");
    }
});

//#region debounce

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

//#region RoomService

$(document.body).on("change", ".roomService", function () {
    calculateServiceCharge();
});

//#endregion