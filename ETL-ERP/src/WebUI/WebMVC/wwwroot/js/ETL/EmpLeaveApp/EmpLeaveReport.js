$(document).ready(function () {
    $("#LeaveReportSec").hide();
    $("#LeaveStatementSec").hide();
});
$(document.body).on("click", "#PrintBtn", function () {
    LeaveEmployee();
});

$(document.body).on("click", "#LeaveReportBtn", function () {
    const model = {
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
        EmpDepatmentId: $("#EmpDepatmentId").val()
    }

    if (model.FormDateStr == "") {
        failedMsg("From Date Not Found..!!");
    }
    else {
        const url = API + "EmpLeaveApp/LeaveReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        if (rData != null) {
            $("#LeaveReportSec").show();
            generateReportTable(rData);
        }
    });
    }
});

$(document.body).on("click", "#reportPdf", function () {
    const model = {
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
        EmpDepatmentId: $("#EmpDepatmentId").val()
    }

    if (!hasAnyError(model.FormDateStr)) {
        const url = `${API}EmpLeaveApp/LeaveReportPrint?fromDate=${model.FormDateStr}&toDate=${model.ToDateStr}&dptId=${model.EmpDepatmentId}`;
        window.open(url, "_blank");
    }
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#LeaveReportTableBody").empty();

        data.forEach((v, i) => {
            const serialCell = `<td>${i + 1}</td>`;
            const employeeCell = `<td class='text-start'><b>${v.employeeName}</b><br/>${v.employeeCode}</td>`;
            const departmentCell = `<td>${v.empDepartment}</td>`;
            const designationCell = `<td>${v.empDesignation}</td>`;
            const joinDateCell = `<td>${convertJsonFullDateForView(new Date(v.empJoinDate))}</td>`;
            const casualCell = `<td>${v.casualLeave}</td>`;
            const medicalCell = `<td>${v.medicalLeave}</td>`;
            const maturnityCell = `<td>${v.maturnityLeave}</td>`;
            const earnCell = `<td>${v.earnLeave}</td>`;
            const withoutCell = `<td>${v.leaveWithoutPay}</td>`;
            const studyCell = `<td>${v.studyLeave}</td>`;
            const totalCell = `<td>${v.totalLeave}</td>`;

            const row = `<tr>${serialCell}${employeeCell}${departmentCell}${designationCell}${joinDateCell}${casualCell}${medicalCell}${maturnityCell}${earnCell}${withoutCell}${studyCell}${totalCell}</tr>`;

            $("#LeaveReportTableBody").append(row);
        });
    } else {
        $("#LeaveReportTableBody").html("<tr><td colSpan='12'><b>No Data Found</b></td></tr>");
    }

}


$(document.body).on("click", "#EmpLeaveStatementBtn", function () {
    const model = {
        EmployeeId: $("#EmployeeId").val(),
        SelectYear: $("#SelectYear").val(),
    }

    const url = `${API}EmpLeaveApp/EmpLeaveStatement?employeeId=${model.EmployeeId}&selectYear=${model.SelectYear}`;
    const params = { model: model };
    $.post(url, params, function (rData) {
        if (rData != null) {
            console.log(rData);
            $("#LeaveStatementSec").show();
            generateStatementTable(rData);
        }
    });
});



function generateStatementTable(data) {
    if (true) {
        $("#LeaveStatementTableBody").empty();

        let html = `<div class='row'>`;
        html += `<div class='col-md-2' style='height:120px;margin-left:26px;'> <img src ='${data.empPhotoUrl}' style='width: 100%;height: 100%;'/> </div>`;
        html += `<div class='col-md-6 ml-2'>`;
        html += `<div><b>Name : </b>${data.employeeName}</div>`;
        html += `<div><b>Code : </b>${data.code}</div>`;
        html += `<div><b>Gender : </b>${data.genderText}</div>`;
        html += `<div><b>Designation : </b>${data.empDesignation}</div>`;
        html += `<div><b>Department : </b>${data.empDepartment}</div>`;
        html += `<div><b>Join Date : </b>${convertJsonOnlyDate(data.joinDate)}</div>`;
        html += `</div>`;
        html += `</div>`;

        $("#HeaderPart").html(html);

        let totalleaveBalance = 0;
        let totalleaveTaken = 0;
        let totalleaveRemain = 0;
        let totalcFBalance = 0;
        let totalSum = 0;

        data.leaveBalanceVms.forEach((v, i) => {
            const serialCell = `<td>${i + 1}</td>`;
            const leaveTypeName = `<td>${v.leaveTypeName}</td>`;
            const leaveBalance = `<td class='text-end'>${v.currentYearBalance}</td>`;
            const cFBalance = `<td class='text-end'>${v.carryForwardBalance}</td>`;
            const totalSumBalance = `<td class='text-end'>${(v.leaveBalance)}</td>`;
            const leaveTaken = `<td class='text-end'>${v.leaveTaken}</td>`;
            const leaveRemain = `<td class='text-end'>${v.leaveRemain}</td>`;

            const row = `<tr>${serialCell}${leaveTypeName}${leaveBalance}${cFBalance}${totalSumBalance}${leaveTaken}${leaveRemain}</tr>`;

            $("#LeaveStatementTableBody").append(row);

            totalleaveBalance = (totalleaveBalance + v.leaveBalance);
            totalcFBalance = (totalcFBalance + v.carryForwardBalance);
            totalSum = (totalSum + (v.leaveBalance + v.carryForwardBalance));
            totalleaveTaken = (totalleaveTaken + v.leaveTaken);
            totalleaveRemain = (totalleaveRemain + v.leaveRemain);
        });

        const totalRow = `<tr>
                            <td colspan='2' class='text-end'><b>Total</b></td>
                            <td class='text-end'><b>${totalleaveBalance}</b></td>
                            <td class='text-end'><b>${totalcFBalance}</b></td>>
                            <td class='text-end'><b>${totalSum}</b></td>>
                            <td class='text-end'><b>${totalleaveTaken}</b></td>
                            <td class='text-end'><b>${totalleaveRemain}</b></td>>
                          </tr>`;

        $("#LeaveStatementTableBody").append(totalRow);
    } else {
        $("#LeaveStatementTableBody").html("<tr><td colSpan='9'><b>No Data Found</b></td></tr>");
    }

}

function LeaveEmployee() {
    const EmployeeId = $("#EmployeeId").val();
    const SelectYear = $("#SelectYear").val();

    const url = `${API}EmpLeaveApp/EmpLeaveStatementPrint?employeeId=${EmployeeId}&selectYear=${SelectYear}`;
    window.open(url, "_blank");
}