$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#AccGroupSearchTable")) {
        const table = $("#AccGroupSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#AccGroupSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "AccGroup/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "name" },
            { "data": "groupCode" },
            { "data": "groupShortCode" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("AccGroupSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}AccGroup/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("AccGroupSearchTable");
}
