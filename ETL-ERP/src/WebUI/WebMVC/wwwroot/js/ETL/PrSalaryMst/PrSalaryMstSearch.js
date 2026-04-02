$(document).ready(function () {
    search();
})

function getMonthNameByIndex(index) {
    const index2 = parseInt(index - 1);
    var monthName = month[index2];
    return monthName;
}

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#PayrollSearchTable")) {
        const table = $("#PayrollSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#PayrollSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PrSalaryMst/Search",
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
            {
                "render": function (data, type, item) {
                    const salaryDate = convertJsonFullDateForView(item.salaryDate);
                    return salaryDate;
                }
            },
            { "data": "totalEmployee" },
            { "data": "totalSalary" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("PayrollSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}PrSalaryMst/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("PayrollSearchTable");
}