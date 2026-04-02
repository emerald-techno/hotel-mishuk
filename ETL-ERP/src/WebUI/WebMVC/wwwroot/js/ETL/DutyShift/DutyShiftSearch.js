$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#DutyShiftSearchTable")) {
        const table = $("#DutyShiftSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#DutyShiftSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "DutyShift/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "shiftName" },
            { "data": "shiftCode" },
            {
                "render": function (data, type, item) {

                    let shift = "";

                    if (item.shiftType == 'P') {
                        shift = `<div>Permanent</div>`;
                    } else {
                        shift = `<div>Duty</div>`;
                    }

                    let div = `<div class='d-flex flex-column'>${shift}</div>`;

                    return div;
                }
            },
            { "data": "startTimeStr" },
            { "data": "endTimeStr" },
            {
                "render": function (data, type, item) {

                    let pay = "";

                    if (item.payType == 'M') {
                        pay = `<div>Monthly</div>`;
                    } else {
                        pay = `<div>Shift Wise</div>`;
                    }

                    let div = `<div class='d-flex flex-column'>${pay}</div>`;

                    return div;
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("DutyShiftSearchTable", oTable);

                    let editButton = `<a class='mr-2' href='${API}DutyShift/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    return `<div style="font-size: 18px;"><div>` + editButton + ` ` + deleteButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("DutyShiftSearchTable");
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
                const url = `${API}DutyShift/Delete/${id}`;

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
