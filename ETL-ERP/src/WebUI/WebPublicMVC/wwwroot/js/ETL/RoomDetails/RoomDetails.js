//const API = "/../../";


$(document).ready(function () {
    const firstThumb = $('.room-detail_thumbs .thumb-link').first();
    const firstImageSrc = firstThumb.find('img').attr('src');

    $('#largeImage').attr('src', firstImageSrc);

    firstThumb.addClass('active');

    $('.room-detail_thumbs .thumb-link').on('click', function (event) {
        event.preventDefault();
        const newSrc = $(this).find('img').attr('src');
        $('#largeImage').attr('src', newSrc);
        $('.thumb-link').removeClass('active');
        $(this).addClass('active');
    });
});


$(document.body).on("click", "#Bookingbtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        RoomCount: $("#roomCount").val(),
        RoomCategoryId: $(this).data("id"),
    }

    //if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
    //}
    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
        //const url = `${API}Home/Reservation?arrivedDate=${model.StrFromDate}&depatureDate=${model.StrToDate}&categoryId=${model.RoomCategoryId}&RoomCount=${model.RoomCount}`;
        const url = `${API}Home/Reservation?arrivedDate=${model.StrFromDate}&depatureDate=${model.StrToDate}&categoryId=${model.RoomCategoryId}`;
        window.open(url, "_self");
    }
    else {
        swal({
            icon: "error",
            title: "Something went wrong...",
            text: "Please Select the Correct Arrival & the Departure Date..!!"
        });
    }

});