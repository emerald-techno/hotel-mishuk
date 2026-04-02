$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#HallSearchTable")) {
        const table = $("#HallSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#HallSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "HallInfo/Search",
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
            { "data": "hallName" },
            { "data": "hallInformation" },
            { "data": "otherInfo" },
            { "data": "rent" },
            { "data": "vat" },
            { "data": "serviceCharge" },
            { "data": "totalRent" },
            { "data": "remakrs" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("HallSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}HallInfo/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let editButton = `<a class='mr-2' href='${API}HallInfo/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + viewBtn + ` ` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("HallSearchTable");
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
                const url = `${API}HallInfo/Delete/${id}`;

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
