$(document).ready(function () {
    $('#MDiscount').val(0);
    $('#MVat').val(0);
    $('#MTax').val(0);
});

$(document.body).on("hidden.bs.modal", "#serviceAddModal", function () {
    clearModal('#serviceAddModal');
});

$(document.body).on("click", "#PayBillBtn", function () {
    const billId = $("#BillId").val();
    const payAmount = $("#PaidAmount").val();

    if (billId > 0 && payAmount > 0) {
        $("#PayBillForm").submit();
    }
});

//$(document.body).on("click", "#ServiceAddBtn", function () {
//    const billId = parseInt($("#MBillId").val());
//    const serviceId = parseInt($("#MServiceId").val());
//    const rate = parseFloat($("#MRate").val());
//    const qty = parseFloat($("#MQty").val());
//    const amount = parseFloat($("#MAmount").val());
//    const vat = parseFloat($("#MVat").val());
//    const tax = parseFloat($("#MTax").val());
//    const discount = parseFloat($("#MDiscount").val());
//    const netAmount = parseFloat($("#MNetAmount").val());

//    if (serviceId > 0 && rate > 0 && qty > 0) {
//        const url = API + "Bill/BillServiceAdd";

//        const params = {
//            billId: billId,
//            serviceId: serviceId,
//            rate: rate,
//            quantity: qty,
//            amount: amount,
//            vat: vat,
//            tax: tax,
//            discount: discount,
//            netAmount: netAmount
//        };

//        $.post(url, params, function (rData) {
//            if (rData == true) {
//                successMsg("Service Add Successful");
//                $("#serviceAddModal").modal('hide');
//                clearServiceForm();

//                setTimeout(() => {
//                    window.location.href = API + "Bill/Details/" + billId;
//                }, 3000);
//            } else {
//                failedMsg("Service Entry Failed");
//            }
//        }).fail(function () {
//            failedMsg("Service Entry Failed");
//        })
//    } else {
//        failedMsg("Information Is Not Correct..!");
//    }
//})

function clearServiceForm() {
    $("#MServiceId").val("").trigger("change");
    $("#MRate").val("");
    $("#MQty").val("");
    $("#MAmount").val("");
    $("#MVat").val("");
    $("#MTax").val("");
    $("#MDiscount").val("");
    $("#MNetAmount").val("");
}

let complimentServiceList = [];

$(document.body).on("click", ".service_item", function () {

    const index = $(this).attr("data-index");

    console.log("data-index: ", index);

    const isChecked = $(this).is(":checked");

    if (index > -1) {
        if (isChecked) {
            const serviceId = $(`#sa_id_${index}`).val();

            if (serviceId > 0) {
                complimentServiceList.push(serviceId);
            }
        } else {
            const serviceId = $(`#sa_id_${index}`).val();

            var removeIndex = complimentServiceList.findIndex(x => x == serviceId);

            if (removeIndex > -1) {
                complimentServiceList.splice(removeIndex, 1);
            } else {
                console.log("Item can't be remove");
            }
        }
    }
});

$(document.body).on("click", "#complimentSubmitBtn", function () {
    const billId = parseInt($("#BillId").val());
    const remarks = $("#CmpRemarks").val();

    const allService = true;

    if (billId > 0 && allService == true) {
        const url = API + "Bill/MakeBillComplimentary";

        const params = {
            billId: billId,
            remarks: remarks,
            allService: allService,
            serviceIds: complimentServiceList
        }

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Bill Compliment Successful");
                $("#complimentModal").modal('hide');

                setTimeout(() => {
                    window.location.href = API + "Bill/Details/" + billId;
                }, 2000);
            } else {
                failedMsg("Comliment Entry Failed");
            }
        }).fail(function () {
            failedMsg("Comliment Entry Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
});

//#region Discount_Update

$(document.body).on("change", "#U_SpecialDiscount", function () {
    const totalAmount = $("#U_TotalAmount").val();
    const discount = $("#U_Discount").val();
    const specialDiscount = $("#U_SpecialDiscount").val();
    const paidAmount = $("#U_PaidAmount").val();

    const totalDiscount = parseFloat(discount) + parseFloat(specialDiscount);
    const netAmount = parseFloat(totalAmount) - totalDiscount;

    if (paidAmount > netAmount) {
        $("#U_SpecialDiscount").val(0);
        return failedMsg("Paid Amount Is Higher Than Net Amount");
    }

    $("#U_NetAmount").val(netAmount);
})

$(document.body).on("click", "#DiscountUpdateBtn", function () {
    const billId = parseInt($("#U_BillId").val());
    const specialDiscount = parseFloat($("#U_SpecialDiscount").val());

    if (billId > 0) {
        const url = API + "Bill/DiscountUpdate";

        const params = {
            billId: billId,
            discount: specialDiscount,
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Adjust Discount Update Successfull");
                $("#discountUpdateModal").modal('hide');
                clearDiscountForm();

                setTimeout(() => {
                    window.location.href = API + "Bill/Details/" + billId;
                }, 500);
            } else {
                failedMsg("Adjust Discount Update Failed");
            }
        }).fail(function (e) {

            failedMsg(`${e.responseJSON.errors}`);
        })
    } else {
        failedMsg("Payment Information Is Not Correct..!");
    }
})

function clearDiscountForm() {
    $("#U_SpecialDiscount").val("");
}


//#endregion

//#region CloseBill

$(document.body).on("click", "#BillCloseBtn", function () {
    const billId = parseInt($("#BillId").val());

    if (billId > 0) {
        swal({
            title: "Close Bill",
            text: "Are you sure you want to close the bill? Further any service add can't be possible",
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
                const url = API + "Bill/BillClose";

                const params = {
                    billId: billId
                }

                $.post(url, params, function (rData) {
                    if (rData == true) {
                        successMsg("Bill Close Successful");
                        setTimeout(() => {
                            window.location.href = API + "Bill/Details/" + billId;
                        }, 1000);
                    } else {
                        failedMsg("Bill Close Failed");
                    }
                }).fail(function () {
                    failedMsg("Bill Close Failed");
                })
            } else {
                failedMsg("Submission Failed..!");
            }
        });
    } else {
        failedMsg("Information Is Not Correct..!");
    }
});

//#endregion