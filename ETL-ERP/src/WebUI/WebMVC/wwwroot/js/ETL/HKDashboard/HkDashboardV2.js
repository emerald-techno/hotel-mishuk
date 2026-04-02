var roomList = [];
var sameStatusSelectedRoomList = [];
var queryDateForService = '';
const serviceCode = 'SR000009';

// -----------------------------
// Status enums
// -----------------------------
const RoomAvailabilityStatus = Object.freeze({
    All: -1,
    AVAILABLE: 0,
    OCCUPIED: 1,
    OUT_OF_ORDER: 2,
    TODAY_CHECKIN: 3,
    EXPECTED_CHECKOUT: 4,
    RESERVED: 5,
    CHECKED_OUT: 6,
    NOT_MATCHED: 7
});

const CleaningStatus = Object.freeze({
    VACANT_CLEAN: 0,
    VACANT_DIRTY: 1,
    OCCUPIED: 2,
    CHECK_OUT: 3,
    OUT_OF_ORDER: 4,
});

// -----------
// Theme maps
// -----------
const statusTheme = {
    [RoomAvailabilityStatus.AVAILABLE]: { border: "#2f9d6a", bg: "#e7f9f0" },
    [RoomAvailabilityStatus.OCCUPIED]: { border: "#1f6fb4", bg: "#eef6ff" },
    [RoomAvailabilityStatus.OUT_OF_ORDER]: { border: "#c62828", bg: "#ffebee" },
    [RoomAvailabilityStatus.TODAY_CHECKIN]: { border: "#8e70e8", bg: "#f3efff" },
    [RoomAvailabilityStatus.RESERVED]: { border: "#6610f2", bg: "#f3e8ff" },
    [RoomAvailabilityStatus.EXPECTED_CHECKOUT]: { border: "#ffc107", bg: "#fff8e1" },
    [RoomAvailabilityStatus.CHECKED_OUT]: { border: "#fd7e14", bg: "#fff3e6" },
    [RoomAvailabilityStatus.NOT_MATCHED]: { border: "#dc3545", bg: "#ffeaea" }
};
//const statusTheme = {
//    [RoomAvailabilityStatus.AVAILABLE]: {
//        border: "#15803D",  // Green 600
//        bg: "#F0FDF4",      // Green 50
//        text: "#14532D"     // Green 900
//    },

//    [RoomAvailabilityStatus.OCCUPIED]: {
//        border: "#1D4ED8",  // Blue 600
//        bg: "#EFF6FF",      // Blue 50
//        text: "#1E3A8A"     // Blue 800
//    },

//    [RoomAvailabilityStatus.OUT_OF_ORDER]: {
//        border: "#DC2626",  // Red 600
//        bg: "#FEF2F2",      // Red 50
//        text: "#7F1D1D"     // Red 900
//    },

//    [RoomAvailabilityStatus.TODAY_CHECKIN]: {
//        border: "#7C3AED",  // Violet 600
//        bg: "#F5F3FF",      // Violet 50
//        text: "#4C1D95"     // Violet 900
//    },

//    [RoomAvailabilityStatus.RESERVED]: {
//        border: "#0D9488",  // Teal 600
//        bg: "#F0FDFA",      // Teal 50
//        text: "#134E4A"     // Teal 900
//    },

//    [RoomAvailabilityStatus.EXPECTED_CHECKOUT]: {
//        border: "#EA580C",  // Orange 600
//        bg: "#FFF7ED",      // Orange 50
//        text: "#7C2D12"     // Orange 900
//    },

//    [RoomAvailabilityStatus.CHECKED_OUT]: {
//        border: "#475569",  // Slate 600
//        bg: "#F8FAFC",      // Slate 50
//        text: "#1E293B"     // Slate 900
//    },

//    [RoomAvailabilityStatus.NOT_MATCHED]: {
//        border: "#BE123C",  // Rose 700
//        bg: "#FFF1F2",      // Rose 50
//        text: "#881337"     // Rose 900
//    }
//};


// -----------------------------
// Document ready (entry point)
// -----------------------------
$(document).ready(function () {
    getRoomData();
    checkSelectAllBtnAvailability(sameStatusSelectedRoomList);
    checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList);


    $(".footer-fix").hide();
    $(".filter-body").hide();
    $(".toggle-icon").addClass("rotate");

    $(".filter-header").click(function () {

        $(".filter-body").stop(true, true).slideToggle(300);

        $(".toggle-icon").toggleClass("rotate");

    });

    const $nav = $('.main-nav');
    const $header = $('.page-main-header');
    const $toggleNavTop = $('#sidebar-toggle');

    $nav.addClass('close_icon');
    $header.addClass('close_icon');
    $toggleNavTop.attr('checked', false);

    const today = $("#StrQueryDate").val();

    const formatDate = (date) => {
        if (!hasAnyError(date)) {
            const value = date.split(/\//);
            const result = [value[2], value[1], value[0]].join('/');
            return result;
        }
    };
    queryDateForService = formatDate(today);

    getExtraServiceCount(queryDateForService, serviceCode);
});

