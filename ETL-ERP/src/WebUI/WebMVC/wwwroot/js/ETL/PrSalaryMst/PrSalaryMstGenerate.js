$(document.body).on("change", ".pay-cal", function () {
    const index = $(this).attr("data-index");
    if (index > -1) {
        payrollCalculation(index);
    }
});

function calculateDepartmentSubtotal(index) {
    const dptIndex = $(`#NetSalary_${index}`).attr('data-dpt-index');
    let deptTotal = 0;

    $(`input[data-dpt-index='${dptIndex}']`).each(function () {
        deptTotal += parseFloat($(this).val()) || 0;
    });

    $(`#DptTotalSalary_${dptIndex}`).val(deptTotal);
}

function payrollCalculation(index) {
    if (index > -1) {
        const overTime = parseFloat($(`#A_ColC_${index}`).val());
        const foodBill = parseFloat($(`#D_ColE_${index}`).val());
        const absent = parseFloat($(`#D_ColU_${index}`).val());
        const loan = parseFloat($(`#D_ColJ_${index}`).val());

        const fixedGross = parseFloat($(`#FGrossSalary_${index}`).val());
        //const fixedDiduction = parseFloat($(`#FTotalDeduction_${index}`).val());

        let grossSalaryTotal = (fixedGross + overTime);
        let deductionTotal = (loan + foodBill + absent);

        $(`#GrossSalary_${index}`).val(grossSalaryTotal);
        $(`#TotalDeduction_${index}`).val(deductionTotal);

        let netTotal = grossSalaryTotal - deductionTotal;

        $(`#NetSalary_${index}`).val(netTotal);

        calculateDepartmentSubtotal(index);
        calculateGrandTotal();
    }
}

function calculateGrandTotal() {
    let totalNetSalary = 0;
    let serial = 0;

    $('#PayrollTable tbody tr').not(':last').each(function () {
        const netSalary = parseFloat($(this).find('td:last-child input').not('.subtotal').val()) || 0;
        serial += 1;
        console.log(`each tr amount: ${serial}`, netSalary);
        totalNetSalary += netSalary;
    });

    console.log('Total Net Salary:', totalNetSalary);

    $("#GrandTotalSalary").val(totalNetSalary);
}


$(document.body).on("click", "#GaneratePayrollBtn", function () {
    loadReportPartial();
});

function loadReportPartial() {

    const year = $("#Year").val();
    const month = $("#Month").val();
    const salaryDateStr = $("#SalaryDateStr").val();


    if (year > 0 && month > 0) {
        const url = API + "PrSalaryMst/GetPayrollPartial";
        const params = { year: year, month: month, salaryDateStr: salaryDateStr };
        loadPayrollPartialWithParams(url, params, "#PayrollPartialDiv", null);
    }

}

function loadPayrollPartialWithParams(url, params, targetEl, callBackF, scrollDiv) {
    $(targetEl).html("");
    if (!hasAnyError(url) && !hasAnyError(targetEl)) {
        startUiBlock();
        $.post(url, params, function (rData) {
            stopUiBlock();
            if (!hasAnyError(rData)) {
                $(targetEl).html(rData);
                if (!hasAnyError(scrollDiv) && convertStringToBool(scrollDiv) == true) {
                    scrollDiv(targetEl);
                }

                callNextFunction(callBackF);
            } else {
                failedMsg("No Payroll Found");
            }
        });

    }
}