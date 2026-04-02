$(document).ready(function () {
    search();
})

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#ReviewEmpLeaveAppSearchTable")) {
        const table = $("#ReviewEmpLeaveAppSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ReviewEmpLeaveAppSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "LvAppReviewer/Search",
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
                    let appNo = `<small>${item.applicationNo}</small>`;
                    let leaveType = `<small>${item.leaveTypeName}</small>`;
                    const result = `<div style='text-align:left;'> ${appNo}<br/> ${leaveType}</div>`;
                    return result;
                }
            },
            {
                "render": function (data, type, item) {
                    const fromDate = convertJsonFullDateForView(item.appFromDate);
                    return fromDate;
                }
            },
            {
                "render": function (data, type, item) {
                    const toDate = convertJsonFullDateForView(item.appToDate);
                    return toDate;
                }
            },
            {
                "data": "appStatusText"
            },
            {
                "data": "slNoText"
            },
            {
                "data": "statusText"
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("ReviewEmpLeaveAppSearchTable", oTable);

                    let viewBtn = `<a class='m-2' href='${API}EmpLeaveAppOnline/Details/${item.leaveAppId}' title='View'><i class="fa fa-search"></i></a>`;
                    /*let editButton = `<a class='m-2' href='${API}EmpLeaveApplication/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;*/
                    /*                let deleteButton = `<a class='m-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;*/
                    return `<div style='font-size: 20px;text-align: center;'>${viewBtn}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("ReviewEmpLeaveAppSearchTable");
}


function getSearchObject() {
    const model = {
        ReviewerId: $("#ReviewerId").val()
    }

    return model;
}