$(document.body).on("change", "#IssueDeptId", function () {
    const dptId = $(this).val();
    if (dptId > 0) {
        _dropdownManager.getRequsitionByDptId(dptId, "#ReqMstId", null, null);
    }
});

$(document.body).on("change", "#IssueEmpId", function () {
    const empId = $(this).val();
    if (empId > 0) {
        _dropdownManager.getRequsitionByEmpId(empId, "#ReqMstId", null, null);
    }
});

$(document.body).on("change", "#ReqMstId", function () {
    const reqId = $(this).val();
    if (reqId > 0) {
        const url = API + "RequsitionInfo/GetIssueItemsByReqId?reqId=" + reqId;
        $.get(url, function (rData) {
            if (rData != undefined) {
                if (rData.length > 0) {

                    issueDetailArray = [];

                    console.log("issue-items", rData);

                    rData.forEach(v => {

                        const model = {
                            itemId: v.itemId,
                            itemName: v.itemName,
                            itemUnitId: v.itemUnitId,
                            itemUnitName: v.itemUnitName,
                            issueQty: v.issueQty,
                            approveQty: v.approveQty,
                            stock: v.stock,
                            alreadyIssuedQty: v.alreadyIssuedQty,
                            unitPrice: v.unitPrice
                        }
                        console.log(model)

                        issueDetailArray.push(model);
                    });

                    createDetailTable();
                }
            }
        });
    }
});

$(document.body).on("change", ".IssueQty", function () {
    const index = $(this).attr("data-index");
    const value = $(`#IssueQty_${index}`).val();

    if (index > -1) {

        var aprQty = issueDetailArray[index].approveQty;
        var stock = issueDetailArray[index].stock;

        if (value <= aprQty && value <= stock) {
            $(`#IssueQty_Msg_${index}`).html('');

            issueDetailArray[index].issueQty = value;
        } else {
            $(`#IssueQty_Msg_${index}`).html('Approve Qty Or Stock Exeeted..!');
        }
    }
})

$(document.body).on("click", "#CreateIssueSubmitBtn", function () {
    var departmentId = $("#IssueDeptId").val();

    if (!(departmentId > 0)) {
        return failedMsg("Department is mandatory for issue items...!");
    }

    if (issueDetailArray.length > 0) {

        let isValid = true;
        let msg = "";

        issueDetailArray.forEach(v => {
            if (!(v.issueQty > 0)) {
                isValid = false;
                msg = "Issue Quantity Have To More Than Zero";
            }

            if (v.issueQty > v.stock) {
                isValid = false;
                msg = "Issue Quantity Have To Less Than Stock";
            }
        });

        if (isValid) {
            startFormPosting("#CreateIssueForm");
        } else {
            failedMsg(msg)
        }
    } else {
        failedMsg("You did not add any items to issue !!");
    }
});


function createDetailTable() {

    $("#IssueItemTableTbody").empty();

    if (issueDetailArray.length > 0) {

        issueDetailArray.forEach((modelObject, index) => {
            const slNo = `<td>${index + 1}</td>`;
            //-- not necessary but added for db validation
            const hiddenRcvItemUnitId = `<input type='hidden' name='TranDtls[${index}].RcvItemUnitId' value='${modelObject.itemUnitId}' />`;

            const itemCell = `<td><input type='hidden' name='TranDtls[${index}].ItemId' value='${modelObject.itemId}' />${modelObject.itemName}</td>`;
            const unitCell = `<td>${hiddenRcvItemUnitId}<input type='hidden' name='TranDtls[${index}].ItemUnitId' value='${modelObject.itemUnitId}' />${modelObject.itemUnitName}</td>`;
            const aprQtyCell = `<td>${modelObject.approveQty}</td>`;
            const alreadyIssueQtyCell = `<td>${modelObject.alreadyIssuedQty}</td>`;

            let itemIssueQty = modelObject.alreadyIssuedQty < modelObject.approveQty ? (modelObject.approveQty - modelObject.alreadyIssuedQty) : 0;
            if (modelObject.stock < itemIssueQty) {
                itemIssueQty = modelObject.stock;
            }

            issueDetailArray[index].issueQty = itemIssueQty;

            const issueQtyCell = `<td>  
                                    <input type='number' id='IssueQty_${index}' data-index='${index}' name='TranDtls[${index}].ItemQty' value='${itemIssueQty}' class='form-control text-center IssueQty'/>
                                    <span id='IssueQty_Msg_${index}'></span>
                                  </td>`;
            const stockCell = `<td>
                                <input type='hidden' name='TranDtls[${index}].UnitPrice' value='${modelObject.unitPrice}' />
                                <input type='number' name='TranDtls[${index}].Stock' value='${modelObject.stock}' class='form-control text-center' readonly/>
                            </td>`;
            const actionCell = `<td><button type='button' class='btn btn-icon btn-outline-danger' onclick='deleteItemRow(${index})'><i class='fa fa-trash'></i></button></td>`;

            const row = "<tr id=item" + index + ">" + slNo + itemCell + unitCell + aprQtyCell + alreadyIssueQtyCell + issueQtyCell + stockCell + actionCell + "</tr>";
            $("#IssueItemTableTbody").append(row)
        });
    } else {
        addNoDataFoundFooterWithMsg("#IssueItemTableTbody", "No Item Added For Issue !");
    }
}

function deleteItemRow(index) {
    if (index > - 1) {
        issueDetailArray.splice(index, 1);
    }
    createDetailTable();
}