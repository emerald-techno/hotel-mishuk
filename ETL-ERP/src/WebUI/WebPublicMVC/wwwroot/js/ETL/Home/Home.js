//const API = "/../../";
$(document.body).on("click", "#AvaillibilityBtn", function () {
   
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        RoomCount: $("#roomCount").val()
    }

    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
        const url = `${API}Home/Reservation?arrivedDate=${model.StrFromDate}&depatureDate=${model.StrToDate}`;

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
