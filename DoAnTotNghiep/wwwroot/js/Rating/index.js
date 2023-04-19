function renderRatingForm(IdRequest, IdRating) {
    let url = window.location.origin + "/Rating/Form?idRequest=" + IdRequest;
    if (IdRating != null) {
        url += "&idRating=" + IdRating;
    }
    $.get(
        url,
        function (data) {
            $("#renderModal").html(data);
        }
    );
}

function renderFrameRating(idHouse) {
    $.get(
        window.location.origin + "/Rating/GetByHouse?Id=" + idHouse,
        function (data) {
            $("#tab6").html(data);
        }
    );
}