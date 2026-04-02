$(document).ready(function () {
    search();
});

function getMonthNameByIndex(index) {
    const index2 = parseInt(index - 1);
    var monthName = month[index2];
    return monthName;
}

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#MonthlyAttSearchTable")) {
        const table = $("#MonthlyAttSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#MonthlyAttSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "MonthlyAttSheet/Search",
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
            { "data": "totalDay" },
            { "data": "holiday" },
            { "data": "offDay" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("MonthlyAttSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}MonthlyAttSheet/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("MonthlyAttSearchTable");
}