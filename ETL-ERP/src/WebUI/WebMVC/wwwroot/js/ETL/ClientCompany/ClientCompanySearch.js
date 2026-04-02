$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#ClientSearchTable")) {
        const table = $("#ClientSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ClientSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "ClientCompany/Search",
            type: "POST",
            data: params,
        },
        error(e) {
            failedMsg(e);
        },

        "columns": [

            { "data": "serialNo" },
            { "data": "name" },
            { "data": "address" },
            { "data": "mobile" },
            { "data": "email" },
            { "data": "webSite" },
            {
                "data": "logoUrl",
                "render": function (data) {
                    if (data) {
                        return `<img src="${data}" alt="Logo" style="width: 50px; height: auto; border-radius: 5px;" />`;
                    } else {
                        return `<span>No Image</span>`;
                    }
                }
            },

            //{
            //    "data": null,
            //    "render": function (data, type, item) {
            //        return `<div class="text-center">
            //                    <span><b>${item.quantity}</b> ${item.unitName}</span>
            //                </div>`;
            //    }
            //},
            //{
            //    "data": null,
            //    "render": function (data, type, item) {
            //        return `<div class="text-center">
            //                    <span><b>${item.convertedQuantity}</b> ${item.convertedUnitName}</span>
            //                </div>`;
            //    }
            //},
            //{
            //    "data": null,
            //    "render": function (data, type, item) {
            //        return `<div class="text-center">
            //                    <span>${item.quantity} ${item.unitName} → ${item.convertedQuantity} ${item.convertedUnitName}</span>
            //                </div>`;
            //    }
            //},
            //{
            //    "data": "isActive",
            //    "render": function (data) {
            //        return data
            //            ? "<i class='icofont icofont-ui-press text-success text-center'></i>"
            //            : "<i class='icofont icofont-ui-press text-danger text-center'></i>";
            //    }
            //},
            {
                "data": null,
                "render": function (data, type, item) {
                    let editButton = `<a class='mr-2' href='${API}ClientCompany/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;

                    return `<div style="font-size: 18px;" class="d-flex justify-content-center"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("ClientSearchTable");
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
            const url = `${API}ClientCompany/Delete/${id}`;

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