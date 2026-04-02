
let model = {}
let empSalaries = [];
let empGroosSalary = 0;
let globalAddValue = 0;

$(document).ready(function () {
})

$(document.body).on("change", "#employeeId", function () {
    const employeeId = $(this).val();

    if (employeeId > 0) {
        const url = `${API}Employee/GetEmployeeById/${employeeId}`;

        $.get(url, function (data) {
            if (data != null || data != undefined) {

                empGroosSalary = data.salary;
                const html = `<label class='form-label'>Groos Salary: </label> <input readonly value="${data.salary}" class="form-control" />`;

                $("#EmpGSalary").html(html);
            }
        }).fail(function () {
            alert("error");
        });

    }
})

$(".checkBoxClass").change(function () {
    if (this.checked) {
        const index = $(this).attr("data-index");

        const salaryPartId = $("#partId_" + index).val();
        const salaryPartCode = $("#partCode_" + index).val();
        const employeeId = $("#employeeId").val();
        const partType = $("#partType_" + index).val();
        const valueType = $("#valueType_" + index).val();
        const salaryValue = $("#salaryValue_" + index).val();
        const salaryRemarks = $("#salaryRemarks_" + index).val();

        model = {
            salaryPartId: salaryPartId,
            employeeId: employeeId,
            partType: partType,
            valueType: valueType,
            value: salaryValue,
            remarks: salaryRemarks,

            //--
            partCode: salaryPartCode
        }

        if (model.employeeId > 0) {
            empSalaries.push(model);
        } else {
            failedMsg("Add Employee First");
            this.checked = false;
        }


    } else {

        const index2 = $(this).attr("data-index");

        const salaryPartId2 = $("#partId_" + index2).val();

        const arrayIndex = empSalaries.findIndex(c => c.salaryPartId === salaryPartId2);

        if (arrayIndex > -1) {
            empSalaries.splice(arrayIndex, 1);
        }
    }
});


$(document.body).on("click", "#salaryButton", function () {
    if (hasDataInArray(empSalaries)) {

        const url = API + "PrEmpSalaryPart/Create";
        const employeeSalaries = { PrEmpSalaryParts: empSalaries };
        const params = {
            vm: employeeSalaries
        }

        $.post(url, params, (rData) => {
            if (!hasAnyError(rData)) {
                successMsg();
                setTimeout(() => {
                    window.location.href = API + "PrEmpSalaryPart/Create"
                }, 3000);
            } else {
                failedMsg();
            }
        });

    } else {
        failedMsg("Sorry! Salary Part can not Assigned to employees !");
    }
})


$(".salaryValueclass").change(function () {
    const index = $(this).attr("data-index");

    const salaryPartId = $("#partId_" + index).val();
    const salValue = $("#salaryValue_" + index).val();

    const arrayIndex = empSalaries.findIndex(c => c.salaryPartId === salaryPartId);

    if (arrayIndex > -1) {
        empSalaries[arrayIndex].value = salValue;
    }
})

$(".salaryTypeClass").change(function () {
    const index = $(this).attr("data-index");

    const salaryPartId = $("#partId_" + index).val();
    const salValueType = $("#valueType_" + index).val();

    const arrayIndex = empSalaries.findIndex(c => c.salaryPartId === salaryPartId);

    if (arrayIndex > -1) {
        empSalaries[arrayIndex].valueType = salValueType;
    }

})

//function calculateSalaryParts() {
//    let additionValue = 0;
//    let deductionValue = 0;

//    if (hasDataInArray(empSalaries)) {
//        const additionParts = empSalaries.filter(c => c.partType == "A");
//        const deductionParts = empSalaries.filter(c => c.partType == "D");

//        let basicAmount = 0;

//        if (additionParts.length > 0) {
//            additionValue = 0;

//            additionParts.forEach(v => {
//                if (v.partCode == "BASIC") {
//                    basicAmount = v.valueType == "P" ? percentCalculation(v.value, empGroosSalary) : parseFloat(v.value);
//                    additionValue += parseFloat(basicAmount);
//                } else {
//                    additionValue += v.valueType == "P" ? percentCalculation(v.value, basicAmount) : parseFloat(v.value);
//                }
//            })
//        }

//        if (deductionParts.length > 0) {
//            deductionValue = 0;

//            deductionParts.forEach(v => {
//                deductionValue += v.valueType == "P" ? percentCalculation(v.value, basicAmount) : parseFloat(v.value);

//            })
//        }
//    }

//    const html = `<p>Addition Part Total: ${additionValue}</p><p>Diduction Part Total: ${deductionValue}</P>`;

//    $("#totalValue").html(html);
//    globalAddValue = additionValue;

//}


function percentCalculation(percentValue, totalNumber) {
    const result = (percentValue * totalNumber) / 100;
    return result;
}


//$(document.body).on("change", ".salaryTypeClass", function () {
//    const courseId = $(this).val();
//    if (courseId > 0) {
//        _dropdownManager.getSemesterByCourseSelectListItems(courseId, "#SemesterId", null, null);

//        getSubjectByCourse();
//    }
//})