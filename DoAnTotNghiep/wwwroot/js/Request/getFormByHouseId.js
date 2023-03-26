function getRequestFormByHouseId(houseId) {
    $.get(
        window.location.origin + "/Request/FormWithHouseId?idHouse=" + houseId,
        function (data) {
            $("#renderModal").html(data);
        }
    )
}