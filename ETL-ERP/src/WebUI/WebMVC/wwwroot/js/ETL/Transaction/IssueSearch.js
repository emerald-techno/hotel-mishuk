
$(document).ready(function () {
    search();
});

$(document.body).on("click", "#IssueSearchBtn", function () {
    search();
});


function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#IssueSearchTable")) {
        const table = $("#IssueSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#IssueSearchTable").DataTable({
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
            {
                "render": function (data, type, item) {
                    const issueInfo = `<div>
                                    <h5>${item.tranNo}</h5>
                                    <h5>${convertJsonFullDateForView(new Date(item.tranDate))}</h5>
                                  </div>`;
                    return issueInfo;
                }
            },
            {
                "render": function (data, type, item) {
                    const reqInfo = `<div>
                                    <h5>${item.reqNo}</h5>
                                    <h5>${convertJsonFullDateForView(new Date(item.reqDate))}</h5>
                                  </div>`;
                    return reqInfo;
                }
            },
            { "data": "issueDeptName" },
            { "data": "issueEmpName" },
            { "data": "actionByName" },
            {
                "render": function (data, type, item) {

                    showTotalRowCountSpanInDataTable("IssueSearchTable", oTable);


                    const viewBtn = `<a class='mr-2' href='${API}Issue/Details/${item.id}' target='_blank' ><i class="fa fa-search"></i></a>`;
                    const editBtn = `<a class='mr-2' href='${API}Transaction/IssueSearchEdit/${item.id}'><i class="fa fa-edit"></i></a>`;
                    const deleteBtn = `<a class='mr-2' href='${API}Transaction/IssueSearchDelete/${item.id}'><i class="fa fa-trash"></i></a>`;



                    return `<div style="font-size: 18px;">`
                        + `<div>` + viewBtn + `</div>` +
                        `</div>`;
                }
            }

        ]
    });

    addTotalRowCountSpanInDataTable("IssueSearchTable");

}

function getSearchObject() {
    const model = {
        TranType: $("#TranType").val(),
        IssueDeptId: $("#IssueDeptId").val(),
        IssueEmpId: $("#IssueEmpId").val(),
        ReqMstId: $("#ReqMstId").val(),
        SFromDate: $("#SFromDate").val(),
        SToDate: $("#SToDate").val(),
    };
    return model;
}

