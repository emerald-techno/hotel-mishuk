
$(document).ready(function () {
    $(".form-control").attr("autocomplete", "off");
});

$(document.body).on("change", "#FinYearId", function () {
    const fincYearId = $(this).val();

    getFincYearById(fincYearId);
});

$(document.body).on("click", "#ShowReportBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        FinYearId: $("#FinYearId").val(),
        HeadId: $("#HeadId").val(),
        MishukLedgerId: $("#MishukLedgerId").val()        
    }

    //let AccountName = $("#HeadId option:selected").text();

    $("#ltReportFilter").html("From Date : " + model.StrFromDate + ", To Date : " + model.StrToDate);
    const url = API + "AccTranMst/HeadWiseBalance";
    const params = { model: model };
    $.post(url, params, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});

$(document.body).on("click", "#TrialBalancePrintBtn", function () {
    const model = {
        StrFromDate: $("#StrFromDate").val(),
        StrToDate: $("#StrToDate").val(),
        FinYearId: $("#FinYearId").val(),
        HeadId: $("#HeadId").val(),
        MishukLedgerId: $("#MishukLedgerId").val(),
    }

    if (!hasAnyError(model.StrFromDate) && !hasAnyError(model.StrToDate) && model.FinYearId > 0) {
        const url = `${API}AccTranMst/HeadWiseBalancePrint?fromDate=${model.StrFromDate}&toDate=${model.StrToDate}&finYearId=${model.FinYearId}&accountId=${model.MishukLedgerId}&headId=${model.HeadId}`;
        window.open(url, "_blank");
    }
});

function generateReportTable(data) {
    if (data.length > 0) {
        $("#ReportContainer").empty();
        $("#ReportContainer").append(data);

    } else {
        $("#ReportContainer").html("<b>No Data Found</b>");
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