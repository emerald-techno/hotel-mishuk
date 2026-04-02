$(document).ready(function () {
    search();
})

$(document.body).on("click", "#LeaveCfSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObjectModel();

    if ($.fn.DataTable.isDataTable("#LeaveCfSearchTable")) {
        const table = $("#LeaveCfSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#LeaveCfSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "LeaveCf/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    let employeeName = `<strong>${item.employeeName}</strong>`;
                    let employeeCode = `<small>${item.employeeCode}</small>`;
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

                    const result = `<div class='row'><div class='col-md-4'><div>${img}</div></div><div class='col-md-8' style='text-align:left;'>${employeeName}<br />${employeeCode}</div></div>`;
                    return result;

                }
            },
            { "data": "designationName" },
            { "data": "departmentName" },
            { "data": "leaveTypeName" },
            { "data": "leaveYear" },
            { "data": "leaveBalance" },
            { "data": "cfBalance" },
            { "data": "leaveEnjoyed" },
            { "data": "leaveSale" }
        ]
    });

    addTotalRowCountSpanInDataTable("LeaveCfSearchTable");
}

function getSearchObjectModel() {
    const model = {
        EmployeeId: $("#EmployeeId").val()
    }
    return model;
}