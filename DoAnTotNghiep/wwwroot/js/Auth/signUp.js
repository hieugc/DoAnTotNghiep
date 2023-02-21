var signUpForm = document.getElementById("signUpModal");

function freshFormSignUp() {
    $('#signUpModal').modal('toggle');

    signUpForm.getElementsByClassName("modal-body")[0].classList.add("sigshow");
    signUpForm.getElementsByClassName("modal-body")[0].classList.remove("sighide");

    signUpForm.getElementsByClassName("modal-body")[1].classList.add("sighide");
    signUpForm.getElementsByClassName("modal-body")[1].classList.remove("sigshow");
    signUpForm.getElementsByClassName("modal-body")[2].classList.add("sighide");
    signUpForm.getElementsByClassName("modal-body")[2].classList.remove("sigshow");
}

function showFormSignUp(step, prevstep) {
    signUpForm.getElementsByClassName("modal-body")[step].classList.add("sigshow");
    signUpForm.getElementsByClassName("modal-body")[step].classList.remove("sighide");

    signUpForm.getElementsByClassName("modal-body")[prevstep].classList.add("sighide");
    signUpForm.getElementsByClassName("modal-body")[prevstep].classList.remove("sigshow");
}