$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#TableSearchTable")) {
        const table = $("#TableSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#TableSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Table/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "tableNo" },
            {
                "render": function (data, type, item) {
                    let text = ``;

                    if (item.isActive) {
                        text = "<span class='badge badge-primary'>Active</span>";
                    } else {
                        text = "<span class='badge badge-danger'>In-Active</span>";
                    }
                    return "<div>" + text + "</div>";
                }
            },
            { "data": "capacity" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("TableSearchTable", oTable);

                    let deleteButton = "";
                    let editButton = `<a class='mr-2' href='${API}Table/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    if (window.isSuperAdmin) {
                        deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    }
                    
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("TableSearchTable");
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
                const url = `${API}Table/Delete/${id}`;

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
