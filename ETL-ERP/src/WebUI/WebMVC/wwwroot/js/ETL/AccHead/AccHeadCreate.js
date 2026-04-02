$(document).ready(function () {
    $(".form-control").attr("autocomplete", "off");
});

$(document.body).on("change", "#GroupId", function () {
    const groupId = $(this).val();

    if (groupId > 0) {
        _dropdownManager.getHeadByGroupSelectListItems(groupId, "#ParentHeadId", null, null);
    };
})