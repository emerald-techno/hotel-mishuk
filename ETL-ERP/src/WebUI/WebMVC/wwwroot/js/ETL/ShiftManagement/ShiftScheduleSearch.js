$(document).ready(function () {
    search();
});

$(document.body).on("click", "#SearchBtn", function () {
    search();
});


function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#ShiftScheduleSearchTable")) {
        const table = $("#ShiftScheduleSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { searchModel: searchVm };
    }

    const oTable = $("#ShiftScheduleSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,



        "ajax": {
            url: API + "ShiftManagement/Search",
            type: "POST",
            data: params
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            {
                "render": function (data, type, item) {
                    return getMonthNameWithYear(item.monthStr);
                }
            },
            {
                "render": function (data, type, item) {
                    return convertJsonFullDateForView(new Date(item.startDate));
                }
            },
            {
                "render": function (data, type, item) {
                    let date = "";

                    if (item.endDate != null) {
                        date = convertJsonFullDateForView(new Date(item.endDate));
                    }

                    return date;
                }
            },
            { "data": "permanentShiftName" },
            { "data": "dutyShiftName" },
            { "data": "employeeName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("ShiftScheduleSearchTable", oTable);

                    const deleteBtn = `<a class='mr-2 href='${API}Order/Delete/${item.id}' class='gray-s' title='DELETE'><i class="fa fa-trash"></i></a>`;

                    return `<div style="font-size: 18px;">`
                        + `<div>` + deleteBtn + `</div>` +
                        `</div>`;
                }
            }

        ]
    });

    addTotalRowCountSpanInDataTable("ShiftScheduleSearchTable");

}

function getSearchObject() {
    const model = {
        SelectYear: $("#SelectYear").val(),
        SelectMonth: $("#SelectMonth").val(),
        DutyShiftId: $("#DutyShiftId").val(),
        EmployeeId: $("#EmployeeId").val(),
        PermanentShiftId: $("#PermanentShiftId").val(),
    };
    return model;
}

function getMonthNameWithYear(dateString) {
    const months = [
        "January", "February", "March", "April",
        "May", "June", "July", "August",
        "September", "October", "November", "December"
    ];

    const date = parseCustomDateString(dateString);
    const month = months[date.getMonth()];
    const year = date.getFullYear();

    return `${month} ${year}`;
}

function parseCustomDateString(dateString) {
    // Split the date string into day, month, and year
    var dateParts = dateString.split('/');
    var day = parseInt(dateParts[0], 10);
    var month = parseInt(dateParts[1], 10) - 1; // Adjust month to zero-based index
    var year = parseInt(dateParts[2], 10);

    // Create a new Date object
    var parsedDate = new Date(year, month, day);

    return parsedDate;
}