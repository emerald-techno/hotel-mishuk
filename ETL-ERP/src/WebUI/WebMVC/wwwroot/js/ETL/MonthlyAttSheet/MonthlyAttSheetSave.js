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