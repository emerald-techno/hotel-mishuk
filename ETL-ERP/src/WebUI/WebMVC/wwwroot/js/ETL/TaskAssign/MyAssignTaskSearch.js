let taskList = [];

$(document).ready(function () {
    search();
})

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#AssignTaskTable")) {
        const table = $("#AssignTaskTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#AssignTaskTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "TaskAssign/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "assignRoomNo" },
            { "data": "actionByName" },
            {
                "render": function (data, type, item) {
                    const assignDate = convertJsonFullDateForView(item.assignDate);
                    return assignDate;
                }
            },
            { "data": "roomWiseTaskList" },
            { "data": "taskCount" },
            {
                "render": function (data, type, item) {
                    let assignStatus = "";

                    if (item.assignStatus == 0) {
                        assignStatus = `<div style='font-size: 12px' class='badge badge-secondary'>Assigned</div>`;
                    } else if (item.assignStatus == 1) {
                        assignStatus = `<div style='font-size: 12px' class='badge badge-info'>Running</div>`;
                    } else if (item.assignStatus = 2) {
                        assignStatus = `<div style='font-size: 12px' class='badge badge-primary'>Completed</div>`;
                    } else if (item.assignStatus = 3) {
                        assignStatus = `<div style='font-size: 12px' class='badge badge-danger'>Cancel</div>`;
                    } else {
                        assignStatus = "";
                    }

                    return assignStatus;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("AssignTaskTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}PrEmpSalaryPart/Details?employeeId=${item.employeeId}' title='View'><i class="fa fa-search"></i></a>`;

                    let statusActionBtn = `<a class="btn btn-primary status-btn" href="#" data-bs-toggle="modal" data-bs-target="#statusUpdateModal" data-assign-id='${item.assignId}' title='Status Update'> <i class="fa fa-book"></i> </a>`;

                    return `<div style="font-size: 18px; text-align: center;"><div>` + statusActionBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("AssignTaskTable");
}

$(document.body).on("click", ".status-btn", function () {
    const assignId = $(this).attr("data-assign-id");

    if (assignId > 0) {
        taskList = [];

        const url = `${API}TaskAssign/GetTaskByAssignId/${assignId}`;

        $.get(url, function (rData) {
            if (rData) {
                console.log("room assign task list: ", rData);
                taskList = rData;

                renderUpdateTask();
            } else {
                console.log("task not found...!!");
            }
        })
    } else {
        console.log("assign id not found...!!");
    }
})

function renderUpdateTask() {
    if (taskList.length > 0) {
        taskList.forEach((v, i) => {

            let completeCheckBox = `<input class='complete_check' type='checkbox' data-assign-id='${v.id}'/>`;

            const checkBoxCell = `<td><div class='text-center'>${completeCheckBox}</div></td>`;

            const assignDate = convertJsonFullDateForView(v.assignDate);

            const taskInfoCell = `<td> <input type='hidden' id='task_id_${i}' value='${v.taskId}'/> ${v.taskName} <br/> <span> ${assignDate} </span> </td>`;

            const completeRemarksCell = `<td> <input type='text' class='form-control' id='complete_remarks_${i}'/> </td>`;

            const row = `<tr style='font-size: smaller;'>${checkBoxCell}${taskInfoCell}${completeRemarksCell}</tr>`;

            $("#UpdateStatusTableTbody").append(row);
        })
    }
}

function getSearchObject() {
    const model = {
        HouseKeeperId: $("#HouseKeeperId").val()
    };

    return model;
}
