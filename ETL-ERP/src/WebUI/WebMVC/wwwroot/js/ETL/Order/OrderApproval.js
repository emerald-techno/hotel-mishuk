let approveDtlist = [];

$(document).ready(function () {
    getOrderInfo();
})

$(document.body).on("click", "#ApproveBtn", function () {
    const orderId = $("#Id").val();
    //getOrderInfo();
    const model = {
        Id: orderId,
        Status: 3,
        ApprovalDtls: approveDtlist
    }

    if (model.Id > 0) {
        const url = API + "Order/ApproveOrder";

        const params = { modelVm: model };

        $.post(url, params, function (rData) {
            if (!hasAnyError(rData) && rData == true) {
                console.log(rData);
                successMsg("Order Approved !");

                setUrl(API + "Order/ApprovalSearch");
                reloadPage();
            } else {
                failedMsg("Order Approve Failed !");
            }
        }).fail(
            failedMsg("Order Approve Failed !")
        );
    }
});

$(document.body).on("click", "#RejectBtn", function () {
    const orderId = $("#Id").val();

    if (orderId > 0) {
        const url = API + "Order/RejectOrder";

        const params = { orderId: orderId };

        $.post(url, params, function (rData) {
            if (!hasAnyError(rData) && rData == true) {
                console.log(rData);
                successMsg("Order Rejected !");

                setUrl(API + "Order/ApprovalSearch");
                reloadPage();
            } else {
                failedMsg("Order Rejection Failed !");
            }
        }).fail("Order Rejection Failed !")
    }
});

$(document.body).on("change", ".ApproveQty", function () {
    const index = $(this).attr("data-index");
    const detailId = $(this).attr("data-id");
    const qtyValue = $(`#ApproveQty_${index}`).val();

    if (detailId > 0) {
        const arrayIndex = approveDtlist.findIndex(x => x.id == detailId);

        approveDtlist[arrayIndex].aprOrderQty = qtyValue;

        calculateSubTotal(index, qtyValue, approveDtlist[arrayIndex].rate)
    }
});

function calculateSubTotal(index, qty, rate) {
    let subtotal = qty * rate;
    $(`#SubTotal_${index}`).val(subtotal);

    calculateNetTotal();
}


function calculateNetTotal() {
    var sum = 0;

    $.each(approveDtlist, function (index, value) {
        sum += parseFloat(value.aprOrderQty) * parseFloat(value.rate);
    });

    $("#net-total").val(sum);
}

function getOrderInfo() {
    const orderId = $("#Id").val();

    const url = API + "Order/GetById/" + orderId;

    $.get(url, function (rData) {
        if (rData !== undefined && rData.orderDtls.length > 0) {
            approveDtlist = [];

            rData.orderDtls.forEach(v => {
                const model = {
                    id: v.id,
                    aprOrderQty: v.orderQty,
                    rate: v.rate
                }
                approveDtlist.push(model);
            });
        }
    });
}