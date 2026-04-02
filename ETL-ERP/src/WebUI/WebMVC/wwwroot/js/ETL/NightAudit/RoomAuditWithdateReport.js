$(document).ready(function () {

    loadReportPartial();
    loadFdPaymentReportPartial();
    loadRsOrderReportPartial();
    loadRsOrderPaymentReportPartial();
    loadServiceReportPartial();
    loadHallReportPartial();
});

//#region Report Load On Button Click
$(document.body).on("click", "#ShowReportBtn", function () {
    loadReportPartial();
    loadFdPaymentReportPartial();
    loadRsOrderReportPartial();
    loadRsOrderPaymentReportPartial();
    loadServiceReportPartial();
    loadHallReportPartial();
});

//#endregion

//#region Room_Audit
function loadReportPartial() {

    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const url = API + "NightAudit/GetRoomAuditHtml";
        const params = { businessDateStr: auditDateStr };

        loadRoomAuditPartialWithParams(url, params, "#RoomAuditPartialDiv", null);

    }

}

function loadRoomAuditPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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
            } else {
                failedMsg("No Room Audit Info Found");
            }
        });

    }
}

// Room Audit Print
$(document.body).on("click", "#RoomAuditPrintBtn", function () {
    const model = {
        businessDateStr: $("#BusinessDateStr").val()
    };
    const url = `${API}NightAudit/GetRoomAuditHtmlPrint?businessDateStr=${model.businessDateStr}`;
    window.open(url, "_blank");
});

//#endregion

//#region PaymentTransection Audit
function loadFdPaymentReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const paymentAuditUrl = API + "NightAudit/GetPaymentTransactionHtml";
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
            } else {
                failedMsg("No Payment Transaction Audit Info Found");
            }
        });
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

//#endregion

//#region Restarant_order_Audit

function loadRsOrderReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const restaurentAuditUrl = API + "NightAudit/GetRestaurantAuditHtml";
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
            } else {
                failedMsg("No Restaurent Audit Info Found");
            }
        });
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


//#endregion

//#region Rs_Order_Payment_Audit
function loadRsOrderPaymentReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const rsPaymentAuditUrl = API + "NightAudit/GetRestaurantPaymentAuditHtml";
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
            } else {
                failedMsg("No Restaurant Transection Audit Info Found");
            }
        });
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

//#endregion

//#region Service_Audit
function loadServiceReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const serviceAuditUrl = API + "NightAudit/GetServiceAuditHtml";
        const params = { businessDateStr: auditDateStr };
    
        loadServiceAuditPartialWithParams(serviceAuditUrl, params, "#ServiceAuditPartialDiv", null);
    }

}

function loadServiceAuditPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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
            } else {
                failedMsg("No Service Audit Info Found");
            }
        });
    }
}
//  Service Audit Print
$(document.body).on("click", "#ServiceAuditPrintBtn", function () {
    const model = {
        businessDateStr: $("#BusinessDateStr").val()
    };
    const url = `${API}NightAudit/ServiceAuditPrint?businessDateStr=${model.businessDateStr}`;
    window.open(url, "_blank");
});


//#endregion

//#region HallAudit

function loadHallReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const hallAuditUrl = API + "NightAudit/GetHallAuditHtml";
        const params = { businessDateStr: auditDateStr };

        loadHallAuditPartialWithParams(hallAuditUrl, params, "#hallAuditPartialDiv", null);
    }

}

function loadHallAuditPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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
            } else {
                failedMsg("No Service Audit Info Found");
            }
        });
    }
}

// Hall Audit Print
$(document.body).on("click", "#HallAuditPrintBtn", function () {
    const model = {
        businessDateStr: $("#BusinessDateStr").val()
    };
    const url = `${API}NightAudit/HallAuditPrint?businessDateStr=${model.businessDateStr}`;
    window.open(url, "_blank");
});


//#endregion