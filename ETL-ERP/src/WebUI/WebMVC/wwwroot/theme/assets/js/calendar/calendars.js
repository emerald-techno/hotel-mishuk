'use strict';

/* eslint-disable require-jsdoc, no-unused-vars */

var CalendarList = [];

var scheduleType = {
    class: 1,
    exam: 2,
    assignment: 3,
    classTest: 4
}

function CalendarInfo() {
    this.id = null;
    this.name = null;
    this.checked = true;
    this.color = null;
    this.bgColor = null;
    this.borderColor = null;
    this.dragBgColor = null;
    this.type = null;
}

function addCalendar(calendar) {
    CalendarList.push(calendar);
}

function findCalendar(id) {
    var found;

    CalendarList.forEach(function(calendar) {
        if (calendar.id === id) {
            found = calendar;
        }
    });

    return found || CalendarList[0];
}

function hexToRGBA(hex) {
    var radix = 16;
    var r = parseInt(hex.slice(1, 3), radix),
        g = parseInt(hex.slice(3, 5), radix),
        b = parseInt(hex.slice(5, 7), radix),
        a = parseInt(hex.slice(7, 9), radix) / 255 || 1;
    var rgba = 'rgba(' + r + ', ' + g + ', ' + b + ', ' + a + ')';

    return rgba;
}

(function() {
    var calendar;
    var id = 0;

    calendar = new CalendarInfo();
    id += 1;
    calendar.id = String(id);
    calendar.name = 'My Class';
    calendar.color = '#ffffff';
    calendar.bgColor = '#24695c';
    calendar.dragBgColor = '#24695c';
    calendar.borderColor = '#24695c';
    calendar.type = scheduleType.class;
    addCalendar(calendar);

    calendar = new CalendarInfo();
    id += 1;
    calendar.id = String(id);
    calendar.name = 'My Exam';
    calendar.color = '#ffffff';
    calendar.bgColor = '#ba895d';
    calendar.dragBgColor = '#ba895d';
    calendar.borderColor = '#ba895d';
    calendar.type = scheduleType.exam;
    addCalendar(calendar);

    calendar = new CalendarInfo();
    id += 1;
    calendar.id = String(id);
    calendar.name = 'Assignment';
    calendar.color = '#ffffff';
    calendar.bgColor = '#ff5583';
    calendar.dragBgColor = '#ff5583';
    calendar.borderColor = '#ff5583';
    calendar.type = scheduleType.assignment;
    addCalendar(calendar);

    calendar = new CalendarInfo();
    id += 1;
    calendar.id = String(id);
    calendar.name = 'Class Test';
    calendar.color = '#ffffff';
    calendar.bgColor = '#03bd9e';
    calendar.dragBgColor = '#03bd9e';
    calendar.borderColor = '#03bd9e';
    calendar.type = scheduleType.classTest;
    addCalendar(calendar);

    //calendar = new CalendarInfo();
    //id += 1;
    //calendar.id = String(id);
    //calendar.name = 'Travel';
    //calendar.color = '#ffffff';
    //calendar.bgColor = '#1b4c43';
    //calendar.dragBgColor = '#1b4c43';
    //calendar.borderColor = '#1b4c43';
    //addCalendar(calendar);

    //calendar = new CalendarInfo();
    //id += 1;
    //calendar.id = String(id);
    //calendar.name = 'etc';
    //calendar.color = '#ffffff';
    //calendar.bgColor = '#9d9d9d';
    //calendar.dragBgColor = '#9d9d9d';
    //calendar.borderColor = '#9d9d9d';
    //addCalendar(calendar);

    //calendar = new CalendarInfo();
    //id += 1;
    //calendar.id = String(id);
    //calendar.name = 'Birthdays';
    //calendar.color = '#ffffff';
    //calendar.bgColor = '#e2c636';
    //calendar.dragBgColor = '#e2c636';
    //calendar.borderColor = '#e2c636';
    //addCalendar(calendar);

    calendar = new CalendarInfo();
    id += 1;
    calendar.id = String(id);
    calendar.name = 'National Holidays';
    calendar.color = '#ffffff';
    calendar.bgColor = '#d22d3d';
    calendar.dragBgColor = '#d22d3d';
    calendar.borderColor = '#d22d3d';
    calendar.type = 0;
    addCalendar(calendar);
})();