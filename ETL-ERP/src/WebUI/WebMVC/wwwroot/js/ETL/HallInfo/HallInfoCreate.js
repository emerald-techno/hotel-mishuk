$(document.body).on("change", "#Rent", function () {
    sumTotal();
});

$(document.body).on("change", "#ServiceCharge", function () {
    sumTotal();
});

$(document.body).on("change", "#Vat", function () {
    sumTotal();
});

$(document.body).on("change", "#RoomCategoryId", function () {
    getCategoryInfo();
});

function percentCalculation(percentValue, total) {
    const percentResult = (parseFloat(percentValue) * parseFloat(total)) / 100;
    return percentResult;
}

function sumTotal() {
    const rent = parseFloat($("#Rent").val());
    const serviceCharge = parseFloat($("#ServiceCharge").val());

    const vat = parseFloat($("#Vat").val());
    const vatAmount = percentCalculation(vat, rent);
    console.log("Vat Amount: ", vatAmount);

    const total = rent + serviceCharge + vatAmount;

    $("#TotalRent").val(total);
}