var Annotation = {
    Form_OnInit: function () {
        $(document).on('change', ':file', function () {
            var input = $(this),
                numFiles = input.get(0).files ? input.get(0).files.length : 1,
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.parent().find('input:eq(0)').first().val("True");
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
    },
    Form_OnSubmit: function (evt, ctr) {
        Helper.AjaxFormSubmit(
            ctr,
            evt,
            'annotation-form-container',
            'application_loading',
            null,
            null,
            function () {
                Helper.ShowNotification('error', 'Note', 'Something went wrong.', 10000);
            },
            Annotation.Form_OnInit
        );
    },
    ToSlimScroll: function () {
        $('#annotation-list-box').slimScroll({
            height: '250px'
        });
    }
};