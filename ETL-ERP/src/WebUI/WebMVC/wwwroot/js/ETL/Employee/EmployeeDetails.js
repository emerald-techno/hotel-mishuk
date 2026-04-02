$(document).ready(function () {
    const dptId = $("#DepartmentId").val();
    const desId = $("#DesignationId").val();

    $("#PreDepartmentId").val(dptId).trigger("change");
    $("#PreDesignationId").val(desId).trigger("change");

    getEmployeeJournal();
    getEmployeeEducation();
    getEmployeeExperience();
    getEmployeeReference();
    getEmployeeNomine();
    getEmployeeEmergency();
    getEmployeeTraining();
    getEmployeePosting();
    getEmployeeAction();
})

$(document.body).on("click", "#PrintBtn", function () {
    LeaveEmployee();
});

function getFormData(object) {
    const formData = new FormData();
    Object.keys(object).forEach(key => formData.append(key, object[key]));
    return formData;
}

//#region Journal Section

$(document.body).on("click", "#EmpJournalSave", function () {

    const empId = $("#EmployeeId").val();
    const isAjaxPost = $("#IsAjaxPost").val();
    const journalName = $("#JournalName").val();
    const journalTitle = $("#JournalTitle").val();
    const journalAbout = $("#JournalAbout").val();
    const docFile = $("#DocFile").prop("files");

    var model = {
        name: journalName,
        title: journalTitle,
        about: journalAbout,
        isAjaxPost: isAjaxPost,
        employeeId: empId
    }

    var formData = getFormData(model);
    formData.append("docFile", docFile[0]);

    console.log(formData);

    $.ajax({
        url: API + "EmpJournal/Create",
        type: 'POST',
        data: formData,
        cache: false,
        processData: false,  // tell jQuery not to process the data
        contentType: false,  // tell jQuery not to set contentType
        success: function (result) {
            console.log(result);
            $('#journalModal').modal('hide');
            getEmployeeJournal();

            $('#journalModal form')[0].reset();
            $("#JournalName").val('').trigger(update);
            $("#JournalTitle").val('').trigger(update);
            $("#JournalAbout").val('').trigger(update);
        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
        }
    });
});

function getEmployeeJournal() {
    const empId = $("#Id").val();

    const url = API + "EmpJournal/GetEmpJournalByEmpId?empId=" + empId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("Journal", rData);
            renderJournalTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-journal", function () {
    const eduId = $(this).attr('data-id');

    if (eduId > 0) {
        swal({
            title: "Delete Journal",
            text: "Do you want to delete education info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpJournal/Delete/" + eduId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Journal Info Deleted");
                            getEmployeeJournal();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Journal Info Remain Safe")
                }
            })
    }
})

function renderJournalTableBody(dataList) {
    if (dataList.length > 0) {

        $("#JournalTBody").empty();

        dataList.forEach(v => {
            const nameCell = `<td>${v.name}</td>`;
            const titleCell = `<td>${v.title}</td>`;
            const submitDateCell = `<td>${v.submitDate}</td>`;
            const aboutCell = `<td>${v.about}</td>`;
            const certificateCell = `<td class='text-center' style='font-size:x-large;'><a href='${v.docUrl}' title='Education Certificate PDF' target='_blank'><i class="fa fa-file-pdf-o"></i></a></td>`;
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-journal" data-id='${v.id}'></i></td>`;

            const row = `<tr>${nameCell}${titleCell}${submitDateCell}${aboutCell}${certificateCell}${actionCell}</tr>`;

            $("#JournalTBody").append(row);
        })
    } else {
        $("#JournalTBody").empty();
        addNoDataFoundFooter("#JournalTBody")
    }
}

//#endregion

//#region Education Section

