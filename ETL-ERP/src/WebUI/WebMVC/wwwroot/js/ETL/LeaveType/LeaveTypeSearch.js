$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#LeaveTypeSearchTable")) {
        const table = $("#LeaveTypeSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#LeaveTypeSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "LeaveType/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "typeName" },
            { "data": "balance" },
            {
                "render": function (data, type, item) {
                    let genderType = "";
                    if (item.gender == 'F') genderType = "Female"
                    else if (item.gender == 'A') genderType = "All";
                    return genderType;
                }
            },
            {
                "render": function (data, type, item) { 

                    showTotalRowCountSpanInDataTable("LeaveTypeSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}LeaveType/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    /*let deleteButton = `<a class='ml-2' href='${API}LeaveType/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;*/
                    return `<div style="font-size: 18px;"><div>` + editButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("LeaveTypeSearchTable");
}