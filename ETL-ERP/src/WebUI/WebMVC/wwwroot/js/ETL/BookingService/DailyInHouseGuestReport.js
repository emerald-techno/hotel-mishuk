$(document).ready(function () {
    ShowReportHtml();
})


$(document.body).on("click", "#ShowReportBtn", function () {

    ShowReportHtml();
});
function ShowReportHtml() {
    const model = {
        StrReportDate: $("#StrReportDate").val(),
    }
    const url = `${API}BookingService/DailyInHouseGuestReport`;
    const params = { vm: model };
    $.post(url, params, function (rData) {
        if (rData != null) {
            generateReportTable(rData);
        }
    });

    $("#modal-head").html(`<h4>Complimentary Breakfast Entry For ${model.StrReportDate}</h4>`);
}

$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        StrReportDate: $("#StrReportDate").val(),
    }

    if (!hasAnyError(model.StrReportDate) && !hasAnyError(model.StrReportDate)) {
        const url = `${API}BookingService/DailyInHouseGuestReportPrint?strReportDate=${model.StrReportDate}`;
        window.open(url, "_blank");
    }

    //const url = `${API}BookingService/InHouseGuestReportPrint`;
    //window.open(url, "_blank");
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<b>No Data Found</b>");
    }

}

$(document.body).on("click", "#CbfSubmitBtn", function () {
    const breakfastAmount = parseInt($("#CbfAmount").val());

    if (breakfastAmount > -1) {
        const url = API + "BookingService/ChangeCbf";

        const params = {
            breakfastAmount: breakfastAmount,
            breakfastDateStr: $("#StrReportDate").val(),
        }

        $.post(url, params, function (rData) {
            if (rData == true) {
                successMsg("CBF Entry Successful");
                $("#cbfAmountModal").modal('hide');

                //setTimeout(() => {
                //    window.location.href = API + "BookingService/DailyInHouseGuest";
                //}, 2000);

                ShowReportHtml();
            } else {
                failedMsg("CBF Entry Failed");
            }
        }).fail(function () {
            failedMsg("CBF Entry Failed");
        })
    } else {
        failedMsg("Information Is Not Correct..!");
    }
})