$(document).ready(function () {
    $("#table-section").hide();
});

$(document.body).on("click", "#EmpAttReportBtn", function () {
    $("#table-section").show();
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#EmpDailyAttendanceSearchTable")) {
        const table = $("#EmpDailyAttendanceSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#EmpDailyAttendanceSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,
        "bFilter": false,
        "ajax": {
            url: API + "EmpAttendance/EmployeeAttendanceDailyReport",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "attendDateStr" },
            { "data": "inTimeStr" },
            { "data": "outTimeStr" },
            {
                "render": function (data, type, item) {
                    let empLate = "";
                    if (item.late) {
                        empLate = `<i class="icofont icofont-ui-check" style="color: tomato;"></i>`;
                    }
                    else {
                        empLate = "";
                    }
                    return empLate;
                }
            },
            {
                "render": function (data, type, item) {
                    let Lunch = "";
                    if (item.isLunch) {
                        Lunch = `<i class="icofont icofont-ui-check" style="color: green;"></i>`;
                    }
                    else {
                        Lunch = "";
                    }
                    return Lunch;
                }
            },
            {
                "render": function (data, type, item) {
                    let empStatus = "";
                    if (item.present) {
                        empStatus = `<div style='font-size: 12px' class='badge badge-primary'>Present</div>`;
                    } else if (item.absent) {
                        empStatus = `<div style='font-size: 12px' class='badge badge-danger'>Absent</div>`;
                    } else if (item.leave) {
                        empStatus = `<div style='font-size: 12px' class='badge badge-secondary'>Leave</div>`;
                    } else if (item.offDay) {
                        empStatus = `<div style='font-size: 12px' class='badge badge-info'>OffDay</div>`;
                    } else if (item.holiday) {
                        empStatus = `<div style='font-size: 12px' class='badge badge-warning'>HoliDay</div>`;
                    } else if (item.noEntry) {
                        empStatus = `<div style='font-size: 12px' class='badge bg-dark'>No Entry</div>`;
                    } else {
                        empStatus = "";
                    }
                    return empStatus;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("EmpDailyAttendanceSearchTable");
    $("#printBtnSection").html(`<button class='btn btn-primary' type='button' id='printBtn'>Print</button>`);
}
$(document.body).on("click", "#printBtn", function () {
    printReport();
});

function printReport() {
    const employeeId = $("#EmployeeId").val();
    const formDateStr = $("#FormDateStr").val();
    const toDateStr = $("#ToDateStr").val();
    const departmentId = $("#DepartmentId").val();
    const designationId = $("#DesignationId").val();

    if (employeeId > 0) {
        const url = `${API}EmpAttendance/PrintEmpJobCard?empId=${employeeId}&formDateStr=${formDateStr}&toDateStr=${toDateStr}&departmentId=${departmentId}&designationId=${designationId}`;
        window.open(url, "_blank");
    }
}

function getSearchObject() {
    const model = {
        EmployeeId: $("#EmployeeId").val(),
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
        DepartmentId: $("#DepartmentId").val(),
        DesignationId: $("#DesignationId").val()
    };
    return model;
}