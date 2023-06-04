function create() {
    $.get(
        window.location.origin + "/Utilities/Create",
        function (data) {
            $("#renderModal").html(data);
        }
    );
}

function submitCreate() {
    let form = $("#form-create");
    $.validator.unobtrusive.parse(form);
    if (form.valid()) {
        let data = {
            id: $("#Id").val(),
            content: $("#Content").val(),
            icon: $("#Icon").val()
        };
        $.ajax({
            url: window.location.origin + "/Utilities/Create",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
                showNotification("Khởi tạo thành công", "Đã thêm tiện ích mới", 1);
                $("#list-admin_item").append(item(result.data));
                $("#utilitiesModal").click();
            },
            error: function (error) {
                console.log(error);
                showNotification("Khởi tạo thất bại", "", 1);
                form.valid();
            }
        });
    }
}
function item(_item) {
    return `<tr><td><strong>#${_item.id}</strong></td><td>${_item.content}</td><td>${_item.icon}</td><td><div class="dropdown"><button type="button" class="btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="bx bx-dots-vertical-rounded"></i></button><div class="dropdown-menu"><span class="dropdown-item" onclick="update(${_item.id})"><i class="bx bx-edit-alt me-1"></i> Chỉnh sửa</span><span class="dropdown-item" onclick="remove(${_item.id})"><i class="bx bx-trash me-1"></i> Xóa</span></div></div></td></tr>`;
}
function update(id) {
    $.get(
        window.location.origin + `/Utilities/Update?id=${encodeURIComponent(id)}`,
        function (data) {
            $("#renderModal").html(data);
        }
    );
}

function remove(id) {

}