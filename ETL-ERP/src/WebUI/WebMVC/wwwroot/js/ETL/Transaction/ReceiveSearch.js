
$(document).ready(function () {
    search();
});

$(document.body).on("click", "#SearchBtn", function () {
    search();
});


function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#ReceiveSearchTable")) {
        const table = $("#ReceiveSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ReceiveSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Receive/Search",
            type: "POST",
            data: params
        }, error(e) {
            failedMsg(e);
        },

        "columns": [

            { "data": "serialNo" },
            { "data": "tranNo" },
            {
                "data": "tranDate",
                "render": function (data, type, item) {
                    return convertJsonFullDateForView(new Date(item.tranDate));
                }

            },
            { "data": "orderNo" },
            {
                "data": "orderDate",
                "render": function (data, type, item) {
                    let orderDateStr = "";

                    if (item.orderDate != null) {
                        orderDateStr = convertJsonFullDateForView(new Date(item.orderDate));
                    }

                    return orderDateStr;
                }

            },
            { "data": "supplierName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("ReceiveSearchTable", oTable);

                    let editBtn = "";

                    if (item.canUpdate == true) {
                        editBtn = `<a class='mr-2' href='${API}Receive/Edit/${item.id}'><i class="fa fa-edit"></i></a>`;
                    }

                    const viewBtn = `<a class='mr-2' href='${API}Receive/Details/${item.id}' target='_blank' ><i class="fa fa-search"></i></a>`;
                    const deleteBtn = `<a class='mr-2' href='${API}Transaction/ReceiveDelete/${item.id}'><i class="fa fa-trash"></i></a>`;

                    return `<div style="font-size: 18px;">`
                        + `<div>` + viewBtn + editBtn + `</div>` +
                        `</div>`;
                }
            }

        ]
    });

    addTotalRowCountSpanInDataTable("ReceiveSearchTable");

}

function getSearchObject() {
    const model = {
        TranType: $("#TranType").val(),
        SupplierId: $("#SupplierId").val(),
        OrderId: $("#OrderId").val(),
        SFromDate: $("#SFromDate").val(),
        SToDate: $("#SToDate").val(),
    };
    return model;
}