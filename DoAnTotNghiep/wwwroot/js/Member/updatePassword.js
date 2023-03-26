function submitPassword() {
    let form = $("#pwdForm");
    $.validator.unobtrusive.parse(form);
    if (form.valid()) {
        let data = {
            password: $("#Password").val(),
            confirmPassword: $("#ConfirmPassword").val(),
        };

        $.ajax({
            url: window.location.origin + "/user/UpdatePassword",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                if (result.status == 200) {
                    showNotification("Thông báo", "Cập nhật mật khẩu thành công", 1);
                    $("#clickChangePWDModal").click();
                }
                else {
                    showNotification("Thông báo", "Cập nhật mật khẩu thất bại", 0);
                }
                
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}

function getChangePasswordModal() {
    $.get(
        window.location.origin + "/User/UpdatePassword",
        function (data) {
            $("#renderModal").html(data);            
        }
    )
}