const DiscountTypeEnum = {
    percent: 0,
    amount : 1
};
$(function () {

});
$("#applyDiscount").click(function () {
    var fromDate = $(".date-time-picker-from-date").val();
    var toDate = $(".date-time-picker-to-date").val();
    var discountType = $("#discountType").val();
    var discountAmount = $("#discountAmount").val();
    if (hasAnyError(fromDate)) {
        return failedMsg("Please select from date..!!")
    }
    if (hasAnyError(toDate)) {
        return failedMsg("Please select to date..!!")
    }
    if (hasAnyError(discountAmount)) {
        return failedMsg("Please mention discount amount according to discount type..!!")
    }

    if (discountType == DiscountTypeEnum.percent && discountAmount > 100) {
        return failedMsg("Please enter discount under 100 when percent is selected..!!")
    }

    loadData();
});
function loadData() {
    $("#discountTableContainer").empty();
    var form = $("#discountForm");
    var formData = form.serialize();

    $.ajax({
        url: API + 'RoomCategory/DiscountSetupPrepareData',
        type: 'POST',
        data: formData,
        success: function (result) {
            $("#discountTableContainer").html(result);
        },
        error: function () {
            alert("Something went wrong while applying discount.");
        }
    })

};

$("#discountTableContainer").on("change", ".discount-type", function () {
    let index = $(this).attr("data-index");

    if (index > -1) {
        $(`#discount_amount_${index}`).val(0)
        calculateDiscount(index);
    }

});
$(document).on("focusin", ".discount-amount", function () {
    $(this).data("prev", $(this).val());
});

$("#discountTableContainer").on("change", ".discount-amount", function () {
    let index = $(this).attr("data-index");
    var discountType = $(`#discount_type_${index}`).val();
    var discountAmount = $(`#discount_amount_${index}`).val();

    if (discountType == DiscountTypeEnum.percent && discountAmount > 100) {
        failedMsg("Discount Percent Cant be Greater than 100%");
        var previousValue = parseFloat($(this).data("prev"));
        $(this).val(previousValue);
        return;
    }
    if (index > -1) {
        calculateDiscount(index);
    }

});

function calculateDiscount(index) {
    let rackRate = parseFloat($(`#rack-rate_${index}`).val()) || 0;
    let type = $(`#discount_type_${index}`).val();
    let discount = parseFloat($(`#discount_amount_${index}`).val()) || 0;

    let newRate = rackRate;

    if (type == DiscountTypeEnum.percent) {
        newRate = rackRate - (rackRate * discount / 100);
    } else if (type == DiscountTypeEnum.amount) {
        newRate = rackRate - discount;
    }

    if (newRate < 0) newRate = 0;

    $(`#offer_rate_${index}`).val(newRate.toFixed(2));
}

//if ($("#discount-type").val() == 0 && $("#discountAmount").val() == 100) {
//    failedMsg("Percent value can't be greater then 100");
//    $("#discountAmount").val(0)
//}


$(document).on("click", "#discountMapSubmitBtn", function (e) {
    e.preventDefault();

    let form = $("#discountSubmitForm");
    let formData = form.serialize();
    
    $.ajax({
        url: API + 'RoomCategory/SubmitDiscountSetupData',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response == true) {
                successMsg("Discounts saved successfully!");
                // optional: refresh table or redirect
                // location.reload();
            } else {
                failedMsg(response.message || "Something went wrong.");
            }
        },
        error: function () {
            failedMsg("Server error occurred while saving discount setup.");
        }
    });
});