// -------
// Helpers
// -------
function getColors(v) {
    if (v.cleaningStatus === CleaningStatus.OUT_OF_ORDER) {
        return {
            btnColor: statusTheme[RoomAvailabilityStatus.OUT_OF_ORDER].bg,
            borderColor: statusTheme[RoomAvailabilityStatus.OUT_OF_ORDER].border
        };
    }

    const statusCfg = statusTheme[v.status] || { border: "#999", bg: "#f9f9f9" };

    let btnColor = statusCfg.bg;

    switch (v.cleaningStatus) {
        case CleaningStatus.VACANT_DIRTY:
            btnColor = "#fffbe6";
            break;
        case CleaningStatus.CHECK_OUT:
            btnColor = "#fff4e6";
            break;
    }

    return {
        btnColor,
        borderColor: statusCfg.border
    };
}

function getMopIcon(cleanStatus) {
    return cleanStatus === CleaningStatus.VACANT_DIRTY ? "<i class='icofont icofont-mop me-1' title='Needs cleaning'></i>" : "";
}

function getStatusBadgeColor(v) {
    switch (v.status) {
        case RoomAvailabilityStatus.AVAILABLE:
            return "#2f9d6a";
        case RoomAvailabilityStatus.OCCUPIED:
            return v.isTodayCheckout ? "#ba895d" : "#1f6fb4";
        case RoomAvailabilityStatus.OUT_OF_ORDER:
            return "#c62828";
        case RoomAvailabilityStatus.TODAY_CHECKIN:
            return "#8e70e8";
        case RoomAvailabilityStatus.EXPECTED_CHECKOUT:
            return "#ffc107";
        case RoomAvailabilityStatus.RESERVED:
            return "#6610f2";
        case RoomAvailabilityStatus.CHECKED_OUT:
            return "#fd7e14";
        case RoomAvailabilityStatus.NOT_MATCHED:
            return "#dc3545";
        default:
            return "#999";
    }
}

function getCategoryText(v) {
    if (v.categoryName && v.categoryName.length > 15) {
        return v.categoryName.substring(0, 15) + "...";
    } else if (v.categoryName) {
        return v.categoryName;
    } else if (v.roomCategoryName && v.roomCategoryName.length > 15) {
        return v.roomCategoryName.substring(0, 15) + "...";
    } else if (v.roomCategoryName) {
        return v.roomCategoryName;
    }
    return "N/A";
}

// -----------------------------
// Rendering: status counts
// -----------------------------
function renderStatusCount(rData) {

    if (!rData || rData.length === 0) {
        $(".status-pill span[id$='-count']").html("(0)");
        return;
    }
    const availableCount = rData.availableCount;

    const occupiedCount = rData.occupiedCount;

    const oooCount = rData.oooCount;

    const cinCount = rData.cInCount;

    const coutCount = rData.cOutCount;

    $(".room-status").html(`Room Status (${rData.allRoomCount ?? 0})`);
    $("#available-count").html(`(${availableCount})`);
    $("#occupied-count").html(`(${occupiedCount})`);
    $("#ooo-count").html(`(${oooCount})`);
    $("#cin-count").html(`(${cinCount})`);
    $("#cout-count").html(`(${coutCount})`);
}

//function renderStatusCount(rData) {

//    if (!rData || rData.length === 0) {
//        $(".status-pill span[id$='-count']").html("(0)");
//        return;
//    }

//    const statusMap = {
//        AVAILABLE: "#available-count",
//        OCCUPIED: "#occupied-count",
//        OUT_OF_ORDER: "#ooo-count",
//        TODAY_CHECKIN: "#cin-count",
//        EXPECTED_CHECKOUT: "#cout-count"
//    };

//    Object.keys(statusMap).forEach(key => {

//        const count = rData.filter(x => x.status == RoomAvailabilityStatus[key]).length;

//        $(statusMap[key]).html(`<br/>(${count})`);
//    });
//}


