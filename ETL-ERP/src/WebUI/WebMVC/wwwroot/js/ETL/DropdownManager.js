class DropdownManager {

    getSemesterByCourseSelectListItems(courseId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "Semester/GetSemesterJsonData";
            const params = { courseId: courseId };
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getBatchByCourseSessionSelectListItems(courseId, sessionId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "Batch/GetBatchJsonData";
            const params = { courseId: courseId, sessionId: sessionId };
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }


    getBatchsByCourseId(courseId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "Batch/GetBatchsByCourseId";
            const params = { courseId: courseId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getStudentsBySemesterId(semesterId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "StudentInfo/GetStudentBySemesterId";
            const params = { semesterId: semesterId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getStudentsByBatchId(batchId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "StudentInfo/GetStudentByBatchId";
            const params = { batchId: batchId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getForigenStudentsByBatchId(batchId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "StudentInfo/GetForigenStudentByBatchId";
            const params = { batchId: batchId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getSemesterByBatchCourseId(batchId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "Semester/GetSemesterByBatchId";
            const params = { batchId: batchId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getAcademicExamBySemesterId(semesterId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "Semester/GetExamBySemesterJsonData";
            const params = { semesterId: semesterId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getUnitBySemesterId(semesterId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "Unit/GetUnitBySemesterId";
            const params = { semesterId: semesterId }
            const rData = createSelectList(url, params, targetEl, selectedData, callBackF);
            console.log(rData);
        } else {
            clearDropDown(targetEl);
        }
    }

    getTeacherByCourseId(courseId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "CourseTeacherMap/GetTeacherByCourseId";
            const params = { courseId: courseId }
            const rData = createSelectList(url, params, targetEl, selectedData, callBackF);
            console.log(rData);
        } else {
            clearDropDown(targetEl);
        }
    }

    getEmployeeByDepartmentId(departmentId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "LeaveAppReviewer/GetEmployeeByDepartmentId";
            const params = { departmentId: departmentId }
            const rData = createSelectList(url, params, targetEl, selectedData, callBackF);
            console.log(rData);
        } else {
            clearDropDown(targetEl);
        }
    }

    getHeadByGroupSelectListItems(groupId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "AccHead/GetHeadJsonData";
            const params = { groupId: groupId };
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getLeaveTypeByEmployeGenderSelectListItems(employeeId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "EmpLeaveApplication/GetLeaveByGenderJsonData";
            const params = { employeeId: employeeId };
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getSubjectByBatchId(batchId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "SubjectClassSetup/GetSubjectByBatchJsonData";
            const params = { batchId: batchId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getDepartmentByDesignationId(designationId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "Designation/GetDeptByDesignationId";
            const params = { desId: designationId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getRequsitionByDptId(deptId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "RequsitionInfo/GetRequisitionByDptId";
            const params = { dptId: deptId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getRequsitionByEmpId(empId, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "RequsitionInfo/GetRequisitionByEmpId";
            const params = { empId: empId }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getRoomByCategoryId(categoryId, startDate, endDate, targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "RoomInfo/GetRoomByCategoryId";
            const params = { categoryId: categoryId, checkInDate: startDate, checkOutDate: endDate }
            createSelectList(url, params, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }

    getItemSelectListItems(targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "ItemInfo/GetItemJsonData";
            createSelectList(url, null, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }
    getFoodItemSelectListItems(targetEl, selectedData, callBackF) {
        if (!hasAnyError(targetEl)) {
            const url = API + "FoodItem/GetFoodItemJsonData";
            createSelectList(url, null, targetEl, selectedData, callBackF);
        } else {
            clearDropDown(targetEl);
        }
    }
}