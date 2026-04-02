
class PartialViewManager {

    getAnyPartialView(url, params, targetEl, callBackF) {
        if (!hasAnyError(url) && !hasAnyError(params) && !hasAnyError(targetEl)) {
            startUiBlock();
            if (!hasAnyError(params)) {
                $.post(url, params, function (rData) {
                    stopUiBlock();
                    if (!hasAnyError(rData)) {
                        $(targetEl).html(rData);
                        callNextFunction(callBackF);
                    }
                });
            } else if (!hasAnyError(url) && !hasAnyError(targetEl)) {
                $.post(url, function (rData) {
                    stopUiBlock();
                    if (!hasAnyError(rData)) {
                        $(targetEl).html(rData);
                        callNextFunction(callBackF);
                    }
                });
            }

        }
    }


    //getDesignationDetailPartial(id, targetEl, callBackF) {

    //    const url = API + "Designation/GetDetailPartial";

    //    if (id > 0) {
    //        const params = { id: id };
    //        this.getAnyPartialView(url, params, targetEl, callBackF);
    //    }
    //}

    //getTransactionDetailPartial(id, targetEl, callBackF) {
    //    const url = API + "Transaction/GetDetailPartial";
    //    if (id > 0) {
    //        const params = { id: id };
    //        this.getAnyPartialView(url, params, targetEl, callBackF);
    //    }
    //}

    getRequsitionInfoDetailPartial(id, targetEl, callBackF) {
        const url = API + "RequsitionInfo/GetDetailPartial";
        if (id > 0) {
            const params = { id: id };
            this.getAnyPartialView(url, params, targetEl, callBackF);
        }
    }

    //getCsMstDetailPartial(id, targetEl, callBackF) {
    //    const url = API + "CsMst/GetDetailPartial";
    //    if (id > 0) {
    //        const params = { id: id };
    //        this.getAnyPartialView(url, params, targetEl, callBackF);
    //    }
    //}

    //getOrderDetailPartial(id, targetEl, callBackF) {
    //    const url = API + "Order/GetDetailPartial";
    //    if (id > 0) {
    //        const params = { id: id };
    //        this.getAnyPartialView(url, params, targetEl, callBackF);
    //    }
    //}

    //getSupplierDetailPartial(id, targetEl, callBackF) {
    //    const url = API + "Supplier/GetDetailPartial";
    //    if (id > 0) {
    //        const params = { id: id };
    //        this.getAnyPartialView(url, params, targetEl, callBackF);
    //    }
    //}

}
