var SMS = {
    Form_OnInit: function () {
        $(document).on('change', ':file', function () {
            var input = $(this),
                numFiles = input.get(0).files ? input.get(0).files.length : 1,
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, label]);
        });

        $(':file').on('fileselect', function (event, numFiles, label) {
            var input = $(this).parents('.input-group').find(':text'),
                log = numFiles > 1 ? numFiles + ' files selected' : label;
            if (input.length) {
                input.val(log);
            } else {
                if (log) alert(log);
            }
        });

        Helper.BindToDataTable('#record-list-table', {
        });

        Portal.Exec_FormBehavior();
    },

    Form_OnSubmit: function (evt, ctr) {
        Helper.AjaxFormSubmit(
            ctr,
            evt,
            'record-form-container',
            'application_loading',
            null,
            null,
            function () {
                Helper.ShowNotification('error', 'SMS Import', 'Something went wrong.', 10000);
            },
            SMS.Form_OnInit
        );
    },

    BulkTransactBtn_OnClick: function (evt, action) {
        if (table != null) {
            table.destroy();
        }
        evt.preventDefault();
        evt.stopImmediatePropagation();
        swal({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {
                $('#application_loading').css("display", "block");
                $('#action').val(action);
                $('#BulkTransact_Form').submit();
            }
        });
    }
}

var SMSPublic = {
    Form_OnLoad: function (ajaxCall) {
        Portal.Exec_FormBehavior();
        if (ajaxCall) {
            LoadReCaptCha();
        }
    },

    CalcRemainingMsg: function () {
        var text_length = $('#MessageBody').val().length;
        var text_remaining = text_max - text_length;
        $('#charcount').text(text_remaining + ' characters remaining');
    }
}