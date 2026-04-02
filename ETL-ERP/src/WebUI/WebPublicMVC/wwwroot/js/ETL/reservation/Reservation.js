$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const RoomCount = urlParams.get('RoomCount') || "1";
    console.log('RoomCount from URL:', RoomCount);


    //$("#roomCount").val(RoomCount);


    //console.log('roomCount selected:', $("#roomCount").val());


    updateCalendar(currentDate);
});

$(document.body).on("click", "#AvaillibilityBtn", function () {


    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        RoomCount: $("#roomCount").val()
    };
    //const RoomCount = $("#roomCount").val();
    //let totalAdultCount = 0;
    //let totalChildCount = 0;

    //// sum rooms , adults and children
    //$(".check_availability_group").each(function () {
    //    totalAdultCount += parseInt($(this).find("select").eq(0).val());
    //    totalChildCount += parseInt($(this).find("select").eq(1).val());
    //});
    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate)) {
        const url = `${API}Home/ChooseRoom?arrivedDate=${model.StrFromDate}&depatureDate=${model.StrToDate}&RoomCount=${model.RoomCount}`;

        window.open(url, "_self");
    }
    else {
        swal({
            icon: "error",
            title: "Oops...",
            text: "Please Select the Correct Arrival & the Departure Date..!!"
        });
    }

});

//function createRoomSection(roomNumber) {
//    return `
//            <div class="check_availability_group">
//                <span class="label-group">ROOM ${roomNumber}</span>
//                <div class="check_availability-field_group">
//                    <div class="check_availability-field">
//                        <label>Adult</label>
//                        <select class="form-control">
//                            <option>1</option>
//                            <option>2</option>
//                            <option>3</option>
//                            <option>4</option>
//                            <option>5</option>
//                            <option>6</option>
//                        </select>
//                    </div>
//                    <div class="check_availability-field">
//                        <label>Child</label>
//                        <select class="form-control">
//                            <option>0</option>
//                            <option>1</option>
//                            <option>2</option>
//                            <option>3</option>
//                            <option>4</option>
//                            <option>5</option>
//                            <option>6</option>
//                        </select>
//                    </div>
//                </div>
//            </div>`;
//}

//// changes to the room count dropdown
//$('#roomCount').on('change', function () {
//    const numberOfRooms = parseInt($(this).val());
//    const $roomContainer = $('#roomContainer');
//    $roomContainer.empty();

//    // create the required number of rooms
//    for (let i = 1; i <= numberOfRooms; i++) {
//        $roomContainer.append(createRoomSection(i));
//    }
//});

//$('#roomCount').trigger('change');


// Calendar
let currentDate = new Date();
let selectedArrival = null;
let selectedDeparture = null;

const $monthDisplay = $('.reservation-calendar_month');
const $yearDisplay = $('.reservation-calendar_year');
const $calendarBody = $('#calendar-body');


function updateCalendar(date) {
    const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    const year = date.getFullYear();
    const month = date.getMonth();

    // Set month and year in the display
    $monthDisplay.text(monthNames[month]);
    $yearDisplay.text(year);

    // Generate calendar for the selected month and year
    $calendarBody.empty();
    const firstDay = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    // Fill in the blank cells for days before the 1st of the month
    let $row = $('<tr></tr>');
    for (let i = 0; i < firstDay; i++) {
        $row.append('<td></td>');
    }

    // Fill in the actual days of the month
    for (let day = 1; day <= daysInMonth; day++) {
        const $cell = $('<td></td>');
        const $anchor = $(`<a href="#"><small>${day}</small></a>`);
        const currentDate = new Date(year, month, day);
        if (selectedArrival || selectedDeparture) {
            //if (selectedArrival && selectedDeparture && selectedArrival.getTime() === selectedDeparture.getTime()) {
            //    if (currentDate.getTime() === selectedArrival.getTime()) {
            //        $anchor.append('<span>Arrive & Depart</span>');
            //        $cell.addClass('current-select');
            //    }
            //}
            //else {
                if (selectedArrival && currentDate.getTime() === selectedArrival.getTime()) {
                    $anchor.append('<span>Arrive</span>');
                    $cell.addClass('current-select');
                }
                if (selectedDeparture && currentDate.getTime() === selectedDeparture.getTime()) {
                    $anchor.append('<span>Depart</span>');
                    $cell.addClass('current-select');
                }

                if (selectedArrival && selectedDeparture && currentDate >= selectedArrival && currentDate <= selectedDeparture) {
                    $cell.addClass('current-select');
                }
            //}

        }





        // select arrival and departure
        $anchor.on('click', function (event) {
            event.preventDefault();
            const selectedDate = currentDate;

            if (!selectedArrival || (selectedArrival && selectedDeparture)) {
                // Select or reset arrival date
                selectedArrival = selectedDate;
                selectedDeparture = null;
            } else if (selectedArrival && selectedDate > selectedArrival) {
                // Select departure date
                selectedDeparture = selectedDate;
            } else {

                selectedArrival = selectedDate;
                selectedDeparture = null;
            }
            updateCalendar(date);
        });

        $cell.append($anchor);
        $row.append($cell);

        // Break row after Saturday
        if ((day + firstDay) % 7 === 0) {
            $calendarBody.append($row);
            $row = $('<tr></tr>');
        }
    }


    $calendarBody.append($row);
}

function captureDates() {
    const arriveDate = new Date($('#StrFromDate').val());
    const departDate = new Date($('#StrToDate').val());

    selectedArrival = !isNaN(arriveDate.getTime()) ? arriveDate : null;
    selectedDeparture = !isNaN(departDate.getTime()) ? departDate : null;

    updateCalendar(currentDate);
}

$('#StrFromDate').on('change', captureDates);
$('#StrToDate').on('change', captureDates);

$('.reservation-calendar_prev').on('click', function () {
    currentDate.setMonth(currentDate.getMonth() - 1);
    updateCalendar(currentDate);
});

$('.reservation-calendar_next').on('click', function () {
    currentDate.setMonth(currentDate.getMonth() + 1);
    updateCalendar(currentDate);
});