// -----------------------------
// Rendering: room cards
// -----------------------------
function renderRoomBox(rData) {
    $("#room-section").empty();

    if (rData && rData.length > 0) {
        console.log("Rendering rooms: ", rData);
        rData.forEach((v, i) => {
            const { btnColor, borderColor } = getColors(v);
            const statusBadgeColor = getStatusBadgeColor(v);

            /*if (v.status === RoomAvailabilityStatus.OUT_OF_ORDER && v.cleaningStatus === CleaningStatus.OUT_OF_ORDER) v.statusText = "";*/
            if (v.status === RoomAvailabilityStatus.OCCUPIED && v.isTodayCheckout) v.statusText = "Expected C/Out Today";

            const categoryText = getCategoryText(v);
            const mopIcon = getMopIcon(v.cleaningStatus);

            let checkBoxhtml = "";

            checkBoxhtml = v.cleaningStatus != CleaningStatus.OCCUPIED
                ? `<input type='checkbox' data-index='${i}' data-room-id='${v.roomId}' data-cleaningstatus='${v.cleaningStatus}' data-status='${v.status}' data-housekeeperid='${v.assignedHouseKeeperId}' class='room-checkbox' id = 'room-checkbox_${i}'>`
                : `<input type='checkbox' data-index='${i}' data-room-id='${v.roomId}' data-cleaningstatus='${v.cleaningStatus}' data-status='${v.status}' class='room-checkbox' id = 'room-checkbox_${i}' disabled>`;

            var categoryHtml = "";
            categoryHtml = `<div style="font-size:15px; font-weight:500; color:#333;" 
                                     title='${(v.categoryName && v.categoryName.length > 0) ? v.categoryName : (v.roomCategoryName && v.roomCategoryName.length > 0 ? v.roomCategoryName : "")}'>
                                    ${categoryText}
                                </div>`;

            var assignedIcon = ``;
            assignedIcon = v.assignedHouseKeeperId > 0
                ? `<i class="icofont icofont-under-construction-alt room-checkbox" title="Assigned Housekeeper" style="font-size:18px; color:#6c757d;"></i>`
                : ``;

            var cleaningStatusText = v.cleaningStatus == CleaningStatus.VACANT_CLEAN ? "|| VC" : v.cleaningStatus == CleaningStatus.VACANT_DIRTY ? "|| VD" : ""
            const html = `<div class="room-card" style="background:${btnColor}; border-left:6px solid ${borderColor}; align-items:baseline;">

                            <!-- Checkbox -->
                            ${assignedIcon}

                            <!-- Room Info -->
                            <div style="flex:1; margin-left:8px;">
                                <h6 style="font-weight:600; margin-bottom:4px; margin-top:0;">Room ${v.roomNo}</h6>
                            </div>

                            <!-- Status & Cleaning Status -->
                            <div style="text-align:left; min-width:160px; margin-left:12px;">

                            <div class='d-flex align-items-center hk_room_modal' style='gap: 10px' data-room-id='${v.roomId}' data-bs-toggle='modal' data-bs-target='#roomModalCenter' 
                                data-room-id='${v.roomId}' data-cleaningstatus='${v.cleaningStatus}' data-status='${v.status}' data-housekeeperid='${v.assignedHouseKeeperId}'>

                                <div style="display:inline-block; background:${statusBadgeColor}; color:#fff; padding:4px 8px; border-radius:6px; font-size:12px; margin-bottom:6px;">
                                    ${v.statusText || '—'}
                                </div>
                                <div style="font-size:13px; color:#444;">
                                    ${cleaningStatusText || ''}
                                </div>
                            </div>

                                
                            </div>
                        </div>`;

            $("#room-section").append(html);
        });

    }
    //else {
    //    $("#room-section").html(`<h4 class='text-center'>No Room Found</h4>`);
    //    $(".room-status").html(`Room Status (0)`);
    //}
}

// -----------------------------
// Data retrieval
// -----------------------------
function getRoomData() {
    const queryDate = $("#StrQueryDate").val();

    const searchVm = getSearchObject();

    //if (!hasAnyError(availabilityStatus) && !hasAnyError(cleaningStatus)) {
    //    searchVm.HouseKeeperAvailabilityStatus = availabilityStatus,
    //    searchVm.CleaningStatus = cleaningStatus
    //}

    if (queryDate != null && queryDate != "") {
        var url = `${API}HkDashboard/HKDashBoardData?selectDate=${queryDate}&roomNo=${searchVm.RoomNo}`;

        if (searchVm.HouseKeeperAvailabilityStatus > -1 || searchVm.CleaningStatus > -1 || searchVm.RoomCategoryId > 0 || searchVm.HouseKeeperId > 0) {
            url = `${API}HkDashboard/HKDashBoardData?selectDate=${queryDate}&roomStatus=${searchVm.HouseKeeperAvailabilityStatus}&cleanStatus=${searchVm.CleaningStatus}&categoryId=${searchVm.RoomCategoryId}&houseKeeperId=${searchVm.HouseKeeperId}&roomNo=${searchVm.RoomNo}`;
        }
        $.get(url, function (rData) {
            if (rData.rooms) {
                //console.log("room-list: ", rData);
                roomList = rData.rooms;
                renderStatusCount(rData);
                renderRoomBox(roomList);

                sameStatusSelectedRoomList = [];
                checkSelectAllBtnAvailability(sameStatusSelectedRoomList);
                checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList);
            } else {
                console.log("No Room list Found...!");
            }
        });
    }
}

