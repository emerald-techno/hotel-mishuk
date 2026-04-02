 let arreareList = [];

$(document.body).on("click", "#AddArrearEmpBtn", function () {
    const employeeId = $("#EmployeeId").val();
    const employeeName = $("#EmployeeId option:selected").text();
    const amount = $("#Amount").val();
    const remarks = $("#DtlRemarks").val();

    if (employeeId > 0) {
        const exist = arreareList.find(x => x.employeeId == employeeId);

        if (!exist) {
            const model = {
                employeeId: employeeId,
                employeeName: employeeName,
                amount: amount,
                remarks: remarks
            }

            arreareList.push(model);
        } else {
            alert("Already Added");
        }
    }

    generateArreareTableBody();

})
$(document.body).on("click", ".rmv-arrear", function () {
    const index = $(this).attr('data-index');

    if (index > -1) {
        arreareList.splice(index, 1);

        if (index == 0) {
            $("#ArrearTbody").empty();
        } else {
            generateArreareTableBody();
        }
    }
})

function generateArreareTableBody() {
    if (arreareList.length > 0) {

        $("#ArrearTbody").empty();

        arreareList.forEach((v, i) => {
            const slNoCell = `<td>${(i+1)}</td>`;
            const employeeCell = `<td><input type='hidden' name='PrArrearDtls[${i}].EmployeeId' value='${v.employeeId}'/>${v.employeeName}</td>`;
            const amountCell = `<td><input type='hidden' name='PrArrearDtls[${i}].Amount' value='${v.amount}'/>${v.amount}</td>`;
            const remarksCell = `<td><input type='hidden' name='PrArrearDtls[${i}].Remarks' value='${v.remarks}'/>${v.remarks}</td>`;
            const actionCell = `<td><i class="fa fa-trash rmv-arrear" data-index='${i}'></i></td>`;

            const row = `<tr>${slNoCell}${employeeCell}${amountCell}${remarksCell}${actionCell}</tr>`;

            $("#ArrearTbody").append(row);
        })
    }
}