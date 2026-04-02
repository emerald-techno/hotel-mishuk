$(document).ready(function () {
    search();
})

$(document.body).on("click", "#RoomSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#RoomViewSearchTable")) {
        const table = $("#RoomViewSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#RoomViewSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "RoomAssign/RoomViewSearch",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "roomNo" },
            { "data": "roomCategoryName" },
            {
                "render": function (date, type, item) {

                    let color = 'btn-outline-primary';

                    if (item.houseKeeperAvailabilityStatus == 1) {
                        color = 'btn-outline-info';
                    } else if (item.houseKeeperAvailabilityStatus == 2) {
                        color = 'btn-outline-danger';
                    }

                    const availability = `<button class="btn ${color} btn-xs availability_btn" type="button" data-bs-toggle="modal" data-bs-target="#availabilityModal" data-room-id='${item.id}'>
                                            ${item.availabilityStatusText}
                                        </button>`;
                    return availability;
                }

            },
            {
                "render": function (date, type, item) {

                    let color = 'btn-outline-primary';

                    if (item.cleaningStatus == 1) {
                        color = 'btn-outline-danger';
                    } else if (item.cleaningStatus == 2) {
                        color = 'btn-outline-info';
                    } else if (item.cleaningStatus == 3) {
                        color = 'btn-outline-warning';
                    }

                    const cleaning = `<button class="btn ${color} btn-xs status_btn" type="button" data-bs-toggle="modal" data-bs-target="#statusModal" data-room-id='${item.id}'>
                                        ${item.cleaningStatusText}
                                    </button>`;
                    return cleaning;
                }

            },
            {
                "render": function (date, type, item) {

                    let assignIconLink = `<a class='assign_btn' style='margin-right:3px;font-size:18px;' href='#' title='View' data-bs-toggle="modal" data-bs-target="#assignModal" data-room-id='${item.id}'>
                    <i class="fa fa-hand-o-right"></i></a>`;

                    let clearIconLink = `<a style='margin-right: 3px; font-size:18px;' href='${API}RoomAssign/UnassignSingleRoom/${item.houseKeeperAssignId}' title='View'><i class="fa fa-times"></i></a>`;

                    let houseKeeper = '';

                    if (item.houseKeeperAssignId != null && item.houseKeeperAssignId > 0) {
                        houseKeeper = `<b>${item.houseKeeperName}</b> ${clearIconLink}`;
                    } else {
                        houseKeeper = `${assignIconLink}`;
                    }

                    return houseKeeper;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("RoomViewSearchTable", oTable);

                    return `<div>${item.houseKeeperStatusText}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("RoomViewSearchTable");
}

//#region Change Availability
$(document.body).on("click", ".availability_btn", function () {
    const roomId = $(this).data("room-id");
    if (roomId > 0) {
        $("#SelectedRoomId_One").val(roomId);
    } else {
        failedMsg("Room Information Not Found...!!");
    }
});

$(document.body).on("change", "#RoomAvailability", function () {
    const roomAvailabilityStatus = $(this).val();
    const roomId = $("#SelectedRoomId_One").val();

    if (roomId > 0 && roomAvailabilityStatus >= 0) {
        const url = API + "RoomAssign/AvailabilityStatusChange";

        const params = {
            availabilityStatus: roomAvailabilityStatus,
            roomId: roomId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {

                search();
                successMsg("Availability Change Successful");
                $("#availabilityModal").modal('hide');
                clearAvailabilityForm();

            } else {
                failedMsg("Availability Change Failed");
            }
        }).fail(function () {
            failedMsg("Availability Change Failed");
        })
    } else {
        console.log("Failed");
    }
});

function clearAvailabilityForm() {
    $("#SelectedRoomId_One").val("");
    $("#RoomAvailability").val("").trigger("change");
}

//#endregion

//#region Change Clean Status
$(document.body).on("click", ".status_btn", function () {
    const roomId = $(this).data("room-id");
    if (roomId > 0) {
        $("#SelectedRoomId_Two").val(roomId);
    } else {
        failedMsg("Room Information Not Found...!!");
    }
});

$(document.body).on("change", "#RoomStatus", function () {
    const roomCleanStatus = $(this).val();
    const roomId = $("#SelectedRoomId_Two").val();

    if (roomId > 0 && roomCleanStatus >= 0) {
        const url = API + "RoomAssign/CleanStatusChange";

        const params = {
            cleanStatus: roomCleanStatus,
            roomId: roomId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {

                search();
                successMsg("Clean Status Change Successful");
                $("#statusModal").modal('hide');
                clearStatusForm();

            } else {
                failedMsg("Clean Status Change Failed");
            }
        }).fail(function () {
            failedMsg("Clean Status Change Failed");
        })
    } else {
        console.log("Failed");
    }
});

function clearStatusForm() {
    $("#SelectedRoomId_Two").val("");
    $("#RoomStatus").val("").trigger("change");
}

//#endregion

//#region Assign HouseKeepper
$(document.body).on("click", ".assign_btn", function () {
    const roomId = $(this).data("room-id");
    if (roomId > 0) {
        $("#SelectedRoomId_HK").val(roomId);
    } else {
        failedMsg("Room Information Not Found...!!");
    }
});

$(document.body).on("change", "#AssignId", function () {
    const houseKeeperId = $(this).val();
    const roomId = $("#SelectedRoomId_HK").val();

    if (roomId > 0 && houseKeeperId > 0) {
        const url = API + "RoomAssign/AssignSingleRoom";

        const params = {
            houseKeeperId: houseKeeperId,
            roomId: roomId
        };

        $.post(url, params, function (rData) {
            if (rData == true) {

                search();
                successMsg("Keeper Assign Successful");
                $("#assignModal").modal('hide');
                clearAssignForm();

            } else {
                failedMsg("Keeper Assign Failed");
            }
        }).fail(function () {
            failedMsg("Keeper Assign Failed");
        })
    } else {
        console.log("Failed");
    }
});

function clearAssignForm() {
    $("#SelectedRoomId_HK").val("");
    $("#AssignId").val("").trigger("change");
}

//#endregion

function getSearchObject() {
    const model = {
        RoomCategoryId: $("#RoomCategoryId").val(),
        HouseKeeperAvailabilityStatus: $("#HouseKeeperStatus").val(),
        CleaningStatus: $("#CleaningStatus").val(),
    };
    return model;
}

$(document.body).on("click", "#printBtn", function () {
    printReport();
});

function printReport() {
    const model = {
        RoomCategoryId: $("#RoomCategoryId").val(),
        HouseKeeperAvailabilityStatus: $("#HouseKeeperStatus").val(),
        CleaningStatus: $("#CleaningStatus").val(),
    };

    const url = `${API}RoomAssign/HkRoomViewPrint?roomCategoryId=${model.RoomCategoryId}&houseKeeperStatus=${model.HouseKeeperAvailabilityStatus}&cleaningStatus=${model.CleaningStatus}`;
    window.open(url, "_blank");
}