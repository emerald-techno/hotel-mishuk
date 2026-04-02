$(document).ready(function () {
    search();
})

$(document.body).on("click", "#EmployeeSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#EmployeeSearchTable")) {
        const table = $("#EmployeeSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#EmployeeSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Employee/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
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

                    return img;
                }
            },
            { "data": "code" },
            {
                "render": function (data, type, item) {
                    const name = `<div><a target='_blank' href='${API}Employee/Details/${item.id}'>${item.name}<a/></div>`;
                    return name;
                }
            },
            { "data": "designationName" },
            { "data": "departmentName" },
            { "data": "employeeStatusText" },
            {
                "render": function (data, type, item) {
                    let joinDate = convertJsonFullDateForView(new Date(item.joinDate));
                    return joinDate;

                }
            },
            { "data": "salary" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("EmployeeSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' href='${API}Employee/Details/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let editButton = `<a style='margin-left: 3px; font-size:18px;' href='${API}Employee/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let disableButton = `<a style='margin-left: 5px; font-size:18px;' href='${API}Employee/Disable/${item.id}' title='Disable'><i class="fa fa-ban"></i></a>`;
                    return `<div>${viewBtn}${editButton}${disableButton}</div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("EmployeeSearchTable");
}


function getSearchObject() {
    const model = {
        DepartmentId: $("#DepartmentId").val(),
        DesignationId: $("#DesignationId").val(),
        EnableStatus: $("#EnableStatus").val()
    };
    return model;
}

$(document.body).on("click", "#printBtn", function () {
    printReport();
});

function printReport() {
    const model = {
        DepartmentId: $("#DepartmentId").val(),
        DesignationId: $("#DesignationId").val(),
        EnableStatus: $("#EnableStatus").val()
    }

    const url = `${API}Employee/EmployeeListPrint?desId=${model.DesignationId}&dptId=${model.DepartmentId}&enstatus=${model.EnableStatus}`;
    window.open(url, "_blank");
}


$(document.body).on("click", "#ExcelBtn", function () {
    ExcelReport();
});

function ExcelReport() {
    const model = {
        DepartmentId: $("#DepartmentId").val(),
        DesignationId: $("#DesignationId").val(),
    }
    const url = `${API}Employee/ExcelFileDownload?desId=${model.DesignationId}&dptId=${model.DepartmentId}`;
    window.open(url, "_self");
}