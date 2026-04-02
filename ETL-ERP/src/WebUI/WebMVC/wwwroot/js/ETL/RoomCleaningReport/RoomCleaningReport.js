$(document.body).on("click", "#ShowReportBtn", function () {
    loadData();
});

$(document.body).on("click", "#ReportPrintBtn", function (e) {
    e.preventDefault();

    const model = getSearchObject();
    const url = `${API}RoomAssign/RoomCleaningReportPrint?strFromDate=${model.StrFromDate}&strToDate=${model.StrToDate}&houseKeeperId=${model.HouseKeeperId}&status=${model.Status}`;
   
    window.open(url, "_blank");
});



function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<b>No Data Found</b>");
    }
}


$(document.body).on("keyup", "#RoomNo", function () {
    loadData();
});

function loadData() {
    const searchVm = getSearchObject();

    let hasError = false;

    if (hasAnyError(searchVm.StrFromDate)) {
        failedMsg("Please check From Date");
        hasError = true;
    }

    if (hasAnyError(searchVm.StrToDate)) {
        failedMsg("Please check To Date");
        hasError = true;
    }

    if (hasError) return;

    const url = API + "RoomAssign/RoomCleaningReport";
    const params = { model: searchVm };
    $.post(url, params, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
}


function getSearchObject() {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        HouseKeeperId: $("#HouseKeeperId").val() ?? "",
        RoomNo: $("#RoomNo").val() ?? "",
        Status: $("#Status").val()
    };
    return model;
}