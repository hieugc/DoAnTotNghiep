var messageRequestModal = document.getElementById("messageRequestModal");
function showListHouseModal(index) {
    messageRequestModal.getElementsByClassName("list-house")[index].style.display = "flex";
    messageRequestModal.getElementsByClassName("modal-content")[0].style.display = "none";
}
function hideListHouseModal(index) {
    messageRequestModal.getElementsByClassName("list-house")[index].style.display = "none";
    messageRequestModal.getElementsByClassName("modal-content")[0].style.display = "flex";
}


function showNotification(head, body, status) {
    if (status == 1) {
        $("#notification-head").addClass("text-success");
        $("#notification-head").removeClass("text-danger");
    }
    else {
        $("#notification-head").removeClass("text-success");
        $("#notification-head").addClass("text-danger");
    }
    $("#notification-head").html(head);
    $("#notification-body").html(body);
    $("#toastPlacement").show();

    setTimeout(function () {
        $("#toastPlacement").hide();
    }, 2000);
}