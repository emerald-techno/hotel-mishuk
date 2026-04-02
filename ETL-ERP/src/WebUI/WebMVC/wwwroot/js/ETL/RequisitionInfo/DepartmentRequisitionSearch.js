$(document).ready(function () {
    search();
});

$(document.body).on("click", "#RequsitionSearchButton", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#RequsitionInfoSearchTable")) {
        const table = $("#RequsitionInfoSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#RequsitionInfoSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,



        "ajax": {
            url: API + "RequsitionInfo/Search",
            type: "POST",
            data: params
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "reqNo" },
            {
                "render": function (data, type, item) {

                    return convertJsonFullDateForView(new Date(item.reqDate));
                }
            },
            { "data": "reqByName" },
            {
                "render": function (data, type, item) {
                    var p = item.priority;
                    if (p == 'N') {
                        return "<span style='font-size: 12px' class='badge badge-warning'>Normal</span>";
                    } else if (p == 'H') {
                        return "<span style='font-size: 12px' class='badge badge-danger'>High</span>";
                    } else if (p == 'A') {
                        return "<span style='font-size: 12px' class='badge badge-info'>Argent</span>";
                    }
                    return "Not Define";
                }
            },
            {
                "render": function (data, type, item) {
                    let status = ``;

                    if (item.status == 0) {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-primary">${item.statusText}</span>`;
                    } else if (item.status == 1) {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-warning">${item.statusText}</span>`;
                    } else if (item.status == 2) {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-danger">${item.statusText}</span>`;
                    } else if (item.status == 3) {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-success">${item.statusText}</span>`;
                    }

                    return status;

                }
            },
            {
                "render": function (data, type, item) {
                    let isStatus = ``;

                    if (item.isStatus == 0) {
                        isStatus = `<span style='font-size: 12px' class="badge badge-pill badge-info">${item.isStatusText}</span>`;
                    } else if (item.isStatus == 1) {
                        isStatus = `<span style='font-size: 12px' class="badge badge-pill badge-warning">${item.isStatusText}</span>`;
                    } else if (item.isStatus == 2) {
                        isStatus = `<span style='font-size: 12px' class="badge badge-pill badge-success">${item.isStatusText}</span>`;
                    } else if (item.isStatus == 3) {
                        isStatus = `<span style='font-size: 12px' class="badge badge-pill badge-secondary">${item.isStatusText}</span>`;
                    }

                    return isStatus;

                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("RequsitionInfoSearchTable", oTable);

                    let editBtn = ``;


                    const reqType = $("#reqType").val();

                    if (item.status == 0) {
                        editBtn = `<a class='mr-2' href='${API}RequsitionInfo/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    }

                    //if (item.reqType == 'I') {
                    //    viewBtn = `<a class='mr-2 base-view-btn' href='#' data-url='${API}RequsitionInfo/GetDetailPartial?id=${item.id}&reqType=I'><i class="fa fa-eye"></i></a>`;
                    //}
                    //else if (item.reqType == 'S') {
                    //    viewBtn = `<a class='mr-2 base-view-btn' href='#' data-url='${API}RequsitionInfo/GetDetailPartial?id=${item.id}&reqType=S'><i class="fa fa-eye"></i></a>`;
                    //}

                    const viewBtn = `<a class='mr-2' href='${API}RequsitionInfo/Details/${item.id}'><i class="fa fa-eye"></i></a>`;
                    const deleteBtn = `<a class='mr-2' href='${API}Designation/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;

                    return `<div style="font-size: 18px;">`
                        + `<div>` + viewBtn + editBtn /*+ deleteBtn */ + `</div>` +
                        `</div>`;
                }
            }

        ]
    });

    addTotalRowCountSpanInDataTable("RequsitionInfoSearchTable");
}

function getSearchObject() {
    const model = {
        DeptId: $("#DeptId").val(),
        ReqById: $("#ReqById").val(),
        SFromDate: $("#SFromDate").val(),
        SToDate: $("#SToDate").val()
    };
    return model;
}