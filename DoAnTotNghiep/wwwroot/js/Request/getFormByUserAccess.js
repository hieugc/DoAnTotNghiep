function getRequestFormByUserAccess(userAccess) {
    $.get(
        window.location.origin + "/Request/FormWithUserAccess?userAccess=" + userAccess,
        function (data) {
            $("#renderModal").html(data);
        }
    )
}