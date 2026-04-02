$(document.body).on("change", "#EmployeeId", function () {
  const empId = $(this).val();

  if (empId > 0) {
    search(empId);
  }
});

function search(empId) {
  const searchVm = getSearchObject(empId);

  if ($.fn.DataTable.isDataTable("#EmpLeaveReviewerSearchTable")) {
    const table = $("#EmpLeaveReviewerSearchTable").DataTable();
    table.destroy();
  }

  var params = "";
  if (!hasAnyError(searchVm)) {
    params = { SearchModel: searchVm };
  }

  const oTable = $("#EmpLeaveReviewerSearchTable").DataTable({
    aLengthMenu: DataTable.lengthMenu,
    iDisplayLength: DataTable.displayLength,
    processing: DataTable.processing,
    serverSide: DataTable.serverSide,
    ordering: false,

    ajax: {
      url: API + "EmpLeaveReviewer/Search",
      type: "POST",
      data: params,
    },
    success(r) {
      console.log(r);
    },
    error(e) {
      failedMsg(e);
    },

    columns: [
      { data: "serialNo" },
      { data: "reviewerName" },
      { data: "altReviewerName" },
      {
        render: function (data, type, item) {
          let finalReviewer = "";

          if (item.isFinalReviewer) {
            finalReviewer = "Final Approver";
          }
          return finalReviewer;
        },
      },
      { data: "slNoText" },
      {
        render: function (data, type, item) {
          showTotalRowCountSpanInDataTable(
            "EmpLeaveReviewerSearchTable",
            oTable
          );

          /*let editButton = `<a class='mr-2' href='${API}EmpLeaveReviewer/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;*/
          let deleteButton = `<a class='ml-2' data-Id="${item.id}" id="emp-leave-reviewer-dltbtn" onClick="deleteOperation(${item.id})" title='Delete'><i class="fa fa-trash"></i></a>`;
          return (
            `<div style="font-size: 18px;"><div>` +
            deleteButton +
            `</div></div>`
          );
        },
      },
    ],
  });

  addTotalRowCountSpanInDataTable("EmpLeaveReviewerSearchTable");
}

function getSearchObject(empId) {
  const model = {
    EmployeeId: empId,
  };
  return model;
}
function deleteOperation(id) {
  $.ajax({
    type: "GET",
    url: `${API}EmpLeaveReviewer/Delete/${id}`,
    success: function (response) {
      search(EmpLeaveReviewerObject.EmployeeId);
    },
  });
}
