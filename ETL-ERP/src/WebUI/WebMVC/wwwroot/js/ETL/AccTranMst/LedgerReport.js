//$(document.body).on("click", "#LedgerReportBtn", function () {
//    const model = {
//        StrFromDate: $("#StrFromDate").val(),
//        StrToDate: $("#StrToDate").val(),
//        LedgerId: $("#LedgerId").val()
//    }
//    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
//    const url = API + "AccTranMst/LedgerReport";
//    const params = { model: model };
//    $.post(url, params, function (rData) {
//        //console.log(rData);
//        if (rData != null) {
//            generateReportTable(rData);
//        }
//    });
//});


//$(document.body).on("click", "#LedgerPrintBtn", function () {
//    const model = {
//        StrFromDate: $("#StrFromDate").val(),
//        StrToDate: $("#StrToDate").val(),
//        LedgerId: $("#LedgerId").val()
//    }

//    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate) && model.LedgerId > 0) {
//        const url = `${API}AccTranMst/LedgerReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&ledgerId=${model.LedgerId}`;
//        window.open(url, "_blank");
//    }
//});

//function generateReportTable(data) {
//    if (data.length > 0) {
//        $("#LedgerReportContainer").empty();
//        $("#LedgerReportContainer").append(data);

//    } else {
//        $("#LedgerReportContainer").html("<b>No Data Found</b>");
//    }

//}

$(document).ready(function () {
    $(".form-control").attr("autocomplete", "off");
})
$(document.body).on("change", "#FinYearId", function () {
    const fincYearId = $(this).val();

    getFincYearById(fincYearId);
});

$(document.body).on("click", "#LedgerReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        LedgerId: $("#LedgerId").val(),
        FinYearId: $("#FinYearId").val(),
        MishukLedgerId: $("#MishukLedgerId").val(),
    }
    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "AccTranMst/LedgerReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        //console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});


$(document.body).on("click", "#LedgerPrintBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        LedgerId: $("#LedgerId").val(),
        FinYearId: $("#FinYearId").val(),
        MishukLedgerId: $("#MishukLedgerId").val(),
    }

    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate) && model.LedgerId > 0 && model.FinYearId > 0) {
        const url = `${API}AccTranMst/LedgerReportPrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&ledgerId=${model.LedgerId}&finYearId=${model.FinYearId}&mishukLedgerId=${model.MishukLedgerId}`;
        window.open(url, "_blank");
    }
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#LedgerReportContainer").empty();
        $("#LedgerReportContainer").append(data);

    } else {
        $("#LedgerReportContainer").html("<b>No Data Found</b>");
    }

}

function getFincYearByDate(date) {
    if (!hasAnyError(date)) {
        const url = `${API}AccTranMst/GetFincYearByDate?dateStr=${date}`;
        $.get(url, function (rData) {
            if (rData != null) {
                $("#FinYearId").val(rData.id).trigger(update);
            } else {
                failedMsg("Finc Year Not Found..!")
            }
        })
    }
}

function getFincYearById(id) {
    if (id > 0) {
        const url = `${API}AccTranMst/GetFincYearById?fincYearId=${id}`;
        $.get(url, function (rData) {
            if (rData != null) {
                console.log(rData);

                const startDateStr = moment(rData.yearStartDate).format("DD/MM/YYYY");
                $("#StrFromDate").val(startDateStr);

                const endDateStr = moment(rData.yearEndDate).format("DD/MM/YYYY");
                $("#StrToDate").val(endDateStr);

            } else {
                failedMsg("Finc Year Not Found..!")
            }
        })
    }
}