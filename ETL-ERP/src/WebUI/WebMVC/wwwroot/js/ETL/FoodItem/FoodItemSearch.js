$(document).ready(function () {
    search();


    //$(".edit-button").each(function () {
    //    var button = $(this);
    //    var foodItemId = button.attr("data-id");

    //    if (foodItemId) {
    //        $.ajax({
    //            url: '../../FoodItem/CheckFoodItemAvailability/'+foodItemId,
    //            type: 'POST',
    //            contentType: 'application/json',
    //            data: JSON.stringify({ id: foodItemId }),
    //            success: function (response) {
    //                if (response.exists) {
                        
    //                    button.hide();
    //                }
    //            },
    //            error: function (error) {
    //                console.error("Error checking food item availability:", error);
    //            }
    //        });
    //    }
    //});


})
$(document.body).on("click", "#ItemSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

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
            url: API + "FoodItem/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "categoryName" },
            {
                "render": function (data, type, item) {
                    console.log(item)

                    let setMenuHtml = item.isSetMenuItem ? `<span class='badge badge-primary'>Set Menu</span>` : '';

                    const itemInfo = `<div>
                                        <h5>
                                            ${item.itemName}
                                            ${setMenuHtml}
                                        </h5>
                                        <b>${item.itemCode}</b>
                                      </div>`;

                    return itemInfo;
                }
            },
            {
                "render": function (data, type, item) {

                    let rate = `<b>Rate: ${item.rate}</b>`;
                    let offerRate = `<b>Offer Rate: ${item.offerRate}</b>`;
                    let vat = `<b>VAT: ${item.vat}</b>`;
                    let netRate = `<b>Net Rate: ${item.netRate}</b>`;

                    let div = `<div class='d-flex flex-column'>${rate}${offerRate}${vat}${netRate}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {
                    let img = '';

                    if (!hasAnyError(item.photoUrl)) {
                        img = `<div class='avatar' style='width: 50px; height: 60px; margin: auto';>
                                    <img src ='${item.photoUrl}' style='width: 100%;height: 100%;'/>
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

                    showTotalRowCountSpanInDataTable("FoodItemSearchTable", oTable);

                    let editButton = `<a class='mr-2 edit-button' href='${API}FoodItem/Edit/${item.id}' data-id="${item.id}" title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    let detailsButton = `<a class='ml-2' data-id='${item.id}' href='${API}FoodItem/Details/${item.id}' title='Details'><i class="fa fa-search"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + ` ` + detailsButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("FoodItemSearchTable");
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
                const url = `${API}FoodItem/Delete/${id}`;

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

function getSearchObject() {
    const model = {
        CategoryId: $("#CategoryId").val()
    };
    return model;
}
