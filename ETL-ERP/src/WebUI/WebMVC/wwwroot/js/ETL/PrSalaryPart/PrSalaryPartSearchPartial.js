$(document).ready(function () {
    search();
})

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#PrSalaryPartSearchPartialTable")) {
        const table = $("#PrSalaryPartSearchPartialTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#PrSalaryPartSearchPartialTable").DataTable({
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
            {
                "render": function (data, type, item) {
                    let partName = `${item.partName}`;
                    let employeeWise = item.isEmpWise ? ' --- (Employee Wise)' : '';
                    
                    return partName + employeeWise;
                }
            },
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
            {
                "render": function (data, type, item) {
                    showTotalRowCountSpanInDataTable("PrSalaryPartSearchPartialTable", oTable);
                    return item.value;
                }
            }
                    
        ]
        
    });
    
    addTotalRowCountSpanInDataTable("PrSalaryPartSearchPartialTable");
}

function getSearchObject() {
    const model = {
        IsEnable: true
    };
    return model;
}