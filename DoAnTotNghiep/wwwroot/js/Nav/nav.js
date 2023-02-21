var menuTab = document.getElementsByClassName("menu-tab")[0];
var backgroundMenuTab = document.getElementsByClassName("lg-device")[0];
var btnCloseMenuTab = document.getElementsByClassName("turn-off")[0];

function closeMenuTab() {
    backgroundMenuTab.style.width = "0";
}

function showMenuTab() {
    backgroundMenuTab.style.width = "100%";
}

function refresh() {
    backgroundMenuTab.style = null;
}

menuTab.addEventListener("click", showMenuTab);
btnCloseMenuTab.addEventListener("click", closeMenuTab);
window.addEventListener("click", function (e) {
    if (e.target == backgroundMenuTab) closeMenuTab();
});
window.addEventListener("resize", function () {
    if (this.innerWidth > 800) refresh();
});