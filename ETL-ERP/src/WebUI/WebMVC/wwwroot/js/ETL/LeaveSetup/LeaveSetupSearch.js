$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#LeaveSetupSearchTable")) {
        const table = $("#LeaveSetupSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#LeaveSetupSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "LeaveSetup/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "leaveTypeName" },
            { "data": "leaveLimitTypeText" },
            { "data": "leaveBalance" },
            { "data": "genderText" },
            { "data": "isCarryForwardText" },
            { "data": "leaveAfter" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("LeaveSetupSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}LeaveSetup/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` </div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("LeaveSetupSearchTable");
}