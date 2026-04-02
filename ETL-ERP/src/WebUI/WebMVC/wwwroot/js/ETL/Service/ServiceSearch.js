$(document).ready(function () {
    search();
})


function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#ServiceSearchTable")) {
        const table = $("#ServiceSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ServiceSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Service/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "serviceName" },
            { "data": "serviceCode" },
            {

                "render": function (data, type, item) {
                    return item.ledgerName ? ` (${item.ledgerCode}) - ${item.ledgerName} ` : '';
                }
            },
            {

                "render": function (data, type, item) {
                    return item.isExtra ? 'Extra Service' : '';
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("ServiceSearchTable", oTable);

                    let editButton = "";
                    if (window.isDevelopement) {
                        editButton = `<a class='mr-2' href='${API}Service/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    }
                    /*let editButton = `<a class='mr-2' href='${API}Service/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;*/
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("ServiceSearchTable");
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
                const url = `${API}Service/Delete/${id}`;

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
