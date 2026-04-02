$(document.body).on("change", "#LeaveTypeId", function () {
    const leaveTypeId = $(this).val();

    if (leaveTypeId > 0) {
        const url = `${API}LeaveSetup/GetLeaveTypeJsonDataById/${leaveTypeId}`;

        $.get(url, function (rData) {
            if (rData != null) {
                console.log(rData);
                renderLeaveCard(rData);
            }
        }).fail(function () {
            failedMsg("Leave Type Information Not Found..!!");
        })
    }
});

function renderLeaveCard(leaveType) {

    $("#leaveBalance").val(leaveType.balance);
    let leaveGender = "";
    if (leaveType.gender == "A") {
        leaveGender = "All"
    }
    else {
        leaveGender = "Female"
    }
    $("#leaveGender").val(leaveGender);
    $("#Gender").val(leaveType.gender);
}
