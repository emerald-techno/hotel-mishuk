$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#ArrearSearchTable")) {
        const table = $("#ArrearSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ArrearSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PrArrearMst/Search",
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

                    showTotalRowCountSpanInDataTable("ArrearSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}PrArrearMst/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn  + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("ArrearSearchTable");
}