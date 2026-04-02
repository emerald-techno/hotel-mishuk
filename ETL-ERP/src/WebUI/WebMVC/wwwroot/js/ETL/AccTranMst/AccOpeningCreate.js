let legerList = [];

$(document).ready(function () {
    $(".form-control").attr("autocomplete", "off");

    const date = $("#VcDateStr").val();
    getFincYearByDate(date);
    search();
})

$(document.body).on("click", "#AddLegerBtn", function () {
    const ledgerId = $("#LedgerId").val();
    const ledgerName = $("#LedgerId option:selected").text();
    const drAmountBDT = $("#DrAmountBDT").val();
    const crAmountBDT = $("#CrAmountBDT").val();

    if (ledgerId > 0) {
        const exist = legerList.find(x => x.ledgerId == ledgerId);

        if (!exist) {
            const model = {
                id: 0,
                ledgerId: ledgerId,
                ledgerName: ledgerName,
                amountDr: drAmountBDT,
                amountCr: crAmountBDT,
            }

            legerList.push(model);

            $("#LedgerId").val("").trigger(update);
            $("#DrAmountBDT").val('');
            $("#CrAmountBDT").val('');

        } else {
            failedMsg("Already Added");
        }

        generateLedgerTableBody();
    }

})

$(document.body).on("click", ".rmv-ledger", function () {
    const index = $(this).attr('data-index');

    if (index > -1) {
        legerList.splice(index, 1);

        generateLedgerTableBody();
    }
})


function generateLedgerTableBody() {
    if (legerList.length > 0) {

        console.log(legerList);

        $("#LedgerTbody").empty();

        legerList.forEach((v, i) => {
            if (v.ledgerId > 0) {

                let slNoCell = `<td>${(i + 1)}</td>`;

                let idCell = `<input type='hidden' name='AccTranDtls[${i}].Id' value='${v.id}' />`;

                let hiddenLedger = ``;
                if (v.amountDr != null && v.amountDr > 0) {
                    hiddenLedger = `<input type='hidden' name='AccTranDtls[${i}].ledgerDrId' value='${v.ledgerId}' />`;
                } else if (v.amountCr != null && v.amountCr > 0) {
                    hiddenLedger = `<input type='hidden' name='AccTranDtls[${i}].ledgerCrId' value='${v.ledgerId}' />`;
                }

                let ledgerCell = `<td>${idCell}${hiddenLedger}${v.ledgerName}</td>`;

                let amountDrCell = `<td class='text-end'><input type='hidden' name='AccTranDtls[${i}].amountDr' value='${v.amountDr}'/>${v.amountDr}</td>`;
                let amountCrCell = `<td class='text-end'><input type='hidden' name='AccTranDtls[${i}].amountCr' value='${v.amountCr}'/>${v.amountCr}</td>`;
                let actionCell = `<td><i class="fa fa-trash rmv-ledger" data-index='${i}'></i></td>`;

                const row = `<tr>${slNoCell}${ledgerCell}${amountDrCell}${amountCrCell}${actionCell}</tr>`;

                $("#LedgerTbody").append(row);
            }
        })
        const totalDrValue = parseFloat(calculateDebitTotal());
        const totalCrValue = parseFloat(calculateCreditTotal());

        const totalCell = `<tr><td colSpan='2' class='text-end'><b>Total</b></td><td class='text-end'>
                           <input type='hidden' name='TotalAmount' value='${totalDrValue}'/><b>${totalDrValue}</b></td>
                           <td class='text-end'><b>${totalCrValue}</b></td><td></td></tr>`;

        $("#LedgerTbody").append(totalCell);

        //if (totalDrValue == totalCrValue) {
        //    $('#submit-button').removeAttr('disabled');
        //} else {
        //    $('#submit-button').attr('disabled', 'disabled');
        //}

    } else {
        $("#LedgerTbody").empty();
        $("#LedgerTbody").html("<tr><td colspan='5' class='text-center'><b>Please Add Some Ledger Information</b></td></tr>");
    }
}

function calculateDebitTotal() {
    if (legerList.length > 0) {
        const sum = legerList.reduce((accumulator, object) => {
            let accValue = parseFloat(accumulator);
            let amountValue = 0;
            if ($.isNumeric(object.amountDr)) {
                amountValue = parseFloat(object.amountDr);
            }
            let result = parseFloat(accValue) + parseFloat(amountValue);
            return result;
        }, 0);
        return sum;
    }
}

