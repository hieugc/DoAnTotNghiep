function renderReportForm(UserAccess) {
    $.get(
        window.location.origin + "/Report/Form?userAccess=" + UserAccess,
        function (data) {
            $("#renderModal").html(data);
        }
    );
}