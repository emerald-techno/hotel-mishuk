$(document.body).on("click", "#ReviewBtn", function () {
    const id = $("#ReviewerId").val();
    const remarks = $("#Remarks").val();

    const appId = $("#AppId").val();

    var model = {
        id: id,
        remarks: remarks
    }

    if (model.id > 0) {

        const url = API + "LvAppReviewer/ReviewApp";
        const params = {
            vm: model
        }

        $.post(url, params, (rData) => {
            if (!hasAnyError(rData)) {
                successMsg();
                setTimeout(() => {
                    window.location.href = API + "EmpLeaveAppOnline/Details/" + appId;
                }, 3000);
            } else {
                failedMsg();
            }
        });

    } else {
        failedMsg("Sorry! Review Application Failed !");
    }
});


$(document.body).on("click", "#ApproveBtn", function () {
    const id = $("#AprReviewerId").val();
    const remarks = $("#AprRemarks").val();
    const aprFromDate = $("#AprFromDateStr").val();
    const aprToDate = $("#AprToDateStr").val();

    const appId = $("#AppId").val();

    var model = {
        id: id,
        aprFromDateStr: aprFromDate,
        aprToDateStr: aprToDate,
        remarks: remarks
    }

    if (model.id > 0) {

        const url = API + "LvAppReviewer/ApproveApp";
        const params = {
            vm: model
        }

        $.post(url, params, (rData) => {
            if (!hasAnyError(rData)) {
                successMsg();
                setTimeout(() => {
                    window.location.href = API + "EmpLeaveAppOnline/Details/" + appId;
                }, 3000);
            } else {
                failedMsg();
            }
        });

    } else {
        failedMsg("Sorry! Approve Application Failed !");
    }
});


$(document.body).on("click", "#RejectBtn", function () {
    const id = $("#RejReviewerId").val();
    const remarks = $("#RejRemarks").val();

    const appId = $("#AppId").val();

    var model = {
        id: id,
        remarks: remarks
    }

    if (model.id > 0) {

        const url = API + "LvAppReviewer/RejectApp";
        const params = {
            vm: model
        }

        $.post(url, params, (rData) => {
            if (!hasAnyError(rData)) {
                successMsg();
                setTimeout(() => {
                    window.location.href = API + "EmpLeaveAppOnline/Details/" + appId;
                }, 3000);
            } else {
                failedMsg();
            }
        });

    } else {
        failedMsg("Sorry! Reject Application Failed !");
    }
});