$(document).ready(function () {
    search();
});

$(document.body).on("click", "#OrderSearchBtn", function () {
    search();
});


function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#OrderSearchTable")) {
        const table = $("#OrderSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { searchModel: searchVm };
    }

    const oTable = $("#OrderSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,



        "ajax": {
            url: API + "Order/Search",
            type: "POST",
            data: params
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },            
            {
                "render": function (data, type, item) {
                    return item.supplierName;
                }
            },
            { "data": "orderNo" },
            {
                "render": function (data, type, item) {

                    return convertJsonFullDateForView(new Date(item.orderDate));
                }
            },
            {
                "render": function (data, type, item) {
                    let date = "";

                    if (item.deliveryDate != null) {
                        date = convertJsonFullDateForView(new Date(item.deliveryDate));
                    }

                    return date;
                }
            },
            {
                "render": function (data, type, item) {

                    let req = "";

                    if (!hasAnyError(item.reqNo)) {
                        req = item.reqNo + `<br />` + convertJsonFullDateForView(new Date(item.reqDate));
                    }
                    return req;
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

                    showTotalRowCountSpanInDataTable("OrderSearchTable", oTable);

                    const viewBtn = `<a class='mr-2' style='margin-right: 3px;' href='${API}Order/Details/${item.id}'><i class="fa fa-search"></i></a>`;
                    const editBtn = `<a class='mr-2 href='${API}Order/Edit/${item.id}' title='View'><i class="fa fa-edit"></i></a>`;
                    const deleteBtn = `<a class='mr-2 href='${API}Order/Delete/${item.id}' class='gray-s' title='Edit'><i class="fa fa-trash"></i></a>`;

                    return `<div style="font-size: 18px;">`
                        + `<div>` + viewBtn + `</div>` +
                        `</div>`;
                }
            }

        ]
    });

    addTotalRowCountSpanInDataTable("OrderSearchTable");

}

function getSearchObject() {
    const model = {
        ReqId: $("#ReqId").val(),
        SupplierId: $("#SupplierId").val(),
        SFromDate: $("#SFromDate").val(),
        SToDate: $("#SToDate").val(),
        OrderType: "I",
    };
    return model;
}