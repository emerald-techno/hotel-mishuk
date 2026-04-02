
$(document).ready(function () {
    search();
});

$(document.body).on("click", "#SearchBtn", function () {
    search();
});


function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#ConsumptionSearchTable")) {
        const table = $("#ConsumptionSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#ConsumptionSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Issue/Search",
            type: "POST",
            data: params
        }, error(e) {
            failedMsg(e);
        },

        "columns": [

            { "data": "serialNo" },
            { "data": "tranNo" },
            {
                "data": "tranDate",
                "render": function (data, type, item) {
                    return convertJsonFullDateForView(new Date(item.tranDate));
                }

            },
            { "data": "issueRoomNo" },
            { "data": "remarks" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("ConsumptionSearchTable", oTable);


                    const viewBtn = `<a class='mr-2' href='${API}HKConsumption/Details/${item.id}' target='_blank' ><i class="fa fa-search"></i></a>`;
                    const editBtn = `<a class='mr-2' href='${API}Transaction/IssueSearchEdit/${item.id}'><i class="fa fa-edit"></i></a>`;
                    const deleteBtn = `<a class='mr-2' href='${API}Transaction/IssueSearchDelete/${item.id}'><i class="fa fa-trash"></i></a>`;



                    return `<div style="font-size: 18px;">`
                        + `<div>` + viewBtn + `</div>` +
                        `</div>`;
                }
            }

        ]
    });

    addTotalRowCountSpanInDataTable("ConsumptionSearchTable");

}

function getSearchObject() {
    const model = {
        TranType: $("#TranType").val(),
        IssueRoomId: $("#IssueRoomId").val(),
        IssueDeptId: $("#IssueDeptId").val()
    };
    return model;
}

