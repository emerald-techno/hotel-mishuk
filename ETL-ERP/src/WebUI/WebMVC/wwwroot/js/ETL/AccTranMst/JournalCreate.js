let legerList = [];

$(document).ready(function () {
    $(".form-control").attr("autocomplete", "off");

    const date = $("#VcDateStr").val();
    getFincYearByDate(date);
})


$(document.body).on("click", "#AddLegerBtn", function () {
    const ledgerDrId = $("#LedgerDrId").val();
    const ledgerDrName = $("#LedgerDrId option:selected").text();
    const ledgerCrId = $("#LedgerCrId").val();
    const ledgerCrName = $("#LedgerCrId option:selected").text();
    const amountBDT = $("#AmountBDT").val();

    if (ledgerDrId > 0 && ledgerCrId > 0) {

        if (ledgerDrId == ledgerCrId) {
            return errorMsg("Debit & Credit Head Can't Be Same..!!");
        }

        const exist = legerList.find(x => x.ledgerDrId == ledgerDrId && x.ledgerCrId == ledgerCrId);

        if (!exist) {
            const model = {
                ledgerDrId: ledgerDrId,
                ledgerDrName: ledgerDrName,
                ledgerCrId: ledgerCrId,
                ledgerCrName: ledgerCrName,
                amountDr: amountBDT,
                amountCr: amountBDT,
            }

            legerList.push(model);

            $("#LedgerDrId").val("").trigger(update);
            $("#LedgerCrId").val("").trigger(update);
            $("#AmountBDT").val('');

        } else {
            const index = legerList.findIndex(x => x.ledgerDrId == ledgerDrId && x.ledgerCrId == ledgerCrId);

            legerList[index].amountDr = parseFloat(legerList[index].amountDr) + parseFloat(amountBDT);
            legerList[index].amountCr = parseFloat(legerList[index].amountCr) + parseFloat(amountBDT);

            //alert("Already Added");
        }

        generateLedgerTableBody();
    }

})

$(document.body).on("click", ".rmv-ledger", function () {
    const index = $(this).attr('data-index');

    if (index > -1) {
        legerList.splice(index, 1);

        /*generateLedgerTableBody();*/

        if (index == 0) {
            $("#LedgerTbody").empty();
        } else {
            generateLedgerTableBody();
        }
    }  
})


function generateLedgerTableBody() {
    if (legerList.length > 0) {

        $("#LedgerTbody").empty();

        legerList.forEach((v, i) => {
            if (v.ledgerDrId > 0 && v.ledgerCrId > 0) {

                let slNoCell = `<td>${(i + 1)}</td>`;
                let ledgerDrCell = `<td><input type='hidden' name='AccTranDtls[${i}].ledgerDrId' value='${v.ledgerDrId}'/>${v.ledgerDrName}</td>`;
                let ledgerCrCell = `<td><input type='hidden' name='AccTranDtls[${i}].ledgerCrId' value='${v.ledgerCrId}'/>${v.ledgerCrName}</td>`;
                let amountCell = `<td class='text-end'><input type='hidden' name='AccTranDtls[${i}].amountDr' value='${v.amountDr}'/>`;
                amountCell += `<input type='hidden' name='AccTranDtls[${i}].amountCr' value='${v.amountDr}'/>`;
                amountCell += `${v.amountDr}</td>`;
                let actionCell = `<td><i class="fa fa-trash rmv-ledger" data-index='${i}'></i></td>`;

                const row = `<tr>${slNoCell}${ledgerDrCell}${ledgerCrCell}${amountCell}${actionCell}</tr>`;

                $("#LedgerTbody").append(row);
            } 
        })
        const totalValue = parseFloat(calculateTotal());
        const totalCell = `<tr><td colSpan='3' class='text-end'><b>Total</b></td><td class='text-end'>
                           <input type='hidden' name='TotalAmount' value='${totalValue}'/><b>${totalValue}</b></td><td></td></tr>`;
        $("#LedgerTbody").append(totalCell);
    }
}

function calculateTotal() {
    if (legerList.length > 0) {
        const sum = legerList.reduce((accumulator, object) => {
            return parseFloat(accumulator) + parseFloat(object.amountDr);
        }, 0);
        return sum;
    }
}

function getFincYearByDate(date) {
    if (!hasAnyError(date)) {
        const url = `${API}AccTranMst/GetFincYearByDate?dateStr=${date}`;
        $.get(url, function (rData) {
            if (rData != null) {
                console.log(rData);
                $("#FinYearId").val(rData.id).trigger(update);
            } else {
                failedMsg("Finc Year Not Found..!")
            }
        })
    }
}