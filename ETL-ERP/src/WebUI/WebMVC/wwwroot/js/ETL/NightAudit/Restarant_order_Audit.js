$(document).ready(function () {
    $("#Restaurent_ApproveBtn").hide();

    loadRsOrderReportPartial();
});

function RsOrdersPartialReady() {
    $("#Restaurent_ApproveBtn").hide();

    var selectAllOrders = document.getElementById("selectAll_Rs_Orders");
    var orderCheckboxes = document.querySelectorAll(".rs_order_checkbox");

    selectAllOrders.addEventListener("change", function () {
        for (var i = 0; i < orderCheckboxes.length; i++) {
            orderCheckboxes[i].checked = selectAllOrders.checked;
            addRestaurentOrderAuditId(orderCheckboxes[i]);
        }
    });

    for (var i = 0; i < orderCheckboxes.length; i++) {
        orderCheckboxes[i].addEventListener("change", function () {
            if (!this.checked) {
                selectAllOrders.checked = false;
            }
        });
    }
}

function loadRsOrderReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const restaurentAuditUrl = API + "NightAudit/GeRestaurantAuditPartial";
        const params = { businessDateStr: auditDateStr };

        loadRestaurantAuditPartialWithParams(restaurentAuditUrl, params, "#RestaurantAuditPartialDiv", null);
    }

}

function loadRestaurantAuditPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
    $(targetEl).html("");
    if (!hasAnyError(url) && !hasAnyError(targetEl)) {
        startUiBlock();

        $.post(url, params, function (rData) {
            stopUiBlock();
            
            if (!hasAnyError(rData)) {
                $(targetEl).html(rData);

                if (!hasAnyError(scrollDiv) && convertStringToBool(scrollDiv) == true) {
                    scrollDiv(targetEl);
                }
                callNextFunction(RsOrdersPartialReady);
            } else {
                failedMsg("No Restaurent Audit Info Found");
            }
        });
    }
}


//#region Payment Transaction Audit

let orderIds = [];

function addRestaurentOrderAuditId(checkbox) {
    const existId = orderIds.find(c => c == checkbox.value);

    if (checkbox.checked && existId == null) {
        orderIds.push(checkbox.value);
    }
    else {
        const index = orderIds.findIndex(c => c == checkbox.value);

        if (index > -1) {
            orderIds.splice(index, 1);
        }
    }

    if (orderIds != null && orderIds.length > 0) {
        $("#Restaurent_ApproveBtn").show();
        console.log('orderIds', orderIds);
    } else {
        $("#Restaurent_ApproveBtn").hide();
    }
}

$(document.body).on("click", ".restaurent_single_apr_btn", function () {
    const index = $(this).attr("data-index");
    const auditId = $(this).attr("data-audit-id");

    if (auditId > 0) {
        const existId = orderIds.find(c => c == auditId);

        if (existId == null) {
            orderIds.push(auditId);
            
        }
        else {
            const index = orderIds.findIndex(c => c == existId);

            if (index > -1) {
                orderIds.splice(index, 1);
            }
        }

        if (orderIds != null && orderIds.length > 0) {
            const url = API + "NightAudit/MultiRsOrderAuditApproval";
            const params = { ids: orderIds };

            $.post(url, params, function (rData) {
                if (rData == true) {
                    successMsg("Restaurent Food Order Auditted Approve Successfully...!!");
                    setTimeout(() => {
                        //loadRsOrderReportPartial();
                        window.location.reload();
                    }, 1000);
                } else {
                    failedMsg("Restaurent Food Order Audit Failed..!!");
                }
            }).fail(function () {
                failedMsg("Restaurent Food Order Audit Failed..!!");
            })

        } else {
            failedMsg("Restaurent Food Order Audit Failed..!!");
        }
    }
});

$(document.body).on("click", "#Restaurent_ApproveBtn", function () {
    submitRsFoodOrderAudit();
})

function submitRsFoodOrderAudit() {
    if (orderIds != null && orderIds.length > 0) {
        const url = API + "NightAudit/MultiRsOrderAuditApproval";
        const params = { ids: orderIds };

        $("#Restaurent_ApproveBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Marks Restaurent Food Order Auditted Successfully...!!");
                setTimeout(() => {
                    //loadRsOrderReportPartial();
                    window.location.reload();
                }, 1500);
            } else {
                failedMsg("Restaurent Food Order Audit Failed..!!");
            }

            $("#Restaurent_ApproveBtn").prop("disabled", false).text("Approve");
        }).fail(function () {
            failedMsg("Restaurent Food Order Audit Failed..!!");
            $("#Restaurent_ApproveBtn").prop("disabled", false).text("Approve");
        })
    }

}


// Restaurant Audit Print
$(document.body).on("click", "#RestaurantAuditPrintBtn", function () {
    const model = {
        businessDateStr: $("#BusinessDateStr").val()
    };
    const url = `${API}NightAudit/RestaurantAuditPrint?businessDateStr=${model.businessDateStr}`;
    window.open(url, "_blank");
});
