function renderRatingForm(IdRequest, IdRating) {
    let url = window.location.origin + "/CircleRating/Form?idRequest=" + IdRequest;
    if (IdRating != null) {
        url += "&idWaitingRating=" + IdRating;
    }
    $.get(
        url,
        function (data) {
            $("#renderModal").html(data);
        }
    );
}