let DptLeaveReviewerObject = {};
let ExistingReviewers = [];

$(document.body).on("change", "#DepartmentId", function () {
  const dptId = $(this).val();
  const searchVm = getSearchObject(dptId);
  DptLeaveReviewerObject.DepartmentId = $(this).val();

  var params = "";
  if (!hasAnyError(searchVm)) {
    params = { SearchModel: searchVm };
  }

  if (dptId > 0) {
    $.ajax({
      type: "post",
      url: API + "DptLeaveReviewer/Search",
      data: params,
      success: function (response) {
        console.log(response.data);
        ExistingReviewers = response.data;
      },
    });
  }
});
////////////
$("#isFinalReviewer").change(() => {
  ResetErrorMsgSpan("IsFinalReviewer");
});
$("#ReviewerId").change(() => {
  ResetErrorMsgSpan("AltReviewerId");
});
$("#AltReviewerId").change(() => {
  ResetErrorMsgSpan("ReviewerId");
});
///On Submit button click event
$("#dpt-reviewer-submit-btn").click(function (e) {
  e.preventDefault();
  //#region Getting Form Values
  DptLeaveReviewerObject.DepartmentId = $("#DepartmentId").val();
  DptLeaveReviewerObject.ReviewerId = $("#ReviewerId").val();
  DptLeaveReviewerObject.ALtReviewerId = $("#AltReviewerId").val();
  DptLeaveReviewerObject.SlNo = $("#SlNo").val();
  DptLeaveReviewerObject.isFinalReviewer =
    $("#isFinalReviewer").prop("checked");
  //#endregion
  //update the existing reviewer;
  const dptId = DptLeaveReviewerObject.DepartmentId;
  const searchVm = getSearchObject(dptId);

  var params = "";
  if (!hasAnyError(searchVm)) {
    params = { SearchModel: searchVm };
  }
  $.ajax({
    type: "post",
    url: API + "DptLeaveReviewer/Search",
    data: params,
    success: function (response) {
      // console.log(response.data);
      ExistingReviewers = response.data;

      //#region Checking Validations
      //check if department id is selected
      if (
        DptLeaveReviewerObject.DepartmentId == "" ||
        DptLeaveReviewerObject.ReviewerId == "" ||
        DptLeaveReviewerObject.ALtReviewerId == ""
      )
        return;
      //   console.log(DptLeaveReviewerObject);
      //   if Final reviewer is already set
      let isFinalReviewerAlreadySet =
        checkIfFinalReviewerIsAlreadySet(ExistingReviewers);
      if (isFinalReviewerAlreadySet) {
        AddErrorMsgSpan(
          "Remove or Edit the final reviewer to add more reviewers",
          "IsFinalReviewer"
        );
        return;
      }
      //  check if same person pair is already set to reviwer / alt reviewer
      if (
        checkSamePersonPairAlreadyExists(
          ExistingReviewers,
          DptLeaveReviewerObject.ReviewerId,
          DptLeaveReviewerObject.ALtReviewerId
        )
      ) {
        AddErrorMsgSpan("This Reviewer is already set!", "ReviewerId");
        return;
      }
      //  same name twice
      if (
        checkIfReviewerAndAltReviewerIsSame(
          DptLeaveReviewerObject.ReviewerId,
          DptLeaveReviewerObject.ALtReviewerId
        )
      ) {
        AddErrorMsgSpan(
          "Reviewer, Alt reviewer cant be same!",
          "AltReviewerId"
        );
        return;
      }
      // final reviewer already exists
      // if (DptLeaveReviewerObject.isFinalReviewer) {
      //   if (isFinalReviewerAlreadySet) {
      //     AddErrorMsgSpan("Final reviewer is already set!", "IsFinalReviewer");
      //     return;
      //   }
      // }

      //
      if (
        checkIfReviewerSlNoAlreadyExists(
          ExistingReviewers,
          DptLeaveReviewerObject.SlNo
        )
      ) {
        AddErrorMsgSpan(
          `${DptLeaveReviewerObject.SlNoText} already exists!`,
          "SlNo"
        );
        return;
      }
      //#endregion

      //Call Ajax to post method
      $.ajax({
        type: "post",
        url: API + "DptLeaveReviewer/Create",
        data: DptLeaveReviewerObject,
        success: function (response) {
          if (response) {
            ///Reload the data
            search(DptLeaveReviewerObject.DepartmentId);
            //clear input fields
            clearInputFields();
          }
        },
      });
    },
  });
});
//////Check Functions

const checkIfReviewerAndAltReviewerIsSame = (reviewerId, AltReviewerId) => {
  if (reviewerId == AltReviewerId) {
    return true;
  }
  return false;
};
const checkIfFinalReviewerIsAlreadySet = (ExistingReviewers) => {
  return ExistingReviewers.some((x) => x.isFinalReviewer == true);
};
const checkIfReviewerSlNoAlreadyExists = (ExistingReviewers, SlNo) => {
  if (ExistingReviewers.some((x) => x.slNo == SlNo)) {
    DptLeaveReviewerObject.SlNoText = ExistingReviewers.find(
      (x) => x.slNo == SlNo
    ).slNoText;
  }

  return ExistingReviewers.some((x) => x.slNo == SlNo);
};
const checkSamePersonPairAlreadyExists = (
  ExistingReviewers,
  ReviewerId,
  AltReviewerId
) => {
  let result = ExistingReviewers.filter(
    (x) =>
      x.reviewerId == ReviewerId ||
      x.reviewerId == AltReviewerId ||
      x.altReviewerId == ReviewerId ||
      x.altReviewerId == AltReviewerId
  );
  return result.length != 0;
};
const AddErrorMsgSpan = (message, valmsgfor) => {
  $(`.text-danger[data-valmsg-for=${valmsgfor}]`).text(message);
};
const ResetErrorMsgSpan = (valmsgfor) => {
  $(`.text-danger[data-valmsg-for=${valmsgfor}]`).text("");
};

///
function getSearchObject(dptId) {
  const model = {
    DepartmentId: dptId,
  };
  return model;
}

// [SAMPLE OBJECT
//     {
//         "id": 3,
//         "reviewFor": 0,
//         "slNo": 1,
//         "slNoText": "FIRST APPROVER",
//         "isFinalReviewer": true,
//         "departmentId": 240,
//         "departmentName": "Usha Barai",
//         "reviewerId": 18,
//         "reviewerName": "Prof. Dr. Daulatuzzaman",
//         "altReviewerId": 17,
//         "altReviewerName": "Mr. Hadi",
//         "userId": 0,
//         "serialNo": 1,
//         "canCreate": false,
//         "canUpdate": false,
//         "canView": false,
//         "canDelete": false,
//         "departmentLookUp": null,
//         "reviewerLookUp": null,
//         "altReviewerLookUp": null,
//         "slNoLookUp": null
//     }
// ]

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

function deleteOperation(id) {
  $.ajax({
    type: "GET",
    url: `${API}DptLeaveReviewer/Delete/${id}`,
    success: function (response) {
      search(DptLeaveReviewerObject.DepartmentId);
    },
  });
}

function clearInputFields() {
  $("#ReviewerId").val("").trigger(update);
  $("#AltReviewerId").val("").trigger(update);
  // $("#SlNo").val("").trigger(update);
  $("#IsFinalReviewer").prop("checked", false).trigger(update);
}
