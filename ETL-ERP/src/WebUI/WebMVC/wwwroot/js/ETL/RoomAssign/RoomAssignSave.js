
//#region Room Search

$(document.body).on("change", "#HouseKeeperId", function () {
    var houseKeeperId = $("#HouseKeeperId").val();
    if (houseKeeperId > 0) {
        roomSearch();
    }
});

function roomSearch() {
    const searchVm = getRoomSearchObject();

    if ($.fn.DataTable.isDataTable("#RoomInfoTable")) {
        const table = $("#RoomInfoTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#RoomInfoTable").DataTable({
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
            {
                "render": function (data, type, item) {

                    let roomNo = `<b>${item.roomNo}</b>`;
                    let categoryName = `<small>${item.roomCategoryName}</small>`;
                    let floorName = `<small>${item.floorName}</small>`;

                    let div = `<div class='d-flex flex-column'>${roomNo}${categoryName}${floorName}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("RoomInfoTable", oTable);

                    let assignCheckBox = `<input class='assign_check text-center' type='checkbox' data-room-id='${item.id}'/>`;
                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}RoomInfo/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;

                    return `<div class='text-center'>${assignCheckBox}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("RoomInfoTable");
}

function getRoomSearchObject() {
    const model = {
        houseKeeperId: $("#HouseKeeperId").val()
    };
    return model;
}

//#endregion

//#region Assinged Rooms

$(document.body).on("change", "#HouseKeeperId", function () {
    var houseKeeperId = $("#HouseKeeperId").val();
    if (houseKeeperId > 0) {
        assignedRoomSearch();
    }
});

function assignedRoomSearch() {
    const searchVm = getAssignedRoomSearchObject();

    if ($.fn.DataTable.isDataTable("#AssignRoomInfoTable")) {
        const table = $("#AssignRoomInfoTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#AssignRoomInfoTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "RoomAssign/GetAssignRooms",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {

                    console.log("room-info:", item);
                    let roomNo = `<b>${item.roomNo}</b>`;
                    let categoryName = `<small>${item.roomCategoryName}</small>`;
                    let floorName = `<small>${item.roomFloorName}</small>`;

                    let div = `<div class='d-flex flex-column'>${roomNo}${categoryName}${floorName}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {

                    let status = "";

                    if (item.status == 0) {
                        status = `<div style='font-size: 12px' class='badge badge-info'>Assigned</div>`;
                    } else if (item.status == 1) {
                        status = `<div style='font-size: 12px' class='badge badge-secondary'>Running</div>`;
                    } else if (item.status == 2) {
                        status = "<div style='font-size: 12px' class='badge badge-primary'>Complete</div>";
                    } else if (item.status == 3) {
                        status = "<div style='font-size: 12px' class='badge badge-danger'>Canceled</div>";
                    }

                    let div = `<div>${status}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("AssignRoomInfoTable", oTable);

                    let unassignCheckBox = `<input class='unassign_check' type='checkbox' data-room-id='${item.roomId}'/>`;

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}RoomInfo/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;

                    return `<div class='text-center'>${unassignCheckBox}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("AssignRoomInfoTable");
}

function getAssignedRoomSearchObject() {
    const model = {
        houseKeeperId: $("#HouseKeeperId").val()
    };
    return model;
}

//#endregion


//#region Assign Room

let assignRoomList = [];

$(document.body).on("click", ".assign_check", function () {

    const roomId = $(this).attr("data-room-id");
    const houseKeeperId = $("#HouseKeeperId").val();

    console.log("data-room-id: ", roomId);

    const isChecked = $(this).is(":checked");

    if (roomId > 0 && houseKeeperId > 0) {
        if (isChecked) {
            const model = {
                houseKeeperId: houseKeeperId,
                roomId: roomId
            };

            console.log("model: ", model);

            if (model.roomId > 0 && model.houseKeeperId > 0) {
                assignRoomList.push(model);
            }
        } else {
            var removeIndex = assignRoomList.findIndex(x => x.roomId == roomId);

            if (removeIndex > -1) {
                assignRoomList.splice(removeIndex, 1);
            } else {
                console.log("Room can't be remove");
            }
        }
    }
});

$(document.body).on("click", "#AssignRoomBtn", function () {
    if (assignRoomList != null && assignRoomList.length > 0) {
        const url = API + "RoomAssign/AssignRoom";

        const params = {
            roomAssignVms: assignRoomList
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Assign Room Successful");

                //setTimeout(() => {
                //    window.location.href = API + "RoomAssign/AssignRoom";
                //}, 2000);

                roomSearch();
                assignedRoomSearch();

                assignRoomList = [];
                unAssignRoomList = [];
            } else {
                failedMsg("Room Assign Failed");
            }
        }).fail(function () {
            failedMsg("Room Assign Failed");
        })
    } else {
        failedMsg("No Room Found To Assigned...!!");
    }
})

//#endregion

//#region UnAssign Room

let unAssignRoomList = [];

$(document.body).on("click", ".unassign_check", function () {

    const roomId = $(this).attr("data-room-id");
    const houseKeeperId = $("#HouseKeeperId").val();

    const isChecked = $(this).is(":checked");

    if (roomId > 0 && houseKeeperId > 0) {
        if (isChecked) {
            const model = {
                houseKeeperId: houseKeeperId,
                roomId: roomId
            };

            console.log("model: ", model);

            if (model.roomId > 0 && model.houseKeeperId > 0) {
                unAssignRoomList.push(model);
            }
        } else {
            var removeIndex = unAssignRoomList.findIndex(x => x.roomId == roomId);

            if (removeIndex > -1) {
                unAssignRoomList.splice(removeIndex, 1);
            } else {
                console.log("Room can't be remove");
            }
        }
    }
});

$(document.body).on("click", "#UnAssignRoomBtn", function () {
    if (unAssignRoomList != null && unAssignRoomList.length > 0) {
        const url = API + "RoomAssign/UnassignRoom";

        const params = {
            roomAssignVms: unAssignRoomList
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("UnAssign Room Successful");

                //setTimeout(() => {
                //    window.location.href = API + "RoomAssign/AssignRoom";
                //}, 2000);

                roomSearch();
                assignedRoomSearch();

                assignRoomList = [];
                unAssignRoomList = [];
            } else {
                failedMsg("Room UnAssign Failed");
            }
        }).fail(function () {
            failedMsg("Room UnAssign Failed");
        })
    } else {
        failedMsg("No Room Found To UnAssigned...!!");
    }
})

//#endregion