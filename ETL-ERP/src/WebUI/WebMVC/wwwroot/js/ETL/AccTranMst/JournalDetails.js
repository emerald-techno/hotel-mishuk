$(document).ready(function () {
    getVcNotes();
    getVcFiles();
})

function getFormData(object) {
    const formData = new FormData();
    Object.keys(object).forEach(key => formData.append(key, object[key]));
    return formData;
}

//#region File Section

$(document.body).on("click", "#VcFileSave", function () {

    const vcId = $("#VcId").val();
    const isAjaxPost = $("#IsAjaxPost").val();
    const fileName = $("#FileName").val();
    const remarks = $("#FileRemarks").val();
    const uploadFile = $("#UploadFile").prop("files");

    var model = {
        tranMstId: vcId,
        fileName: fileName,
        remarks: remarks,
        isAjaxPost: isAjaxPost
    }


    var formData = getFormData(model);
    formData.append("uploadFilePick", uploadFile[0]);

    console.log(formData);

    $.ajax({
        url: API + "AccTranFile/Create",
        type: 'POST',
        data: formData,
        cache: false,
        processData: false,  // tell jQuery not to process the data
        contentType: false,  // tell jQuery not to set contentType
        success: function (result) {
            $('#fileModal').modal('hide');
            getVcFiles();

            $('#fileModal form')[0].reset();

        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
        }
    });


});

function getVcFiles() {
    const vcId = $("#Id").val();

    const url = API + "AccTranFile/GetVcFileByVcId?vcId=" + vcId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            renderFileTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-file", function () {
    const fileId = $(this).attr('data-id');

    if (fileId > 0) {
        swal({
            title: "Delete Voucher",
            text: "Do you want to delete voucher info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "AccTranFile/Delete/" + fileId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Voucher File Deleted");
                            getVcFiles();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Voucher Info Remain Safe")
                }
            })
    }
})

function renderFileTableBody(dataList) {
    if (dataList.length > 0) {

        $("#VcFileTBody").empty();

        dataList.forEach(v => {
            const dateCell = `<td>${convertJsonFullDateForView(v.uploadDate)}</td >`;
            const nameCell = `<td>${v.fileName}</td>`;
            const fileCell = `<td class='text-center' style='font-size:x-large;'><a href='${v.uploadFile}' title='Document' target='_blank'><i class="fa fa-file-pdf-o"></i></a></td>`;
            const actionCell = `<td class='text-center' style='font-size:x-large;'><i class="fa fa-trash rmv-file" data-id='${v.id}'></i></td>`;

            const row = `<tr>${dateCell}${nameCell}${fileCell}${actionCell}</tr>`;

            $("#VcFileTBody").append(row);
        })
    } else {
        $("#VcFileTBody").empty();
        addNoDataFoundFooter("#VcFileTBody")
    }
}

//#endregion

//#region Note Section

$(document.body).on("click", "#VcNoteSave", function () {

    $("#NoteTypeErrMsg").html('');

    const vcId = $("#VcId").val();
    const isAjaxPost = $("#IsAjaxPost").val();
    const noteType = $("#NoteType").val();
    const noteDesc = $("#NoteDesc").val();

    var model = {
        tranMstId: vcId,
        noteType: noteType,
        noteDesc: noteDesc,
        isAjaxPost: isAjaxPost
    }

    if (hasAnyError(model.noteType)) {
        return $("#NoteTypeErrMsg").html('Select Note Type Please...!');
    }

    $.ajax({
        url: API + "AccTranNote/Create",
        type: 'POST',
        data: model,
        success: function (result) {
            $('#noteModal').modal('hide');
            getVcNotes();

            $("#NoteType").val("").trigger(update);
            $('#noteModal form')[0].reset();

        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
        }
    });


});

function getVcNotes() {
    const vcId = $("#Id").val();

    const url = API + "AccTranNote/GetVcNotes?vcId=" + vcId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            renderNoteTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-note", function () {
    const noteId = $(this).attr('data-id');

    if (noteId > 0) {
        const url = API + "AccTranNote/Delete/" + noteId;
        $.get(url, function (rData) {
            if (rData == true) {
                successMsg("Voucher Note Deleted");
                getVcNotes();
            } else {
                failedMsg("failed to delete");
            }
        });
    }
})

function renderNoteTableBody(dataList) {
    if (dataList.length > 0) {

        $("#VcNoteTBody").empty();

        dataList.forEach(v => {
            const dateCell = `<td>${convertJsonFullDateForView(v.noteDate)}</td >`;
            const typeCell = `<td>${v.noteType}</td>`;
            const descCell = `<td>${v.noteDesc}</td>`;
            const actionCell = `<td class='text-center' style='font-size:x-large;'><i class="fa fa-trash rmv-note" data-id='${v.id}'></i></td>`;

            const row = `<tr>${dateCell}${typeCell}${descCell}${actionCell}</tr>`;

            $("#VcNoteTBody").append(row);
        })
    } else {
        $("#VcNoteTBody").empty();
        addNoDataFoundFooter("#VcNoteTBody")
    }
}

//#endregion

//#region print voucher

$(document.body).on("click", "#VoucherPrintBtn", function () {
    const model = {
        VoucherId: $("#VcId").val()
    }

    if (model.VoucherId > 0) {
        const url = `${API}AccTranMst/VoucherPrint?id=${model.VoucherId}`;
        window.open(url, "_blank");
    }
});

//#endregion

//#region Remove_Voucher    

$(document.body).on("click", ".voucher_delete_btn", function () {
    const voucherId = $(this).attr("data-id");
    if (voucherId > 0) {
        deleteVoucher(voucherId);
    }
});


function deleteVoucher(id) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure to delete this Order?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                const url = `${API}AccTranMst/DeleteVoucher?voucherId=${id}`;
                $.get(url, function (rData) {
                    if (rData) {
                        console.log(rData)
                        successMsg("Successfully Deleted!");

                        setTimeout(() => {
                            window.location.href = API + "AccTranMst/VoucherSearch";
                        }, 1000);

                    } else {
                        console.log("error")
                        failedMsg("Delete Failed...!")
                    }

                }).fail(function (e) {
                    failedMsg(e.responseText);
                })
            }
        })
}

//#endregion