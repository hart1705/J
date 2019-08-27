var DAR = {
    RecordList_OnLoad: function () {
        var options = {
            dom: '<"col-md-7 pull-left"l><"col-md-3 pull-left"f><"col-md-2 pull-left"B>rtip',
            buttons: [
                {
                    extend: 'excelHtml5', //'pdfHtml5'
                    title: 'Daily Activity Records',
                    text: '<span class="fa fa-download btn-flat"></span> Export Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5]
                    }
                }
            ],
            responsive: true,
            "columnDefs": [
                {
                    "targets": [4],
                    "visible": false,
                    "searchable": false
                }
            ],
            "order": [[3, "asc"]]
        };
        Helper.BindToDataTable("#vc-dar-table", options);
    }
}