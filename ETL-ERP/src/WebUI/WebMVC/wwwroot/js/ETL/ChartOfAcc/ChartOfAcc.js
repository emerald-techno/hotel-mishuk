function OpenHeadLedgerCreateForm(headId, headName) {
    CheckIsLedgerFoundForHead(headId, headName)
}

function CheckIsLedgerFoundForHead(headId,headName) {
    if (!hasAnyError(headId)) {
        const url = `${API}AccGroup/IsLedgerFoundInHead?Id=${headId}`;
        $.get(url, function (rData) {
            if (rData != null) {
                if (rData == true) {
                    $('#hdHeadId').val(headId);
                    $('#lblHeadName').html(headName);
                    $('#ledgerModal').modal('show');
                }
                else {
                    $('#hdParentHeadId').val(headId);
                    $('#lblParentHead').html(headName);
                    $('#hdHeadId').val(headId);
                    $('#lblHeadName').html(headName);
                    $('#headModal').modal('show');
                }
                
                console.log(rData);
                //$("#FinYearId").val(rData.id).trigger(update);
            } else {
                //failedMsg("Finc Year Not Found..!")
            }
        })
    }
}

$(document.body).on("click", "#btnCreateLedger", function () {
    $('#headModal').modal('hide');
    $('#ledgerModal').modal('show');
})

$(document.body).on("click", "#btnSaveLedger", function () {
    const headId = $("#hdHeadId").val();
    const ledgerName = $("#txtLedgerName").val();

    if (headId > 0) {
        const url = API + "AccLedger/CreateFromCoa";
        const params = { headId: headId, ledgerName: ledgerName };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Leadger Successful Created");
                setTimeout(() => {
                    window.location.href = API + "AccGroup/ChartOfAcc";
                }, 2000);
            } else {
                failedMsg("Ledger Saved Failed");
            }
        }).fail(function () {
            failedMsg("Ledger Saved Failed");
        })
    } else {
        failedMsg("Head Id Not Found");
    }

    console.log(headId + " " + ledgerName);
})

$(document.body).on("click", "#btnSaveHead", function () {
    const headId = $("#hdParentHeadId").val();
    const headName = $("#txtHeadName").val();

    if (headId > 0) {
        const url = API + "AccHead/CreateFromCoa";
        const params = { parentHeadId: headId, headName: headName };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Head Successful Created");
                setTimeout(() => {
                    window.location.href = API + "AccGroup/ChartOfAcc";
                }, 2000);
            } else {
                failedMsg("Head Saved Failed");
            }
        }).fail(function () {
            failedMsg("Head Saved Failed");
        })
    } else {
        failedMsg("Parent head Id Not Found");
    }


    console.log(headId + " " + headName);
})