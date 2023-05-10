function updateStatus(id, status, idCircle) {
    let data = { id: id, idCircle: idCircle, status: status };
    $.ajax({
        url: window.location.origin + "/CircleRequest/UpdateStatus",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                $.get(
                    window.location.origin + "/CircleRequest/Detail?Id=" + idCircle,
                    function (data) {
                        $(`.card-${idCircle}`).replaceWith(data);
                    }
                )
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}