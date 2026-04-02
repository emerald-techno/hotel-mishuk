$(document).ready(function () {
    search();
})

function search() {
    const searchVm = {};

    if ($.fn.DataTable.isDataTable("#PfSettlementSearchTable")) {
        const table = $("#PfSettlementSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#PfSettlementSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "PfSettlement/Search",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "employeeName" },
            { "data": "statusText" },
            {
                "render": function (data, type, item) {
                    let settlementDate = convertJsonFullDateForView(item.settlementDate);
                    return settlementDate;

                }
            },
            {
                "render": function (data, type, item) {
                    let statusDate = convertJsonFullDateForView(item.statusDate);
                    return statusDate;

                }
            },
            {
                "render": function (data, type, item) {
                    let pfStartDate = convertJsonFullDateForView(item.pfStartDate);
                    return pfStartDate;

                }
            },
            {
                "render": function (data, type, item) {
                    let pfEndDate = convertJsonFullDateForView(item.pfEndDate);
                    return pfEndDate;

                }
            },
            { "data": "totalAmount" },
            { "data": "interest" },
            //{
            //    "render": function (data, type, item) {
            //        let compContributionType = "";
            //        if (item.compCon == 'N') compContributionType = "Not Contribute"
            //        else if (item.compCon == 'H') compContributionType = "Half";
            //        else if (item.compCon == 'F') compContributionType = "Full";
            //        return compContributionType;
            //    }
            //},
            { "data": "compConTypeText"},
            { "data": "compCon" },
            { "data": "empCon" },
            { "data": "pfLength" },
            { "data": "tax" },
            { "data": "vat" },
            { "data": "netAmount" },
            {
                "render": function (data, type, item) {
                    let approveDate = convertJsonFullDateForView(item.approveDate);
                    return approveDate;

                }
            },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("PfSettlementSearchTable", oTable);

                    let editButton = `<a style='margin-right: 3px; font-size:18px;' href='${API}PfSettlement/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    return `<div style="font-size: 18px; text-align: center;"><div>` + editButton + `</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("PfSettlementSearchTable");
}