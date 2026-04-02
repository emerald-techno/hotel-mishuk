$(document.body).on("click", "#GaneratePfBtn", function () {
    loadReportPartial();
});

function loadReportPartial() {

    const year = $("#Year").val();
    const month = $("#Month").val();

    if (year > 0 && month > 0) {
        const url = API + "PfFundMst/GetPfFundReportPartial";
        const params = { year: year, month: month };
        loadPfPartialWithParams(url, params, "#PfPartialDiv", null);
    }

}

function loadPfPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
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
                failedMsg("No Pf Found");
            }
        });

    }
}