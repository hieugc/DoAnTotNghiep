function renderReportForm(UserAccess) {
    $.get(
        window.location.origin + "/Report/Form?userAccess=" + encodeURIComponent(UserAccess),
        function (data) {
            $("#renderModal").html(data);
        }
    );
}