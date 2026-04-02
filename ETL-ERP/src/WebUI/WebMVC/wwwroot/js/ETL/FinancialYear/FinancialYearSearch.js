$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#FinancialYearSearchTable")) {
        const table = $("#FinancialYearSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#FinancialYearSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "SetFincYear/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "yearName" },
            { "data": "yearStartDate" },
            { "data": "yearEndDate" },
            {
                "render": function (data, type, item) {

                    let activeStatus = "";

                    if (item.isActive)
                    {
                        activeStatus = `<div style='font-size: 12px' class='badge badge-primary'>Active</div>`;
                    }
                    else
                    {
                        activeStatus = `<div style='font-size: 12px' class='badge badge-warning'>Disable</div>`;
                    }

                    let div = `<div>${activeStatus}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("FinancialYearSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}SetFincYear/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    //let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton +/* ` ` + deleteButton +*/ `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("FinancialYearSearchTable");
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
                const url = `${API}SetFincYear/Delete/${id}`;

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
