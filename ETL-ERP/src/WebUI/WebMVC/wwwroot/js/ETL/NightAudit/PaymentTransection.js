$(document).ready(function () {
    $("#FD_TransApproveBtn").hide();

    loadFdPaymentReportPartial();
});

function fdPaymentPartialReady() {
    $("#FD_TransApproveBtn").hide();

    var selectAllPayments = document.getElementById("selectAll_FD_Transections");
    var paymentCheckboxes = document.querySelectorAll(".fd_payment_checkbox");

    selectAllPayments.addEventListener("change", function () {
        for (var i = 0; i < paymentCheckboxes.length; i++) {
            paymentCheckboxes[i].checked = selectAllPayments.checked;
            addPaymentTransactionAuditId(paymentCheckboxes[i]);
        }
    });

    for (var i = 0; i < paymentCheckboxes.length; i++) {
        paymentCheckboxes[i].addEventListener("change", function () {
            if (!this.checked) {
                selectAllPayments.checked = false;
            }
        });
    }
}

function loadFdPaymentReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const paymentAuditUrl = API + "NightAudit/GetPaymentTransectionAuditPartial";
        const params = { businessDateStr: auditDateStr };

        loadPaymentTransectionAuditPartialWithParams(paymentAuditUrl, params, "#PaymentAuditPartialDiv", null);
    }

}

function loadPaymentTransectionAuditPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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
                callNextFunction(fdPaymentPartialReady);
            } else {
                failedMsg("No Payment Transaction Audit Info Found");
            }
        });
    }
}


//#region Payment Transaction Audit

let paymentIds = [];

function addPaymentTransactionAuditId(checkbox) {
    const existId = paymentIds.find(c => c == checkbox.value);

    if (checkbox.checked && existId == null) {
        paymentIds.push(checkbox.value);
    }
    else {
        const index = paymentIds.findIndex(c => c == checkbox.value);

        if (index > -1) {
            paymentIds.splice(index, 1);
        }
    }

    if (paymentIds != null && paymentIds.length > 0) {
        $("#FD_TransApproveBtn").show();
    } else {
        $("#FD_TransApproveBtn").hide();
    }
}

$(document.body).on("click", ".fd_payment_single_apr_btn", function () {
    const index = $(this).attr("data-index");
    const auditId = $(this).attr("data-audit-id");

    if (auditId > 0) {
        const existId = paymentIds.find(c => c == auditId);

        if (existId == null) {
            paymentIds.push(auditId);
        }
        else {
            const index = paymentIds.findIndex(c => c == existId);

            if (index > -1) {
                paymentIds.splice(index, 1);
            }
        }

        if (paymentIds != null && paymentIds.length > 0) {
            const url = API + "NightAudit/FoPaymentAuditApproval";
            const params = { ids: paymentIds };

            $.post(url, params, function (rData) {
                if (rData == true) {
                    successMsg("Fornt Office Payment Auditted Approve Successfully...!!");
                    setTimeout(() => {
                        //loadFdPaymentReportPartial();

                        window.location.reload();
                    }, 1000);
                } else {
                    failedMsg("Fornt Office Payment Audit Failed..!!");
                }
            }).fail(function () {
                failedMsg("Fornt Office Payment Audit Failed..!!");
            })

        } else {
            failedMsg("Fornt Office Payment Audit Failed..!!");
        }
    }
});

$(document.body).on("click", "#FD_TransApproveBtn", function () {
    submitFoPaymentAudit();
})

function submitFoPaymentAudit() {
    if (paymentIds != null && paymentIds.length > 0) {
        const url = API + "NightAudit/FoPaymentAuditApproval";
        const params = { ids: paymentIds };

        $("#FD_TransApproveBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Fornt Office Payment Auditted Successfully...!!");
                setTimeout(() => {
                    //loadFdPaymentReportPartial();
                    window.location.reload();
                }, 1500);
            } else {
                failedMsg("Fornt Office Payment Audit Failed..!!");
            }

            $("#FD_TransApproveBtn").prop("disabled", false).text("Approve");
        }).fail(function () {
            failedMsg("Fornt Office Payment Audit Failed..!!");
            $("#FD_TransApproveBtn").prop("disabled", false).text("Approve");
        })
    }

}


//Payment Transaction Print
$(document.body).on("click", "#PaymentTransAuditPrintBtn", function () {
    const model = {
        businessDateStr: $("#BusinessDateStr").val()
    };
    const url = `${API}NightAudit/PaymentTransactionAuditPrint?businessDateStr=${model.businessDateStr}`;
    window.open(url, "_blank");
});