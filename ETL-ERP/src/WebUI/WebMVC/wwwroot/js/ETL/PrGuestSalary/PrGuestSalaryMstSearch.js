$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#GuestSalarySearchTable")) {
        const table = $("#GuestSalarySearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#GuestSalarySearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PrGuestSalaryMst/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "year" },
            {
                "render": function (data, type, item) {
                    const monthName = getMonthNameByIndex(item.month);
                    return monthName;
                }
            },
            { "data": "employeeCount" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("GuestSalarySearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}PrGuestSalaryMst/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let editButton = `<a class='mr-2' href='${API}PrGuestSalaryMst/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("GuestSalarySearchTable");
}