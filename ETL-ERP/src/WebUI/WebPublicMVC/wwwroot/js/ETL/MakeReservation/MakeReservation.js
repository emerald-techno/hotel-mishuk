//const API = "/../../";
const fields = ["#Name", "#Mobile", "#Email", "#Address", "#TotalRoomCount", "#Adult", "#Child"];
let currentIndex = 0;
$(document).ready(function () {
    $(fields[currentIndex]).focus().on("change", function handleNext() {
        currentIndex++;
        if (currentIndex < fields.length) {
            $(fields[currentIndex]).focus().on("change", handleNext);
        }
    });
});

const model = {
    arrivedDate: $("#")
}
