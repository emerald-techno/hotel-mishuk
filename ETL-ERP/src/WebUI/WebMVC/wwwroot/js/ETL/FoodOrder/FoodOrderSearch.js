$(document).ready(function () {
    search();
})

$(document.body).on("click", "#FoodOrderSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();
    /*const searchVm = {};*/

    if ($.fn.DataTable.isDataTable("#FoodOrderSearchTable")) {
        const table = $("#FoodOrderSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#FoodOrderSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "FoodOrder/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    let orderDate = convertJsonFullDateForView(new Date(item.orderDate));
                    const bookingInfo = `<div>
                                    <h5>${item.orderNo}</h5>
                                    <b>${orderDate}</b>
                                  </div>`;

                    return bookingInfo;
                },
            },
            {
                "render": function (data, type, item) {

                    let room = item.roomId != null && item.roomId > 0 ? `<b>Room No: ${item.roomNo}</b>` : "";

                    const bookingInfo = `<div>
                                    <h5>${item.customerName}</h5>
                                    <b>${item.customerTypeName}</b><br/>
                                    ${room}
                                  </div>`;

                    return bookingInfo;
                },
            },
            {
                "render": function (date, type, item) {

                    let color = 'btn-outline-secondary';

                    if (item.orderStatus == 1) {
                        color = 'btn-outline-primary';
                    } else if (item.orderStatus == 2) {
                        color = 'btn-outline-danger';
                    }

                    let orderStatus = `<button class="btn ${color} btn-xs status_btn" type="button" data-bs-toggle="modal" data-bs-target="#statusModal" data-order-id='${item.id}'>
                                        ${item.orderStatusText}
                                    </button>`;

                    if (item.orderStatus != 0) {
                        orderStatus = `<button class="btn ${color} btn-xs" type="button">
                                        ${item.orderStatusText}
                                    </button>`;
                    }

                    return orderStatus;
                }
            },
            {
                "render": function (data, type, item) {

                    let paymentStatus = "";

                    if (item.paymentStatus == 0) {
                        paymentStatus = `<div style='font-size: 12px' class='badge badge-info'>Pending</div>`;
                    } else if (item.paymentStatus == 1) {
                        paymentStatus = `<div style='font-size: 12px' class='badge badge-warning'>PartialPayment</div>`;
                    } else if (item.paymentStatus == 2) {
                        paymentStatus = "<div style='font-size: 12px' class='badge badge-primary'>Full Paid</div>";
                    } else if (item.paymentStatus == 3) {
                        paymentStatus = "<div style='font-size: 12px' class='badge badge-secondary'>Refund</div>";
                    }

                    let div = `<div>${paymentStatus}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {
                    console.log("item", item);
                    const totalBill = `<div class='d-flex flex-column'>
                                    <b>Order Amount: ${item.orderAmount}</b>
                                    <b>Service Charge: ${item.serviceCharge}</b>
                                    <b>Discount: ${item.discount}</b>
                                    <b>Net Amount: ${item.netAmount}</b>
                                  </div>`;

                    return totalBill;
                },
            },
            { "data": "saleByFullName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("FoodOrderSearchTable", oTable);
                    let editButton = '';
                    let refundButton = '';

                    if (item.orderType == 1) {
                        if (item.orderStatus == 0) {
                            editButton = `<a style='margin-left: 3px; font-size:18px;' href='${API}FoodOrder/UpdateReservation/${item.id}' title='Update'><i class="fa fa-edit"></i></a>`;
                        }

                        if (item.orderStatus == 2 && item.paymentStatus != 0) {
                            refundButton = `<a style='margin-left: 3px; font-size:18px;' href='${API}FoodOrder/RefundReservation/${item.id}' title='Refund'><i class="fa fa-refresh"></i></a>`;
                        }
                    }

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}FoodOrder/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    //let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>${viewBtn}${editButton}${refundButton}</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("FoodOrderSearchTable");
}

function getSearchObject() {
    const model = {
        OrderType: $("#OrderType").val(),
        OrderStatus: $("#OrderStatus").val(),
        PaymentStatus: $("#PaymentStatus").val(),
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val()
    };

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

//$(document.body).on("click", ".delconfirm", function () {
//    swal({
//        title: "Delete Confirmation",
//        text: "Are you sure to delete this item?",
//        icon: "warning",
//        buttons: true,
//        dangerMode: true,
//    })
//        .then((result) => {
//            if (result) {
//                var id = $(this).attr("data-id");
//                const url = `${API}Bill/Delete/${id}`;

//                $.get(url, function (rData) {
//                    if (rData) {
//                        successMsg("Deleted Successfully");
//                    } else {
//                        failedMsg("Deleted Failed...!")
//                    }
//                    search();
//                })
//            }
//        })
//})

//#region Change Order Status
$(document.body).on("click", ".status_btn", function () {
    const orderId = $(this).data("order-id");

    if (orderId > 0) {
        $("#SelectedOrderId").val(orderId);
    } else {
        failedMsg("Order Information Not Found...!!");
    }
});

$(document.body).on("change", "#SelectedOrderStatus", function () {
    const orderStatus = $(this).val();
    const orderId = $("#SelectedOrderId").val();

    if (orderId > 0 && orderStatus >= 0) {
        const url = API + "FoodOrder/FoodOrderStatusChange";

        const params = {
            orderStatus: orderStatus,
            orderId: orderId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {

                search();
                successMsg("Order Status Change Successful");
                $("#statusModal").modal('hide');
                clearStatusForm();

            } else {
                failedMsg("Order Status Change Failed");
            }
        }).fail(function () {
            failedMsg("Order Status Change Failed");
        })
    } else {
        console.log("Failed");
    }
});

function clearStatusForm() {
    $("#SelectedOrderId").val("");
    $("#SelectedOrderStatus").val("").trigger("change");
}

//#endregion