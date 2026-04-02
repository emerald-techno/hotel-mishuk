$(document.body).on("change", "#EmployeeId", function () {
    const employeeId = $(this).val();

    if (employeeId > 0) {
        const url = `${API}LeaveCf/GetEmployeeJsonDataById/${employeeId}`;

        $.get(url, function (rData) {
            if (rData != null) {
                console.log("employee", rData);
                renderEmployeeCard(rData);
            }
        }).fail(function () {
            failedMsg("Employee leave carry information Not Found..!!");
        })
    }
});

function renderEmployeeCard(employee) {
    let html = `<div class='row'>`;
    html += `<div class='col-md-3' style='height:120px;'> <img src ='${employee.photoUrl}' style='width: 100%;height: 100%;'/> </div>`;
    html += `<div class='col-md-6 ml-2'>`;
    html += `<div><h5>${employee.name}</h5></div>`;
    html += `<div>${employee.code}</div>`;
    html += `<div>${employee.designationName}</div>`;
    html += `<div>${employee.departmentName}</div>`;
    html += `</div>`;
    html += `</div>`;

    $("#LeaveCardSec").html(html);
}