$(document.body).on("click", "#EmpEducationSave", function () {

    const empId = $("#EmployeeId").val();
    const isAjaxPost = $("#IsAjaxPost").val();
    const courseName = $("#CourseName").val();
    const institutionName = $("#InstitutionName").val();
    const boardUniversity = $("#BoardUniversity").val();
    const subjectGroup = $("#SubjectGroup").val();
    const yearFrom = $("#YearFrom").val();
    const yearTo = $("#YearTo").val();
    const passingYear = $("#PassingYear").val();
    const result = $("#Result").val();
    const certificateFile = $("#CertificateFile").prop("files");

    var model = {
        courseName: courseName,
        institutionName: institutionName,
        boardUniversity: boardUniversity,
        subjectGroup: subjectGroup,
        yearFrom: yearFrom,
        yearTo: yearTo,
        passingYear: passingYear,
        result: result,
        isAjaxPost: isAjaxPost,
        employeeId: empId
    }


    var formData = getFormData(model);
    formData.append("certificateFile", certificateFile[0]);

    console.log(formData);

    $.ajax({
        url: API + "EmpEducation/Create",
        type: 'POST',
        data: formData,
        cache: false,
        processData: false,  // tell jQuery not to process the data
        contentType: false,  // tell jQuery not to set contentType
        success: function (result) {
            console.log(result);
            $('#educationModal').modal('hide');
            getEmployeeEducation();

            $('#educationModal form')[0].reset();
            $("#CourseName").val('').trigger(update);
            $("#InstitutionName").val('').trigger(update);
            $("#BoardUniversity").val('').trigger(update);
            $("#SubjectGroup").val('').trigger(update);
            $("#YearFrom").val('').trigger(update);
            $("#YearTo").val('').trigger(update);
            $("#PassingYear").val('').trigger(update);
            $("#Result").val('').trigger(update);

        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
        }
    });


});

function getEmployeeEducation() {
    const empId = $("#Id").val();

    const url = API + "EmpEducation/GetEducationByEmpId?empId=" + empId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("education", rData);
            renderEducationTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-education", function () {
    const eduId = $(this).attr('data-id');

    if (eduId > 0) {
        swal({
            title: "Delete Education",
            text: "Do you want to delete education info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpEducation/Delete/" + eduId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Education Info Deleted");
                            getEmployeeEducation();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Education Info Remain Safe")
                }
            })
    }
})

function renderEducationTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpEducationTBody").empty();

        dataList.forEach(v => {
            const courseCell = `<td>${v.courseName}</td>`;
            const institutionCell = `<td>${v.institutionName}</td>`;
            const universityCell = `<td>${v.boardUniversity}</td>`;
            const fromYearCell = `<td>${v.yearFrom}</td>`;
            const toYearCell = `<td>${v.yearTo}</td>`;
            const subjectCell = `<td>${v.subjectGroup}</td>`;
            const passYearCell = `<td>${v.passingYear}</td>`;
            const certificateCell = `<td class='text-center' style='font-size:x-large;'><a href='${v.certificateUrl}' title='Education Certificate PDF' target='_blank'><i class="fa fa-file-pdf-o"></i></a></td>`;
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-education" data-id='${v.id}'></i></td>`;

            const row = `<tr>${courseCell}${institutionCell}${universityCell}${fromYearCell}${toYearCell}${subjectCell}${passYearCell}${certificateCell}${actionCell}</tr>`;

            $("#EmpEducationTBody").append(row);
        })
    } else {
        $("#EmpEducationTBody").empty();
        addNoDataFoundFooter("#EmpEducationTBody")
    }
}

//#endregion

//#region Experience Section

