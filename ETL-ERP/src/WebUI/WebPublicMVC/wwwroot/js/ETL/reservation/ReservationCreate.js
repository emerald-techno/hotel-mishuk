let roomList = [];
let selectedRoomList = [];

$(function () {
    loadRoom();
    renderSelectedRoomData();
});

$(document.body).on("click", "#AvaillibilityBtn", function () {
    loadRoom();
});

function selectedCategoryCheck() {
    let categoryId = $("#RoomCategoryId").val();

    if (categoryId > 0) {

        calculateTotal(parseInt(categoryId));
    }
}

function loadRoom() {
    roomList = [];

    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val()
    };

    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
        $("#room-section").hide();
        $("#room-loading").show();

        const url = `${API}Home/GetAvailableRoomCategory?arrivedDate=${model.StrFromDate}&depatureDate=${model.StrToDate}`;

        $.get(url, function (rData) {
            if (rData) {
                console.log("category-list:", rData);

                if (rData.length > 0) {
                    rData.forEach(v => {
                        roomList.push(v);
                    });
                }
                setTimeout(function () {
                    renderRoomSection();
                    $("#room-loading").hide();
                    $("#room-section").show();
                }, 1000)

                selectedCategoryCheck();

            } else {
                console.log("No Room Found...!");
            }
        })
    }
    else {
        swal({
            icon: "error",
            title: "Oops...",
            text: "Please Select the Correct Arrival & the Departure Date..!!"
        });
    }
}

function renderRoomSection() {
    //const $section = $("#room-section");
    //$section.hide();

    if (roomList.length > 0) {
        $("#room-section").empty();

        roomList.forEach((v, i) => {
            console.log(v)
            let existInSelected = selectedRoomList.find(x => x.roomCategoryId == v.id);
            let bookBtnHtml = existInSelected != null
                ? `<a class="awe-btn awe-btn-default bookRoomLink booked-btn" href="#" id="bookRoomLink_${i}" data-index="${i}" data-id='${v.id}'>BOOKED (${existInSelected.roomCount})</a>`
                : `<a class="awe-btn awe-btn-default bookRoomLink" href="#" id="bookRoomLink_${i}" data-index="${i}" data-id='${v.id}'>BOOK ROOM</a>`;

            let img = '';

            if (!hasAnyError(v.photoUrl)) {
                img = `<div class='reservation-room_img'>
                            <a href='${API}Home/RoomDetails/${v.id}'><img src='${v.photoUrl}'/> </a>
                        </div>`;
            } else {
                img = `<div class='reservation-room_img'>
                            <a href='${API}Home/RoomDetails/${v.id}'><img src ='${API}theme/images/No_Image_Available.jpg'/></a>
                        </div>`;
            }
            let priceHtml = '';

            if (v.offerRate > 0) {
                priceHtml = `<p class='reservation-room_price'>
                                <span class='reservation-room_amout'>৳${v.offerRate}</span>
                                </br>
                                <span class='reservation-room_amout ' style='font-size: 18px; color: #999; text-decoration: line-through;'>৳${v.rent}</span> / day
                            </p>`;
            } else {
                priceHtml = `<p class='reservation-room_price'>
                                <span class='reservation-room_amout '>৳${v.rent}</span> / day
                            </p>`;
            }

            let roomItemHtml = `<div class='reservation-room_item'>
                                    <h2 class='reservation-room_name'><a href='${API}Home/RoomDetails/${v.id}'>${v.categoryName}</a></h2>
                                    ${img}
                                    <div class='reservation-room_text'>
                                        <div class='reservation-room_desc'>
                                            <p>${v.categoryName}</p>
                                            <ul>
                                                <li>Bed: ${v.bedNumber}</li>
                                                <li>AC: ${v.isAc ? 'YES' : 'NO'}</li>
                                                <li>Balcony: ${v.isBalcony ? 'YES' : 'NO'}</li>
                                            </ul>
                                        </div>
                                        <a href='${API}Home/RoomDetails/${v.id}' class='reservation-room_view-more'>View More Infomation</a>
                                        <div class='clear'></div>
                                        ${priceHtml}
                                        ${bookBtnHtml}
                                    </div>
                                </div>`;

            $("#room-section").append(roomItemHtml);
        });

        $("#room-section").fadeIn(500);
    } else {
        $("#room-section").html("<h3>No Available Room Found..!!</h3>");
    }
}

