$(document).ready(function () {
    $("#Service_ApproveBtn").hide();

    loadServiceReportPartial();
});

function ServicePartialReady() {
    $("#Service_ApproveBtn").hide();

    var selectAllservice = document.getElementById("selectAll_Service");
    var serviceCheckboxes = document.querySelectorAll(".service_checkbox");

    selectAllservice.addEventListener("change", function () {
        for (var i = 0; i < serviceCheckboxes.length; i++) {
            serviceCheckboxes[i].checked = selectAllservice.checked;
            addServiceAuditId(serviceCheckboxes[i]);
        }
    });

    for (var i = 0; i < serviceCheckboxes.length; i++) {
        serviceCheckboxes[i].addEventListener("change", function () {
            if (!this.checked) {
                selectAllservice.checked = false;
            }
        });
    }
}

function loadServiceReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const serviceAuditUrl = API + "NightAudit/GetServiceAuditPartial";
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
                callNextFunction(ServicePartialReady);
            } else {
                failedMsg("No Service Audit Info Found");
            }
        });
    }
}


//#region Payment Transaction Audit

let dtlIds = [];

function addServiceAuditId(checkbox) {
    const existId = dtlIds.find(c => c == checkbox.value);

    if (checkbox.checked && existId == null) {
        dtlIds.push(checkbox.value);
    }
    else {
        const index = dtlIds.findIndex(c => c == checkbox.value);

        if (index > -1) {
            dtlIds.splice(index, 1);
        }
    }

    if (dtlIds != null && dtlIds.length > 0) {
        $("#Service_ApproveBtn").show();
    } else {
        $("#Service_ApproveBtn").hide();
    }
}

$(document.body).on("click", ".service_single_apr_btn", function () {
    const auditId = $(this).attr("data-audit-id");

    if (auditId > 0) {
        const existId = dtlIds.find(c => c == auditId);

        if (existId == null) {
            dtlIds.push(auditId); 
        }
        else {
            const index = dtlIds.findIndex(c => c == existId);

            if (index > -1) {
                dtlIds.splice(index, 1);
            }
        }

        if (dtlIds != null && dtlIds.length > 0) {
            const url = API + "NightAudit/MultiExtraServiceAuditApproval";
            const params = { ids: dtlIds };

            $.post(url, params, function (rData) {
                if (rData == true) {
                    successMsg("Extra Service Auditted Approve Successfully...!!");
                    setTimeout(() => {
                        //loadServiceReportPartial();
                        window.location.reload();
                    }, 1000);
                } else {
                    failedMsg("Extra Service Audit Failed..!!");
                }
            }).fail(function () {
                failedMsg("Extra Service Audit Failed..!!");
            })

        } else {
            failedMsg("Extra Service Audit Failed..!!");
        }
    }
});

$(document.body).on("click", "#Service_ApproveBtn", function () {
    submitExtraServiceAudit();
})

function submitExtraServiceAudit() {
    if (dtlIds != null && dtlIds.length > 0) {
        const url = API + "NightAudit/MultiExtraServiceAuditApproval";
        const params = { ids: dtlIds };

        $("#Service_ApproveBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Extra Service Auditted Successfully...!!");
                setTimeout(() => {
                    //loadServiceReportPartial();
                    window.location.reload();
                }, 1500);
            } else {
                failedMsg("Extra Service Failed..!!");
            }

            $("#Service_ApproveBtn").prop("disabled", false).text("Approve");
        }).fail(function () {
            failedMsg("Extra Service Failed..!!");
            $("#Service_ApproveBtn").prop("disabled", false).text("Approve");
        })
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