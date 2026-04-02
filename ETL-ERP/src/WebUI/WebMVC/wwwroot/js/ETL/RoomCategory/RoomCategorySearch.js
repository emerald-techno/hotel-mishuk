$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#RoomCategorySearchTable")) {
        const table = $("#RoomCategorySearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#RoomCategorySearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "RoomCategory/Search",
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
            { "data": "categoryName" },
            {
                "render": function (data, type, item) {
                    let status = ``;

                    if (item.isAc) {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-success">Available</span>`;
                    }
                    else {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-danger">Un Available</span>`;
                    }
                    return status;
                }
            },
            {
                "render": function (data, type, item) {
                    let status = ``;

                    if (item.isBalcony) {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-success">Available</span>`;
                    }
                    else {
                        status = `<span style='font-size: 12px' class="badge badge-pill badge-danger">Un Available</span>`;
                    }
                    return status;
                }
            },
            { "data": "bedTypeName" },
            { "data": "bedNumber" },
            { "data": "capacity" },
            { "data": "otherInfo" },
            { "data": "rent" },
            { "data": "serviceCharge" },
            { "data": "totalRent" },
            { "data": "remarks" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("RoomCategorySearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}RoomCategory/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("RoomCategorySearchTable");
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
                const url = `${API}RoomCategory/Delete/${id}`;

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
