$(document).ready(function () {
    search();
})

$(document.body).on("click", "#InventoryBillPaymentSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#InventoryBillPaymentSearchTable")) {
        const table = $("#InventoryBillPaymentSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#InventoryBillPaymentSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "InventoryBillPayment/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    const billInfo = `<div>
                                    <h5>Bill: ${item.billNo}</h5>
                                    <h5>${item.billDate}</h5>
                                  </div>`;
                    return billInfo;
                }
            },
            {
                "render": function (data, type, item) {
                    const billInfo = `<div>
                                    <h5>Order: ${item.orderMstNo}</h5>
                                    <h5>${item.orderDate}</h5>
                                  </div>`;
                    return billInfo;
                }
            },
            { "data": "supplierName" },
            { "data": "billByName" },
            { "data": "billAmount" },
            {
                "render": function (data, type, item) {

                    let pay = "";

                    if (item.payMode == 0) {
                        pay = `<div>Cash</div>`;
                    } else if (item.payMode == 1){
                        pay = `<div>Bank</div>`;
                    } else{
                        pay = `<div>BKash</div>`;
                    }

                    let div = `<div class='d-flex flex-column'>${pay}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("InventoryBillPaymentSearchTable", oTable);

                    let detailsButton = `<a class='mr-2' href='${API}InventoryBillPayment/Details/${item.id}' target='_blank'><i class="fa fa-search"></i></a>`;
                    const viewBtn = `<a class='mr-2' href='${API}Issue/Details/${item.id}' target='_blank' ><i class="fa fa-search"></i></a>`;
                    //let editButton = `<a class='mr-2' href='${API}InventoryBillPayment/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    //let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + detailsButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("InventoryBillPaymentSearchTable");
}

function getSearchObject() {
    const model = {
        OrderMstId: $("#OrderMstId").val(),
        SupplierId: $("#SupplierId").val()
    };
    return model;
}

$(document.body).on("click", ".delconfirm", function () {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this item?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                var id = $(this).attr("data-id");
                const url = `${API}InventoryBillPayment/Delete/${id}`;

                $.get(url, function (rData) {
                    if (rData) {
                        successMsg("Deleted Successfully");
                    } else {
                        failedMsg("Deleted Failed...!")
                    }
                    search();
                })
            }
        })
})