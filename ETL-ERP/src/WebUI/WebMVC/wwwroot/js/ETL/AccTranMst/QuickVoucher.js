$(document).ready(function () {
    quickLedgerReport();
    $("#Amount").val('');
    bottomScroll();

    $('#LedgerId').select2('focus');
    $(".form-control").attr("autocomplete", "off");
});

function bottomScroll() {
    const scrollableDiv = document.getElementById("scrollableDiv");

    // Function to scroll to the bottom
    function scrollToBottom() {
        scrollableDiv.scrollTop = scrollableDiv.scrollHeight;
    }

    // Set up a MutationObserver to watch for added nodes (childList)
    const observer = new MutationObserver(() => {
        scrollToBottom();
    });

    // Options for the observer: watch for child nodes being added
    const config = { childList: true, subtree: true };

    // Start observing the scrollableDiv
    observer.observe(scrollableDiv, config);
}

$(document.body).on("change", "#LedgerId", function () {
    $('#DrCr').select2('focus');
})

function quickLedgerReport() {
    const model = {
        StrFromDate: $("#VcDateStr").val(),
        StrToDate: $("#VcDateStr").val(),
        LedgerId: $("#MishukLedgerId").val(),
        FinYearId: $("#CurrentFincYearId").val()
    }
    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "AccTranMst/QuickLedgerReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}


function getReportByDate(date) {
    if (!hasAnyError(date)) {
        quickLedgerReport();
    }
}

$(document.body).on("click", "#QuickLedgerPrintBtn", function () {
    const model = {
        StrFromDate: $("#VcDateStr").val(),
        StrToDate: $("#VcDateStr").val(),
        LedgerId: $("#MishukLedgerId").val(),
        FinYearId: $("#CurrentFincYearId").val()
    }

    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate) && model.LedgerId > 0 && model.FinYearId > 0) {
        const url = `${API}AccTranMst/QuickLedgerReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&ledgerId=${model.LedgerId}&finYearId=${model.FinYearId}`;
        window.open(url, "_blank");
    }
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#LedgerReportContainer").empty();
        $("#LedgerReportContainer").append(data);

        bottomScroll();
    } else {
        $("#LedgerReportContainer").html("<b>No Data Found</b>");
    }

}

$(document.body).on("click", ".UpdateVcBtn", function () {
    const voucherId = $(this).attr("data-id");
    console.log("voucher-id", voucherId);

    if (voucherId > 0) {
        const url = `${API}AccTranMst/GetQuickVoucherById?vcId=${voucherId}`;
        $.get(url, function (rData) {
            if (rData != null) {
                console.log("quick-voucher:", rData);

                $("#UpdateVcId").val(voucherId);
                $("#UpdateVoucherNo").val(rData.vcNo);
                $("#UpdateNarration").val(rData.narration);
                $("#UpdateAmount").val(rData.amount);

                $(".update-date").datepicker({
                    dateFormat: 'dd/mm/yy',
                });

                $("#UpdateVcDate").val(rData.vcDateStr);
                

                $("#UpdateLedgerId").val(rData.ledgerId).trigger(update);
                $("#UpdateDrCr").val(rData.drCr).trigger(update);
            } else {
                failedMsg("Voucher Not Found..!")
            }
        })
    }
});

