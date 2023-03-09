﻿var IdHouseDelete = null;
function deleteHouse(id) {
    if (IdHouseDelete == null) {
        IdHouseDelete = id;
        $("#confirmContent").html("Bạn có muốn xóa căn nhà này chứ???");
        $("#confirmClick").click();
    }
    else {
        showNotification("Thông báo", "Hệ thống đang xử lý", 0);
    }
}
function confirmDeleteHouse(bool) {
    if (bool == true) {
        let Id = IdHouseDelete;
        $.ajax({
            url: window.location.origin + "/House/Delete",
            headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() },
            data: JSON.stringify(Id),
            dataType: "json",
            contentType: "application/json",
            type: "DELETE",
            success: function (result) {
                deleteHouse = null;
                console.log(result);
                if (result.status == 200) {
                    showNotification("Thao tác thành công", result.message, 1);
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
        deleteHouse = null;
    }
}