// --------
// Filters
// --------
//$(".status-pill").on("click", function () {

//    let value = $("#HouseKeeperStatus").val();

//    if (value !== "" && parseInt(value) > -1) {
//        $("#CleaningStatus").val(-1).trigger(update); // reset cleaning
//    }
//});

//$("#CleaningStatus").on("change", function () {

//    let value = $(this).val();

//    if (value !== "" && parseInt(value) > -1) {
//        $("#HouseKeeperStatus").val(RoomAvailabilityStatus.All); // reset availability
//    }
//});

$(document).on("click", ".status-pill", function () {

    $(".status-pill").removeClass("active");
    $(this).addClass("active");

    const statusKey = $(this).data("status");
    const statusValue = RoomAvailabilityStatus[statusKey];

    $("#HouseKeeperStatus").val(statusValue);
    $("#CleaningStatus").val(-1).trigger(update);


    getRoomData();
});

$(document).on('click', '.dropdown-item', function () {
    $('.dropdown-toggle').dropdown('hide');
});

$(document.body).on("change", "#CleaningStatus", function () {
    let value = $("#CleaningStatus").val();

    if (value !== "" && parseInt(value) > -1) {
        $("#HouseKeeperStatus").val(RoomAvailabilityStatus.All); // reset availability
    }
    getRoomData();
});

$(document.body).on("keyup", "#roomSearch", function () {
    getRoomData();
});

function getSearchObject() {
    return {
        RoomCategoryId: $("#RoomCategoryId").val(),
        RoomNo: !hasAnyError($("#roomSearch").val()) ? $("#roomSearch").val() : "",
        HouseKeeperAvailabilityStatus: $("#HouseKeeperStatus").val(),
        HouseKeeperId: $("#employeeId").val(),
        CleaningStatus: $("#CleaningStatus").val()
    };
}

// -----------------------------
// Checkbox selection logic
// -----------------------------
$(document).on('change', '.room-checkbox', function (event) {
    event.stopPropagation();

    let availableCheckboxes = $(`.room-checkbox[data-cleaningstatus!="${CleaningStatus.OCCUPIED}"]`);

    const selectedCheckbox = $(this);
    const index = selectedCheckbox.data('index');
    const selectedCleaningStatus = $(`#room-checkbox_${index}`).data('cleaningstatus');
    const selectedStatus = $(`#room-checkbox_${index}`).data('status');
    const selectedRoomId = selectedCheckbox.data('room-id');
    const assignedHouseKeeperId = selectedCheckbox.data('housekeeperid');

    if (selectedCheckbox.is(':checked')) {
        const model = {
            roomId: selectedRoomId,
            cleaningStatus: selectedCleaningStatus,
            status: selectedStatus,
            assignedHouseKeeperId: assignedHouseKeeperId
        };

        const alreadyExists = sameStatusSelectedRoomList.some(x => x.roomId === selectedRoomId);
        if (!alreadyExists) sameStatusSelectedRoomList.push(model);

        checkCheckBoxAvailability(selectedCleaningStatus, assignedHouseKeeperId);

    } else {
        sameStatusSelectedRoomList = sameStatusSelectedRoomList.filter(x => x.roomId !== selectedRoomId);

        if (sameStatusSelectedRoomList.length === 0) {
            availableCheckboxes.prop('disabled', false).css('opacity', '1');
        }
    }

    checkSelectAllBtnAvailability(sameStatusSelectedRoomList);
    checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList);
});

function checkCheckBoxAvailability(selectedCleaningStatus, selectedHouseKeeperId) {
    $('.room-checkbox').each(function () {
        const currentCleaningStatus = $(this).data('cleaningstatus');
        const currentHouseKeeperId = $(this).data('housekeeperid');

        // If Selected room HAS an assigned housekeeper
        if (selectedHouseKeeperId && selectedHouseKeeperId > 0) {
            if (currentHouseKeeperId && currentHouseKeeperId > 0) {
                $(this).prop('disabled', false).css('opacity', '1');
            } else {
                $(this).prop('disabled', true).css('opacity', '0.5');
            }
        }
        // If Selected NO assigned housekeeper → match only cleaning status
        else {
            if (currentCleaningStatus === selectedCleaningStatus) {
                $(this).prop('disabled', false).css('opacity', '1');
            } else {
                $(this).prop('disabled', true).css('opacity', '0.5');
            }
        }
    });
}

