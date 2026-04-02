$(document).ready(function () {
    renderInitRow();
});

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

function getCategoryInfo() {
    const categoryId = $("#RoomCategoryId").val();

    if (categoryId > 0) {
        const url = API + "RoomCategory/GetRoomCatgoryInfoById/" + categoryId;

        $.get(url, function (rData) {
            if (rData !== undefined) {
                console.log("category-info", rData);

                $("#Rent").val(rData.rent);
                $("#ServiceCharge").val(rData.serviceCharge);
                $('#IsAc').prop('checked', rData.isAc);
                $('#IsBelcony').prop('checked', rData.isBalcony);

                $("#NumberOfBed").val(rData.bedNumber);
                $("#Person").val(rData.capacity);

                $("#BedTypeId").val(rData.bedTypeId).trigger("change");

                sumTotal();
            }
        });
    } else {
        $("#Rent").val(0);
        $("#ServiceCharge").val(0);
        $('#IsAc').prop('checked', false);
        $('#IsBelcony').prop('checked', false);

        $("#NumberOfBed").val(0);
        $("#Person").val(0);
        $("#BedTypeId").val("").trigger("change");

        sumTotal();
    }
}

function renderInitRow() {

    let btn = `<button class='btn btn-primary mr-2' type='button' data-bs-toggle='modal' data-bs-target='#facilityModal'> <i class='fa fa-plus-square'></i> Add Facility</button>`;

    let row = `<tr><td colspan='3' class='text-center'> ${btn} </td></tr>`;

    $("#RoomFacilityTBody").append(row);
}

//$(document).ready(function () {
//    $("#facilityModal").click(function () {
//        // Check if the checkbox is checked
//        if ($("#facilityList").is(":checked")) {
//            // Get the value of the checked checkbox
//            var checkboxValue = $("#facilityList").val();

//            // Display the value (you can use it as needed)
//            console.log("Checkbox value: " + checkboxValue);
//        } else {
//            console.log("Checkbox is not checked");
//        }
//    });
//});

let facilityList = [];

$(document.body).on("click", ".facility_item", function () {

    const index = $(this).attr("data-index");

    console.log("data-index: ", index);

    const isChecked = $(this).is(":checked");

    if (index > -1) {
        if (isChecked) {
            const categoryId = $(`#fa_cat_id_${index}`).val();
            const categoryName = $(`#fa_cat_name_${index}`).val();
            const facilityId = $(`#fa_id_${index}`).val();
            const facilityName = $(`#fa_name_${index}`).val();

            const model = {
                categoryId: categoryId,
                categoryName: categoryName,
                facilityId: facilityId,
                facilityName: facilityName
            };

            console.log("model: ", model);

            if (model.facilityId > 0) {
                facilityList.push(model);
            }
        } else {
            const facilityId = $(`#fa_id_${index}`).val();

            var removeIndex = facilityList.findIndex(x => x.facilityId == facilityId);

            if (removeIndex > -1) {
                facilityList.splice(removeIndex, 1);
            } else {
                console.log("Item can't be remove");
            }
        }
    }
});

$(document.body).on("click", "#facilitySubmitBtn", function () {
    $("#facilityModal").modal('hide');
    renderFacilityTBody();
})

function renderFacilityTBody() {

    $("#RoomFacilityTBody").empty();

    if (facilityList.length > 0) {
        facilityList.forEach((v, i) => {
            const categoryCell = `<td>${v.categoryName}</td>`;
            const facilityCell = `<td>
                                    <input type='hidden' name='RoomFacilityMaps[${i}].FacilityId' value='${v.facilityId}'/> 
                                    <input type='hidden' name='RoomFacilityMaps[${i}].FacilityName' value='${v.facilityName}'/>
                                    ${v.facilityName}
                                  </td>`;

            const actionCell = `<td><a class='ml-2 fa_delete' data-index='${i}' href='#' title='Delete'><i class="fa fa-trash"></i></a></td>`;

            const row = `<tr>${categoryCell}${facilityCell}${actionCell}</tr>`;

            $("#RoomFacilityTBody").append(row);
        });

        renderInitRow();
    } else {
        renderInitRow();
    }


}