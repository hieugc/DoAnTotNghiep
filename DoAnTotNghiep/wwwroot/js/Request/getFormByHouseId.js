function getRequestFormByHouseId(houseId) {
    $.get(
        window.location.origin + "/Request/FormWithHouseId?idHouse=" + encodeURIComponent(houseId),
        function (data) {
            $("#renderModal").html(data);
        }
    );
}