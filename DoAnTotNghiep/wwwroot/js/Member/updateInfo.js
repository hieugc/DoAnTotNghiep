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
                    showNotification("Thông báo", "Cập nhật thành công", 1);
                }
                else {
                    showNotification("Thông báo", "Cập nhật thất bại", 0);
                }

            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}

let src = null;

function changeAvt(element) {
    const files = element.files[0];
    if (files) {
        const fileReader = new FileReader();
        fileReader.readAsDataURL(files);
        fileReader.addEventListener("load", function () {
            src = this.result;
            $(".avt").html(`<img src="${this.result}" alt="Hình ảnh avt" />`);
            $("#saveImagebtn").show();
        });
    }
}
function SaveImage() {
    let form = $("#InfoForm");
    $.validator.unobtrusive.parse(form);
    if (form.valid() && src != null) {
        let data = {
            lastName: $("#LastName").val(),
            firstName: $("#FirstName").val(),
            phoneNumber: $("#PhoneNumber").val(),
            birthDay: $("#BirthDay").val(),
            gender: Boolean($("input[name='gender']:checked").val()),
            email: $("#Email").val(),
            image: {
                name: "null",
                id: null,
                data: src
            }
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
                    showNotification("Thông báo", "Cập nhật thành công", 1);
                    $("#saveImagebtn").hide();
                }
                else {
                    showNotification("Thông báo", "Cập nhật thất bại", 0);
                }

            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}