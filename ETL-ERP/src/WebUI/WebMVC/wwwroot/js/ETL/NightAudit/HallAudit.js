$(document).ready(function () {
    $("#Hall_ApproveBtn").hide();

    loadHallReportPartial();
});

function HallPartialReady() {
    $("#Hall_ApproveBtn").hide();

    var selectAllHall = document.getElementById("selectAll_Hall");
    var hallCheckboxes = document.querySelectorAll(".hall_checkbox");

    selectAllHall.addEventListener("change", function () {
        for (var i = 0; i < hallCheckboxes.length; i++) {
            hallCheckboxes[i].checked = selectAllHall.checked;
            addHallAuditId(hallCheckboxes[i]);
        }
    });

    for (var i = 0; i < hallCheckboxes.length; i++) {
        hallCheckboxes[i].addEventListener("change", function () {
            if (!this.checked) {
                selectAllHall.checked = false;
            }
        });
    }
}

function loadHallReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const hallAuditUrl = API + "NightAudit/GetHallAuditPartial";
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
                callNextFunction(HallPartialReady);
            } else {
                failedMsg("No Service Audit Info Found");
            }
        });
    }
}


//#region Payment Transaction Audit

let bookingHallIds = [];

function addHallAuditId(checkbox) {
    const existId = bookingHallIds.find(c => c == checkbox.value);

    if (checkbox.checked && existId == null) {
        bookingHallIds.push(checkbox.value);
    }
    else {
        const index = bookingHallIds.findIndex(c => c == checkbox.value);

        if (index > -1) {
            bookingHallIds.splice(index, 1);
        }
    }

    if (bookingHallIds != null && bookingHallIds.length > 0) {
        $("#Hall_ApproveBtn").show();
        console.log('bookingHallIds', bookingHallIds);
    } else {
        $("#Hall_ApproveBtn").hide();
    }
}

$(document.body).on("click", ".hall_single_apr_btn", function () {
    const index = $(this).attr("data-index");
    const auditId = $(this).attr("data-audit-id");

    if (auditId > 0) {
        const existId = bookingHallIds.find(c => c == auditId);

        if (existId == null) {
            bookingHallIds.push(auditId);
            
        }
        else {
            const index = bookingHallIds.findIndex(c => c == existId);

            if (index > -1) {
                bookingHallIds.splice(index, 1);
            }
        }

        if (bookingHallIds != null && bookingHallIds.length > 0) {
            const url = API + "NightAudit/MultiHallAuditApproval";
            const params = { ids: bookingHallIds };

            $.post(url, params, function (rData) {
                if (rData == true) {
                    successMsg("Room Auditted Approve Successfully...!!");
                    setTimeout(() => {
                        //loadHallReportPartial();
                        window.location.reload();
                    }, 1000);
                } else {
                    failedMsg("Room Audit Failed..!!");
                }
            }).fail(function () {
                failedMsg("Room Audit Failed..!!");
            })

        } else {
            failedMsg("Room Audit Failed..!!");
        }
    }
});

$(document.body).on("click", "#Hall_ApproveBtn", function () {
    submitHallAudit();
})

function submitHallAudit() {
    if (bookingHallIds != null && bookingHallIds.length > 0) {
        const url = API + "NightAudit/MultiHallAuditApproval";
        const params = { ids: bookingHallIds };

        $("#Hall_ApproveBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Marks Room Auditted Successfully...!!");
                setTimeout(() => {
                    //loadHallReportPartial();
                    window.location.reload();
                }, 1500);
            } else {
                failedMsg("Room Audit Failed..!!");
            }

            $("#Hall_ApproveBtn").prop("disabled", false).text("Approve");
        }).fail(function () {
            failedMsg("Room Audit Failed..!!");
            $("#Hall_ApproveBtn").prop("disabled", false).text("Approve");
        })
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

