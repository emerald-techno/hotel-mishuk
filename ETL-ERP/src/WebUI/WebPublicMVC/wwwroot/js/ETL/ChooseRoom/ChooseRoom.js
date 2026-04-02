//const API = "/../../";
let urlParams = "";
let RoomCount = 0;
$(document).ready(function () {
     urlParams = new URLSearchParams(window.location.search);
     RoomCount = urlParams.get('RoomCount') || "1";
    console.log('RoomCount from URL:', RoomCount);
});
$(".bookRoomLink").on("click", function (e) {
    e.preventDefault();

    const href = $(this).attr("href");
    const newUrl = `${href}&RoomCount=${parseInt(encodeURIComponent(RoomCount))}`;

    window.location.href = newUrl;
});