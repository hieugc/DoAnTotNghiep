var IdRequestDelete = null;


function deleteRequest(id) {
    if (IdRequestDelete == null) {
        IdRequestDelete = id;
        $("#confirmContent").html("Bạn có muốn xóa căn nhà này chứ???");
        $("#confirmClick").click();
    }
    else {
        showNotification("Thông báo", "Hệ thống đang xử lý", 0);
    }
}
function confirmDeleteHouse(bool) {
    if (bool == true) {
        let Id = IdRequestDelete;
        $.ajax({
            url: window.location.origin + "/Request/Delete",
            data: JSON.stringify(Id),
            dataType: "json",
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                IdRequestDelete = null;
                console.log(result);
                if (result.status == 200) {
                    showNotification("Thao tác thành công", result.message, 1);
                    $(".card-" + Id).remove();
                }
                else if (result.status >= 500) {
                    showNotification("Thao tác thất bại", result.message, 0);
                }
                else {
                    showNotification("Thao tác thất bại", result.message, 0);
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    else {
        IdRequestDelete = null;
    }
}