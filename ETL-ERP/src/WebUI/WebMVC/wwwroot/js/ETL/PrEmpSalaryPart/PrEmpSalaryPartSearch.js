$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#PrEmpSalaryPartSearchTable")) {
        const table = $("#PrEmpSalaryPartSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#PrEmpSalaryPartSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PrEmpSalaryPart/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "employeeName" },
            { "data": "employeeCode" },            
            { "data": "empDesignation" },            
            { "data": "empDepartment" },            
            { "data": "salaryPartsName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("PrEmpSalaryPartSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}PrEmpSalaryPart/Details?employeeId=${item.employeeId}' title='View'><i class="fa fa-search"></i></a>`;
                    /*let editButton = `<a class='mr-2' href='${API}PrEmpSalaryPart/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;*/
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("PrEmpSalaryPartSearchTable");
}