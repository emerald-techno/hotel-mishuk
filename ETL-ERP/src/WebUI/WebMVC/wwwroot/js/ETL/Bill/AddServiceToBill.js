let serviceChargePercent = 0;
let vatPercent = 0;
let discountPer = 0;
let lastChanged = "";

$(document).ready(function () {
    $(".datepicker-from-date , .datepicker-to-date").datetimepicker({
        timepicker: false,
        format: 'd/m/Y'
    });

    initializeServiceAddModal();

});


$('#serviceAddModal').on('shown.bs.modal', function () {
    $('.dd-type').select2({
        dropdownParent: $('#serviceAddModal')
    });
});

function initializeServiceAddModal() {
    $('#MQty').val(1);
    $('#MDiscount').val(0);
    $('#MVat').val(0);
    $('#MServiceCharge').val(0);
    $('#MGrossTotal').val(0);
    $('#MServiceChargePercent').val(serviceChargePercent);
    $('#MVatPercent').val(vatPercent);
    $('#MDiscountPercent').val(discountPer);

    $('#EBRate').val(500);
    $('#EBQty').val(1);
    $('#EBServiceChargePercent').val(0);
    $('#EBServiceCharge').val(0);
    $('#EBVatPercent').val(0);
    $('#EBVat').val(0);
    $('#EBGrossTotal').val(0);

    let checkInDate = $("#checkInTime").text().split(" ")[0];
    let checkOutDate = $("#checkOutTime").text().split(" ")[0];

    $('#EBFrom').val(checkInDate);
    $('#EBTo').val(checkOutDate);
    var nights = getDayDiffBetweenTwoDates(checkInDate, checkOutDate) - 1;
    $("#EBNights").val(nights);
}

//#region Service Add Calculation
$(document.body).on('change', '#EBFrom, #EBTo', function () {
    var fromDate = $("#EBFrom").val();
    var toDate = $("#EBTo").val();
    var nights = 0;

    if (!hasAnyError(fromDate) && !hasAnyError(toDate)) {
        nights = getDayDiffBetweenTwoDates(fromDate, toDate) - 1;
        $("#EBNights").val(nights);
        calculateCharges();
    }
});

function calculateCharges() {
    const activeTab = $('#serviceTabs .nav-link.active').attr('id');

    let rate = 0;
    let quantity = 0;
    let nights = 0;

    if (activeTab === 'service-tab') {

        rate = parseFloat($('#MRate').val()) || 0;
        quantity = parseFloat($('#MQty').val()) || 0;

        calculateExtraServiceCharge(rate, quantity);
    }
    else {
        rate = parseFloat($('#EBRate').val()) || 0;
        quantity = parseFloat($('#EBQty').val()) || 0;
        nights = parseFloat($("#EBNights").val()) || 0;

        if (!(nights > 0)) {
            if (hasAnyError($("#EBFrom").val())) {
                return failedMsg("Please Select Starting Date");

            }
            else if (hasAnyError($("#EBTo").val())) {
                return failedMsg("Please Select Ending Date");
            }
        }

        calculateExtraBedCharge(rate, quantity, nights);
    }

}
function calculateExtraServiceCharge(rate, quantity) {
    let amount = rate * quantity;
    $("#MAmount").val(amount.toFixed(2));

    let discount = parseFloat($("#MDiscount").val()) || 0;
    let discountPercent = parseFloat($("#MDiscountPercent").val()) || 0;

    if (lastChanged === "discountAmount") {
        discountPercent = (discount / amount) * 100;
        $("#MDiscountPercent").val(discountPercent.toFixed(2));
    } else {
        discount = (amount * discountPercent) / 100;
        $("#MDiscount").val(discount.toFixed(2));
    }

    let grossTotal = amount - discount;
    $("#MGrossTotal").val(grossTotal.toFixed(2));
    $("#MSegregatedAmount").val(grossTotal.toFixed(2));

    let serviceCharge = parseFloat($("#MServiceCharge").val()) || 0;
    let serviceChargePercent = parseFloat($("#MServiceChargePercent").val()) || 0;

    if (lastChanged === "serviceChargeAmount") {
        serviceChargePercent = (serviceCharge / grossTotal) * 100;
        $("#MServiceChargePercent").val(serviceChargePercent.toFixed(2));
    } else {
        serviceCharge = (grossTotal * serviceChargePercent) / 100;
        $("#MServiceCharge").val(serviceCharge.toFixed(2));
    }

    let amountAfterSC = grossTotal + serviceCharge;

    let vatAmount = parseFloat($("#MVat").val()) || 0;
    let vatPercent = parseFloat($("#MVatPercent").val()) || 0;

    if (lastChanged === "vatAmount") {
        vatPercent = (vatAmount / amountAfterSC) * 100;
        $("#MVatPercent").val(vatPercent.toFixed(2));
    } else {
        vatAmount = (amountAfterSC * vatPercent) / 100;
        $("#MVat").val(vatAmount.toFixed(2));
    }

    let netAmount = grossTotal + serviceCharge + vatAmount;
    $("#MNetAmount").val(netAmount.toFixed(2));
}