function checkSelectAllBtnAvailability(sameStatusSelectedRoomList) {
    if (sameStatusSelectedRoomList.length > 0) {
        var cleaningStatus = sameStatusSelectedRoomList[0].cleaningStatus;

        var assignedHouseKeeperIds = sameStatusSelectedRoomList.map(x => x.assignedHouseKeeperId);
        var allAssigned = sameStatusSelectedRoomList.every(x => assignedHouseKeeperIds.includes(x.assignedHouseKeeperId) && x.assignedHouseKeeperId != null);
        var anyAssigned = sameStatusSelectedRoomList.some(x => assignedHouseKeeperIds.includes(x.assignedHouseKeeperId) && x.assignedHouseKeeperId != null);

        $("#select-all").show();

        hideAllActions();

        if (cleaningStatus === CleaningStatus.VACANT_DIRTY && !anyAssigned) {

            buttonsForDirtyUnAssignedRooms();
        }

        else if (anyAssigned) {
            $("#assignHouseKeeperSection").show();
            $("#houseKeeperId").val(sameStatusSelectedRoomList[0].assignedHouseKeeperId).trigger(update).prop("disabled", true);
            $("#cleanBtn").show();
        }

    } else {
        $("#select-all").hide();
    }
}

function hideAllActions() {
    $("#assignHouseKeeperSection").hide();
    $("#houseKeeperId").val("").trigger(update).prop("disabled", false);
    $("#assignBtn").hide();
    $("#assignCleanBtn").hide();
    $("#cleanBtn").hide();
}
function buttonsForDirtyUnAssignedRooms() {
    $("#assignCleanBtn").show();
    $("#assignBtn").show();
    $("#assignHouseKeeperSection").show();
}

function checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList) {

    $("#outOfOrderBtn,#makeRoomAvailableBtn").hide();
    const cleaningStatus = parseInt(sameStatusSelectedRoomList[0]?.cleaningStatus);

    const showOutOfOrder =
        cleaningStatus !== CleaningStatus.OCCUPIED &&
        cleaningStatus !== CleaningStatus.OUT_OF_ORDER;

    const showAvailable =
        cleaningStatus === CleaningStatus.OUT_OF_ORDER;

    $("#outOfOrderBtn").toggle(showOutOfOrder);
    $("#makeRoomAvailableBtn").toggle(showAvailable);
}


$(document).on('change', '#select-all', function () {
    const isChecked = $('#select-all').is(':checked');

    if (isChecked) {
        let targetStatus = null;
        if (sameStatusSelectedRoomList.length > 0) {
            targetStatus = sameStatusSelectedRoomList[0].cleaningStatus;
        }

        if (targetStatus === null || targetStatus === undefined) {
            const allStatuses = $('.room-checkbox').map(function () {
                return $(this).data('cleaningstatus');
            }).get();

            targetStatus = allStatuses.sort((a, b) =>
                allStatuses.filter(v => v === a).length - allStatuses.filter(v => v === b).length
            ).pop();
        }

        $('.room-checkbox').each(function () {
            const $cb = $(this);
            const currentStatus = $cb.data('cleaningstatus');
            const roomId = $cb.data('room-id');

            if (currentStatus === targetStatus) {
                $cb.prop('checked', true).prop('disabled', false).css('opacity', '1');

                const alreadyExists = sameStatusSelectedRoomList.some(x => x.roomId === roomId);
                if (!alreadyExists) {
                    sameStatusSelectedRoomList.push({ roomId, cleaningStatus: currentStatus });
                }

            } else {
                $cb.prop('checked', false).prop('disabled', true).css('opacity', '0.5');
            }
        });
        if (targetStatus != CleaningStatus.OCCUPIED) {
            //$("#status-change-section").show();
            checkSelectAllBtnAvailability(sameStatusSelectedRoomList);
        }

    } else {
        let availableCheckboxes = $(`.room-checkbox[data-cleaningstatus!="${CleaningStatus.OCCUPIED}"]`);

        availableCheckboxes.prop('checked', false).prop('disabled', false).css('opacity', '1');
        sameStatusSelectedRoomList = [];
        checkSelectAllBtnAvailability(sameStatusSelectedRoomList);
        checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList);
    }

});

