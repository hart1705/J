var Helper = {
    BootstrapModal_OnSuccess: function (ctl) {
        var targetModal = $(ctl).attr('data-target');
        $(targetModal).modal().show();
    },

    ShowNotification: function (type, title, message, timeout, target) {
        var options = {};
        if (type != null && type != undefined) {
            if (timeout != undefined && timeout != null) {
                toastr.options = {
                    "timeOut": timeout
                }
            }
            else {
                toastr.options = {
                    "timeOut": 5000
                }
            }
            toastr.options["target"] = target;
            toastr[type](message, title);
        }
    },

    BindToDataTable: function (tableId, dataTableOptions) {
        var table = null;
        if (!$.fn.DataTable.isDataTable(tableId)) {
            if (dataTableOptions != null && dataTableOptions != undefined) {
                table = $(tableId).DataTable(dataTableOptions);
            }
            else {
                table = $(tableId).DataTable();
            }
        }
        return table;
    },

    BindToDataTableSelectableRow: function (tableId, dataTableOptions, baseURL) {
        var tableRowBody = $(tableId + ' tbody tr');
        var id = "";

        var table = Helper.BindToDataTable(tableId, dataTableOptions);

        tableRowBody.click(function (event) {
            tableRowBody.removeClass("selected");
            if (Helper.HasAttr($(this), 'id')) {
                $(this).addClass("selected");
                id = $(this).attr('id');
            }
        });

        if (baseURL != null) {
            tableRowBody.on('doubletap', function () {
                window.location = baseURL + id;
            }).on('dblclick', function () {
                window.location = baseURL + id;
            });
        }
        return table;
    },

    HasAttr: function (control, attr) {
        var attr = $(control).attr(attr);
        // For some browsers, `attr` is undefined; for others, `attr` is false. Check for both.
        if (typeof attr !== typeof undefined && attr !== false) {
            return true;
        }
        return false;
    },

    AjaxGetRequest: function (reqUrl, onSuccessCallBack, onFailureCallBack) {
        $.ajax({
            url: reqUrl,
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',
            success: function (data) {
                if (onSuccessCallBack && typeof (onSuccessCallBack) === "function") {
                    onSuccessCallBack(data);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
                if (onFailureCallBack && typeof (onFailureCallBack) === "function") {
                    onFailureCallBack();
                }
            },
            complete: function () {
            }
        })
    },

    AjaxFormSubmit: function (src, evt, targetId, loadingAnimationId, onBeginCallBack, onSuccessCallBack, onFailureCallBack, onCompleteCallBack) {
        evt.preventDefault();
        evt.stopImmediatePropagation();
        var formData = new FormData($(src)[0]);
        var hasTarget = !(targetId == undefined || targetId == null);
        if (loadingAnimationId == undefined || loadingAnimationId == null) {
            loadingAnimationId = 'application_loading';
        }
        $.ajax({
            url: $(src).attr('action'),
            type: $(src).attr('method'),
            data: formData,
            timeout: 600000,
            beforeSend: function () {
                $('#' + loadingAnimationId).css('display', 'block');
                if (onBeginCallBack && typeof (onBeginCallBack) === "function") {
                    onBeginCallBack();
                }
            },
            success: function (data) {
                if (hasTarget) {
                    $('#' + targetId).html(data);
                }
                if (onSuccessCallBack && typeof (onSuccessCallBack) === "function") {
                    onSuccessCallBack();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
                if (onFailureCallBack && typeof (onFailureCallBack) === "function") {
                    onFailureCallBack();
                }
                else {
                    Helper.ShowToastrNotification("error", "ERROR", data.statusText);
                }
            },
            complete: function () {
                $('#' + loadingAnimationId).css('display', 'none');
                if (onCompleteCallBack && typeof (onCompleteCallBack) === "function") {
                    onCompleteCallBack();
                }
            },
            cache: false,
            contentType: false,
            processData: false
        });
    }
}