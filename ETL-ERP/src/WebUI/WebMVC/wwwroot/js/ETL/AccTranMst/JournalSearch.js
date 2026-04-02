$(document).ready(function () {
    search();

    $(".form-control").attr("autocomplete", "off");
})

$(document.body).on("click", "#JournalSearchBtn", function () {
    search();
});

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#JournalSearchTable")) {
        const table = $("#JournalSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#JournalSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "AccTranMst/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "vcNo" },
            {
                "render": function (data, type, item) {
                    let vcDate = convertJsonFullDateForView(new Date(item.vcDate));
                    return vcDate;
                }
            },
            { "data": "narration" },
            { "data": "strTotalAmount" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("JournalSearchTable", oTable);

                    let viewBtn = `<a style='margin-right: 3px; font-size:18px;' target='_blank' href='${API}AccTranMst/JournalDetails/${item.id}' title='View'><i class="fa fa-search"></i></a>`;
                    let printBtn = `<a style='margin-right: 3px; font-size:18px;' target='_blank' href='${API}AccTranMst/VoucherPrint/${item.id}' title='View'><i class="fa fa-print"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + viewBtn + printBtn + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("JournalSearchTable");
}

function getSearchObject() {
    const model = {
        /*VcType: "J",*/
        VcType: $("#VcType").val(),
        FormDateStr: $("#FormDateStr").val(),
        ToDateStr: $("#ToDateStr").val()
    };
    return model;
}
