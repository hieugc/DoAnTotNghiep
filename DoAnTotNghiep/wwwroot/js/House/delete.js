var IdHouseDelete = null;
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
            type: "POST",
            success: function (result) {
                IdHouseDelete = null;
                console.log(result);
                if (result.status == 200) {
                    showNotification("Thao tác thành công", result.message, 1);
                    listHouse = listHouse.filter(function (model) {
                        return model.id != Id;
                    })
                    $(`.house-id-${Id}`).remove();
                    reloadPage();
                }
            },
            error: function (error) {
                showNotification("Thao tác thất bại", error.responseJSON.message, 0);
            }
        });
    }
    else {
        IdHouseDelete = null;
    }
}