function calculateExtraBedCharge(rate, quantity, nights) {

    let amount = rate * quantity * nights;
    $("#EBAmount").val(amount.toFixed(2));
    $("#EBSegregatedAmount").val(amount.toFixed(2));

    let grossTotal = amount;
    let serviceCharge = parseFloat($("#EBServiceCharge").val()) || 0;
    let serviceChargePercent = parseFloat($("#EBServiceChargePercent").val()) || 0;

    if (lastChanged === "EBServiceCharge") {
        serviceChargePercent = (grossTotal > 0) ? (serviceCharge / grossTotal) * 100 : 0;
        $("#EBServiceChargePercent").val(serviceChargePercent.toFixed(2));
    } else {
        serviceCharge = (grossTotal * serviceChargePercent) / 100;
        $("#EBServiceCharge").val(serviceCharge.toFixed(2));
    }

    let amountAfterSC = grossTotal + serviceCharge;

    let vatAmount = parseFloat($("#EBVat").val()) || 0;
    let vatPercent = parseFloat($("#EBVatPercent").val()) || 0;

    if (lastChanged === "EBVat") {
        vatPercent = (amountAfterSC > 0) ? (vatAmount / amountAfterSC) * 100 : 0;
        $("#EBVatPercent").val(vatPercent.toFixed(2));
    } else {
        vatAmount = (amountAfterSC * vatPercent) / 100;
        $("#EBVat").val(vatAmount.toFixed(2));
    }

    let netAmount = grossTotal + serviceCharge + vatAmount;
    $("#EBNetAmount").val(netAmount.toFixed(2));
}

$(document.body).on("click", ".segregateBtn", function () {
    const activeTab = $('#serviceTabs .nav-link.active').attr('id');

    if (activeTab === 'service-tab') {
        let service = {
            rate: parseFloat($("#MRate").val()) || 0,
            qty: parseFloat($("#MQty").val()) || 0,
            amount: parseFloat($("#MAmount").val()) || 0,
            gross: parseFloat($("#MGrossTotal").val()) || 0,
            segragatedAmount: parseFloat($("#MSegregatedAmount").val()) || 0,
            discountPercent: parseFloat($("#MDiscountPercent").val()) || 0,
            serviceChargePercent: parseFloat($("#MServiceChargePercent").val()) || 10,
            vatPercent: parseFloat($("#MVatPercent").val()) || 15,
            netAmount: parseFloat($("#MNetAmount").val()) || 0
        };

        let segregated = calculateSegregatedServiceTotal(service);

        //$("#MRate").val(segregated.rate.toFixed(2));
        $("#MDiscount").val(segregated.discount.toFixed(2));
        $("#MServiceCharge").val(segregated.serviceCharge.toFixed(2));
        $("#MVat").val(segregated.vat.toFixed(2));
        $("#MNetAmount").val(segregated.totalAmount);
        $("#MSegregatedAmount").val(segregated.segragatedAmount);
        $("#MServiceChargePercent").val(segregated.serviceChargePercent);
        $("#MVatPercent").val(segregated.vatPercent);
    }
    else {
        // for Extra bed service
        let service = {
            rate: parseFloat($("#EBRate").val()) || 0,
            qty: parseFloat($("#EBQty").val()) * parseFloat($("#EBNights").val()) || 0,
            amount: parseFloat($("#EBAmount").val()) || 0,
            segragatedAmount: parseFloat($("#EBSegregatedAmount").val()) || 0,
            serviceChargePercent: parseFloat($("#EBServiceChargePercent").val()) || 10,
            vatPercent: parseFloat($("#EBVatPercent").val()) || 15,
            netAmount: parseFloat($("#EBNetAmount").val()) || 0
        };

        let segregated = calculateSegregatedServiceTotal(service);

        //$("#EBRate").val(segregated.rate.toFixed(2));
        /*$("#EBDiscount").val(segregated.discount.toFixed(2));*/
        $("#EBSegregatedAmount").val(segregated.segragatedAmount);
        $("#EBServiceChargePercent").val(segregated.serviceChargePercent);
        $("#EBServiceCharge").val(segregated.serviceCharge.toFixed(2));
        $("#EBVatPercent").val(segregated.vatPercent);
        $("#EBVat").val(segregated.vat.toFixed(2));
        $("#EBNetAmount").val(segregated.totalAmount);
    }

});

