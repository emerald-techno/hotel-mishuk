$(document).ready(function () {
    $("#PaySalaryBtn").hide();
});

$(document.body).on("click", "#ApprovePrBtn", function () {
    const prId = $("#PrId").val();
    const remarks = $("#ApproveRemarks").val();

    if (prId > 0) {
        const url = API + "PrSalaryMst/ApprovePayroll";
        const params = { id: prId, remarks: remarks };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Payroll Approval Successful");
                setTimeout(() => {
                    window.location.href = API + "PrSalaryMst/Details/" + prId;
                }, 3000);
            } else {
                failedMsg("Payroll Approval Failed");
            }
        }).fail(function () {
            failedMsg("Payroll Approval Failed");
        })
    } else {
        failedMsg("Payroll Id Not Found");
    }

})

$(document.body).on("click", "#PaySalaryBtn", function () {
    submitPaySalary();
})

let paidSalaryDtlIds = [];

function addPaidSalaryId(checkbox) {
    const existId = paidSalaryDtlIds.find(c => c == checkbox.value);

    if (checkbox.checked && existId == null) {
        paidSalaryDtlIds.push(checkbox.value);
    }
    else {
        const index = paidSalaryDtlIds.findIndex(c => c == checkbox.value);

        if (index > -1) {
            paidSalaryDtlIds.splice(index, 1);
        }
    }

    if (paidSalaryDtlIds != null && paidSalaryDtlIds.length > 0) {
        $("#PaySalaryBtn").show();
    } else {
        $("#PaySalaryBtn").hide();
    }
}

function submitPaySalary() {
    const prId = $("#PrId").val();

    if (paidSalaryDtlIds != null && paidSalaryDtlIds.length > 0) {
        const url = API + "PrSalaryMst/PayMultiSalary";
        const params = { ids: paidSalaryDtlIds };

        $("#PaySalaryBtn").prop("disabled", true).text("Processing...");

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Marks Salary Set As Paid");
                setTimeout(() => {
                    window.location.href = API + "PrSalaryMst/Details/" + prId;
                }, 2000);
            } else {
                failedMsg("Set As Paid Failed");
            }

            $("#PaySalaryBtn").prop("disabled", false).text("Submit");
        }).fail(function () {
            failedMsg("Set As Paid Failed");
            $("#PaySalaryBtn").prop("disabled", false).text("Submit");
        })
    }

}

var selectAll = document.getElementById("selectAll");
var checkboxes = document.querySelectorAll(".checkbox");

selectAll.addEventListener("change", function () {
    for (var i = 0; i < checkboxes.length; i++) {
        checkboxes[i].checked = selectAll.checked;
        addPaidSalaryId(checkboxes[i]);
    }
});

for (var i = 0; i < checkboxes.length; i++) {
    checkboxes[i].addEventListener("change", function () {
        if (!this.checked) {
            selectAll.checked = false;
        }
    });
}