$(document.body).on("click", ".bookRoomLink", function (e) {
    e.preventDefault();

    let categoryId = $(this).attr("data-id");

    calculateTotal(categoryId);
    $("#room-toast").fadeIn().delay(500).fadeOut();
});

$(document.body).on("change", ".room-count-input", function (e) {
    e.preventDefault();

    let index = $(this).attr("data-index");

    if (index > -1) {
        let roomCount = parseFloat( $(`#room-category_${index}`).val());
        let roomCategoryId = $(`#room-category_id_${index}`).val();

        let categoryInfo = roomList.find(x => x.id == roomCategoryId);
        let existCategory = selectedRoomList.find(x => x.roomCategoryId == roomCategoryId);

        if (existCategory != null) {
            existCategory.roomCount = roomCount;
            existCategory.totalRent = categoryInfo.offerRate ? categoryInfo.offerRate * existCategory.roomCount * existCategory.totalDays : categoryInfo.totalRent * existCategory.roomCount * existCategory.totalDays;
        }

        renderSelectedRoomData();
    }

});

function calculateTotal(categoryId = null) {
    let strFromDate = $("#StrFromDate").val();
    let strToDate = $("#StrToDate").val();
    let totalDays = serviceDateDifference();

    if (categoryId > 0) {
        let categoryInfo = roomList.find(x => x.id == categoryId);

        if (categoryInfo != null && categoryInfo != undefined) {
            console.log("Offerrate", categoryInfo)
            let existCategory = selectedRoomList.find(x => x.roomCategoryId == categoryInfo.id);

            let model = {
                roomCategoryId: categoryInfo.id,
                roomCategoryName: categoryInfo.categoryName,
                roomRent: categoryInfo.offerRate > 0 ? categoryInfo.offerRate : categoryInfo.totalRent,
                roomCount: 1,
                checkInDate: strFromDate,
                checkOutDate: strToDate,
                adult: categoryInfo.capacity,
                totalDays: totalDays,
                totalRent: categoryInfo.offerRate > 0 ? categoryInfo.offerRate * 1 * totalDays : categoryInfo.totalRent * 1 * totalDays
            }

            if (existCategory != null) {
                existCategory.roomCount = existCategory.roomCount + 1;
                existCategory.totalRent = categoryInfo.offerRate > 0 ? categoryInfo.offerRate * existCategory.roomCount * existCategory.totalDays
                    : categoryInfo.totalRent * existCategory.roomCount * existCategory.totalDays;
            } else {
                selectedRoomList.push(model);
            }
        }

        renderSelectedRoomData();
        renderRoomSection();
    }
}


function serviceDateDifference() {
    const checkInTime = $("#StrFromDate").val();
    const checkOutTime = $("#StrToDate").val();

    if (checkInTime == null || checkOutTime == null) {
        return;
    }

    //const fromDate = new Date(checkInTime);
    const fromDate = convertStrToJsDate(checkInTime);
    fromDate.setHours(0, 0, 0, 0);

    //const toDate = new Date(checkOutTime);
    const toDate = convertStrToJsDate(checkOutTime);
    toDate.setHours(0, 0, 0, 0);

    // Calculating difference in milliseconds
    const timeDifference = toDate - fromDate;

    // Converting milliseconds to days
    const days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    console.log("Time difference in days:", days);

    return days;
}