function calculateSegregatedServiceTotal(service) {
    const qty = service.qty || 1;

    const inclusive = calculateInclusiveServiceRate(service.rate, service.vatPercent, service.serviceChargePercent);

    service.baseRate = parseFloat(inclusive.rate.toFixed(2));
    service.baseDiscount = parseFloat(inclusive.discount.toFixed(0));
    service.baseServiceCharge = parseFloat(inclusive.serviceCharge.toFixed(2));
    service.baseVat = parseFloat(inclusive.vat.toFixed(2));
    service.baseAmount = parseFloat(inclusive.total.toFixed(2));

    service.segragatedAmount = parseFloat((inclusive.rate * qty).toFixed(2));
    service.discount = parseFloat((inclusive.discount * qty).toFixed(0));

    service.serviceChargePercent = parseFloat((inclusive.serviceChargePercent).toFixed(2));
    service.serviceCharge = parseFloat((inclusive.serviceCharge * qty).toFixed(2));

    service.vatPercent = parseFloat((inclusive.vatPercent).toFixed(2));
    service.vat = parseFloat((inclusive.vat * qty).toFixed(2));
    service.totalAmount = parseFloat((service.segragatedAmount + service.serviceCharge + service.vat).toFixed(0));

    return service;
}

function calculateInclusiveServiceRate(totalInclusiveAmount, vatPercent, serviceChargePercent) {
    const discountPercent = parseFloat($("#MDiscountPercent").val()) || 0;

    let discount = 0;
    if (discountPercent > 0) {
        discount = getAmountAfterPercent(totalInclusiveAmount, discountPercent);
    }
    const grossAmount = totalInclusiveAmount - discount;

    const serviceChargeRate = serviceChargePercent >= 0 ? serviceChargePercent : 10;
    const vatRate = vatPercent >= 0 ? vatPercent : 15;

    const totalFactor = 1 + (serviceChargeRate / 100) + ((1 + serviceChargeRate / 100) * vatRate / 100);

    const baseRate = grossAmount / totalFactor;
    const serviceCharge = baseRate * (serviceChargeRate / 100);
    const vat = (baseRate + serviceCharge) * (vatRate / 100);

    return {
        rate: parseFloat(baseRate.toFixed(2)),
        discount: parseFloat(discount.toFixed(2)),
        serviceCharge: parseFloat(serviceCharge.toFixed(2)),
        vat: parseFloat(vat.toFixed(2)),
        total: parseFloat(totalInclusiveAmount),
        vatPercent: parseFloat(vatRate.toFixed(2)),
        serviceChargePercent: parseFloat(serviceChargeRate.toFixed(2))
    };
}

$(document.body).on("click", "#ServiceAddBtn", function () {

    const activeTab = $('#serviceTabs .nav-link.active').attr('id');

    if (activeTab === 'service-tab') {
        addServiceToBill();
    }
    else {
        addExtraBedServiceToBill();
    }
});


