$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#EmpAttendanceSearchTable")) {
        const table = $("#EmpAttendanceSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#EmpAttendanceSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "EmpAttendance/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "name" },
            { "data": "attendDateStr" },
            { "data": "AttendTimeStr"},
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("EmpAttendanceSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}EmpAttendance/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2' href='${API}EmpAttendance/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("EmpAttendanceSearchTable");
}