//#region Change Availability and cleaning_Status
$(document.body).on("click", "#cleanBtn", function () {
    const roomAvailabilityStatus = Number($("#RoomAvailability").val());
    const roomCleanStatus = CleaningStatus.VACANT_CLEAN;
    const roomIds = sameStatusSelectedRoomList.map(x => x.roomId);

    if (!(roomIds.length > 0)) {
        failedMsg("No rooms selected!");
        return;
    }

    if ((roomCleanStatus < 0 || roomCleanStatus === null)) {
        failedMsg("Please select cleaning status..!!");
        return;
    }

    const url = API + "RoomAssign/MultipleRoomStatusUpdate";
    const params = {
        roomIds: roomIds,
        availabilityStatus: roomAvailabilityStatus,
        cleanStatus: roomCleanStatus
    };
    const $btn = $(this);
    $btn.prop("disabled", true);

    $.post(url, params, function (rData) {
        if (rData === true) {
            successMsg("Room Status Update Successful");
            setTimeout(function () {
                window.location.reload();
            }, 1000)
        } else {
            failedMsg("Room Status Update Failed");
        }
    }).fail(function () {
        failedMsg("Room Status Update Failed");
    }).always(function () {
        $btn.prop("disabled", false);
    });
});
//#endregion

//#region HouseKeepper Assign / Assign&Clean
var isClean = false;
$(document.body).on("click", "#assignBtn", function () {
    var isClean = false;
    assignHouseKeeper(isClean)
});
$(document.body).on("click", "#assignCleanBtn", function () {
    isClean = true;
    assignHouseKeeper(isClean)
});
function assignHouseKeeper(isClean) {
    const houseKeeperId = $("#houseKeeperId").val();
    const roomIds = sameStatusSelectedRoomList.map(x => x.roomId);

    if (!(houseKeeperId > 0)) {
        return failedMsg("Please select HouseKeeper..!!");
    }

    if (roomIds.length > 0 && houseKeeperId > 0) {
        const url = API + "RoomAssign/AssignMultipleRoom";

        const params = {
            roomIds: roomIds,
            houseKeeperId: houseKeeperId,
            isClean: isClean
        };

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("Keeper Assign Successful");
                setTimeout(function () {
                    window.location.reload();
                }, 3000)

            } else {
                failedMsg("Keeper Assign Failed");
            }
        }).fail(function () {
            failedMsg("Keeper Assign Failed");
        })
    } else {
        console.log("Failed");
    }
}
//#endregion

//#region make Out of Order
$(document.body).on("click", "#outOfOrderSubmitBtn", function () {
    const OOORemarks = $("#OOORemarks").val();
    const roomIds = sameStatusSelectedRoomList.map(x => x.roomId);

    if (!(OOORemarks.length > 0)) {
        failedMsg("Please provide the reason, why the room is going under out of order..!!");
        return;
    }

    if (!(roomIds.length > 0)) {
        failedMsg("No rooms selected!");
        return;
    }

    const url = API + "RoomAssign/MultipleRoomMakeOOO";
    const params = {
        roomIds: roomIds,
        remarks: OOORemarks
    };
    const $btn = $(this);
    $btn.prop("disabled", true);

    $.post(url, params, function (rData) {
        if (rData === true) {
            successMsg("Room Status Update to Out of Order Successful");
            setTimeout(function () {
                window.location.reload();
            }, 500)
        } else {
            failedMsg("Room Status Update Failed");
        }
    }).fail(function () {
        failedMsg("Room Status Update Failed");
    }).always(function () {
        $btn.prop("disabled", false);
    });
});
//#endregion

//#region Make Available
$(document.body).on("click", "#makeRoomAvailableBtn", function () {
    const roomIds = sameStatusSelectedRoomList.map(x => x.roomId);

    if (!(roomIds.length > 0)) {
        failedMsg("No rooms selected!");
        return;
    }

    const url = API + "RoomAssign/MultipleRoomMakeAvailable";
    const params = {
        roomIds: roomIds
    };
    const $btn = $(this);
    $btn.prop("disabled", true);

    $.post(url, params, function (rData) {
        if (rData === true) {
            successMsg("Room Status Update to Out of Order Successful");
            setTimeout(function () {
                window.location.reload();
            }, 1000)
        } else {
            failedMsg("Room Status Update Failed");
        }
    }).fail(function () {
        failedMsg("Room Status Update Failed");
    }).always(function () {
        $btn.prop("disabled", false);
    });
});
//#endregion