function addServiceToBill() {
    const billId = parseInt($("#MBillId").val());
    const serviceId = parseInt($("#MServiceId").val());
    const serviceDate = convertStrDDMMYYtoMMDDYY($("#MServiceDate").val());
    const bookingRoomId = $("#MBookingRoomId").val() ? parseInt($("#MBookingRoomId").val()) : null;
    const rate = parseFloat($("#MRate").val());
    const qty = parseFloat($("#MQty").val());
    /*const amount = parseFloat($("#MAmount").val());*/
    const amount = parseFloat($("#MSegregatedAmount").val());
    const serviceCharge = parseFloat($("#MServiceCharge").val());
    const vat = parseFloat($("#MVat").val());
    /* const tax = parseFloat($("#MTax").val());*/
    const discount = parseFloat($("#MDiscount").val());
    const netAmount = parseFloat($("#MNetAmount").val());

    if (!(serviceId > 0)) {
        return failedMsg("Please Select Service")
    }
    else if (!(bookingRoomId > 0)) {
        return failedMsg("Please Select Room")
    }
    else if (hasAnyError(serviceDate)) {
        return failedMsg("Please Provide Service Date")
    }
    else if (!(qty > 0)) {
        return failedMsg("Quantity Can't be Smaller or Equal to 0")
    }
    else if (!(netAmount > 0)) {
        return failedMsg("Net Amount Can't be Smaller or Equal to 0")
    }

    if (serviceId > 0 && rate > 0 && qty > 0) {
        const url = API + "Bill/BillServiceAdd";

        const params = {
            billId: billId,
            serviceId: serviceId,
            serviceDate: serviceDate,
            bookingRoomId: bookingRoomId,
            rate: rate,
            quantity: qty,
            amount: amount,
            serviceCharge: serviceCharge,
            vat: vat,
            discount: discount,
            netAmount: netAmount
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Service Add Successful");
                $("#serviceAddModal").modal('hide');
                //clearServiceForm();

                setTimeout(() => {
                    window.location.reload();
                }, 3000);
            } else {
                failedMsg("Service Entry Failed");
            }
        }).fail(function () {
            failedMsg("Service Entry Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
}

function addExtraBedServiceToBill() {
    const billId = parseInt($("#MBillId").val());
    const bookingRoomId = $("#EBRoomId").val() ? parseInt($("#EBRoomId").val()) : null;
    const fromDate = convertStrDDMMYYtoMMDDYY($("#EBFrom").val());
    const toDate = convertStrDDMMYYtoMMDDYY($("#EBTo").val());
    const nights = parseFloat($("#EBNights").val());
    const rate = parseFloat($("#EBRate").val());
    const qty = parseFloat($("#EBQty").val());
    const amount = parseFloat($("#EBSegregatedAmount").val());
    const serviceCharge = parseFloat($("#EBServiceCharge").val());
    const vat = parseFloat($("#EBVat").val());
    const netAmount = parseFloat($("#EBNetAmount").val());

    if (!(bookingRoomId > 0)) {
        return failedMsg("Please Select Room")
    }
    else if (!(qty > 0)) {
        return failedMsg("Quantity Can't be Smaller or Equal to 0")
    }
    else if (!(netAmount > 0)) {
        return failedMsg("Net Amount Can't be Smaller or Equal to 0")
    }

    if (rate > 0 && qty > 0 && !hasAnyError(fromDate) && !hasAnyError(toDate)) {
        const url = API + "Bill/BillServiceAdd";

        const params = {
            billId: billId,
            bookingRoomId: bookingRoomId,
            fromDate: fromDate,
            toDate: toDate,
            nights: nights,
            rate: rate,
            quantity: qty,
            amount: amount,
            serviceCharge: serviceCharge,
            vat: vat,
            netAmount: netAmount
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Extra Bed Add Successful");
                $("#serviceAddModal").modal('hide');
                //clearServiceForm();

                setTimeout(() => {
                    window.location.reload();
                }, 3000);
            } else {
                failedMsg("Service Entry Failed");
            }
        }).fail(function (e) {
            failedMsg(e.responseJSON.errors ? e.responseJSON.errors : "Service Entry Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
}

$(document.body).on("click", "#extraBed-tab", function () {
    calculateCharges();
})
//#endregion

//#region Service_Modal Calculation
$(document.body).on("change", "#MRate,#MQty,#EBRate,#EBQty", function () {
    calculateCharges();
});

$(document).on("change", "#MDiscount", function () {
    lastChanged = "discountAmount";
    calculateCharges();
});
$(document).on("change", "#MDiscountPercent", function () {
    lastChanged = "discountPercent";
    calculateCharges();
});
$(document).on("change", "#MServiceCharge , #EBServiceCharge", function () {
    lastChanged = "serviceChargeAmount";
    calculateCharges();
});
$(document).on("change", "#MServiceChargePercent, #EBServiceChargePercent", function () {
    lastChanged = "serviceChargePercent";
    calculateCharges();
});
$(document).on("change", "#MVat , #EBVat", function () {
    lastChanged = "vatAmount";
    calculateCharges();
});
$(document).on("change", "#MVatPercent, #EBVatPercent", function () {
    lastChanged = "vatPercent";
    calculateCharges();
});

//#endregion

//#region Remove/Delete Service

$(document.body).on("click", ".item_delete_btn", function () {
    const dtlId = $(this).attr("data-id");
    if (dtlId > 0) {
        deleteService(dtlId);
    }
});

function deleteService(id) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this item?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url = API + "Bill/BillServiceDelete/" + id;
                $.get(url, function (rData) {
                    if (rData) {
                        console.log(rData)
                        successMsg("Successfully Deleted!");
                        location.reload();
                    } else {
                        console.log("error")
                        failedMsg("Delete Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}

//#endregion
