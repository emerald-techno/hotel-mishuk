$(document.body).on("click", "#EmpAttendanceReportBtn", function () {

    const model = {
        EmployeeId: $("#EmployeeId").val(),
        Year: $("#Year").val(),
        Month: $("#Month").val()
    }

    if (model.Year > 1999 && model.Month > 0) {
        const url = API + "EmpAttendance/AttendanceReport";
        const params = { model: model };
        $.post(url, params, function (rData) {
            console.log(rData);
            if (rData != null) {
                generateReportTable(rData);
            }
        });
    } else {
        console.log("Sorry! No Data Found !");
    }

});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#EmpAttendanceSearchTableBody").empty();

        data.forEach((v, i) => {
            const serialCell = `<td>${i + 1}</td>`;
            const employeeCell = `<td class='text-start'><b>${v.employeeName}</b><br/>${v.employeeCode}</td>`;
            const departmentCell = `<td>${v.empDepartment}</td>`;
            const designationCell = `<td>${v.empDesignation}</td>`;
            const presentCell = `<td>${v.empPresent}</td>`;
            const absentCell = `<td>${v.empAbsent}</td>`;
            const leaveCell = `<td>${v.empLeave}</td>`;
            const holidayCell = `<td>${v.empHoliday}</td>`;
            const offdayCell = `<td>${v.empOffDay}</td>`;
            const lateCell = `<td>${v.empLateDay}</td>`;

            const row = `<tr>${serialCell}${employeeCell}${departmentCell}${designationCell}${presentCell}${absentCell}${leaveCell}${holidayCell}${offdayCell}${lateCell}</tr>`;

            $("#EmpAttendanceSearchTableBody").append(row);
        });
        $("#printBtnSection").html(`<button class='btn btn-primary' type='button' id='printBtn'>Print</button>`);
    } else {
        $("#EmpAttendanceSearchTableBody").html("<tr><td colSpan='10'><b>No Data Found</b></td></tr>");
    }

}

$(document.body).on("click", "#printBtn", function () {
    printReport();
});

function printReport() {
    const model = {
        EmployeeId: $("#EmployeeId").val(),
        Year: $("#Year").val(),
        Month: $("#Month").val()
    }

    if (model.Year > 1999 && model.Month > 0) {
        const url = `${API}EmpAttendance/PrintMonthlyAttReport?year=${model.Year}&month=${model.Month}&empId=${model.EmployeeId}`;
        window.open(url, "_blank");
    }
}