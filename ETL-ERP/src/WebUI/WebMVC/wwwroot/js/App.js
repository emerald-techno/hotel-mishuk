
/*var _partialViewManager;*/
var _dropdownManager;

$(document).ready(function () {

/*    _partialViewManager = new PartialViewManager();*/
    _dropdownManager = new DropdownManager();

    const bodyMode = localStorage.getItem("body");
    console.log("body-mode", bodyMode);
});


const UserEnum = {
    General: 1,
    Teacher: 2,
    Student: 3
}

const ScheduleType = {
    Class: 1,
    Exam: 2,
    Assignment: 3,
    ClassTest: 4
}

$(document).ready(function () {
    getUserNotification();

    // Set an interval to call the function every 10 seconds (10000 ms)
    //setInterval(getUserNotification, 10000);
});

function getUserNotification() {
    $("#notify-sec").empty();

    const url = `${API}Notification/GetUserLastNotification`;

    $.get(url, function (rData) {
        if (rData) {
            console.log("notification list: ", rData);
            renderNotifySection(rData);
        } else {
            console.log("No Notification Found..!!");
            renderNotifySection(rData);
        }
    })
}

function renderNotifySection(data) {
    if (data.length > 0) {
        let headHtml = `<li>
                           <p class='f-w-700 mb-0'>You have ${data.length} Notifications<span class="pull-right badge badge-primary badge-pill">${data.length}</span></p>
                        </li>
                        <li class='ntf-scrollable-div' id='ntf-msg-sec'><ul id='msg-sec'></ul><li>
                        <li class='text-center'>
                            <a href='${API}Notification/ViewAll'><b>View More</b></a>
                        </li>`;

        $("#notify-sec").html(headHtml);

        data.forEach((v, i) => {

            let ntfColor = v.isSeen ? 'white' : '#f5f7fb';

            let ntfHtml = `<li class='noti-primary' style='background-color:${ntfColor}'>
                                <div class='media'>
                                    <span class='notification-bg bg-light-primary'><i data-feather='check-circle'></i></span>
                                    <div class='media-body bg-light-primary'>
                                        ${v.ntfMsg}
                                    </div>
                                </div>
                            </li>`;

            $("#msg-sec").append(ntfHtml);
        })
    } else {
        let noNotiHtml = `<li>
                           <p class='f-w-700 mb-0'>You have No Notifications<span class="pull-right badge badge-primary badge-pill">${data.length}</span></p>
                        </li>`;

        $("#notify-sec").append(noNotiHtml);

        let viewMoreHtml = `<li class='text-center'>
                               <a href='${API}Notification/ViewAll'><b>View More</b></a>
                            </li>`;

        $("#notify-sec").append(viewMoreHtml);
    }
}

function toggleDropdown() {
    const dropdown = document.getElementById("dropdown");
    dropdown.classList.toggle("active");

    const url = `${API}Notification/MarkSeen`;

    $.get(url, function (rData) {
        if (rData) {
            console.log("Is Seen");
        } else {
            console.log("Not Seen..!!");
        }
    })
}

// Optional: Close dropdown if clicked outside
//document.addEventListener("click", function (event) {
//    const dropdown = document.getElementById("dropdown");
//    if (!dropdown.contains(event.target)) {
//        dropdown.classList.remove("active");
//    }
//});


function parseDDMMMYYYY(s) {
    // Example: "14-Feb-2025"
    var parts = s.split('-');
    var day = parseInt(parts[0], 10);
    var mon = parts[1].toLowerCase().substring(0, 3);
    var year = parseInt(parts[2], 10);

    var months = {
        jan: 0, feb: 1, mar: 2, apr: 3, may: 4, jun: 5,
        jul: 6, aug: 7, sep: 8, oct: 9, nov: 10, dec: 11
    };

    return new Date(year, months[mon], day);
}