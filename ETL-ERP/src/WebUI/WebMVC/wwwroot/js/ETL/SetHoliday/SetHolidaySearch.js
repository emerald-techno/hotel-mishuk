$(document).ready(function () {
    const model = {
        HolidayYear: new Date().getFullYear()
    };

    search(model);
})

$(document.body).on("click", "#HolidaySearchBtn", function () {
    const model = {
        HolidayYear: $("#HolidayYear").val()
    };

    search(model);
});

function search(searchObject) {
    const searchVm = searchObject;

    if ($.fn.DataTable.isDataTable("#SetHolidaySearchTable")) {
        const table = $("#SetHolidaySearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#SetHolidaySearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "SetHoliday/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "holidayYear" },
            { "data": "holidayName" },
            {
                "render": function (data, type, item) {
                    let holidayType = "";
                    if (item.type == "M") holidayType = "Mandatory";
                    else if (item.type == "O") holidayType = "Optional";
                    return holidayType;
                }
            },
            {
                "render": function (data, type, item) {
                    const startDate = convertJsonFullDateForView(item.startDate);
                    return startDate;
                }
            },
            {
                "render": function (data, type, item) {
                    const endDate = convertJsonFullDateForView(item.endDate);
                    return endDate;
                }
            },
            { "data": "length" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("SetHolidaySearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}SetHoliday/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` +  deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("SetHolidaySearchTable");
}

function getSearchObject() {
    const model = {
        HolidayYear: $("#HolidayYear").val()
    };
    return model;
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
                const url = `${API}SetHoliday/Delete/${id}`;

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
