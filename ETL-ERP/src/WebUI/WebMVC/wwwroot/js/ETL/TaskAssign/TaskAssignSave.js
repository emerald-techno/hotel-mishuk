
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

                    let roomCheckBox = `<input class='room_check' type='checkbox' data-assign-id='${item.id}'/>`;

                    return `<div class='text-center'>${roomCheckBox}</div>`;
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

//#region Task Search

$(document.body).on("change", "#HouseKeeperId", function () {
    var houseKeeperId = $("#HouseKeeperId").val();
    if (houseKeeperId > 0) {
        taskSearch();
    }
});

function taskSearch() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#TaskInfoTable")) {
        const table = $("#TaskInfoTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#TaskInfoTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "TaskName/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {

                    let taskName = `<b>${item.name}</b>`;
                    let taskType = `<small>${item.typeName}</small>`;

                    let div = `<div class='d-flex flex-column'>${taskName}${taskType}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {
                    let input = `<input type='text' class='form-control' id='remark_${item.id}'/>`;
                    return input;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("TaskInfoTable", oTable);

                    let assignCheckBox = `<input class='task_assign_check text-center' type='checkbox' data-task-id='${item.id}'/>`;

                    return `<div class='text-center'>${assignCheckBox}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("TaskInfoTable");
}

//#endregion

//#region Assign Room

let assignRoomList = [];

$(document.body).on("click", ".room_check", function () {

    const assignId = $(this).attr("data-assign-id");

    console.log("data-assign-id: ", assignId);

    const isChecked = $(this).is(":checked");

    if (assignId > 0) {
        if (isChecked) {
            const model = {
                assignId: assignId
            };

            if (model.assignId > 0) {
                assignRoomList.push(model);
            }
        } else {
            var removeIndex = assignRoomList.findIndex(x => x.assignId == assignId);

            if (removeIndex > -1) {
                assignRoomList.splice(removeIndex, 1);
            } else {
                console.log("Room can't be remove");
            }
        }
    }
});

//#endregion

//#region Assign Task

let assignTaskList = [];

$(document.body).on("click", ".task_assign_check", function () {

    const taskId = $(this).attr("data-task-id");

    const isChecked = $(this).is(":checked");

    if (taskId > 0) {
        if (isChecked) {

            const remarks = $(`#remark_${taskId}`).val();

            const model = {
                taskId: taskId,
                remarks: remarks
            };

            console.log("task-model: ", model);

            if (model.taskId > 0) {
                assignTaskList.push(model);
            }
        } else {
            var removeIndex = assignTaskList.findIndex(x => x.taskId == taskId);

            if (removeIndex > -1) {
                assignTaskList.splice(removeIndex, 1);
            } else {
                console.log("Task can't be remove");
            }
        }
    }
});

//#endregion

let taskMapList = [];

$(document.body).on("click", "#TaskAssignBtn", function () {

    if (assignRoomList == null && !(assignRoomList.length > 0)) {
        return failedMsg("Please add some room to assign");
    }

    if (assignTaskList == null && !(assignTaskList.length > 0)) {
        return failedMsg("Please add some task to assign");
    }

    if (assignRoomList.length > 0) {
        assignRoomList.forEach(v => {
            if (assignTaskList.length > 0) {
                assignTaskList.forEach(x => {
                    const model = {
                        assignId: v.assignId,
                        taskId: x.taskId,
                        assignRemarks: x.remarks
                    }

                    taskMapList.push(model);
                });
            } else {
                return failedMsg("Please add some task to assign");
            }
        });
    } else {
        return failedMsg("Please add some room to assign");
    }
});

$(document.body).on("click", "#TaskAssignBtn", function () {
    if (taskMapList != null && taskMapList.length > 0) {

        const url = API + "TaskAssign/AssignTask";

        const params = {
            taskAssignSaveVms: taskMapList
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Assign Task Successful");

                //setTimeout(() => {
                //    window.location.href = API + "RoomAssign/AssignRoom";
                //}, 2000);

                taskSearch();
                assignedRoomSearch();

                taskMapList = [];
                assignTaskList = [];
                assignRoomList = [];
            } else {
                failedMsg("Task Assign Failed");
            }
        }).fail(function () {
            failedMsg("Task Assign Failed");
        })
    } else {
        failedMsg("No Task Found To Assigned...!!");
    }
})