$(document.body).on("click", "#EmpExperienceSave", function () {

    const empId = $("#EmployeeId").val();
    const isAjaxPost = $("#IsAjaxPostExp").val();
    const companyName = $("#CompanyName").val();
    const designation = $("#Designation").val();
    const dateFromStr = $("#DateFromStr").val();
    const dateToStr = $("#DateToStr").val();
    const responsibility = $("#Responsibility").val();
    const leftReason = $("#LeftReason").val();
    const lastDrawnSalary = $("#LastDrawnSalary").val();
    const experienceFile = $("#ExperienceFile").prop("files");

    var model = {
        companyName: companyName,
        designation: designation,
        dateFromStr: dateFromStr,
        dateToStr: dateToStr,
        responsibility: responsibility,
        leftReason: leftReason,
        lastDrawnSalary: lastDrawnSalary,
        isAjaxPost: isAjaxPost,
        employeeId: empId
    }


    var formData = getFormData(model);
    formData.append("experienceFile", experienceFile[0]);

    if (model.employeeId > 0) {
        $.ajax({
            url: API + "EmpExperience/Create",
            type: 'POST',
            data: formData,
            processData: false,  // tell jQuery not to process the data
            contentType: false,  // tell jQuery not to set contentType
            success: function (result) {
                console.log(result);
                $('#experienceModal').modal('hide');
                getEmployeeExperience();

                $('#experienceModal form')[0].reset();
                $("#CompanyName").val('').trigger(update);
                $("#Designation").val('').trigger(update);
                $("#DateFromStr").val('').trigger(update);
                $("#DateToStr").val('').trigger(update);
                $("#Responsibility").val('').trigger(update);
                $("#LeftReason").val('').trigger(update);
                $("#LastDrawnSalary").val('').trigger(update);
            },
            error: function (jqXHR) {
            },
            complete: function (jqXHR, status) {
            }
        });
    }
});


function getEmployeeExperience() {
    const empId = $("#Id").val();

    const url = API + "EmpExperience/GetExperienceByEmpId?empId=" + empId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("experience", rData);
            renderExperienceTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-experience", function () {
    const expId = $(this).attr('data-id');

    if (expId > 0) {
        swal({
            title: "Delete Experience",
            text: "Do you want to delete experience info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpExperience/Delete/" + expId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Experience Info Deleted");
                            getEmployeeExperience();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Experience Info Remain Safe")
                }
            })
    }
})


function renderExperienceTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpExperienceTBody").empty();

        dataList.forEach(v => {
            const companyNameCell = `<td>${v.companyName}</td>`;
            const designationCell = `<td>${v.designation}</td>`;
            const dateFromCell = `<td>${convertJsonFullDateForView(v.dateFrom)}</td>`;
            const dateToCell = `<td>${convertJsonFullDateForView(v.dateTo)}</td>`;
            /*const actionCell = `<td><a href='${API}EmpEducation/Details/${v.id}' title='View'><i class="fa fa-search"></i></a></td>`;*/
            const experienceFileCell = `<td class='text-center' style='font-size:x-large;'><a href='${v.experienceUrl}' title='Experience Certificate PDF' target='_blank'><i class="fa fa-file-pdf-o"></i></a></td>`;
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-experience" data-id='${v.id}'></i></td>`;

            const row = `<tr>${companyNameCell}${designationCell}${dateFromCell}${dateToCell}${experienceFileCell}${actionCell}</tr>`;

            $("#EmpExperienceTBody").append(row);
        })
    } else {
        $("#EmpExperienceTBody").empty();
        addNoDataFoundFooter("#EmpExperienceTBody")
    }
}

//#endregion

//#region Reference Section

$(document.body).on("click", "#EmpReferenceSave", function () {
    var model = getReferenceModel();
    saveReferenceData(model);
});

$(document.body).on("click", "#EmpNomineSave", function () {
    var model = getNomineModel();
    saveReferenceData(model);
});

$(document.body).on("click", "#EmpEmergencySave", function () {
    var model = getEmergencyModel();
    saveReferenceData(model);
});


