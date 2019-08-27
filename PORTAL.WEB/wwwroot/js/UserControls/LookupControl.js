var Lookup = {
    ModalBindToDataTable: function (tableId, singleSelect) {
        var table = Helper.BindToDataTable('#' + tableId, {
            "paging": false,
            "info": false,
            "searching": false,
            "scrollY": "250px",
            "scrollCollapse": true
        });

        if (singleSelect !== undefined && singleSelect !== null) {
            $('#' + tableId).on('click', 'tr', function () {
                var lookupSelectedDisplay = $(this).parents().find('.lookup-selected-display').first();
                var lookupSelectedId = $(this).parents().find('.lookup-selected-id').first();
                var tempIcon = $(this).children().find('.lookup-checkicon').first();
                if ($(this).hasClass('selected') && tempIcon.length > 0) {
                    $(this).removeClass('selected');
                    $(tempIcon).css("display", "none");
                    lookupSelectedId.val("");
                    lookupSelectedDisplay.val("");
                }
                else if (tempIcon.length > 0) {
                    table.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                    $(tempIcon).css("display", "block");
                    lookupSelectedId.val($(this).find('input:eq(0)').val());
                    lookupSelectedDisplay.val($(this).find('td:eq(1)').text().trim(" "));
                }
            });
        }
        else {
            $('#' + tableId + ' tbody').on('click', 'tr', function () {
                var tempIcon = $(this).children().find('.lookup-checkicon').first();
                if (tempIcon.length > 0) {
                    $(this).toggleClass('selected');
                }
            });
        }
    },

    BindToDataTable: function (tableId) {
        Helper.BindToDataTable('#' + tableId, {
            "paging": false,
            "searching": false,
            "scrollY": "250px",
            "scrollCollapse": true
        });
    },

    Search_OnKeyPress: function (evt, btnId) {
        if (evt.keyCode === 13) {
            $(btnId).trigger('click');
            evt.preventDefault();
        }
    },

    TableRow_OnClick: function (ctr) {
        var tempIcon = $(ctr).children().find('.lookup-checkicon').first();
        var tempCheckBox = $(ctr).children().find('.lookup-checkbox').first();
        if (tempIcon.length > 0 && tempCheckBox.length > 0) {
            var icon = $(tempIcon);
            var checkbox = $(tempCheckBox);
            if (icon.css('display') === 'none') {
                checkbox.prop("checked", true);
                $(icon).css('display', 'block');
            } else {
                checkbox.prop("checked", false);
                $(icon).css('display', 'none');
            }
        }
    },

    TableRowSingle_OnClick: function (lookupId) {
        $('#' + lookupId + '-lookup-addtable').find('.lookup-checkicon').each(function () {
            $(this).css('display', 'none');
        });
    },

    LookupSingle_OnSelection: function (lookupId, isRemove) {
        var displayVal = $('#' + lookupId + '-lookup-maincontrol').find('input:eq(0)');
        var displayHid = $('#' + lookupId + '-lookup-maincontrol').find('input:eq(1)');

        if (isRemove !== undefined && isRemove !== null) {
            displayVal.val("");
            displayHid.val("");
        }
        else {
            var lookupSelectedDisp = $('#' + lookupId + '-lookup-addcontainer').find('input:eq(0)');
            var lookupSelectedId = $('#' + lookupId + '-lookup-addcontainer').find('input:eq(1)');
            if (lookupSelectedId !== undefined && lookupSelectedId.val() !== "") {
                displayVal.val(lookupSelectedDisp.val());
                displayHid.val(lookupSelectedId.val());
            }
        }
    }
}