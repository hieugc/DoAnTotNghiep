function updateStatus(id, status) {
    let data = {
        id: id,
        status: status
    };
    $.ajax({
        url: window.location.origin + "/Request/UpdateStatus",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            $("#requestHouseModalToggleClick").click();
        },
        error: function (error) {
            console.log(error);
        }
    });
}