var Portal = {
    OnLoad: function () {
        $('.panel-collapse').on('show.bs.collapse', function () {
            $(this).siblings('.panel-heading').addClass('active');
        });

        $('.panel-collapse').on('hide.bs.collapse', function () {
            $(this).siblings('.panel-heading').removeClass('active');
        });
        
        Helper.BindToDataTable('.lookup-table', {
            "paging": false,
            "searching": false,
            "scrollY": "250px",
            "scrollCollapse": true
        });
        Helper.BindToDataTable('.view-datatable');
        $('#record-list-container').removeClass('hidden');
        Annotation.ToSlimScroll();
    },
    Exec_FormBehavior: function () {
        var hasNotification = $("#form-has-notification");
        var redirect = $('#form-redirect');

        if (hasNotification.length > 0) {
            var isError = $('#Notification_IsError').val() == "True";
            var title = $('#Notification_Title').val();
            var msg = $('#Notification_Message').val();
            Helper.ShowNotification(isError ? "error" : "success", title, msg);
        }
        if (redirect.length > 0) {
            var url = $("#PageRedirect_URL").val();
            window.location.href = url;
        }
    }
}