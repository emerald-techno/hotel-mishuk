let guestSalaryList = [];

$(document.body).on("click", "#AddPrGuestSalaryBtn", function () {
    const employeeId = $("#EmployeeId").val();
    const employeeName = $("#EmployeeId option:selected").text();
    const amount = $("#Amount").val();
    const remarks = $("#DtlRemarks").val();

    if (employeeId > 0) {
        const exist = guestSalaryList.find(x => x.employeeId == employeeId);

        if (!exist) {
            const model = {
                employeeId: employeeId,
                employeeName: employeeName,
                amount: amount,
                remarks: remarks
            }

            guestSalaryList.push(model);
        } else {
            alert("Already Added");
        }
    }

    generateguestSalaryTableBody();

})

function generateguestSalaryTableBody() {
    if (guestSalaryList.length > 0) {

        $("#PrGuestSalaryMstTbody").empty();

        guestSalaryList.forEach((v, i) => {
            const slNoCell = `<td>${(i + 1)}</td>`;
            const employeeCell = `<td><input type='hidden' name='PrGuestSalaryDtls[${i}].EmployeeId' value='${v.employeeId}'/>${v.employeeName}</td>`;
            const amountCell = `<td><input type='hidden' name='PrGuestSalaryDtls[${i}].Amount' value='${v.amount}'/>${v.amount}</td>`;
            const remarksCell = `<td><input type='hidden' name='PrGuestSalaryDtls[${i}].Remarks' value='${v.remarks}'/>${v.remarks}</td>`;
            const actionCell = `<td><i class="fa fa-trash"></i></td>`;

            const row = `<tr>${slNoCell}${employeeCell}${amountCell}${remarksCell}${actionCell}</tr>`;

            $("#PrGuestSalaryMstTbody").append(row);
        })
    }
}