$(document.body).on("click", "#UpdateSubmitBtn", function () {

    const vcId = $("#UpdateVcId").val();
    const vcDateStr = $("#UpdateVcDate").val();
    const ledgerId = $("#UpdateLedgerId").val();
    const drCr = $("#UpdateDrCr").val();
    const amount = parseFloat($("#UpdateAmount").val());
    const narration = $("#UpdateNarration").val();
    const mishukLedgerId = $("#MishukLedgerId").val();

    if (ledgerId > 0) {
        const url = API + "AccTranMst/QuickUpdate";

        const params = {
            vcId: vcId,
            vcDateStr: vcDateStr,
            ledgerId: ledgerId,
            drCr: drCr,
            narration: narration,
            amount: amount,
            mishukLedgerId: mishukLedgerId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Voucher Update Successful");
                $("#quickUpdateModal").modal('hide');
                clearUpdateVoucherForm();

                setTimeout(() => {
                    window.location.href = API + "AccTranMst/QuickVoucherEntry";
                }, 1500);
            } else {
                failedMsg("Voucher Update Failed");
            }
        }).fail(function () {
            failedMsg("Voucher Update Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
})

$(document.body).on("click", "#UpdateSubmitBtnRes", function () {

    const vcId = $("#UpdateVcId").val();
    const vcDateStr = $("#UpdateVcDate").val();
    const ledgerId = $("#UpdateLedgerId").val();
    const drCr = $("#UpdateDrCr").val();
    const amount = parseFloat($("#UpdateAmount").val());
    const narration = $("#UpdateNarration").val();
    const mishukLedgerId = $("#MishukLedgerId").val();

    if (ledgerId > 0) {
        const url = API + "AccTranMst/QuickUpdate";

        const params = {
            vcId: vcId,
            vcDateStr: vcDateStr,
            ledgerId: ledgerId,
            drCr: drCr,
            narration: narration,
            amount: amount,
            mishukLedgerId: mishukLedgerId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Voucher Update Successful");
                $("#quickUpdateModal").modal('hide');
                clearUpdateVoucherForm();

                setTimeout(() => {
                    window.location.href = API + "AccTranMst/QuickVoucherEntryForRestaurant";
                }, 1500);
            } else {
                failedMsg("Voucher Update Failed");
            }
        }).fail(function () {
            failedMsg("Voucher Update Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
})


$(document.body).on("click", "#UpdateSubmitBtnResort", function () {

    const vcId = $("#UpdateVcId").val();
    const vcDateStr = $("#UpdateVcDate").val();
    const ledgerId = $("#UpdateLedgerId").val();
    const drCr = $("#UpdateDrCr").val();
    const amount = parseFloat($("#UpdateAmount").val());
    const narration = $("#UpdateNarration").val();
    const mishukLedgerId = $("#MishukLedgerId").val();

    if (ledgerId > 0) {
        const url = API + "AccTranMst/QuickUpdate";

        const params = {
            vcId: vcId,
            vcDateStr: vcDateStr,
            ledgerId: ledgerId,
            drCr: drCr,
            narration: narration,
            amount: amount,
            mishukLedgerId: mishukLedgerId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Voucher Update Successful");
                $("#quickUpdateModal").modal('hide');
                clearUpdateVoucherForm();

                setTimeout(() => {
                    window.location.href = API + "AccTranMst/QuickVoucherEntryForAmariResort";
                }, 1500);
            } else {
                failedMsg("Voucher Update Failed");
            }
        }).fail(function () {
            failedMsg("Voucher Update Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
})


$(document.body).on("click", "#UpdateSubmitBtnKitchen", function () {

    const vcId = $("#UpdateVcId").val();
    const vcDateStr = $("#UpdateVcDate").val();
    const ledgerId = $("#UpdateLedgerId").val();
    const drCr = $("#UpdateDrCr").val();
    const amount = parseFloat($("#UpdateAmount").val());
    const narration = $("#UpdateNarration").val();
    const mishukLedgerId = $("#MishukLedgerId").val();

    if (ledgerId > 0) {
        const url = API + "AccTranMst/QuickUpdate";

        const params = {
            vcId: vcId,
            vcDateStr: vcDateStr,
            ledgerId: ledgerId,
            drCr: drCr,
            narration: narration,
            amount: amount,
            mishukLedgerId: mishukLedgerId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Voucher Update Successful");
                $("#quickUpdateModal").modal('hide');
                clearUpdateVoucherForm();

                setTimeout(() => {
                    window.location.href = API + "AccTranMst/QuickVoucherEntryForKitchen";
                }, 1500);
            } else {
                failedMsg("Voucher Update Failed");
            }
        }).fail(function () {
            failedMsg("Voucher Update Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
})


function clearUpdateVoucherForm() {
    $("#UpdateLedgerId").val("").trigger("change");
    $("#UpdateDrCr").val("").trigger("change");
    $("#UpdateVoucherNo").val("");
    $("#UpdateNarration").val("");
    $("#UpdateVcId").val(0);
}