$(document).ready(function () {
    search();
})

$(document.body).on("click", "#AccLedgerSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#AccLedgerSearchTable")) {
        const table = $("#AccLedgerSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#AccLedgerSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "AccLedger/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "headCode" },
            { "data": "headName" },
            { "data": "ledgerCode" },
            { "data": "ledgerName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("AccLedgerSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}AccLedger/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("AccLedgerSearchTable");
}

function getSearchObject() {
    const model = {
        LedgerName: $("#LedgerName").val(),
        LedgerCode: $("#LedgerCode").val(),
        HeadId: $("#HeadId").val()
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
                const url = `${API}AccLedger/Delete/${id}`;

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
