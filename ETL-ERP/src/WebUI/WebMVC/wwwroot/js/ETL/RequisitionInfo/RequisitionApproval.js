let approveDtlList = [];

$(document).ready(function () {
    getRequsitionInfo();
});

$(document.body).on("click", "#ApproveBtn", function () {

    const reqId = $("#Id").val();

    const model = {
        Id: reqId,
        Status: 3,
        ApprovalDtls: approveDtlList
    }

    if (model.Id > 0) {
        const url = API + "RequsitionInfo/ApproveRequsition";

        const params = { modelVm: model };

        $.post(url, params, function (rData) {

            if (!hasAnyError(rData) && rData == true) {
                console.log(rData);
                successMsg("Requsition Approve");

                setUrl(API + "RequsitionInfo/ApprovalSearch");
                reloadPage();

            } else {
                failedMsg("Requsition Approved Failed !");
            }
        }).fail(
            failedMsg("Requsition Approved Failed !")
        );
    }

});

$(document.body).on("click", "#RejectBtn", function () {

    const reqId = $("#Id").val();

    if (reqId > 0) {
        const url = API + "RequsitionInfo/RejectRequsition";

        const params = { reqId: reqId };

        $.post(url, params, function (rData) {

            if (!hasAnyError(rData) && rData == true) {
                console.log(rData);
                successMsg("Requsition Rejected")
            } else {
                failedMsg("Requsition Rejection Failed !")
            }
        }).fail(
            failedMsg("Requsition Rejection Failed !")
        );
    }

});


$(document.body).on("change", ".ApproveQty", function () {
    const index = $(this).attr("data-index");
    const detailId = $(this).attr("data-id");
    const qtyValue = $(`#ApproveQty_${index}`).val();


    if (detailId > 0) {

        const arrayIndex = approveDtlList.findIndex(x => x.id == detailId);

        approveDtlList[arrayIndex].aprReqQty = qtyValue;
    }

});


function getRequsitionInfo() {

    const reqId = $("#Id").val();

    const url = API + "RequsitionInfo/GetById/" + reqId;
    $.get(url, function (rData) {
        if (rData !== undefined) {

            if (rData.requsitionInfoDtls.length > 0) {

                approveDtlList = [];

                rData.requsitionInfoDtls.forEach(v => {

                    const model = {
                        id: v.id,
                        aprReqQty: v.reqQty
                    }

                    approveDtlList.push(model);
                });

            }
        }
    });

}