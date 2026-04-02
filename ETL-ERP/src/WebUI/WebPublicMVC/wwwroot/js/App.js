
/*var _partialViewManager;*/
var _dropdownManager;

$(document).ready(function () {

/*    _partialViewManager = new PartialViewManager();*/
    _dropdownManager = new DropdownManager();

});


const UserEnum = {
    General: 1,
    Teacher: 2,
    Student: 3
}

const ScheduleType = {
    Class: 1,
    Exam: 2,
    Assignment: 3,
    ClassTest: 4
}