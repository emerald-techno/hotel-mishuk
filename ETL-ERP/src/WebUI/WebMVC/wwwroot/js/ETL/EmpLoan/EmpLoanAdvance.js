$(document.body).on("click", "#AdvanceBtn", function () {
    const loanId = $("#LoanId").val();
    const advAmount = $("#AdvAmount").val();

    if (loanId > 0) {
        const url = API + "EmpLoanMst/AdvanceLoan";
        const params = { id: loanId, advAmount: advAmount };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Loan Advance Calculation Successful");
                setTimeout(() => {
                    window.location.href = API + "EmpLoanMst/Details/" + loanId;
                }, 3000);
            } else {
                failedMsg("Loan Advance Calculation Failed");
            }
        }).fail(function () {
            failedMsg("Loan Advance Calculation Failed");
        })
    } else {
        failedMsg("Loan Id Not Found");
    }

})

$(document.body).on("click", ".pay-instalment", function () {

    const instalmentId = $(this).attr("data-id");
    const instalmentAmount = $(this).attr("data-amount");

    if (instalmentId > 0) {
        $("#InsId").val(instalmentId);
        $("#InsAmount").val(instalmentAmount);
    }
})

$(document.body).on("click", "#PaidBtn", function () {
    const loanId = $("#LoanId").val();
    const id = $("#InsId").val();
    const paidDateStr = $("#PaidDateStr").val();
    const waiverAmount = $("#WaiverAmount").val();
    const remarks = $("#DtlRemarks").val();

    if (id > 0) {
        const url = API + "EmpLoanMst/PaidWithWaiver";
        const params = {
            id: id,
            isPaid: true,
            paidDateStr: paidDateStr,
            waiverAmount: waiverAmount,
            remarks: remarks
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Instalment Payment Successful");
                setTimeout(() => {
                    window.location.href = API + "EmpLoanMst/Details/" + loanId;
                }, 1500);
            } else {
                failedMsg("Instalment Paymen Failed");
            }
        }).fail(function (error) {
            console.log(error);
            failedMsg("Instalment Payment Failed");
        })
    } else {
        failedMsg("Loan Instalment Id Not Found");
    }

})