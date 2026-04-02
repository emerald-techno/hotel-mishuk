$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

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
            { "data": "assignKepperName" },
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

                    //let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}PrEmpSalaryPart/Details?employeeId=${item.employeeId}' title='View'><i class="fa fa-search"></i></a>`;
                    
                    let approveBtn = `<a class='approveModalBtn' data-id='${item.id}' style='margin-right: 3px; font-size:18px;' href='#' title='Approve HK Task' data-bs-toggle="modal" data-bs-target="#approveModal"><i class="icofont icofont-check-alt text-success" ></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + approveBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("AssignTaskTable");
}

$(document.body).on("click", "#approveTaskBtn", function () {
    swal({
        title: "Approve Confirmation",
        text: "Are you sure to approve this task?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const taskId = $(".approveModalBtn").data("id");
                var auditorRemarks = $("#AuditorRemarks").val();
                const url = `${API}TaskAssign/ApproveTask?taskId=${taskId}&auditorRemarks=${auditorRemarks}`;

                $.get(url, function (rData) {
                    if (rData) {
                        successMsg("Cancel Successfully");
                    } else {
                        failedMsg("Cancel Failed...!")
                    }

                    setTimeout(() => {
                        window.location.href = API + "askAssign/Search";
                    }, 500);
                })
            }
        })
});