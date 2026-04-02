$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#LoanSearchTable")) {
        const table = $("#LoanSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#LoanSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "EmpLoanMst/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "employeeName" },
            {
                "render": function (data, type, item) {
                    const passDate = convertJsonFullDateForView(item.loanPassDate);
                    return passDate;
                }
            },
            {
                "render": function (data, type, item) {
                    const payDate = convertJsonFullDateForView(item.loanPayDate);
                    return payDate;
                }
            },
            { "data": "loanAmount" },
            {
                "render": function (data, type, item) {
                    const firstDate = convertJsonFullDateForView(item.firstInsDate);
                    return firstDate;
                }
            },
            { "data": "instalmentCount" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("LoanSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}EmpLoanMst/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let editButton = `<a class='mr-2' href='${API}EmpLoanMst/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("LoanSearchTable");
}