var roomList = [];
var sameStatusSelectedRoomList = [];

// -----------------------------
// Status enums
// -----------------------------
const RoomAvailabilityStatus = Object.freeze({
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

// -----------------------------
// Document ready (entry point)
// -----------------------------
$(document).ready(function () {
    getRoomData();
    checkSelectAllBtnAvailability(sameStatusSelectedRoomList);
    checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList);
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

// -----------------------------
// Rendering: room cards
// -----------------------------
function renderRoomBox(rData) {
    $("#room-section").empty();

    if (rData && rData.length > 0) {
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

            const html = `<div class="room-card me-2 mb-2" style="background:${btnColor}; border-left:6px solid ${borderColor}; align-items:baseline;">

                            <!-- Checkbox -->
                            ${checkBoxhtml}
                            <!-- Room Info -->
                            <div style="flex:1; margin-left:8px;">
                                <h6 style="font-weight:600; margin-bottom:4px; margin-top:0;">Room ${v.roomNo}</h6>
                                <div style="font-size:15px; font-weight:500; color:#333;" 
                                     title='${(v.categoryName && v.categoryName.length > 0) ? v.categoryName : (v.roomCategoryName && v.roomCategoryName.length > 0 ? v.roomCategoryName : "")}'>
                                    ${categoryText}                                    
                                </div>
                            </div>

                            <!-- Status & Cleaning Status -->
                            <div style="text-align:left; min-width:160px; margin-left:12px;">
                                <div style="display:inline-block; background:${statusBadgeColor}; color:#fff; padding:4px 8px; border-radius:6px; font-size:12px; margin-bottom:6px;">
                                    ${v.statusText || '—'}
                                </div>
                                <div style="font-size:13px; color:#444;">
                                    ${mopIcon}${v.cleaningStatusText || ''}
                                </div>

                                <!-- 👤 Assigned Housekeeper -->
                                ${v.assignedHouseKeeperName ? `
                                    <div class="assigned-hk" 
                                         style="display:flex; align-items:center; gap:4px; margin-top:6px; font-size:13px; color:#555;">
                                        
                                        <span class="hk-name badge badge-secondary"
                                              style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis; max-width:130px;">
                                            ${v.assignedHouseKeeperName}
                                        </span>
                                        <i class="icofont icofont-under-construction-alt" 
                                           title="Assigned Housekeeper"
                                           style="font-size:18px; color:#6c757d;"></i>
                                    </div>
                                ` : ''}
                            </div>
                        </div>`;

            $("#room-section").append(html);
        });

        $(".room-status").html(`Room Status (${rData.length})`);
    } else {
        $("#room-section").html(`<h4 class='text-center'>No Room Found</h4>`);
        $(".room-status").html(`Room Status (0)`);
    }
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
            if (rData) {
                roomList = rData.rooms;
                //console.log("room-list: ", roomList);
                renderStatusCount(roomList);
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
$(document).on('click', '.dropdown-item', function () {
    $('.dropdown-toggle').dropdown('hide');
});

$(document.body).on("click", "#RoomSearchBtn", function () {
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

        //$('.room-checkbox').each(function () {
        //    const currentStatus = $(this).data('cleaningstatus');
        //    const currentHouseKeeperId = $(this).data('housekeeperid');

        //    //if (assignedHouseKeeperId !== selectedHouseKeeperId) {
        //    //    $(this).prop('disabled', true).css('opacity', '0.5');
        //    //}
        //    //else if (currentStatus !== selectedCleaningStatus) {
        //    //    $(this).prop('disabled', true).css('opacity', '0.5');
        //    //}
        //    if (assignedHouseKeeperId && assignedHouseKeeperId > 0) {
        //        $('.room-checkbox').each(function () {
        //            const currentHouseKeeperId = $(this).data('housekeeperid');

        //            if (currentHouseKeeperId && currentHouseKeeperId > 0) {
        //                // This checkbox has a housekeeper assigned → ENABLE
        //                $(this).prop('disabled', false).css('opacity', '1');
        //            } else {
        //                // No housekeeper assigned → DISABLE
        //                $(this).prop('disabled', true).css('opacity', '0.5');
        //            }
        //        });
        //    }
        //    // CASE 2: Selected room has NO assigned housekeeper
        //    else {
        //        if (currentStatus === selectedCleaningStatus) {
        //            $(this).prop('disabled', false).css('opacity', '1');
        //        } else {
        //            $(this).prop('disabled', true).css('opacity', '0.5');
        //        }
        //    }
        //});
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

        if (cleaningStatus == CleaningStatus.VACANT_DIRTY) {
            $("#status-change-section").show();

            $("#assignHouseKeeperSection").show();
            $("#submitBtn").hide();

            if (anyAssigned) {
                $("#changeCleaningStatusSection").show();
                $("#assignHouseKeeperSection").hide();
                $("#submitBtn").show();
            }
            else {
                $("#changeCleaningStatusSection").hide();
            }
        }
        else {
            $("#assignHouseKeeperSection").hide();
        }

    } else {
        $("#status-change-section,#select-all").hide();
    }
}

function checkOutOfOrderBtnAvailability(sameStatusSelectedRoomList) {
    var cleaningStatus = sameStatusSelectedRoomList[0]?.cleaningStatus;

    if (sameStatusSelectedRoomList.length > 0) {
        if (cleaningStatus != CleaningStatus.OCCUPIED && cleaningStatus != CleaningStatus.OUT_OF_ORDER) {
            $("#outOfOrderBtn").show();
        }
        else if (cleaningStatus == CleaningStatus.OUT_OF_ORDER) {
            $("#makeRoomAvailableBtn").show();
        }
        else {
            $("#outOfOrderBtn,#makeRoomAvailableBtn").hide();
        }
    }
    else {
        $("#outOfOrderBtn,#makeRoomAvailableBtn").hide();
    }
}

// -----------------------------
// Select-all checkbox logic
// -----------------------------
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
$(document.body).on("click", "#submitBtn", function () {
    const roomAvailabilityStatus = Number($("#RoomAvailability").val());
    const roomCleanStatus = Number($("#RoomStatus").val());
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

//#region Assign HouseKeepper
$(document.body).on("change", "#houseKeeperId", function () {
    const houseKeeperId = $(this).val();
    const roomIds = sameStatusSelectedRoomList.map(x => x.roomId);

    if (roomIds.length > 0 && houseKeeperId > 0) {
        const url = API + "RoomAssign/AssignMultipleRoom";

        const params = {
            roomIds: roomIds,
            houseKeeperId: houseKeeperId
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
});
//#endregion

//#region make Out of Order
$(document.body).on("click", "#outOfOrderSubmitBtn", function () {
    const OOORemarks = $("#OOORemarks").val();
    const roomIds = sameStatusSelectedRoomList.map(x => x.roomId);

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

