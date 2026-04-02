$(document).ready(function () {
    search();
});

$(document.body).on("click", "#SearchBtn", function () {
    search();
});


function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#CustomerSearchTable")) {
        const table = $("#CustomerSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#CustomerSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "Customer/Search",
            type: "POST",
            data: params
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "fullName" },
            { "data": "mobile" },
            { "data": "companyName" },
            { "data": "employeeName" },
            { "data": "guestName" },
            {
                "render": function (data, type, item) {
                    let editButton = "";
                    if (window.isSuperAdmin) {
                        editButton = `<a class='mr-2' href='${API}Customer/Edit/${item.id}' title='Edit'><i class="fa fa-edit"></i></a>`;
                    }
                   
                    let deleteButton = `<a class='ml-2 delconfirm' data-id='${item.id}' href='#' title='Delete'><i class="fa fa-trash"></i></a>`;
                    //let detailsButton = `<a class='ml-2' href='${API}Customer/Details/${item.id}' title='Details'><i class="fa fa-search"></i></a>`;
                    return `<div style="font-size: 18px;"><div>${editButton} ${deleteButton}</div></div>`;
                }
            }
        ]
    });

    addTotalRowCountSpanInDataTable("CustomerSearchTable");

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
                    const id = $(this).attr("data-id");
                    const url = `${API}Customer/Delete/${id}`;

                    $.get(url, function (rData) {
                        if (rData) {
                            successMsg("Deleted Successfully");
                        } else {
                            failedMsg("Delete Failed...!");
                        }
                        search();
                    });
                }
        });
});

function getSearchObject() {
    const model = {
        companyId: $("#CompanyId").val() 
    };
    return model;
}