//$(document.body).on("change", ".pay-cal", function () {
//    const index = $(this).attr("data-index");
//    if (index > -1) {
//        payrollCalculation(index);
//    }
//});

//function payrollCalculation(index) {
//    console.log("test", index);

//    if (index > -1) {
//        const overTime = parseFloat($(`#A_ColC_${index}`).val());
//        const foodBill = parseFloat($(`#D_ColE_${index}`).val());
//        const absent = parseFloat($(`#D_ColU_${index}`).val());

//        const fixedGross = parseFloat($(`#FGrossSalary_${index}`).val());

//        const totalDeduction = parseFloat($(`#TotalDeduction_${index}`).val());
//        const netSalary = parseFloat($(`#NetSalary_${index}`).val());

//        let grossSalaryTotal = (fixedGross + overTime);

//        $(`#GrossSalary_${index}`).val(grossSalaryTotal);
//    }
//}


$(document.body).on("click", "#SavePayrollBtn", function () {
    const year = $("#Year").val();
    const month = $("#Month").val();

    if (year > 0 && month > 0) {
        const url = `${API}PrSalaryMst/IsPayrollExist?year=${year}&month=${month}`;
        $.get(url, function (rData) {
            if (rData) {
                swal({
                    title: "Payroll Already Exists",
                    text: "Do you want to override payroll?",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true,
                })
                    .then((result) => {
                        if (result) {
                            $("#PayrollForm").submit();
                        } else {
                            swal("Old Payroll Remain Safe")
                        }
                    })
            } else {
                $("#PayrollForm").submit();
            }
        })
    }
})