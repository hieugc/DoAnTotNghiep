var messageRequestModal = document.getElementById("messageRequestModal");
function showListHouseModal(index) {
    messageRequestModal.getElementsByClassName("list-house")[index].style.display = "flex";
    messageRequestModal.getElementsByClassName("modal-content")[0].style.display = "none";
}
function hideListHouseModal(index) {
    messageRequestModal.getElementsByClassName("list-house")[index].style.display = "none";
    messageRequestModal.getElementsByClassName("modal-content")[0].style.display = "flex";
}