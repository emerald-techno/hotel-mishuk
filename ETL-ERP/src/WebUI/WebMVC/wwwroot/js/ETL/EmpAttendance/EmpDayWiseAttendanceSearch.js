$(document).ready(function () {
    $("#table-section").hide();
    search();
});

$(document.body).on("click", "#DayWiseAttendanceBtn", function () {
    $("#table-section").show();
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#DayWiseAttendanceSearchTable")) {
        const table = $("#DayWiseAttendanceSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#DayWiseAttendanceSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,
        "bFilter": false,
        "ajax": {
            url: API + "EmpAttendance/DayWiseAttendanceReport",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "employeeName" },
            { "data": "empDesignation" },
            { "data": "empDepartment" },
            { "data": "inTimeStr" },
            { "data": "outTimeStr" },
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
                    }
                    else {
                        empStatus = "";
                    }
                    return empStatus;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("DayWiseAttendanceSearchTable");
    $("#printBtnSection").html(`<button class='btn btn-primary' type='button' id='printBtn'>Print</button>`);
}
$(document.body).on("click", "#printBtn", function () {
    printReport();
});

function printReport() {
    const searchDateStr = $("#SearchDateStr").val();
    const designationId = $("#DesignationId").val();
    const departmentId = $("#DepartmentId").val();

    if (searchDateStr != null) {
        const url = `${API}EmpAttendance/DayWiseAttendanceReportPrint?searchDate=${searchDateStr}&designationId=${designationId}&departmentId=${departmentId}`;
        window.open(url, "_blank");
    }
}

function getSearchObject() {
    const model = {
        SearchDateStr: $("#SearchDateStr").val(),
        DesignationId: $("#DesignationId").val(),
        DepartmentId: $("#DepartmentId").val(),
    };
    return model;
}