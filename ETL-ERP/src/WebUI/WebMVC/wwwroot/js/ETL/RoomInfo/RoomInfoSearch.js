$(document).ready(function () {
    search();
})

$(document.body).on("click", "#RoomSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#RoomSearchTable")) {
        const table = $("#RoomSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#RoomSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "RoomInfo/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "roomCategoryName" },
            { "data": "floorName" },
            { "data": "roomNo" },
            { "data": "bedTypeName" },
            {
                "render": function (data, type, item) {

                    let acStatus = "";
                    let belconyStatus = "";

                    if (item.isAc) {
                        acStatus = `<div>AC: Yes</div>`;
                    } else {
                        acStatus = `<div>AC: No</div>`;
                    }

                    if (item.isBelcony) {
                        belconyStatus = `<div>Belcony: Yes</div>`;
                    } else {
                        belconyStatus = `<div>Belcony: No</div>`;
                    }

                    let div = `<div class='d-flex flex-column'>${acStatus}${belconyStatus}</div>`;

                    return div;
                }
            },
            { "data": "person" },
            {
                "render": function (data, type, item) {

                    let rent = `<b>Rent: ${item.rent}</b>`;
                    let vat = `<b>VAT: ${item.vat}%</b>`;
                    let sc = `<b>S.C.: ${item.serviceCharge}</b>`;
                    let totalRent = `<b>Total: ${item.totalRent}</b>`;

                    let div = `<div class='d-flex flex-column'>${rent}${vat}${sc}${totalRent}</div>`;

                    return div;
                }
            },
            { "data": "bookingStatusText" },
            { "data": "availabilityStatusText" },
            { "data": "cleaningStatusText" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("RoomSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}RoomInfo/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let editButton = `<a style='margin-left: 3px; font-size:18px;' href='${API}RoomInfo/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;

                    return `<div>${viewBtn} ${editButton}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("RoomSearchTable");
}


function getSearchObject() {
    const model = {
        RoomCategoryId: $("#RoomCategoryId").val(),
        FloorId: $("#FloorId").val(),
        BedTypeId: $("#BedTypeId").val(),
        BookingStatus: $("#BookingStatus").val(),
        CleaningStatus: $("#CleaningStatus").val(),
    };
    return model;
}
