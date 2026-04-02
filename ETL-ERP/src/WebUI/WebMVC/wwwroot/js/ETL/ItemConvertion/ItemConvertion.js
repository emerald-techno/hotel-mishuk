$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#FoodItemSearchTable")) {
        const table = $("#FoodItemSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#FoodItemSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "ItemConvertion/Search",
            type: "POST",
            data: params,
        },
        error(e) {
            failedMsg(e);
        },

        "columns": [
           
            { "data": "serialNo" },
            { "data": "itemName" },
            {
                "data": null,
                "render": function (data, type, item) {
                    return `<div class="text-center">
                                <span><b>${item.quantity}</b> ${item.unitName}</span>
                            </div>`;
                }
            },
            {
                "data": null,
                "render": function (data, type, item) {
                    return `<div class="text-center">
                                <span><b>${item.convertedQuantity}</b> ${item.convertedUnitName}</span>
                            </div>`;
                }
            },
            {
                "data": null,
                "render": function (data, type, item) {
                    return `<div class="text-center">
                                <span>${item.quantity} ${item.unitName} → ${item.convertedQuantity} ${item.convertedUnitName}</span>
                            </div>`;
                }
            },
            {
                "data": "isActive",
                "render": function (data) {
                    return data
                        ? "<i class='icofont icofont-ui-press text-success text-center'></i>"
                        : "<i class='icofont icofont-ui-press text-danger text-center'></i>";
                }
            },
            {
                "data": null,
                "render": function (data, type, item) {
                    let editButton = `<a class='mr-2' href='${API}ItemConvertion/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='${API}ItemConvertion/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;
                    
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton +  `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("FoodItemSearchTable");
}