function saveReferenceData(model) {

    if (!hasAnyError(model)) {
        var formData = getFormData(model);
        //formData.append('file', $('#myfile')[0].files[0]); // myFile is the input type="file" control

        console.log(formData);

        if (model.employeeId > 0 && model.refType > 0) {
            $.ajax({
                url: API + "EmpReference/Create",
                type: 'POST',
                data: formData,
                processData: false,  // tell jQuery not to process the data
                contentType: false,  // tell jQuery not to set contentType
                success: function (result) {
                    console.log(result);
                    if (model.refType == 1) {
                        $('#referenceModal').modal('hide');
                        getEmployeeReference();
                    } else if (model.refType == 2) {
                        $('#nomineModal').modal('hide');
                        getEmployeeNomine();
                    } else if (model.refType == 3) {
                        $('#emergencyModal').modal('hide');
                        getEmployeeEmergency();

                        $('#referenceModal form')[0].reset();
                        $("#RefName1").val('').trigger(update);
                        $("#Occupation1").val('').trigger(update);
                        $("#DobStr1").val('').trigger(update);
                        $("#Mobile1").val('').trigger(update);
                        $("#Relation1").val('').trigger(update);
                        $("#Phone1").val('').trigger(update);
                        $("#Address1").val('').trigger(update);

                        $('#nomineModal form')[0].reset();
                        $("#RefName2").val('').trigger(update);
                        $("#Occupation2").val('').trigger(update);
                        $("#DobStr2").val('').trigger(update);
                        $("#Mobile2").val('').trigger(update);
                        $("#Relation2").val('').trigger(update);
                        $("#Phone2").val('').trigger(update);
                        $("#Address2").val('').trigger(update);

                        $('#emergencyModal form')[0].reset();
                        $("#RefName3").val('').trigger(update);
                        $("#Occupation3").val('').trigger(update);
                        $("#DobStr3").val('').trigger(update);
                        $("#Mobile3").val('').trigger(update);
                        $("#Relation3").val('').trigger(update);
                        $("#Phone3").val('').trigger(update);
                        $("#Address3").val('').trigger(update);
                    }
                },
                error: function (jqXHR) {
                },
                complete: function (jqXHR, status) {
                }
            });
        }
    }
}


function getReferenceModel() {
    const empId = $("#EmployeeId1").val();
    const isAjaxPost = $("#IsAjaxPostRef1").val();
    const refName = $("#RefName1").val();
    const occupation = $("#Occupation1").val();
    const mobile = $("#Mobile1").val();
    const relation = $("#Relation1").val();
    const phone = $("#Phone1").val();
    const address = $("#Address1").val();
    const refType = $("#RefType1").val();

    var model = {
        refName: refName,
        refType: refType,
        occupation: occupation,
        address: address,
        mobile: mobile,
        phone: phone,
        relation: relation,
        isAjaxPost: isAjaxPost,
        employeeId: empId
    }

    return model;
}


function getEmployeeReference() {
    const empId = $("#Id").val();

    const url = API + "EmpReference/GetReferenceByEmpId?empId=" + empId + "&refType=1";

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("reference", rData);
            renderReferenceTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-reference", function () {
    const refId = $(this).attr('data-id');

    if (refId > 0) {
        swal({
            title: "Delete Reference",
            text: "Do you want to delete reference info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpReference/Delete/" + refId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Reference Info Deleted");
                            getEmployeeReference();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Reference Info Remain Safe")
                }
            })
    }
})


function renderReferenceTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpReferenceTBody").empty();

        dataList.forEach(v => {
            const refNameCell = `<td>${v.refName}</td>`;
            const occupationCell = `<td>${v.occupation}</td>`;
            const mobileCell = `<td>${v.mobile}</td>`;
            const relationCell = `<td>${v.relation}</td>`;
            /*const actionCell = `<td><a href='${API}EmpEducation/Details/${v.id}' title='View'><i class="fa fa-search"></i></a></td>`;*/
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-reference" data-id='${v.id}'></i></td>`;
            const row = `<tr>${refNameCell}${occupationCell}${mobileCell}${relationCell}${actionCell}</tr>`;

            $("#EmpReferenceTBody").append(row);
        })
    } else {
        $("#EmpReferenceTBody").empty();
        addNoDataFoundFooter("#EmpReferenceTBody")
    }
}

