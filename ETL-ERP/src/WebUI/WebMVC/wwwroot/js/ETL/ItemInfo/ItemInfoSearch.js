$(document).ready(function () {
    search();
})

$(document.body).on("click", "#ItemInfoSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#ItemInfoSearchTable")) {
        const table = $("#ItemInfoSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ItemInfoSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "ItemInfo/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "itemName" },
            { "data": "itemCode" },
            { "data": "categoryName" },
            { "data": "unitName" },
            { "data": "reorderQty" },
            { "data": "maxQty" },
            { "data": "minQty" },
            { "data": "alertQty" },
            { "data": "leadDay" },
            { "data": "remarks" },
            {
                "render": function (data, type, item) {
                    let img = '';

                    if (!hasAnyError(item.photoDocUrl)) {
                        img = `<div class='avatar' style='width: 50px; height: 60px; margin: auto';>
                                    <img src ='${item.photoDocUrl}' style='width: 100%;height: 100%;'/>
                                </div>`;
                    } else {
                        img = `<div class='avatar' style='width: 50px; height: 60px; margin: auto';>
                                    <img src ='${API}/img/No_Image_Available.jpg' style='width: 100%;height: 100%;'/>
                                </div>`;
                    }

                    return img;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("ItemInfoSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}ItemInfo/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let detailButton = `<a class='mr-2' href='${API}ItemInfo/Details/${item.id}' title='Details'><i class="fa fa-search"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + detailButton + ` ` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("ItemInfoSearchTable");
}

function getSearchObject() {
    const model = {
        CategoryId: $("#CategoryId").val(),
        UnitId: $("#UnitId").val()
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
                const url = `${API}ItemInfo/Delete/${id}`;

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