//#region modal logic
const cleaningStatusRenderSection = {
    DashBoard: 1,
    Modal: 2
};
function renderCleaningStatus(v, section) {

    if (section == cleaningStatusRenderSection.DashBoard && v.status != 1) {
        return { roomClass: "", statusClass: "", label: "" };
    }
    //if (section == cleaningStatusRenderSection.Modal && v.status == 2) {
    //    return { roomClass: "", statusClass: "", label: "" };
    //}

    switch (v.cleaningStatus) {
        case 0: return { roomClass: "booked-room", statusClass: "status-clean", label: "V & C" };
        case 1: return { roomClass: "booked-room", statusClass: "status-dirty", label: "V & D" };
        case 2: return { roomClass: "booked-room", statusClass: "status-o", label: "O" };
        case 4: return { roomClass: "booked-room", statusClass: "status-ooo", label: "O.O.O" };
        default: return { roomClass: "booked-room", statusClass: "status-unknown", label: "Unknown" };
    };

}
$(document.body).on("click", ".hk_room_modal", function () {
    const roomId = $(this).attr("data-room-id");
    const queryDate = $("#StrQueryDate").val();

    console.log(`${roomId}-${queryDate}`);

    if (roomId > 0 && queryDate != "") {
        const url = `${API}RoomInfo/GetDayWiseRoomInfo?roomId=${roomId}&queryDateStr=${queryDate}`;
        $.get(url, function (rData) {
            if (rData) {
                console.log("room-info: ", rData);
                renderRoomInfo(rData);
            } else {
                console.log("No Room Info Found...!");
            }
        })
    }
});