function getNomineModel() {
    const empId = $("#EmployeeId2").val();
    const isAjaxPost = $("#IsAjaxPostRef2").val();
    const refName = $("#RefName2").val();
    const occupation = $("#Occupation2").val();
    const dobStr = $("#DobStr2").val();
    const mobile = $("#Mobile2").val();
    const relation = $("#Relation2").val();
    const phone = $("#Phone2").val();
    const address = $("#Address2").val();
    const refType = $("#RefType2").val();
    const ownerShip = $("#OwnerShip2").val();

    var model = {
        refName: refName,
        refType: refType,
        occupation: occupation,
        dobStr: dobStr,
        address: address,
        mobile: mobile,
        phone: phone,
        relation: relation,
        isAjaxPost: isAjaxPost,
        employeeId: empId,
        ownerShip: ownerShip
    }

    return model;
}

function getEmployeeNomine() {
    const empId = $("#Id").val();

    const url = API + "EmpReference/GetReferenceByEmpId?empId=" + empId + "&refType=2";

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("nomine", rData);
            renderNomineTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-nominee", function () {
    const nomId = $(this).attr('data-id');

    if (nomId > 0) {
        swal({
            title: "Delete Nominee",
            text: "Do you want to delete nominee info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpReference/Delete/" + nomId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Nominee Info Deleted");
                            getEmployeeNomine();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Nominee Info Remain Safe")
                }
            })
    }
})


function renderNomineTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpNomineTBody").empty();

        dataList.forEach(v => {
            const refNameCell = `<td>${v.refName}</td>`;
            const occupationCell = `<td>${v.occupation}</td>`;
            const mobileCell = `<td>${v.mobile}</td>`;
            const relationCell = `<td>${v.relation}</td>`;
            /*const actionCell = `<td><a href='${API}EmpEducation/Details/${v.id}' title='View'><i class="fa fa-search"></i></a></td>`;*/
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-nominee" data-id='${v.id}'></i></td>`;
            const row = `<tr>${refNameCell}${occupationCell}${mobileCell}${relationCell}${actionCell}</tr>`;

            $("#EmpNomineTBody").append(row);
        })
    } else {
        $("#EmpNomineTBody").empty();
        addNoDataFoundFooter("#EmpNomineTBody")
    }
}

function getEmergencyModel() {
    const empId = $("#EmployeeId3").val();
    const isAjaxPost = $("#IsAjaxPostRef3").val();
    const refName = $("#RefName3").val();
    const occupation = $("#Occupation3").val();
    const mobile = $("#Mobile3").val();
    const relation = $("#Relation3").val();
    const phone = $("#Phone3").val();
    const address = $("#Address3").val();
    const refType = $("#RefType3").val();

    var model = {
        refName: refName,
        refType: refType,
        occupation: occupation,
        address: address,
        mobile: mobile,
        phone: phone,
        relation: relation,
        isAjaxPost: isAjaxPost,
        employeeId: empId
    }

    return model;
}


function getEmployeeEmergency() {
    const empId = $("#Id").val();

    const url = API + "EmpReference/GetReferenceByEmpId?empId=" + empId + "&refType=3";

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("emergency", rData);
            renderEmergencyTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-emergency", function () {
    const emcId = $(this).attr('data-id');

    if (emcId > 0) {
        swal({
            title: "Delete Emergency",
            text: "Do you want to delete emergency info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpReference/Delete/" + emcId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Emergency Contact Info Deleted");
                            getEmployeeEmergency();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Emergency Info Remain Safe")
                }
            })
    }
})

function renderEmergencyTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpEmergencyTBody").empty();

        dataList.forEach(v => {
            const refNameCell = `<td>${v.refName}</td>`;
            const occupationCell = `<td>${v.occupation}</td>`;
            const mobileCell = `<td>${v.mobile}</td>`;
            const relationCell = `<td>${v.relation}</td>`;
            /*const actionCell = `<td><a href='${API}EmpEducation/Details/${v.id}' title='View'><i class="fa fa-search"></i></a></td>`;*/
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-emergency" data-id='${v.id}'></i></td>`;
            const row = `<tr>${refNameCell}${occupationCell}${mobileCell}${relationCell}${actionCell}</tr>`;

            $("#EmpEmergencyTBody").append(row);
        })
    } else {
        $("#EmpEmergencyTBody").empty();
        addNoDataFoundFooter("#EmpEmergencyTBody")
    }
}

