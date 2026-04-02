let EmpLeaveReviewerObject = {};
let ExistingReviewers = [];

$(document.body).on("change", "#EmployeeId", function () {
  const empId = $(this).val();
  const searchVm = getSearchObject(empId);
  EmpLeaveReviewerObject.EmployeeId = $(this).val();

  var params = "";
  if (!hasAnyError(searchVm)) {
    params = { SearchModel: searchVm };
  }

  if (empId > 0) {
    $.ajax({
      type: "post",
      url: API + "EmpLeaveReviewer/Search",
      data: params,
      success: function (response) {
        // console.log(response.data);
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
$("#emp-reviewer-submit-btn").click(function (e) {
  e.preventDefault();

  //#region Getting Form Values
  EmpLeaveReviewerObject.EmployeeId = $("#EmployeeId").val();
  EmpLeaveReviewerObject.ReviewerId = $("#ReviewerId").val();
  EmpLeaveReviewerObject.ALtReviewerId = $("#AltReviewerId").val();
  EmpLeaveReviewerObject.SlNo = $("#SlNo").val();
  EmpLeaveReviewerObject.isFinalReviewer =
    $("#isFinalReviewer").prop("checked");
  //#endregion
  // check if reviewer and alt reviwer field is empty
  //update the existing reviewer;
  const empId = EmpLeaveReviewerObject.EmployeeId;
  const searchVm = getSearchObject(empId);

  //check if employee id is not selected
  if (
    EmpLeaveReviewerObject.EmployeeId == "" ||
    EmpLeaveReviewerObject.ReviewerId == "" ||
    EmpLeaveReviewerObject.ALtReviewerId == ""
  ) {
    EmpLeaveReviewerObject.EmployeeId == ""
      ? AddErrorMsgSpan("Select an Employee first!", "EmployeeId")
      : false;
    EmpLeaveReviewerObject.ReviewerId == ""
      ? AddErrorMsgSpan("Select a reviewer first!", "ReviewerId")
      : false;
    EmpLeaveReviewerObject.ALtReviewerId == ""
      ? AddErrorMsgSpan("Select an alt reviewer first!", "AltReviewerId")
      : false;

    return;
  }

  var params = "";
  if (!hasAnyError(searchVm)) {
    params = { SearchModel: searchVm };
  }
  //#region getting data from backend before submit
  $.ajax({
    type: "post",
    url: API + "EmpLeaveReviewer/Search",
    data: params,
    success: function (response) {
      // console.log(response.data);
      ExistingReviewers = response.data;

      //#region Checking Validations

      //   console.log(EmpLeaveReviewerObject);
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
          EmpLeaveReviewerObject.ReviewerId,
          EmpLeaveReviewerObject.ALtReviewerId
        )
      ) {
        AddErrorMsgSpan("This reviewer is already set!", "ReviewerId");
        return;
      }
      //  same name twice
      if (
        checkIfReviewerAndAltReviewerIsSame(
          EmpLeaveReviewerObject.ReviewerId,
          EmpLeaveReviewerObject.ALtReviewerId
        )
      ) {
        AddErrorMsgSpan(
          "Reviewer, alternate reviewer can't be same!",
          "AltReviewerId"
        );
        return;
      }

      //
      if (
        checkIfReviewerSlNoAlreadyExists(
          ExistingReviewers,
          EmpLeaveReviewerObject.SlNo
        )
      ) {
        AddErrorMsgSpan(
          `${EmpLeaveReviewerObject.SlNoText} already exists!`,
          "SlNo"
        );
        return;
      }
      //#endregion
      // if final reviewer is selected submit with slno 99
      if (EmpLeaveReviewerObject.isFinalReviewer) {
        EmpLeaveReviewerObject.SlNo = 99;
      }
      //Call Ajax to post method
      $.ajax({
        type: "post",
        url: API + "EmpLeaveReviewer/Create",
        data: EmpLeaveReviewerObject,
        success: function (response) {
          // console.log("after posting ,", response);
          ///Reload the data
          search(EmpLeaveReviewerObject.EmployeeId);
        },
      });
    },
  });
  //#endregion
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
    EmpLeaveReviewerObject.SlNoText = ExistingReviewers.find(
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
function getSearchObject(empId) {
  const model = {
    EmployeeId: empId,
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
//         "employeeId": 240,
//         "employeeName": "Usha Barai",
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
//         "employeeLookUp": null,
//         "reviewerLookUp": null,
//         "altReviewerLookUp": null,
//         "slNoLookUp": null
//     }
// ]

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

function deleteOperation(id) {
  ResetErrorMsgSpan("IsFinalReviewer");
  $.ajax({
    type: "GET",
    url: `${API}EmpLeaveReviewer/Delete/${id}`,
    success: function (response) {
      search(EmpLeaveReviewerObject.EmployeeId);
    },
  });
}
