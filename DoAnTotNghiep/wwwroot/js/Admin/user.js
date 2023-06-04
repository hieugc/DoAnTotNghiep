function Detail(id) {
    $.get(
        window.location.origin + `/User/Details?idUser=${id}`,
        function (data) {
            $("#renderModal").html(AdminModal(data));
            $("#AdminClick").click();
        }
    );
}
function AdminModal(html) {
    return `<div class="modal fade" id="Admin-itemModal" tabindex="-1" aria-hidden="true"><div class="modal-dialog modal-xl" role="document"><div class="modal-content"><div class="modal-header"><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div><div class="modal-body">${html}</div></div></div><button id="AdminClick" type="button" class="btn btn-primary" data-bs-toggle="modal"data-bs-target="#Admin-itemModal" style="display: none;"></button>`;
}

function Report(id) {
    $.get(
        window.location.origin + `/Report/GetByUser?userId=${id}`,
        function (data) {
            $("#renderModal").html(AdminModal(data));
            $("#AdminClick").click();
        }
    );
}

function Ban(id) {
    $.ajax({
        url: window.location.origin + "/User/BanUser",
        data: JSON.stringify(id),
        dataType: "json",
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                $(`.item_user_${id}`).remove();
                showNotification("Thao tác thành công", "Đã cấm người dùng", 1);
            }
        },
        error: function (error) {
            console.log(error);
            showNotification("Thao tác thất bại", error.responseJSON.message, 0);
            runBackStep(getStep());
        }
    });
}
function NumberReport(id) {
    $.get(
        window.location.origin + `/Report/GetNumberByUser?userId=${encodeURIComponent(id)}`,
        function (data) {
            $(`.get_report_${id}`).html(data.data);
        }
    );
}
function NumberSwap(id) {
    $.get(
        window.location.origin + `/User/GetNumberSwap?userId=${encodeURIComponent(id)}`,
        function (data) {
            $(`.get_swap_${id}`).html(data.data);
        }
    );
}
