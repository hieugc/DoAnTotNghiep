function showRequest(idHouse) {
    let request = $(`.model-house-${idHouse} span.value-request`)[0].innerText;
    if (request > 0) {
        $.get(
            window.location.origin + "/Request/GetByHouse?idHouse=" + idHouse,
            function (data) {
                $("#renderModal").html(data);
            }
        )
    }
}