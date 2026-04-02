$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#PfFundSearchTable")) {
        const table = $("#PfFundSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#PfFundSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PfFundOpenning/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    let employeeName = `<b>${item.employeeName}</b>`;
                    let employeeCode = `<small>${item.employeeCode}</small>`;
                    const result = `<div style='text-align:left;'>${employeeName}<br/>${employeeCode}</div>`;
                    return result;
                }
            },
            {
                "render": function (data, type, item) {
                    let receiveDate = convertJsonFullDateForView(new Date(item.pfStartDate));
                    return receiveDate;
                }
            },
            { "data": "empCon" },
            { "data": "compCon" },
            { "data": "interest" }
            //{
            //    "render": function (data, type, item) {

            //        showTotalRowCountSpanInDataTable("PfFundSearchTable", oTable);

            //        let viewButton = `<a style='margin-right: 3px; font-size:18px;' href='${API}PfFundOpenning/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
            //        let editButton = `<a style='margin-left: 3px; font-size:18px;' href='${API}PfFundOpenning/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
            //        return `<div style="font-size: 18px;"><div>` + viewButton + ` ` + editButton + `</div></div>`;
            //    }
            //}
        ]
    });

    addTotalRowCountSpanInDataTable("PfFundSearchTable");
}
