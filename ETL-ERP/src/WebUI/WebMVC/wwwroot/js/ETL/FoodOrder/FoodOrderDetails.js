$(document).ready(function () {
    //loadCustomerSelectList()
    const tableNo = $("#table-no").val();
    const orderNo = $("#order-no").val();
    document.title = `${tableNo} || ${orderNo}`;
});

$('#foodAddModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#foodAddModal') // Ensure dropdown appears within modal
    });
});

$(document.body).on("click", "#PayBillBtn", function () {
    const orderId = $("#OrderId").val();
    const payAmount = $("#PaidAmount").val();

    if (orderId > 0 && payAmount > 0) {
        $("#PayBillForm").submit();
    }
});

//#region Food_Item_Add

$(document.body).on("change", "#MFoodId", function () {
    var foodId = $(this).val();

    if (foodId > 0) {
        const url = `${API}FoodItem/GetFoodInfoById/${foodId}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("food-info: ", rData);

                $("#MQty").val(1);
                $("#MRate").val(rData.rate);

                var amount = parseFloat($("#MRate").val()) * parseFloat($("#MQty").val());
                $("#MAmount").val(amount);

            } else {
                console.log("No Food Info Found...!");
            }
        })
    }
});

$(document.body).on("change", "#MQty", function () {
    calculateTotal();
});

$(document.body).on("change", "#MRate", function () {
    calculateTotal();
});

function calculateTotal() {
    const rate = parseFloat($("#MRate").val());
    const qty = parseFloat($("#MQty").val());
    const amount = rate * qty;
    $("#MAmount").val(amount);
}


function clearFoodItemForm() {
    $("#MFoodId").val("").trigger("change");
    $("#MRate").val("");
    $("#MQty").val("");
    $("#MAmount").val("");
}

//#endregion

//#region Discount_Update

$(document.body).on("click", "#DiscountUpdateBtn", function () {
    const orderId = parseInt($("#U_OrderId").val());
    const discount = parseFloat($("#U_Discount").val());

    if (orderId > 0) {
        const url = API + "FoodOrder/DiscountUpdate";

        const params = {
            orderId: orderId,
            discount: discount,
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Discount Update Successful");
                $("#discountUpdateModal").modal('hide');
                clearDiscountForm();

                setTimeout(() => {
                    window.location.href = API + "FoodOrder/Details/" + orderId;
                }, 1500);
            } else {
                failedMsg("Discount Update Failed");
            }
        }).fail(function (e) {

            failedMsg(`${e.responseJSON.errors}`);
        })
    } else {
        failedMsg("Payment Information Is Not Correct..!");
    }
})

function clearDiscountForm() {
    $("#U_Discount").val("");
}


//#endregion

//#region Remove_Item

$(document.body).on("click", ".item_delete_btn", function () {
    const dtlId = $(this).attr("data-id");
    if (dtlId > 0) {
        deleteItem(dtlId);
    }
});


function deleteItem(id) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this item?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url = API + "FoodOrder/DeleteFoodItem/" + id;
                $.get(url, function (rData) {
                    if (rData) {
                        console.log(rData)
                        successMsg("Successfully Deleted!");
                        location.reload();
                    } else {
                        console.log("error")
                        failedMsg("Delete Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}

//#endregion

//#region Served_Item

$(document.body).on("click", ".item_serve_btn", function () {
    const dtlId = $(this).attr("data-id");
    if (dtlId > 0) {
        servedItem(dtlId);
    }
});


function servedItem(id) {
    if (id > 0) {
        const url = API + "FoodOrder/SingleFoodItemServed/" + id;
        $.get(url, function (rData) {
            if (rData) {
                console.log(rData)
                successMsg("Successfully Served..");
                location.reload();
            } else {
                console.log("error")
                failedMsg("Serve Entry Failed...!")
            }

        })
    }
}

//#endregion

//#region KOT_Print
$(document.body).on("click", ".kot-print", function () {
    const orderId = $("#OrderId").val();
    if (orderId > 0) {
        setTimeout(() => {
            window.location.href = API + "FoodOrder/Details/" + orderId;
        }, 1000);
    }
});
//#endregion

//#region Multiple_Item_Add

let itemList = [];

$(document.body).on("click", "#itemAddBtn", function () {
    const orderId = $("#MOrderId").val();
    const foodId = $("#MFoodId").val();
    const foodItemName = $("#MFoodId option:selected").text();
    const qty = parseFloat($("#MQty").val());
    const rate = parseFloat($("#MRate").val());
    const amount = parseFloat($("#MAmount").val());

    const model = {
        id: 0,
        orderId: orderId,
        foodId: foodId,
        foodItemName: foodItemName,
        quantity: qty,
        rate: rate,
        totalAmount: amount
    }

    if (!(model.quantity > 0)) {
        failedMsg("Please Mention Quantity Properly, It should be greater than Zero");
        return false;
    }

    if (!(model.rate > 0)) {
        failedMsg("Please Mention Item Rate Properly, It should be greater than Zero");
        return false;
    }

    if (model.foodId > 0) {
        const exist = itemList.find(x => x.foodId == model.foodId);

        if (exist != null) {
            return failedMsg("Food Item Already Exists In Table..!!");
        } else {
            itemList.push(model);
        }
    }

    renderTBody();
    clearFoodItemForm();
});

//On table rendered Item's qty change

$(document.body).on("change", ".qty-value", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        calculateTableItemTotal(index);
    }
});

function calculateTableItemTotal(index) {

    const qty = parseFloat($(`#qty_${index}`).val());
    const rate = parseFloat($(`#rate_${index}`).val());

    itemList[index].quantity = qty;
    itemList[index].rate = rate;
    itemList[index].totalAmount = itemList[index].quantity * itemList[index].rate;

    const total = qty * rate;
    $(`#total_${index}`).val(total);
}


function renderTBody() {

    $("#additemTbody").empty();

    if (itemList.length > 0) {


        itemList.forEach((v, i) => {

            const itemCell = `<td>${v.foodItemName}</td>`;
            const qtyCell = `<td class='text-center'><input type='number' class='form-control text-center qty-value'  id='qty_${i}' value='${v.quantity}' data-index='${i}'/></td>`;
            const rateCell = `<td class='text-end'><input type='number' class='form-control text-end rate-value'  id='rate_${i}' value='${v.rate}' data-index='${i}'/></td>`;
            const totalCell = `<td class='text-end'><input type='number' class='form-control text-end' value='${v.totalAmount}' id='total_${i}' readonly/></td>`;
            const actionCell = `<td class='text-center'><i class='fa fa-times fa-2x rmvItem' data-index='${i}'></i></td>`;

            const row = `<tr>${itemCell}${qtyCell}${rateCell}${totalCell}${actionCell}</tr>`;

            $("#additemTbody").append(row);
        })
    }
}

$(document.body).on("click", ".rmvItem", function () {
    const index = $(this).attr("data-index");

    if (index > -1) {
        itemList.splice(index, 1);
    }

    renderTBody();
});



$(document.body).on("click", "#FoodAddBtn", function () {
    const orderId = $("#MOrderId").val();

    if (orderId > 0) {
        const url = API + "FoodOrder/MultipleFoodItemAdd";

        const params = {
            dtos: itemList
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Item Added Successfully");
                $("#foodAddModal").modal('hide');

                setTimeout(() => {
                    window.location.href = API + "FoodOrder/Details/" + orderId;
                }, 1000);
            } else {
                failedMsg("Food Item Add Failed");
            }
        }).fail(function (e) {

            failedMsg(`${e.responseJSON.errors}`);
        })
    } else {
        failedMsg("Food Item Is Not Correct..!");
    }
})




