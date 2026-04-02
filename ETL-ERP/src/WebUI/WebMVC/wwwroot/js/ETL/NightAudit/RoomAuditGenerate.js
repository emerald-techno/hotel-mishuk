$(document).ready(function () {
    $("#MakeApproveBtn").hide();

    const isRoomAuditGenerated = $("#IsRoomAuditGenerated").val();

    if (isRoomAuditGenerated) {
        loadReportPartial();
    }
    /*loadReportPartial();*/
});

$(document.body).on("click", "#GanerateRoomAuditBtn", function () {

    var businessDate = new Date($("#BusinessDateStr").val());
    var actualDate = new Date();

    var businessDateTime = moment(businessDate.setHours(21)).format("DD-MMM-yyyy HH:mm:ss");
    var actualDateTime = moment(actualDate).format("DD-MMM-yyyy HH:mm:ss");
    var isSameDate = moment(actualDateTime).isSame(businessDateTime, 'day') 

    if (isSameDate && actualDateTime < businessDateTime) {
        swal({
            title: "Are you sure to generate night audit now?",
            text: "If generate before 9.00 PM, there some issue might happen..!!",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    loadReportPartial();
                }
            })
    }
    else {
        loadReportPartial();
    }

    
});

//function loadAuditInfoData() {
//    const auditDateStr = $("#BusinessDateStr").val();

//    if (!hasAnyError(auditDateStr)) {

//        const url = `${API}NightAudit/GetAuditInfoByDate?businessDateStr=${auditDateStr}`;
//        $.get(url, function (rData) {
//            if (rData) {
//                console.log("audit-info: ", rData);
//            } else {
//                console.log("No Audit Info Found...!");
//            }
//        })
//    }
//}

$(document.body).on("click", "#GanerateRoomReAuditBtn", function () {
    reAuditRoomAudit();
});

function reAuditRoomAudit() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {

        const url = `${API}NightAudit/RoomReAudit`;
        const params = { businessDateStr: auditDateStr };

        $.post(url, params, function (rData) {
            if (rData.success == true) {
                successMsg(`${rData.reAuditedRooms} Room Re Auditted Successfully...!!`);
                setTimeout(() => {
                    window.location.reload();
                }, 3000);
            } else {
                failedMsg("Room Re Audit Failed..!!");
            }
        }).fail(function () {
            failedMsg("Room Re Audit Failed..!!");
        })
    }
}

function partialReady() {
    $("#MakeApproveBtn").hide();

    var selectAllRooms = document.getElementById("selectAll");
    var roomCheckboxes = document.querySelectorAll(".checkbox");

    selectAllRooms.addEventListener("change", function () {
        for (var i = 0; i < roomCheckboxes.length; i++) {
            roomCheckboxes[i].checked = selectAllRooms.checked;
            addRoomAuditId(roomCheckboxes[i]);
        }
    });

    for (var i = 0; i < roomCheckboxes.length; i++) {
        roomCheckboxes[i].addEventListener("change", function () {
            if (!this.checked) {
                selectAllRooms.checked = false;
            }
        });
    }
}

function loadReportPartial() {
    const auditDateStr = $("#BusinessDateStr").val();

    if (!hasAnyError(auditDateStr)) {
        const url = API + "NightAudit/GetRoomAuditPartial";
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

                callNextFunction(partialReady);
            } else {
                failedMsg("No Room Audit Info Found");
            }
        });

    }
}

//#region room audit approval

let roomAuditIds = [];

function addRoomAuditId(checkbox) {
    const existId = roomAuditIds.find(c => c == checkbox.value);

    if (checkbox.checked && existId == null) {
        roomAuditIds.push(checkbox.value);
    }
    else {
        const index = roomAuditIds.findIndex(c => c == checkbox.value);

        if (index > -1) {
            roomAuditIds.splice(index, 1);
        }
    }

    if (roomAuditIds != null && roomAuditIds.length > 0) {
        $("#MakeApproveBtn").show();
    } else {
        $("#MakeApproveBtn").hide();
    }
}

$(document.body).on("click", ".apr_btn", function () {
    const index = $(this).attr("data-index");
    const auditId = $(this).attr("data-audit-id");

    if (auditId > 0) {
        const existId = roomAuditIds.find(c => c == auditId);

        if (existId == null) {
            roomAuditIds.push(auditId);
        }
        else {
            const index = roomAuditIds.findIndex(c => c == existId);

            if (index > -1) {
                roomAuditIds.splice(index, 1);
            }
        }

        if (roomAuditIds != null && roomAuditIds.length > 0) {
            const url = API + "NightAudit/MultiRoomAuditApproval";
            const params = { ids: roomAuditIds };

            $.post(url, params, function (rData) {
                if (rData == true) {
                    successMsg("Room Auditted Approve Successfully...!!");
                    setTimeout(() => {
                        /*loadReportPartial();*/
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

$(document.body).on("click", "#MakeApproveBtn", function () {
    submitRoomAudit();
})

function submitRoomAudit() {
    if (roomAuditIds != null && roomAuditIds.length > 0) {
        const url = API + "NightAudit/MultiRoomAuditApproval";
        const params = { ids: roomAuditIds };

        $("#MakeApproveBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Marks Room Auditted Successfully...!!");
                setTimeout(() => {
                    window.location.reload();
                }, 1500);
            } else {
                failedMsg("Room Audit Failed..!!");
            }

            $("#MakeApproveBtn").prop("disabled", false).text("Approve");
        }).fail(function () {
            failedMsg("Room Audit Failed..!!");
            $("#MakeApproveBtn").prop("disabled", false).text("Approve");
        })
    }

}

$(document.body).on("click", "#RoomAuditPrintBtn", function () {
    const model = {
        businessDateStr: $("#BusinessDateStr").val()
    };
    const url = `${API}NightAudit/GetRoomAuditHtmlPrint?businessDateStr=${model.businessDateStr}`;
    window.open(url, "_blank");
});

//#endregion