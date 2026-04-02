$(document).ready(function () {
    search();
})

$(document.body).on("click", "#BillSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();
    /*const searchVm = {};*/

    if ($.fn.DataTable.isDataTable("#BillSearchTable")) {
        const table = $("#BillSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#BillSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Bill/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    let bookingDate = convertJsonFullDateForView(new Date(item.bookingDate));
                    const bookingInfo = `<div>
                                    <h5>${item.bookingNo}</h5>
                                    <b>${bookingDate}</b>
                                  </div>`;

                    return bookingInfo;
                },
            },
            {
                "render": function (data, type, item) {
                    let billDate = convertJsonFullDateForView(new Date(item.billDate));
                    const bookingInfo = `<div>
                                    <h5>${item.billNo}</h5>
                                    <b>${billDate}</b>
                                  </div>`;

                    return bookingInfo;
                },
            },
            {
                "render": function (data, type, item) {

                    let billStatus = "";

                    if (item.billStatus == 0) {
                        billStatus = `<div style='font-size: 12px' class='badge badge-primary'>Fresh</div>`;
                    } else if (item.billStatus == 1) {
                        billStatus = `<div style='font-size: 12px' class='badge badge-secondary'>Partial Paid</div>`;
                    } else if (item.billStatus == 2) {
                        billStatus = "<div style='font-size: 12px' class='badge badge-info'>Full Paid</div>";
                    } else if (item.billStatus == 3) {
                        billStatus = "<div style='font-size: 12px' class='badge badge-info'>Refunded</div>";
                    }

                    let div = `<div>${billStatus}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {
                    const totalBill = `<div class='d-flex flex-column'>
                                    <b>Net Amount: ${item.netAmount}</b>
                                    <b>Vat: ${item.vat}</b>
                                    <b>Tax: ${item.tax}</b>
                                    <b>Discount: ${item.discount}</b>
                                    <b>Total Amount: ${item.totalAmount}</b>
                                  </div>`;

                    return totalBill;
                },
            },
            {
                "render": function (data, type, item) {

                    let roomList = `<div><b>${item.roomList}</b></div>`;

                    return roomList;
                },
            },
            { "data": "billByFullName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("BillSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}Bill/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + viewBtn + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("BillSearchTable");
}

function getSearchObject() {
    const model = {
        BillStatus: $("#BillStatus").val(),
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val(),
        BookingNo: $("#BookingNo").val(),
        BillNo: $("#BillNo").val(),
        IsOnlyCheckOutBill: true
    };

    if (model.BookingNo != "" && model.BookingNo != null && model.BookingNo != undefined) {
        model.FormDateStr = '';
        model.ToDateStr = '';
    }

    if (model.BillNo != "" && model.BillNo != null && model.BillNo != undefined) {
        model.FormDateStr = '';
        model.ToDateStr = '';
    }

    return model;
}

function BillStatus() {
    const model = {
        Fresh: 0,
        PartialPaid: 1,
        FullPaid: 2
    }
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
                const url = `${API}Bill/Delete/${id}`;

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