//#endregion


//#region Room_Assign
$('#roomAssignModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#roomAssignModal')
    });
});

$(document.body).on("change", "#RoomId", function () {
    const roomId = $(this).val();
    $("#CustomerId").val('').trigger(update);

    if (roomId > 0) {
        const url = `${API}Customer/GetCustomerInfoByRoomId?roomId=${roomId}`;
        $.get(url, function (rData) {
            if (rData != null) {
                console.log("customer-info: ", rData);
                $("#CustomerId").val(rData.id).trigger(update);
                //calculateServiceCharge();
            } else {
                $("#CustomerId").val('').trigger(update);
                console.log("No Room Guest Found..!!");
            }
        })
    }
});


$(document.body).on("click", "#RoomAssignBtn", function () {
    const orderId = parseInt($("#RM_OrderId").val());
    const roomId = $("#RoomId").val();
    const customerId = parseInt($("#CustomerId").val());
    const isRoomService = $(".roomService").prop("checked");

    if (orderId > 0 && roomId > 0 && customerId > 0) {
        const url = API + "FoodOrder/AssignRoom";

        const params = {
            orderId: orderId,
            roomId: roomId,
            customerId: customerId,
            isRoomService: isRoomService
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Room Assign Successful");
                $("#roomAssignModal").modal('hide');

                setTimeout(() => {
                    window.location.href = API + "FoodOrder/Details/" + orderId;
                }, 1000);
            } else {
                failedMsg("Room Assign Failed..!!");
            }
        }).fail(function (e) {
            failedMsg(`${e.responseJSON.errors}`);
        })
    } else {
        failedMsg("Room Assign Information Is Not Correct..!");
    }
});

