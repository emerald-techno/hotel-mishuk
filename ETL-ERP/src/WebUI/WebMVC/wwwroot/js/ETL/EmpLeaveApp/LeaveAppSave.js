$(document.body).on("click", "#SaveAppBtn", function () {
    const employeeId = $("#EmployeeId").val();
    const leaveTypeId = $("#LeaveTypeId").val();

    if (employeeId > 0 && leaveTypeId > 0) {
        $("#PayrollForm").submit();
    }
})