//#endregion

//#region Training Section

$(document.body).on("click", "#EmpTrainingSave", function () {

    const empId = $("#EmployeeId").val();
    const isAjaxPost = $("#IsAjaxTrainingPosting").val();
    const courseTitle = $("#CourseTitle").val();
    const instituteName = $("#InstituteName").val();
    const location = $("#Location").val();
    const result = $("#TrainingResult").val();
    const dateFromStr = $("#TrainingDateFromStr").val();
    const dateToStr = $("#TrainingDateToStr").val();
    const remarks = $("#TrainingRemarks").val();
    const trainingFile = $("#TrainingFile").prop("files");

    var model = {
        courseTitle: courseTitle,
        instituteName: instituteName,
        location: location,
        dateFromStr: dateFromStr,
        dateToStr: dateToStr,
        result: result,
        remarks: remarks,
        isAjaxPost: isAjaxPost,
        employeeId: empId
    }

    var formData = getFormData(model);
    formData.append("trainingFile", trainingFile[0]);

    console.log(formData);

    if (model.employeeId > 0) {
        $.ajax({
            url: API + "EmpTraining/Create",
            type: 'POST',
            data: formData,
            processData: false,  // tell jQuery not to process the data
            contentType: false,  // tell jQuery not to set contentType
            success: function (result) {
                console.log(result);
                $('#trainingModal').modal('hide');
                getEmployeeTraining();

                $('#trainingModal form')[0].reset();
                $("#CourseTitle").val('').trigger(update);
                $("#InstituteName").val('').trigger(update);
                $("#Location").val('').trigger(update);
                $("#TrainingResult").val('').trigger(update);
                $("#TrainingDateFromStr").val('').trigger(update);
                $("#TrainingDateToStr").val('').trigger(update);
                $("#TrainingRemarks").val('').trigger(update);
            },
            error: function (jqXHR) {
            },
            complete: function (jqXHR, status) {
            }
        });
    }
});

function getEmployeeTraining() {

    const empId = $("#Id").val();

    const url = API + "EmpTraining/GetTrainingByEmpId?empId=" + empId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("training", rData);
            renderTrainingTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-training", function () {
    const trnId = $(this).attr('data-id');

    if (trnId > 0) {
        swal({
            title: "Delete Training",
            text: "Do you want to delete training info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpTraining/Delete/" + trnId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Training Info Deleted");
                            getEmployeeTraining();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Training Info Remain Safe")
                }
            })
    }
})

function renderTrainingTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpTrainingTBody").empty();

        dataList.forEach(v => {
            const courseCell = `<td>${v.courseTitle}</td>`;
            const instituteCell = `<td>${v.instituteName}</td>`;
            const locationCell = `<td>${v.location}</td>`;
            const startDateCell = `<td>${v.dateFromStr}</td>`;
            const endDateCell = `<td>${v.dateToStr}</td>`;
            const resultCell = `<td>${v.result}</td>`;
            const trainingFileCell = `<td class='text-center' style='font-size:x-large;'><a href='${v.trainingFileUrl}' title='Training Certificate PDF' target='_blank'><i class="fa fa-file-pdf-o"></i></a></td>`;
            /*const actionCell = `<td><a href='${API}EmpTraining/Details/${v.id}' title='View'><i class="fa fa-search"></i></a></td>`;*/
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-training" data-id='${v.id}'></i></td>`;
            const row = `<tr>${courseCell}${instituteCell}${locationCell}${startDateCell}${endDateCell}${resultCell}${trainingFileCell}${actionCell}</tr>`;

            $("#EmpTrainingTBody").append(row);
        })
    } else {
        $("#EmpTrainingTBody").empty();
        addNoDataFoundFooter("#EmpTrainingTBody")
    }
}

