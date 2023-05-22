function getRequestFormByUserAccess(userAccess) {
    console.log(userAccess);
    $.get(
        window.location.origin + "/Request/FormWithUserAccess?userAccess=" + encodeURIComponent(userAccess),
        function (data) {
            $("#renderModal").html(data);
        }
    )
}