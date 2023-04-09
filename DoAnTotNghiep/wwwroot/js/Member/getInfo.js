function submitInfo() {
    let form = $("#InfoForm");
    $.validator.unobtrusive.parse(form);
    if (form.valid()) {
        let data = {
            lastName: $("#LastName").val(),
            firstName: $("#FirstName").val(),
            phoneNumber: $("#PhoneNumber").val(),
            birthDay: $("#BirthDay").val(),
            gender: Boolean($("input[name='gender']:checked").val()),
            email: $("#Email").val()
        };

        $.ajax({
            url: window.location.origin + "/user/UpdateInfo",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
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