$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#WaiterSearchTable")) {
        const table = $("#WaiterSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#WaiterSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Waiter/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "name" },
            { "data": "code" },
            { "data": "mobile" },
            { "data": "email" },
            { "data": "salary" },
            { "data": "dobStr" },
            { "data": "joinDateStr" },
            {
                "render": function (data, type, item) {
                    let icon = ``;

                    if (item.isActive) {
                        icon = "<i class='icofont icofont-ui-press text-success'></i>";
                    } else {
                        icon = "<i class='icofont icofont-ui-press text-danger'></i>";
                    }
                    return "<div class='text-center'>" + icon + "</div>";
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("WaiterSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}Waiter/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("WaiterSearchTable");
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
                const url = `${API}Waiter/Delete/${id}`;

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
