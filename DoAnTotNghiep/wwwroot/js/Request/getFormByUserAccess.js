function getRequestFormByUserAccess(userAccess) {
    console.log(userAccess);
    $.get(
        window.location.origin + "/Request/FormWithUserAccess?userAccess=" + userAccess,
        function (data) {
            $("#renderModal").html(data);
        }
    )
}