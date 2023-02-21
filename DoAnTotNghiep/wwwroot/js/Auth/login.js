var loginForm = document.getElementById("loginModal");

function refreshLoginForm() {
    loginForm.getElementsByClassName("modal-body")[0].classList.add("sigshow");
    loginForm.getElementsByClassName("modal-body")[0].classList.remove("sighide");

    loginForm.getElementsByClassName("modal-body")[1].classList.add("sighide");
    loginForm.getElementsByClassName("modal-body")[1].classList.remove("sigshow");
    loginForm.getElementsByClassName("modal-body")[2].classList.add("sighide");
    loginForm.getElementsByClassName("modal-body")[2].classList.remove("sigshow");
    loginForm.getElementsByClassName("modal-body")[3].classList.add("sighide");
    loginForm.getElementsByClassName("modal-body")[3].classList.remove("sigshow");
}

function showForm(step, prevstep) {
    loginForm.getElementsByClassName("modal-body")[step].classList.add("sigshow");
    loginForm.getElementsByClassName("modal-body")[step].classList.remove("sighide");

    loginForm.getElementsByClassName("modal-body")[prevstep].classList.add("sighide");
    loginForm.getElementsByClassName("modal-body")[prevstep].classList.remove("sigshow");
}