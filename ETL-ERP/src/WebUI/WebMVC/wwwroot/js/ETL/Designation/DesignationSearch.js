$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#DesignationSearchTable")) {
        const table = $("#DesignationSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#DesignationSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Designation/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "name" },
            { "data": "code" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("DesignationSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}Designation/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    //let deleteButton = `<a class='ml-2' href='${API}Designation/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;
                    let deleteButton = '';
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("DesignationSearchTable");
}