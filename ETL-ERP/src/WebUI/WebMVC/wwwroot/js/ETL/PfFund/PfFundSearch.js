$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#PfSearchTable")) {
        const table = $("#PfSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#PfSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PfFundMst/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    const passDate = convertJsonFullDateForView(item.fundFromDate);
                    return passDate;
                }
            },
            {
                "render": function (data, type, item) {
                    const passDate = convertJsonFullDateForView(item.fundToDate);
                    return passDate;
                }
            },
            {
                "render": function (data, type, item) {
                    const monthName = getMonthNameByIndex(item.prMstMonth);
                    return monthName;
                }
            },
            { "data": "totalEmpCon" },
            { "data": "totalCompCon" },
            { "data": "interest" },
            { "data": "pfAmount" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("PfSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}PfFundMst/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("PfSearchTable");
}