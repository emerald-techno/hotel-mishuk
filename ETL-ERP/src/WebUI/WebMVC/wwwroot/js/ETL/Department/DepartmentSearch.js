$(document).ready(function () {
    search();
})

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#DepartmentSearchTable")) {
        const table = $("#DepartmentSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#DepartmentSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Department/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "name" },
            { "data": "code" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("DepartmentSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}Department/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    //let deleteButton = `<a class='ml-2' href='${API}Department/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;

                    const value = $("#IsAcademic").val();

                    const isAcademic = $.parseJSON(value.toLowerCase());

                    if (isAcademic == true) {
                        editButton = `<a class='mr-2' href='${API}Department/EditAcademicDepartment/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                        /*deleteButton = `<a class='ml-2' href='${API}Department/DeleteAcademicDepartment/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;*/
                    }

                    return `<div style="font-size: 18px;"><div>` + editButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("DepartmentSearchTable");
}


function getSearchObject() {
    const model = {
        IsAcademic: $("#IsAcademic").val()
    };
    return model;
}