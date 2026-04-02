$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#RoleSearchTable")) {
        const table = $("#RoleSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#RoleSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Role/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "name" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("RoleSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}Role/AddOrEdit?id=${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("RoleSearchTable");
}