
$(document.body).on("change", "#Rent", function () {
    sumTotal();
});

$(document.body).on("change", "#ServiceCharge", function () {
    sumTotal();
});

function sumTotal() {
    const rent = parseFloat($("#Rent").val());
    const serviceCharge = parseFloat($("#ServiceCharge").val());

    const total = rent + serviceCharge;

    $("#TotalRent").val(total);
}