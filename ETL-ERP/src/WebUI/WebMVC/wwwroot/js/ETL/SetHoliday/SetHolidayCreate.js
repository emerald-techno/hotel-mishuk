$(document).ready(function () {

})

$("#EndDateStr").datepicker({
    onSelect: function (d, i) {
        const fromDate = $("#StartDateStr").val();
        const toDate = $("#EndDateStr").val();

        const result = getDayDiffBetweenTwoDates(fromDate, toDate);

        $("#Length").val(result);
    }
});