//#endregion

//#region Posting Section

$(document.body).on("click", "#EmpPostingSave", function () {

    const empId = $("#EmployeeId").val();
    const isAjaxPost = $("#IsAjaxPostPosting").val();
    const preDepartmentId = $("#DepartmentId").val();
    const preDesignationId = $("#DesignationId").val();
    const departmentId = $("#PostDepartmentId").val();
    const designationId = $("#PostDesignationId").val();
    const preNetSalary = $("#PreNetSalary").val();
    const netSalary = $("#NetSalary").val();
    const postDateFromStr = $("#PostDateFromStr").val();
    const postingType = $("#PostingType").val();
    const postRemarks = $("#PostRemarks").val();
    const postingFile = $("#PostingFile").prop("files");

    var model = {
        dateFromStr: postDateFromStr,
        postingType: postingType,
        netSalary: netSalary,
        preNetSalary: preNetSalary,
        remarks: postRemarks,
        isAjaxPost: isAjaxPost,
        employeeId: empId,
        departmentId: departmentId,
        designationId: designationId,
        preDepartmentId: preDepartmentId,
        PreDesignationId: preDesignationId
    }

    var formData = getFormData(model);
    formData.append("postingFile", postingFile[0]);

    $.ajax({
        url: API + "EmpPosting/Create",
        type: 'POST',
        data: formData,
        processData: false,  // tell jQuery not to process the data
        contentType: false,  // tell jQuery not to set contentType
        success: function (result) {
            console.log(result);
            $('#postingModal').modal('hide');
            getEmployeePosting();

            $('#postingModal form')[0].reset();
            $("#PostDepartmentId").val('').trigger(update);
            $("#PostDesignationId").val('').trigger(update);
            $("#PostingType").val('').trigger(update);
        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
        }
    });


});


function getEmployeePosting() {
    const empId = $("#Id").val();

    const url = API + "EmpPosting/GetPostingByEmpId?empId=" + empId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            console.log("posting", rData);
            renderPostingTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-posting", function () {
    const pstId = $(this).attr('data-id');

    if (pstId > 0) {
        swal({
            title: "Delete Posting",
            text: "Do you want to delete posting info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpPosting/Delete/" + pstId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Posting Info Deleted");
                            getEmployeePosting();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Posting Info Remain Safe")
                }
            })
    }
})

function renderPostingTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpPostingTBody").empty();

        dataList.forEach(v => {
            const departmentCell = `<td>${v.preDepartmentName}</td>`;
            const designationCell = `<td>${v.preDesignationName}</td>`;
            const preNetSalaryCell = `<td>${v.preNetSalary}</td>`;
            const preDepartmentCell = `<td>${v.departmentName}</td>`;
            const preDesignationCell = `<td>${v.designationName}</td>`;
            const netSalaryCell = `<td>${v.netSalary}</td>`;
            const postingTypeCell = `<td>${v.postingTypeText}</td>`;
            const postingFileCell = `<td class='text-center' style='font-size:x-large;'><a href='${v.postingDoc}' title='Posting Certificate PDF' target='_blank'><i class="fa fa-file-pdf-o"></i></a></td>`;
            /*const actionCell = `<td><a href='${API}EmpPosting/Details/${v.id}' title='View'><i class="fa fa-search"></i></a></td>`;*/
            const actionCell = `<td class='text-center'><i class="fa fa-times fa-2x rmv-posting" data-id='${v.id}'></i></td>`;
            const row = `<tr>${departmentCell}${designationCell}${preNetSalaryCell}${preDepartmentCell}${preDesignationCell}${netSalaryCell}${postingTypeCell}${postingFileCell}${actionCell}</tr>`;

            $("#EmpPostingTBody").append(row);
        })
    } else {
        $("#EmpPostingTBody").empty();
        addNoDataFoundFooter("#EmpPostingTBody")
    }
}

