$(document.body).on("change", ".cal-pay", function () {
    const index = $(this).attr("data-index");
    if (index > -1) {
        payDaysCalculation(index);
    }
});

function payDaysCalculation(index) {
    if (index > -1) {
        const presentDays = parseFloat($(`#PresentDays_${index}`).val());
        const leaveDays = parseFloat($(`#LeaveDays_${index}`).val());
        const totalDays = parseFloat($(`#TotalDays_${index}`).val());

        let payDays = (presentDays + leaveDays);

        let absentDays = (totalDays - payDays);

        $(`#PayDays_${index}`).val(payDays);
        $(`#AbsentDays_${index}`).val(absentDays);
    }
}

$(document.body).on("click", "#MonthAttSheetSubmitBtn", function () {
    const year = $("#Year").val();
    const month = $("#Month").val();

    if (year > 0 && month > 0) {
        const url = `${API}MonthlyAttSheet/IsSheetExist?year=${year}&month=${month}`;
        $.get(url, function (rData) {
            if (rData) {
                swal({
                    title: "Sheet Already Exists",
                    text: "Do you want to override sheet?",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true,
                })
                    .then((result) => {
                        if (result) {
                            $("#MonthlyAttSheetForm").submit();
                        } else {
                            swal("Old Sheet Remain Safe")
                        }
                    })
            } else {
                $("#MonthlyAttSheetForm").submit();
            }
        })
    }
})