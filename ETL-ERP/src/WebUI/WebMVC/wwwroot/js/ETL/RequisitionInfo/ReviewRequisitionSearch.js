$(document).ready(function () {
    search();
});

$(document.body).on("click", "#ReviewRequsitionSearchButton", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#ReviewRequsitionInfoSearchTable")) {
        const table = $("#ReviewRequsitionInfoSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ReviewRequsitionInfoSearchTable").DataTable({
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
            { "data": "deptName" },
            { "data": "reqByName" },
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

                    showTotalRowCountSpanInDataTable("ReviewRequsitionInfoSearchTable", oTable);

                    let viewBtn = ``;

                    const chk = `<a class='mr-2' href='${API}RequsitionInfo/Details/${item.id}'><i class="fa fa-eye"></i></a>`;

                    if (item.status == 1) {
                        viewBtn = `<a class='mr-2' href='${API}RequsitionInfo/ReviewRequsition/${item.id}'><i class='fa fa-check'></i></a>`;
                    }

                    return `<div style="font-size: 22px;">`
                        + `<div>` + viewBtn + chk + `</div>` +
                        `</div>`;
                }
            }

        ]
    });

    addTotalRowCountSpanInDataTable("ReviewRequsitionInfoSearchTable");
}

function getSearchObject() {
    const model = {
        DeptId: $("#DeptId").val(),
        ReqById: $("#ReqById").val(),
    };
    return model;
}