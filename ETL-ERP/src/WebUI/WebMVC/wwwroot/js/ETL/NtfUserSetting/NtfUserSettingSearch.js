$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#NtfUserSearchTable")) {
        const table = $("#NtfUserSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#NtfUserSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "NtfUser/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "userName" },
            { "data": "eventName" },
            { "data": "isEnable" },
            { "data": "isEmail" },
            { "data": "isSms" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("NtfUserSearchTable", oTable);

                    //let editButton = `<a class='mr-2' href='${API}Designation/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2' href='${API}NtfUser/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("NtfUserSearchTable");
}