function calculateCreditTotal() {
    if (legerList.length > 0) {
        const sum = legerList.reduce((accumulator, object) => {
            let accValue = parseFloat(accumulator);
            let amountValue = 0;
            if ($.isNumeric(object.amountCr)) {
                amountValue = parseFloat(object.amountCr);
            }
            let result = parseFloat(accValue) + parseFloat(amountValue);
            return result;
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

                getOpenignDataByFinYear(rData.id);
            } else {
                failedMsg("Finc Year Not Found..!")
            }
        })
    }
}


$(document.body).on("change", "#FinYearId", function () {
    const finYearId = $(this).val();

    if (finYearId > 0) {
        getOpenignDataByFinYear(finYearId);
    }
})

function getOpenignDataByFinYear(finYearId) {
    if (finYearId > 0) {
        legerList = [];

        const url = `${API}AccTranMst/GetOpenignDataByYear?finYearId=${finYearId}`;

        $.get(url, function (rData) {
            if (rData != null) {
                console.log("Opening Data", rData);

                $("#Id").val(rData.id);
                $("#Narration").val(rData.narration);
                $("#IsOpenignUpdate").val(true);

                if (rData.accTranDtls != null && rData.accTranDtls.length > 0) {
                    rData.accTranDtls.forEach(v => {
                        const model = {
                            id: v.id,
                            ledgerId: (v.amountDr > 0) ? v.ledgerDrId : v.ledgerCrId,
                            ledgerName: (v.amountDr > 0) ? v.ledgerDrName : v.ledgerCrName,
                            amountDr: v.amountDr,
                            amountCr: v.amountCr
                        }

                        legerList.push(model);
                    });

                    generateLedgerTableBody();
                }

            } else {
                //failedMsg("No Opening Data Not Found..!");

                $("#Id").val(0);
                $("#Narration").val("");
                $("#IsOpenignUpdate").val(false);

                generateLedgerTableBody();
            }
        })
    } else {
        console.log("Financial year info not found");
    }
}

function search() {
    const searchVm = getSearchObject();

    if ($.fn.DataTable.isDataTable("#OpeningSearchTable")) {
        const table = $("#OpeningSearchTable").DataTable();
        table.destroy();
    }

    var params = "";
    if (!hasAnyError(searchVm)) {
        params = { SearchModel: searchVm };
    }

    const oTable = $("#OpeningSearchTable").DataTable({
        "aLengthMenu": DataTable.lengthMenu,
        "iDisplayLength": DataTable.displayLength,
        "processing": DataTable.processing,
        "serverSide": DataTable.serverSide,
        "ordering": false,

        "ajax": {
            url: API + "AccTranMst/SearchOpening",
            type: "POST",
            data: params,
        }, error(e) {
            failedMsg(e);
        },

        "columns": [
            { "data": "serialNo" },
            { "data": "ledger" },
            { "data": "amountDr" }
        ]
    });

    addTotalRowCountSpanInDataTable("OpeningSearchTable");
}

function getSearchObject() {
    const model = {
        VcType: "O"
    }

    return model;
}


$(document.body).on("click", "#generate-button", function () {
    const finYearId = $("#FinYearId").val();
    if (finYearId > 0) {
        getClosingDataByFinYear(finYearId);
    }
})


function getClosingDataByFinYear(finYearId) {
    if (finYearId > 0) {
        legerList = [];

        const url = `${API}AccTranMst/GetClosingDataByYear?finYearId=${finYearId}`;

        $.get(url, function (rData) {
            if (rData != null) {
                console.log("Closing Data", rData);

                //$("#Id").val(rData.id);
                //$("#Narration").val(rData.narration);
                //$("#IsOpenignUpdate").val(true);

                if (rData.accTranDtls != null && rData.accTranDtls.length > 0) {
                    rData.accTranDtls.forEach(v => {
                        const model = {
                            id: 0,
                            //ledgerId: (v.amountDr > 0) ? v.ledgerDrId : v.ledgerCrId,
                            //ledgerName: (v.amountDr > 0) ? v.ledgerDrName : v.ledgerCrName,
                            ledgerId: v.ledgerDrId,
                            ledgerName: v.ledgerDrName,
                            amountDr: v.amountDr,
                            amountCr: v.amountCr
                        }

                        legerList.push(model);
                    });

                    generateLedgerTableBody();
                }

            } else {
                //failedMsg("No Opening Data Not Found..!");

                $("#Id").val(0);
                $("#Narration").val("");
                $("#IsOpenignUpdate").val(false);

                generateLedgerTableBody();
            }
        })
    } else {
        console.log("Financial year info not found");
    }
}