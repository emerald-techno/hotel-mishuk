$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#EmpLeaveAppOnlineSearchTable")) {
        const table = $("#EmpLeaveAppOnlineSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#EmpLeaveAppOnlineSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "EmpLeaveApp/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "employeeName" },
            { "data": "leaveTypeName" },
            {
                "render": function (data, type, item) {
                    const submitDate = convertJsonFullDateForView(item.submitDate);
                    return submitDate;
                }
            },
            {
                "render": function (data, type, item) {
                    const fromDate = convertJsonFullDateForView(item.fromDate);
                    return fromDate;
                }
            },
            {
                "render": function (data, type, item) {
                    const toDate = convertJsonFullDateForView(item.toDate);
                    return toDate;
                }
            },
            {
                "data": "statusText"
            },
            {
                "render": function (data, type, item) {
                    const file = `<a href='${item.fileDoc}' class='text-center' style='font-size:x-large;' title='Leave Application PDF' target='_blank'><i class="fa fa-file-pdf-o"></i></a>`;
                    return file;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("EmpLeaveAppOnlineSearchTable", oTable);

                    let viewBtn = `<a class='m-2' href='${API}EmpLeaveAppOnline/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    /*let editButton = `<a class='m-2' href='${API}EmpLeaveApplication/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;*/
                    /*                let deleteButton = `<a class='m-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;*/
                    return `<div style='font-size: 20px;text-align: center;'>${viewBtn}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("EmpLeaveAppOnlineSearchTable");
}
