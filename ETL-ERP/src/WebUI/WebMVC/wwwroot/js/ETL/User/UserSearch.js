$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#UserSearchTable")) {
        const table = $("#UserSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#UserSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "User/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "fullName" },
            { "data": "email" },
            { "data": "mobile" },
            { "data": "userRoleName" },
            //{
            //    "render": function (data, type, item) {

            //        showTotalRowCountSpanInDataTable("UserSearchTable", oTable);

            //        let editButton = `<a class='mr-2' href='${API}User/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
            //        return `<div style="font-size: 18px;"><div>` + editButton + `</div></div>`;
            //    }
            //}
        ]
    });

    addTotalRowCountSpanInDataTable("UserSearchTable");
}