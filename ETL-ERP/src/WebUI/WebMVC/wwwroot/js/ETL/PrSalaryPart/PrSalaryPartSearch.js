$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#PrSalaryPartSearchTable")) {
        const table = $("#PrSalaryPartSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#PrSalaryPartSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PrSalaryPart/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "partName" },
            {
                "render": function (data, type, item) {
                    let partType = ``;
                    if (item.partType == 'A') {
                        partType = "Addition";
                    } else if (item.partType == 'D') {
                        partType = "Deduction";
                    }
                    return partType;
                }
            },
            {
                "render": function (data, type, item) {
                    let valType = ``;
                    if (item.valueType == 'P') {
                        valType = "Percent";
                    } else if (item.valueType == 'A') {
                        valType = "Amount";
                    }
                    return valType;
                }
            },
            { "data": "value" },
            {
                "render": function (data, type, item) {
                    let icon = ``;

                    if (item.isEnable) {
                        icon = "<i class='icofont icofont-ui-press text-success'></i>";
                    } else {
                        icon = "<i class='icofont icofont-ui-press text-danger'></i>";
                    }
                    return "<div class='text-center'>" + icon + "</div>";
                }
            },
            {
                "render": function (data, type, item) {
                    let icon = ``;

                    if (item.isEmpWise) {
                        icon = "<i class='icofont icofont-ui-press text-success'></i>";
                    } else {
                        icon = "<i class='icofont icofont-ui-press text-danger'></i>";
                    }
                    return "<div class='text-center'>" + icon + "</div>";
                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("PrSalaryPartSearchTable", oTable);

                    let editButton = `<b>Restricted</b>`;

                    if (hasAnyError(item.partLink)) {
                        editButton = `<a class='mr-2' href='${API}PrSalaryPart/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    }

                    return `<div style="font-size: 18px; text-align: center;"><div>` + editButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("PrSalaryPartSearchTable");
}