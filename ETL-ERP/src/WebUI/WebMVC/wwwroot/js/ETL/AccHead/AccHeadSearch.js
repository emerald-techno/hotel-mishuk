$(document).ready(function () {
    search();

    $(".form-control").attr("autocomplete", "off");
})

$(document.body).on("click", "#AccHeadSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#AccHeadSearchTable")) {
        const table = $("#AccHeadSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#AccHeadSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "AccHead/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "groupName" },
            { "data": "headCode" },
            { "data": "headName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("AccHeadSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}AccHead/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("AccHeadSearchTable");
}

function getSearchObject() {
    const model = {
        HeadName: $("#HeadName").val(),
        HeadCode: $("#HeadCode").val(),
        GroupId: $("#GroupId").val()
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
                const url = `${API}AccHead/Delete/${id}`;

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
