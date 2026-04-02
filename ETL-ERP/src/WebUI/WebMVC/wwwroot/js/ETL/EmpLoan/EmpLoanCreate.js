let instalmentList = [];

$(document.body).on("click", "#InstalmetGenBtn", function () {

    instalmentList = [];

    const employeeId = $("#EmployeeId").val();
    const loanAmount = $("#LoanAmount").val();
    const interestRate = $("#InterestRate").val();
    const firstInsDate = $("#FirstInsDateStr").val();
    let instalmentAmount = $("#InsAmount").val();

    const insTotalMonth = Math.ceil(loanAmount / instalmentAmount);

    const insDate = convertStrToDate(firstInsDate);
    let calculateLoan = loanAmount;

    for (var i = 0; i < insTotalMonth; i++) {
        let modelDate = new Date(insDate.setMonth(insDate.getMonth()));

        if (i > 0) {
            modelDate = new Date(insDate.setMonth(insDate.getMonth() + 1));
        }

        if (parseInt(calculateLoan) > parseInt(instalmentAmount)) {
            calculateLoan = calculateLoan - instalmentAmount;
        } else {
            instalmentAmount = calculateLoan;
            calculateLoan = 0;
        }

        const totalVal = (parseFloat(calculateLoan) + parseFloat(instalmentAmount));
        const perInterestValue = percentCalculation(interestRate, totalVal);
        const interest = Math.ceil(perInterestValue / 12);

        console.log(`For ${i + 1} instalment interest amount will be ${interest}`);

        const model = {
            instalmentDate: modelDate,
            instalmentAmount: instalmentAmount,
            interestAmount: interest,
            netPay: (parseFloat(instalmentAmount) + parseFloat(interest)),
            loanAmount: calculateLoan
        }

        instalmentList.push(model);
    }

    renderInstalmentTableBody();
});

function renderInstalmentTableBody() {
    if (instalmentList.length > 0) {

        $("#InstalmentTbody").empty();

        instalmentList.forEach((v, i) => {
            const slNoCell = `<td><input type='hidden' name='EmpLoanDtls[${i}].Serial' value='${(i + 1)}'/>${(i + 1)}</td>`;
            const instalmentDateCell = `<td><input type='hidden' name='EmpLoanDtls[${i}].InsDateStr' value='${convertJsonFullDate(v.instalmentDate)}'/>${convertJsonFullDateForView(v.instalmentDate)}</td>`;
            const instalmentAmountCell = `<td>${v.instalmentAmount}</td>`;
            const intersetAmountCell = `<td><input type='hidden' name='EmpLoanDtls[${i}].InterestAmount' value='${v.interestAmount}'/>${v.interestAmount}</td>`;
            const netPayAmountCell = `<td><input type='hidden' name='EmpLoanDtls[${i}].InsAmount' value='${v.netPay}'/>${v.netPay}</td>`;
            const loanAmountCell = `<td><input type='hidden' name='EmpLoanDtls[${i}].LoanAmount' value='${v.loanAmount}'/>${v.loanAmount}</td>`;

            const row = `<tr>${slNoCell}${instalmentDateCell}${instalmentAmountCell}${intersetAmountCell}${netPayAmountCell}${loanAmountCell}</tr>`;

            $("#InstalmentTbody").append(row);
        })
    }
}

function percentCalculation(percentValue, totalNumber) {
    const result = (parseFloat(percentValue) * parseFloat(totalNumber)) / 100;
    return result;
}