function renderSelectedRoomData() {
    if (selectedRoomList.length > 0) {
        $("#selected-room-sec").empty();

        selectedRoomList.forEach((v, i) => {
            let hiddenInput = `<input type='hidden' name='detailVms[${i}].RoomCategoryId' value='${v.roomCategoryId}' id='room-category_id_${i}'/>
                           <input type='hidden' name='detailVms[${i}].RoomCategoryName' value='${v.roomCategoryName}'/>
                           <input type='hidden' name='detailVms[${i}].CheckInDateStr' value='${v.checkInDate}'/>
                           <input type='hidden' name='detailVms[${i}].CheckOutDateStr' value='${v.checkOutDate}'/>
                           <input type='hidden' name='detailVms[${i}].RoomRent' value='${v.offerRate > 0 ? v.offerRate : v.roomRent}'/>
                           <input type='hidden' name='detailVms[${i}].TotalDays' value='${v.totalDays}'/>
                           <input type='hidden' name='detailVms[${i}].Adult' value='${v.adult}'/>
                           <input type='hidden' name='detailVms[${i}].RoomCount' value='${v.roomCount}'/>
                           <input type='hidden' name='detailVms[${i}].TotalRent' value='${v.totalRent}'/>`;

            let roomHtml = `
            <div class="card mb-4 shadow-sm border-0">
                ${hiddenInput}
                <div class="card-body" style="padding:5%">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <div>
                            <h5 class="card-title mb-1 fw-semibold" style="display:flex; justify-Content:space-between;">${v.roomCategoryName} <span><i class="fa fa-times remove-room-btn" data-index="${i}" data-id="${v.roomCategoryId}"></i></span></h5>

                            <small class="text-muted">${v.checkInDate} → ${v.checkOutDate} (${v.totalDays} night${v.totalDays > 1 ? 's' : ''})</small>
                        </div>
                        <div class="text-end">
                            <span class="badge bg-primary fs-6">${v.roomRent} BDT/night</span>
                        </div>
                    </div>
                    <div class="row align-items-center mt-3">
                        <div class="col-md-8" style="display: flex; align-items: center; justify-content: space-between;">
                            <label class="me-2 mb-0 fw-medium">Rooms:</label>
                            <input class="form-control form-control-sm w-50 room-count-input" type="number" data-index='${i}' value="${v.roomCount}" id="room-category_${i}">
                        </div>
                        <div class="col-md-4" style="text-align: center;">
                            <strong class="fs-5 text-success">${v.offerRate > 0 ? v.offerRate : v.totalRent} BDT</strong>
                        </div>
                    </div>
                </div>
            </div>`;

            $("#selected-room-sec").append(roomHtml);
        });

        const total = selectedRoomList.reduce((acc, room) => acc + (room.offerRate > 0 ? room.offerRate : room.totalRent), 0);

        let totalHtml = `<div style="background-color: #f8f9fa; padding: 16px; border-radius: 8px; box-shadow: 0 0.125rem 0.25rem rgba(0,0,0,.075); display: flex; justify-content: space-around; gap:40%;  align-items: center; margin-bottom: 1rem;border: 1px solid #dee2e6;">
                                <h5 style="margin: 0; font-weight: 700; font-size: 1.25rem;">Total Amount</h5>
                                <span class="text-success" style="font-size: 1.5rem; font-weight: 700;">${total} BDT</span>
                        </div>`;

        let buttonHtml = `
        <div class="text-center">
            <button class="btn btn-lg btn-primary px-5 py-2" id="BookingSubmitBtn">Confirm Booking</button>
        </div>`;

        $("#selected-room-sec").append(totalHtml);
        $("#selected-room-sec").append(buttonHtml);

    } else {
        $("#selected-room-sec").html("<h4 class='text-center text-muted'>No rooms selected yet.</h4>");
    }

}
$(document).on("click", ".remove-room-btn", function () {
    let index = $(this).data("index");
    let categoryId = $(this).data("id");

    removeRoom(index, categoryId);
});

function removeRoom(index, categoryId) {
    if (index > -1) {
        selectedRoomList.splice(index, 1);
    }

    let $button = $(`.bookRoomLink[data-id='${categoryId}']`);

    $button.text("Book Room").removeClass("awe-btn-success").addClass("awe-btn-default");

    renderSelectedRoomData();
}


$(document.body).on("click", "#BookingSubmitBtn", function () {
    if (!(selectedRoomList.length > 0)) {
        return failedMsg("No Room Found..!!");
    }

    $("#OnlineRoomForm").submit();
});

