$('#approveModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var groupId = button.data('group-id');
    var headId = button.data('head-id');

    var modal = $(this)

    console.log("group-id", groupId);
    console.log("head-id", headId);

    $("#AccGroupId").val(groupId);
    $("#AccHeadId").val(headId);
});


$('#ladgerModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var headId = button.data('head-id');

    var modal = $(this)

    console.log("head-id", headId);

    $("#lgAccHeadId").val(headId);
});


$(document.body).on("click", "#HeadSubmitBtn", function () {
    const groupId = $("#AccGroupId").val();
    const headId = $("#AccHeadId").val();
    const headName = $("#HeadName").val();

    if (groupId > 0 && headId > 0 && !hasAnyError(headName)) {
        console.log("Submit to Account Head");

        const accHeadUrl = API + "AccHead/Create";
        const params = { parentHeadId: headId, groupId: groupId, headName: headName, isAjaxPost: true };

        $.post(accHeadUrl, params, function (rData) {
            if (rData == true) {
                successMsg("Account Sub Head Added...!");
                setTimeout(() => {
                    window.location.href = API + "AccGroup/ChartOfAcc";
                }, 3000);
            } else {
                failedMsg("Account Sub Head Entry Failed");
            }
        }).fail(function () {
            failedMsg("Account Sub Head Entry Failed");
        })

    }
})

$(document.body).on("click", "#LadgerSubmitBtn", function () {
    const headId = $("#lgAccHeadId").val();
    const ladgerName = $("#LadgerName").val();

    if (headId > 0 && !hasAnyError(ladgerName)) {

        const accLadgerUrl = API + "AccLedger/Create";
        const params = { headId: headId, ledgerName: ladgerName, isAjaxPost: true };

        $.post(accLadgerUrl, params, function (rData) {
            if (rData == true) {

                successMsg("Account Ladger Added...!");

                setTimeout(() => {
                    window.location.href = API + "AccGroup/ChartOfAcc";
                }, 3000);
            } else {
                failedMsg("Account Ladger Entry Failed");
            }
        }).fail(function () {
            failedMsg("Account Ladger Entry Failed");
        })

    }
})