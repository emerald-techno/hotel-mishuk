$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#GuestInfoSearchTable")) {
        const table = $("#GuestInfoSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#GuestInfoSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "GuestInfo/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    let img = '';

                    if (!hasAnyError(item.photoUrl)) {
                        img = `<div class='avatar' style='width: 50px; height: 60px; margin: auto';>
                                    <img src ='${item.photoUrl}' style='width: 100%;height: 100%;'/>
                                </div>`;
                    } else {
                        img = `<div class='avatar' style='width: 50px; height: 60px; margin: auto';>
                                    <img src ='${API}/images/no_avatar.jpg' style='width: 100%;height: 100%;'/>
                                </div>`;
                    }

                    return img;
                }
            },
            { "data": "guestCode" },
            {
                "render": function (data, type, item) {

                    let div = `<div class='d-flex flex-column'>${item.fullName}</div>`;

                    return div;
                }
            },
            { "data": "mobile" },
            { "data": "companyName" },
            {
                "render": function (data, type, item) {

                    let isVip = "";

                    if (item.isVip) {
                        isVip = `<div style='font-size: 12px' class='badge badge-primary m-1'> YES </div>`;
                    } else {
                        isVip = "<div style='font-size: 12px' class='badge badge-danger m-1'> NO </div>";
                    }

                    let div = `<div class='d-flex flex-column'>${isVip}</div>`;

                    return div;
                }
            },
            { "data": "note" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("GuestInfoSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}GuestInfo/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    let printButton = `<a class='mr-2' href='${API}GuestInfo/GuestPrint/${item.id}' target='_blank' title='PRINT'><i class="fa fa-file"></i></a>`;
                    let detailsButton = `<a class='mr-2' href='${API}GuestInfo/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;

                    return `<div style="font-size: 18px;"><div>${detailsButton} ${editButton} ${deleteButton} ${printButton}</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("GuestInfoSearchTable");
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
                const url = `${API}GuestInfo/Delete/${id}`;

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