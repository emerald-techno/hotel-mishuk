$(document.body).on("click", "#ShowReportBtn", function () {
    const d = new Date();
    let text = convertJsonFullDateForView(d);
    const isEnable = $("#ShowWithoutEmployee").is(":checked");

    const model = {
        customerId: $("#CustomerId").val(),
        groupBy: $("#GroupBy").val(),
        customerTypeId: $("#CustomerTypeId").val(),
        showWithoutEmployee: isEnable
    }

    $("#ltReportFilter").html("Date : " + text);
    const url = API + "FoodOrder/RsCustomerWiseDueReport";
    const params = { model: model };
    $.post(url, params, function (rData) {
        console.log(rData);
        if (rData != null) {
            generateReportTable(rData);
        }
    });
});

$(document.body).on("click", "#ReportPrintBtn", function () {
    const model = {
        customerId: $("#CustomerId").val(),
        groupBy: $("#GroupBy").val(),
        customerTypeId: $("#CustomerTypeId").val()
    }

    const url = `${API}FoodOrder/RsCustomerWiseDueReportPrint?customerId=${model.customerId}&groupBy=${model.groupBy}&customerTypeId=${model.customerTypeId}`;
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