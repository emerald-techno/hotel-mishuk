$(document).ready(function () {
    $("#Restaurant_Payments_ApproveBtn").hide();

    loadRsOrderPaymentReportPartial();
});

function RsOrderPaymentsPartialReady() {
    $("#Restaurant_Payments_ApproveBtn").hide();

    var selectAllOrderPayments = document.getElementById("selectAll_Rs_Orders_Payments");
    var orderPaymentCheckboxes = document.querySelectorAll(".rs_order_payment_checkbox");

    selectAllOrderPayments.addEventListener("change", function () {
        for (var i = 0; i < orderPaymentCheckboxes.length; i++) {
            orderPaymentCheckboxes[i].checked = selectAllOrderPayments.checked;
            addRsOrderPaymentsAuditId(orderPaymentCheckboxes[i]);
        }
    });

    for (var i = 0; i < orderPaymentCheckboxes.length; i++) {
        orderPaymentCheckboxes[i].addEventListener("change", function () {
            if (!this.checked) {
                selectAllOrderPayments.checked = false;
            }
        });
    }
}

function loadRsOrderPaymentReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const rsPaymentAuditUrl = API + "NightAudit/GeRestaurantPaymentAuditPartial";
        const params = { businessDateStr: auditDateStr };

        loadRsPaymentAuditPartialWithParams(rsPaymentAuditUrl, params, "#RestaurantTransectionAuditTab", null);
    }

}

function loadRsPaymentAuditPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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
                callNextFunction(RsOrderPaymentsPartialReady);
            } else {
                failedMsg("No Restaurant Transection Audit Info Found");
            }
        });
    }
}


//#region Payment Transaction Audit

let rsPaymentIds = [];

function addRsOrderPaymentsAuditId(checkbox) {
    const existId = rsPaymentIds.find(c => c == checkbox.value);

    if (checkbox.checked && existId == null) {
        rsPaymentIds.push(checkbox.value);
    }
    else {
        const index = rsPaymentIds.findIndex(c => c == checkbox.value);

        if (index > -1) {
            rsPaymentIds.splice(index, 1);
        }
    }

    if (rsPaymentIds != null && rsPaymentIds.length > 0) {
        $("#Restaurant_Payments_ApproveBtn").show();
        console.log('rsPaymentIds', rsPaymentIds);
    } else {
        $("#Restaurant_Payments_ApproveBtn").hide();
    }
}

$(document.body).on("click", ".restaurant_payment_single_apr_btn", function () {
    const index = $(this).attr("data-index");
    const auditId = $(this).attr("data-audit-id");

    if (auditId > 0) {
        const existId = rsPaymentIds.find(c => c == auditId);

        if (existId == null) {
            rsPaymentIds.push(auditId);
            
        }
        else {
            const index = rsPaymentIds.findIndex(c => c == existId);

            if (index > -1) {
                rsPaymentIds.splice(index, 1);
            }
        }

        if (rsPaymentIds != null && rsPaymentIds.length > 0) {
            const url = API + "NightAudit/RsPaymentAuditApproval";
            const params = { ids: rsPaymentIds };

            $.post(url, params, function (rData) {
                if (rData == true) {
                    successMsg("Restaurant Payment Auditted Approve Successfully...!!");
                    setTimeout(() => {
                        //loadRsOrderPaymentReportPartial();
                        window.location.reload();
                    }, 1000);
                } else {
                    failedMsg("Restaurant Payment Audit Failed..!!");
                }
            }).fail(function () {
                failedMsg("Restaurant Payment Audit Failed..!!");
            })

        } else {
            failedMsg("Restaurant Payment Audit Failed..!!");
        }
    }
});

$(document.body).on("click", "#Restaurant_Payments_ApproveBtn", function () {
    submitRsPaymentAudit();
})

function submitRsPaymentAudit() {
    if (rsPaymentIds != null && rsPaymentIds.length > 0) {
        const url = API + "NightAudit/RsPaymentAuditApproval";
        const params = { ids: rsPaymentIds };

        $("#Restaurant_Payments_ApproveBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Marks Room Auditted Successfully...!!");
                setTimeout(() => {
                    //loadRsOrderPaymentReportPartial();
                    window.location.reload();
                }, 1500);
            } else {
                failedMsg("Room Audit Failed..!!");
            }

            $("#Restaurant_Payments_ApproveBtn").prop("disabled", false).text("Approve");
        }).fail(function () {
            failedMsg("Room Audit Failed..!!");
            $("#Restaurant_Payments_ApproveBtn").prop("disabled", false).text("Approve");
        })
    }

}

// Restaurant Payment Audit Print
$(document.body).on("click", "#RestaurantPaymentAuditPrintBtn", function () {
    const model = {
        businessDateStr: $("#BusinessDateStr").val()
    };
    const url = `${API}NightAudit/RestaurantPaymentAuditPrint?businessDateStr=${model.businessDateStr}`;
    window.open(url, "_blank");
});