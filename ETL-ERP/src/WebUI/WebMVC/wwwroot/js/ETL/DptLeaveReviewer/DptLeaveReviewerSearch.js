$(document.body).on("change", "#DepartmentId", function () {
  const dptId = $(this).val();

  if (dptId > 0) {
    search(dptId);
  }
});
$("#dpt-leave-reviewer-dltbtn").click((e) => {
  console.log(e);
  $.ajax({
    type: "GET",
    url: `${API}DptLeaveReviewer/Delete/${e.target}`,
    success: function (response) {
      console.log(response.data);
    },
  });
});

function search(dptId) {
  const searchVm = getSearchObject(dptId);

  if ($.fn.DataTable.isDataTable("#DptLeaveReviewerSearchTable")) {
    const table = $("#DptLeaveReviewerSearchTable").DataTable();
    table.destroy();
  }

  var params = "";
  if (!hasAnyError(searchVm)) {
    params = { SearchModel: searchVm };
  }

  const oTable = $("#DptLeaveReviewerSearchTable").DataTable({
    aLengthMenu: DataTable.lengthMenu,
    iDisplayLength: DataTable.displayLength,
    processing: DataTable.processing,
    serverSide: DataTable.serverSide,
    ordering: false,

    ajax: {
      url: API + "DptLeaveReviewer/Search",
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
            "DptLeaveReviewerSearchTable",
            oTable
          );

          /*let editButton = `<a class='mr-2' href='${API}DptLeaveReviewer/Delete/${item.id}' title='Delete'><i class="fa fa-trash"></i></a>`;*/
          let deleteButton = `<a class='ml-2' data-Id="${item.id}" id="dpt-leave-reviewer-dltbtn" onClick="deleteOperation(${item.id})" title='Delete'><i class="fa fa-trash"></i></a>`;
          return (
            `<div style="font-size: 18px;"><div>` +
            deleteButton +
            `</div></div>`
          );
        },
      },
    ],
  });

  addTotalRowCountSpanInDataTable("DptLeaveReviewerSearchTable");
}

function getSearchObject(dptId) {
  const model = {
    DepartmentId: dptId,
  };
  return model;
}
function deleteOperation(id) {
  $.ajax({
    type: "GET",
    url: `${API}DptLeaveReviewer/Delete/${id}`,
    success: function (response) {
      search(DptLeaveReviewerObject.DepartmentId);
    },
  });
}
