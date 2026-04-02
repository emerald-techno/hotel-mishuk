let modalRoomList = [];

$(document.body).on("click", "#ShowRoomBtn", function () {
    modalRoomList = [];
    $("#room-section").empty();
    loadModalRoomData();
});

function loadModalRoomData() {
    const categoryId = $("#CategoryId").val();
    const checkInTime = $("#date-time-picker-from").datetimepicker('getValue');
    const checkOutTime = $("#date-time-picker-to").datetimepicker('getValue');

    if (checkInTime == null || checkOutTime == null) {
        return failedMsg("Please Select Booked Check In & Out Date..!!");
    }

    const fromDateStr = convertJsToStrDate(new Date(checkInTime));
    const toDateStr = convertJsToStrDate(new Date(checkOutTime));

    if (fromDateStr != "" && toDateStr != "") {

        /*const url = `${API}RoomInfo/GetRoomByDateRange?checkInDate=${fromDateStr}&checkOutDate=${toDateStr}`;*/
        const url = `${API}RoomInfo/GetAvailableRooms?categoryId=${categoryId}&checkInDate=${fromDateStr}&checkOutDate=${toDateStr}`;
        $.get(url, function (rData) {
            if (rData.length > 0) {
                console.log("modal-room-list: ", rData);
                modalRoomList = rData;
                renderModalRooms();
            } else {
                console.log("No Available Room Found...!");
                renderModalRooms();
            }
        })
    }
}

function renderModalRooms() {
    if (modalRoomList.length > 0) {

        $("#room-section").empty();

        modalRoomList.forEach((v, i) => {

            let color = 'btn-primary';


            const roomHtml = `<div class="room-card-box">
                                    <h5>Room ${v.roomNo}</h5></br>
                                    <b>${v.categoryName}</b>
                                </div>`;

            $("#room-section").append(roomHtml);
        });
    } else {
        $("#room-section").html("<h3>No Available Room Found..!!</h3>");
    }
}