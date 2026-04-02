$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#SupplierInfoSearchTable")) {
        const table = $("#SupplierInfoSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#SupplierInfoSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "SupplierInfo/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "supplierName" },
            { "data": "supplierCode" },
            { "data": "mobile" },
            { "data": "phone" },
            { "data": "email" },
            { "data": "address" },
            {
                "render": function (data, type, item) {
                    let img = '';

                    if (!hasAnyError(item.photoDoc)) {
                        img = `<div class='avatar' style='width: 50px; height: 60px; margin: auto';>
                                    <img src ='${item.photoDoc}' style='width: 100%;height: 100%;'/>
                                </div>`;
                    } else {
                        img = `<div class='avatar' style='width: 50px; height: 60px; margin: auto';>
                                    <img src ='${API}/images/no_avatar.jpg' style='width: 100%;height: 100%;'/>
                                </div>`;
                    }

                    return img;
                }
            },
            { "data": "remarks" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("SupplierInfoSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}SupplierInfo/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("SupplierInfoSearchTable");
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
                const url = `${API}SupplierInfo/Delete/${id}`;

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
