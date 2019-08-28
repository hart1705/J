var ApplicationPermission = {
    AccessType_OnChange: function (ctr, actionId, permissionId) {
        $('#actionId').val(actionId);
        $('#permissionId').val(permissionId);
        $('#accessType').val($(ctr).val());
        $('#applicationpermission-change-accesstype-form').submit();
    }
}