function updateStatus(id, status) {
    let data = { id: id, status: status };
    $.ajax({
        url: window.location.origin + "/Request/UpdateStatus",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                $.get(
                    window.location.origin + "/Request/Detail?Id=" + id,
                    function (data) {
                        $(`.card-${id}`).replaceWith(data);
                    }
                )
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}