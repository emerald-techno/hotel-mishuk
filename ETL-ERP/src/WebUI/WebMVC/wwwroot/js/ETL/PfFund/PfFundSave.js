$(document.body).on("click", "#PfSubmitBtn", function () {
    const year = $("#Year").val();
    const month = $("#Month").val();

    if (year > 0 && month > 0) {
        const url = `${API}PfFundMst/IsPfExist?year=${year}&month=${month}`;
        $.get(url, function (rData) {
            if (rData) {
                swal({
                    title: "This Month Pf Report Already Exists",
                    text: "Do you want to override this month?",
                    icon: "warning",
                    buttons: true,
                    dangerMode: true,
                })
                    .then((result) => {
                        if (result) {
                            $("#PfFundMstForm").submit();
                        } else {
                            swal("Old Report Remain Safe")
                        }
                    })
            } else {
                $("#PfFundMstForm").submit();
            }
        })
    }
})