function renderRoomInfo(rData) {
    let html = "";

    if (rData != null && rData != undefined) {

        console.log("room_info:", rData);
        let btnColor = rData.status == 1 ? "btn-info" : rData.status == 2 ? "btn-secondary" : rData.status == 3 ? "btn-primary"
            : rData.status == 4 ? "btn-danger" : rData.status == 5 ? "btn-warning" : "btn-primary";
        $("#rm_info_header").removeClass();
        $("#rm_info_header").addClass("modal-header " + btnColor);

        let acText = rData.isAc ? `<h6 class='f-w-600 txt-primary'>Yes</h6>` : `<h6 class='f-w-600 txt-secondary'>No</h6>`;
        let belconyText = rData.isBelcony ? `<h6 class='f-w-600 txt-primary'>Yes</h6>` : `<h6 class='f-w-600 txt-secondary'>No</h6>`;

        let bookedText = rData.bookingStatus == 1 ? `<h5 class='f-w-600 txt-primary'>Room ${rData.bookingStatusText}</h5>`
            : rData.bookingStatus == 2 ? `<h5 class='f-w-600 txt-info'>Room ${rData.bookingStatusText}</h5>`
                : rData.bookingStatus == 3 ? `<h5 class='f-w-600 txt-secondary'>Room ${rData.bookingStatusText}</h5>` : "";
        let checkoutLink = (rData.bookingStatus == 3) ? `<a class='da-checkout' href='BookingService/CheckOut/${rData.bookingId}'><h5 class='f-w-600 txt-secondary'>Check Out</h5></a>` : ``;
        checkoutLink = (rData.bookingStatus == 2) ? `<a class='da-checkout' href='BookingService/CheckIn/${rData.bookingId}'><h5 class='f-w-600 txt-info'>Check In</h5></a>` : checkoutLink;

        let detailLink = `<a class='bs-detail' href='BookingService/Details/${rData.bookingId}'><i class='fa fa-eye' style='font-size: 22px;' aria-hidden='true'></i></a>`;

        let guestNameText = rData.guestName != null ? `${rData.guestName}` : "N/A";
        let guestMobileText = rData.guestMobile != null ? `${rData.guestMobile}` : "N/A";

        let checkInTime = rData.checkInDate != null ? convertJsonFullDateForView(new Date(rData.checkInDate)) : "";
        let checkOutTime = rData.checkOutDate != null ? convertJsonFullDateForView(new Date(rData.checkOutDate)) : "";

        var cleaningData = renderCleaningStatus(rData, cleaningStatusRenderSection.Modal);
        let cleaningBadge = cleaningData.label
            ? `<span class="badge ${cleaningData.statusClass} ${cleaningData.roomClass} cleaningStatusModal" data-label="${cleaningData.label}" style="transform: rotate(10deg); top: 10px">
                    ${cleaningData.label}
               </span>`
            : "";
        var oooRemarksHtml = rData.oooRemarks?.length > 0 ? `<div class='col-md-3 mb-2'><b>OOO Remarks</b></div>
                                                    <div class='col-md-9 mb-2'>${rData.oooRemarks}</div>`
                                             : `<div class="col-md-3 mb-2"></div>
                                                    <div class="col-md-2 mb-2"></div>`;

        var guestHtml = rData.guestName ? `
                    <hr class=col-md-12/>

                    <div class='col-md-7 mb-3'>${bookedText}</div>
                    <div class='col-md-1 mb-3'></div>
                    <div class='col-md-4 mb-3'></div>
                    <div class="col-md-2 mb-2"><b>Guest Name</b></div>
                    <div class="col-md-5 mb-2">${guestNameText}</div>
                    <div class="col-md-3 mb-2"></div>
                    <div class="col-md-2 mb-2"></div>

                    <div class="col-md-2 mb-2"><b>Guest Mobile</b></div>
                    <div class="col-md-5 mb-2">${guestMobileText}</div>
                    <div class="col-md-3 mb-2"></div>
                    <div class="col-md-2 mb-2"></div>

                    <div class="col-md-2 mb-2"><b>Check In</b></div>
                    <div class="col-md-4 mb-2">${checkInTime}</div>
                    <div class="col-md-2 mb-2">Check Out</div>
                    <div class="col-md-4 mb-2">${checkOutTime}</div>` : "";

        html = `<div class="row">
                    <div class="col-md-2 mb-2"><b>Room No</b></div>
                    <div class="col-md-5 mb-2">${rData.roomNo}</div>
                    <div class="col-md-3 mb-2"><b>Rent</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.rent}</div>
                    <div class="col-md-2 mb-2"><b>Category</b></div>
                    <div class="col-md-5 mb-2">${rData.roomCategoryName}</div>
                    <div class="col-md-3 mb-2"><b>Service Charge</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.serviceCharge}</div>

                    <div class="col-md-2 mb-2"><b>Floor</b></div>
                    <div class="col-md-5 mb-2">${rData.floorName}</div>
                    <div class="col-md-3 mb-2"><b>VAT</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.vat}</div>

                    <div class="col-md-2 mb-2"><b>AC</b></div>
                    <div class="col-md-5 mb-2">${acText}</div>
                    <div class="col-md-3 mb-2"><b>Net Rent</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.totalRent}</div>

                    <div class="col-md-2 mb-2"><b>Belcony</b></div>
                    <div class="col-md-5 mb-2">${belconyText}</div>
                    <div class="col-md-3 mb-2"><b>Cleaning Status</b></div>
                    <div class="col-md-2 mb-2 text-end">${cleaningBadge}</div>

                    <div class="col-md-7 mb-2"><b>${rData.numberOfBed} Bed</b></div>
                    <div class="col-md-3 mb-2"><b>Extra Bed</b></div>
                    <div class="col-md-2 mb-2 text-end">${rData.extraBed ?? 0}</div>

                    <div class="col-md-7 mb-2"><b>${rData.person} Person</b></div>

                    <div class="col-md-3 mb-2"></div>
                    <div class="col-md-2 mb-2"></div>  
                    
                    ${oooRemarksHtml}

                    
                    ${guestHtml}
                    
                </div>`;
    }


    $("#room_info_sec").html(html);
    sameStatusSelectedRoomList = [];

    const alreadyExists = sameStatusSelectedRoomList.some(x => x.roomId === rData.roomId);
    if (!alreadyExists) {
        sameStatusSelectedRoomList.push({ roomId: rData.roomId, cleaningStatus: rData.cleaningStatus, status: rData.status, assignedHouseKeeperId: rData.assignedHouseKeeperId });
    }

    checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList);
    checkSelectAllBtnAvailability(sameStatusSelectedRoomList)
}
//#endregion

//#region Forecast_Report
$(document.body).on("click", "#extraBedReportBtn", function () {

    loadReport(queryDateForService, serviceCode);
});
function loadReport(queryDate, serviceCode) {

    const url = API + `BookingService/ExtraServiceReport?strFromDate=${queryDate}&strToDate=${queryDate}&serviceCode=${serviceCode}`;

    $.post(url, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}


function generateReportTable(data) {
    if (data.length > 0) {
        $("#report-section").empty();
        $("#report-section").append(data);

    } else {
        $("#report-section").html("<h5 style='text-align:center'>No Data Found<h5>");
    }

}


function getExtraServiceCount(queryDate, serviceCode) {
    const url = API + `BookingService/GetExtraServiceCount?strFromDate=${queryDate}&strToDate=${queryDate}&serviceCode=${serviceCode}`;

    $.post(url, function (rData) {
        //console.log(rData);
        if (rData != null) {
            $("#extra-bed-count").html(`(${rData})`);;
        }
    });
}
//#endregion