//#endregion

//#region C_Type and Name Update
$('#cNameUpdateModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#cNameUpdateModal')
    });
});

$('#cTypeUpdateModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#cTypeUpdateModal')
    });
});



$(document.body).on("click", "#typeUpdateBtn", function () {
    const orderId = parseInt($("#CT_OrderId").val());
    const customerTypeId = parseInt($("#CngCustomerTypeId").val());

    if (orderId > 0 && customerTypeId > 0) {
        const url = API + "FoodOrder/CustomerTypeUpdate";

        const params = {
            orderId: orderId,
            customerTypeId: customerTypeId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Customer Type Update Successful");
                $("#cTypeUpdateModal").modal('hide');

                setTimeout(() => {
                    window.location.href = API + "FoodOrder/Details/" + orderId;
                }, 1000);
            } else {
                failedMsg("Customer Type Update Failed..!!");
            }
        }).fail(function (e) {
            failedMsg(`${e.responseJSON.errors}`);
        })
    } else {
        failedMsg("Customer Type Information Is Not Correct..!");
    }
});
$(document.body).on("click", "#nameUpdateBtn", function () {
    const orderId = parseInt($("#CN_OrderId").val());
    const customerId = parseInt($("#CngCustomerId").val());
    const customerTypeId = parseInt($("#CustomerTypeId").val());

    if (orderId > 0 && customerId > 0 && customerTypeId > 0) {
        const url = API + "FoodOrder/CustomerNameUpdate";

        const params = {
            orderId: orderId,
            customerId: customerId,
            customerTypeId: customerTypeId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Customer Name Update Successful");
                $("#cNameUpdateModal").modal('hide');

                setTimeout(() => {
                    window.location.href = API + "FoodOrder/Details/" + orderId;
                }, 1000);
            } else {
                failedMsg("Customer Name Update Failed..!!");
            }
        }).fail(function (e) {
            failedMsg(`${e.responseJSON.errors}`);
        })
    } else {
        failedMsg("Customer Information Is Not Correct..!");
    }
});
//#endregion

//#region Remove_Payment

$(document.body).on("click", ".payment_delete_btn", function () {
    const pmntId = $(this).attr("data-id");
    if (pmntId > 0) {
        deletePayment(pmntId);
    }
});


function deletePayment(id) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this Payment?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url = `${API}FoodOrder/DeletePayment?paymentId=${id}`;
                $.get(url, function (rData) {
                    if (rData) {
                        console.log(rData)
                        successMsg("Successfully Deleted!");

                        setTimeout(() => {
                            window.location.reload()
                        }, 1000);

                    } else {
                        console.log("error")
                        failedMsg("Delete Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}

//#endregion

//#region Remove_Order

$(document.body).on("click", ".order_delete_btn", function () {
    const orderId = $(this).attr("data-id");
    if (orderId > 0) {
        deleteOrder(orderId);
    }
});


function deleteOrder(id) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this Order?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url =`${API}FoodOrder/DeleteOrder?orderId=${id}`;
                $.get(url, function (rData) {
                    if (rData) {
                        console.log(rData)
                        successMsg("Successfully Deleted!");

                        setTimeout(() => {
                            window.location.href = API + "FoodOrder/Search";
                        }, 1000);

                    } else {
                        console.log("error")
                        failedMsg("Delete Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}

//#endregion