//#endregion

//#region DisciplineAction Section

$(document.body).on("click", "#EmpActionSave", function () {

    const empId = $("#EmployeeId").val();
    const isAjaxPost = $("#IsAjaxPostAction").val();
    const actionType = $("#ActionType").val();
    const actionReason = $("#ActionReason").val();
    const actionDesc = $("#ActionDesc").val();
    const committee = $("#Committee").val();
    const actionFile = $("#ActionFile").prop("files");

    var model = {
        actionType: actionType,
        actionReason: actionReason,
        actionDesc: actionDesc,
        committee: committee,
        isAjaxPost: isAjaxPost,
        employeeId: empId
    }

    var formData = getFormData(model);
    formData.append("actionFile", actionFile[0]);

    $.ajax({
        url: API + "EmpDisciplinary/Create",
        type: 'POST',
        data: formData,
        processData: false,  // tell jQuery not to process the data
        contentType: false,  // tell jQuery not to set contentType
        success: function (result) {
            console.log(result);
            $('#disciplineModal').modal('hide');
            getEmployeeAction();

            $('#disciplineModal form')[0].reset();
            $("#ActionType").val('').trigger(update);
            $("#ActionReason").val('').trigger(update);
            $("#ActionDesc").val('').trigger(update);
            $("#Committee").val('').trigger(update);
        },
        error: function (jqXHR) {
        },
        complete: function (jqXHR, status) {
        }
    });


});


function getEmployeeAction() {
    const empId = $("#Id").val();

    const url = API + "EmpDisciplinary/GetEmpDisciplinaryByEmpId?empId=" + empId;

    $.get(url, function (rData) {
        if (rData !== undefined) {
            renderActionTableBody(rData);
        }
    });
}

$(document.body).on("click", ".rmv-disciplinary", function () {
    const dscId = $(this).attr('data-id');

    if (dscId > 0) {
        swal({
            title: "Delete Disciplinary",
            text: "Do you want to delete disciplinary info?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((result) => {
                if (result) {
                    const url = API + "EmpDisciplinary/Delete/" + dscId;
                    $.get(url, function (rData) {
                        if (rData == true) {
                            successMsg("Employee Disciplinary Action Info Deleted");
                            getEmployeeAction();
                        } else {
                            failedMsg("failed to delete");
                        }
                    });
                } else {
                    swal("Disciplinary Info Remain Safe")
                }
            })
    }
})

function renderActionTableBody(dataList) {
    if (dataList.length > 0) {

        $("#EmpActionTBody").empty();

        dataList.forEach(v => {
            const typeCell = `<td>${v.actionTypeText}</td>`;
            const reasonCell = `<td>${v.actionReason}</td>`;
            const commiteeCell = `<td>${v.committee}</td>`;
            const actionFileCell = `<td class='text-center' style='font-size:x-large;'><a href='${v.actionFileUrl}' title='Action Certificate PDF' target='_blank'><i class="fa fa-file-pdf-o"></i></a></td>`;
            /*const actionCell = `<td><a href='${API}EmpDisciplinary/Details/${v.id}' title='View'><i class="fa fa-search"></i></a></td>`;*/
            const actionCell = `<td class='text-center'><i class="fa fa-times rmv-disciplinary fa-2x" data-id='${v.id}'></i></td>`;
            const row = `<tr>${typeCell}${reasonCell}${commiteeCell}${actionFileCell}${actionCell}</tr>`;

            $("#EmpActionTBody").append(row);
        })
    } else {
        $("#EmpActionTBody").empty();
        addNoDataFoundFooter("#EmpActionTBody")
    }
}

//#endregion

function LeaveEmployee() {
    const EmployeeId = $("#Id").val();
    const SelectYear = new Date().getFullYear();

    const url = `${API}EmpLeaveApplication/EmpLeaveStatementPrint?employeeId=${EmployeeId}&selectYear=${SelectYear}`;
    window.open(url, "_blank");
}

