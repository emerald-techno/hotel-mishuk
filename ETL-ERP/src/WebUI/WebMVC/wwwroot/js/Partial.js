
$(document).ready(function () {



    //$(function () {



    //    $(".date-picker").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");
    //    $(".date-time-picker").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");

    //    $(".date-picker-future").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", changeMonth: true, changeYear: true, yearRange: "-100:+50" }).attr("placeholder", "dd/mm/yyyy");
    //    $(".date-time-picker-future").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", changeMonth: true, changeYear: true, yearRange: "-100:+50" }).attr("placeholder", "dd/mm/yyyy");

    //    $(".date-picker-now").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).datepicker("setDate", "0").attr("placeholder", "dd/mm/yyyy");
    //    $(".date-time-picker-now").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).datepicker("setDate", "0").attr("placeholder", "dd/mm/yyyy");

    //    $(".date-picker-dob").datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");
    //    $(".date-time-picker-dob").datetimepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0", changeMonth: true, changeYear: true, yearRange: "-100:+0" }).attr("placeholder", "dd/mm/yyyy");

    //    $(".date-picker-dob").attr("title", "** Note: Date must be 18 Years from To Date");
    //    $(".date-time-picker-dob").attr("title", "** Note: Date must be 18 Years from To Date");

    //    $(".dd-type").select2({ width: "100%" });
    //    $(".dragscroll").prop("title", dragTitle);


    //});

    $(document.body).on("change", ".date-picker", function () {
        var date = tryToDate($(this).val());
        var isFutureDate = checkItsFutureDate(date);
        if (isFutureDate == true) {
            failedMsg("Sorry, Input Date Can't Be Greater Than To date");
            $(this).val("");
        }
    });

    $(document.body).on("change", ".date-picker-now", function () {
        var date = tryToDate($(this).val());
        var isFutureDate = checkItsFutureDate(date);
        if (isFutureDate == true) {
            failedMsg("Sorry, Input Date Can't Be Greater Than To date");
            $(this).datepicker({ dateFormat: "dd/mm/yy", timeFormat: "hh:mm:ss", maxDate: "0" }).datepicker("setDate", "0");
        }
    });

    function tryToDate(dateParam) {
        var dateTime = dateParam.split("");
        var date = dateTime[0].split("/");
        return new Date(date[2], (date[1] - 1), date[0]);
    }

    function checkItsFutureDate(dateTime) {
        var now = new Date();
        var future = false;
        if (Date.parse(now) < Date.parse(dateTime)) {
            future = true;
        }
        return future;
    }

    var lb = '<button type="button" class="btn btn-mini glyphicon glyphicon-menu-left scrollButton scrollLeftButton"></button>';
    var rb = '<button type="button" class="btn btn-mini glyphicon glyphicon-menu-right scrollButton scrollRightButton"></button>';
    $(".scrollButtonDiv").html("");
    $(".scrollButtonDiv").prepend(lb, rb);


    $(".scrollButtonDiv").prepend(getScrollButtons());


    $.validator.unobtrusive.parse(document);


});


