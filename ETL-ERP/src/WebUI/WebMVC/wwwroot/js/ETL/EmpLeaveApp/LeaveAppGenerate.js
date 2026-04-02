$(document.body).on("click", "#GanerateAppBtn", function () {
    loadReportPartial();
});

function loadReportPartial() {

    const leaveTypeId = $("#LeaveTypeId").val();
    const fromDateStr = $("#FromDateStr").val();
    const toDateStr = $("#ToDateStr").val();
    const reason = $("#Reason").val();


    if (leaveTypeId > 0 && !hasAnyError(fromDateStr) && !hasAnyError(toDateStr) && !hasAnyError(reason)) {
        const url = API + "EmpLeaveApp/ApplyPreview";
        const params = { leaveTypeId: leaveTypeId, fromDateStr: fromDateStr, toDateStr: toDateStr, reason: reason  };
        loadPayrollPartialWithParams(url, params, "#ApplyPartialDiv", null);
    }

}

function loadPayrollPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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

                callNextFunction(callBackF);
            } else {
                failedMsg("No Info Found");
            }
        });

    }
}