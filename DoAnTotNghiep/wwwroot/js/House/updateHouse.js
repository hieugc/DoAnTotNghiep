function updateRequestInHouse() {
    for (e in listHouse) {
        getRequestInHouse(listHouse[e].id);
    }
}
function getRequestInHouse(houseId) {
    $.ajax({
        url: window.location.origin + '/Request/NumberRequestInHouse?idHouse=' + houseId,
        contentType: "application/json",
        type: "GET",
        success: function (result) {
            if (result.status == 200) {
                $(`#list-house .model-house-${houseId} .value-request`).html(result.data);
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}