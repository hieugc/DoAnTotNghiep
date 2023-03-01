var houseModal = document.getElementById("houseModalToggle");
var data = {};
initData();

var listHouse = [];


function initData() {
    data = {
        nameHouse: null,
        optionHouse: null,
        descHouse: null,
        peopleHouse: null,
        sleepHouse: null,
        bathHouse: null,
        squareHouse: null,
        locationHouse: null,
        lat: null,
        lng: null,
        priceHouse: null,

        utilities: [],
        rules: [],
        images: []
    };
}
function passStep(step) {
    if (data.nameHouse != null && data.optionHouse != null && data.descHouse != null) {
        if (step == 2) return true;
        else if (step == 3 && data.peopleHouse != null && data.sleepHouse != null
            && data.bathHouse != null && data.squareHouse != null && data.locationHouse != null && data.priceHouse != null) return true;
    }
    return false;
}

function nextStep(step, element) {
    if (passStep(step)) {
        if (element.classList.value.indexOf("btn-no-drop") != -1) {
            element.classList.remove("btn-no-drop");
        }
        houseModal.getElementsByClassName("progress-bar")[0].style.width = (step * 20) + "%";
        houseModal.getElementsByClassName("time-line-step")[step - 1].classList.add("active");
        houseModal.getElementsByClassName("modal-body")[step - 1].classList.add("sigshow");
        houseModal.getElementsByClassName("modal-body")[step - 2].classList.remove("sigshow");
        houseModal.getElementsByClassName("modal-body")[step - 2].classList.add("sighide");
        houseModal.getElementsByClassName("modal-body")[step - 1].classList.remove("sighide");
    }
    else {
        if (element.classList.value.indexOf("btn-no-drop") == -1) {
            element.classList.add("btn-no-drop");
        }
    }
}

function prevStep(step) {
    houseModal.getElementsByClassName("progress-bar")[0].style.width = (step * 20) + "%";
    if (houseModal.getElementsByClassName("time-line-step")[step].classList.contains("active")) {
        houseModal.getElementsByClassName("time-line-step")[step].classList.remove("active");
    }
    houseModal.getElementsByClassName("modal-body")[step - 1].classList.add("sigshow");
    houseModal.getElementsByClassName("modal-body")[step].classList.remove("sigshow");
    houseModal.getElementsByClassName("modal-body")[step].classList.add("sighide");
    houseModal.getElementsByClassName("modal-body")[step - 1].classList.remove("sighide");
}

function isDataPass() {
    if (data.nameHouse != null
        && data.optionHouse != null
        && data.descHouse != null
        && data.peopleHouse != null
        && data.sleepHouse != null
        && data.bathHouse != null
        && data.squareHouse != null
        && data.lat != null
        && data.lng != null
        && data.locationHouse != null
        && data.priceHouse != null
        && data.images.length > 0
    ) return true;
    return false;
}

function finish(url) {
    //ajax create
    if (isDataPass()) {
        $.ajax({
            url: url,
            data: JSON.stringify(data),
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
                houseModal.getElementsByClassName("progress-bar")[0].style.width = "100%";
                setTimeout(function () {
                    $("#houseModalToggleClose").click();
                }, 500);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    else {
        console.log("ok");
    }
}

function refreshHouseModal() {
    houseModal.getElementsByClassName("progress-bar")[0].style.width = "20%";
    houseModal.getElementsByClassName("time-line-step")[1].classList.remove("active");
    houseModal.getElementsByClassName("time-line-step")[2].classList.remove("active");
    houseModal.getElementsByClassName("time-line-step")[3].classList.remove("active");
    houseModal.getElementsByClassName("modal-body")[1].classList.remove("sigshow");
    houseModal.getElementsByClassName("modal-body")[2].classList.remove("sigshow");
    houseModal.getElementsByClassName("modal-body")[3].classList.remove("sigshow");
    houseModal.getElementsByClassName("modal-body")[1].classList.add("sighide");
    houseModal.getElementsByClassName("modal-body")[2].classList.add("sighide");
    houseModal.getElementsByClassName("modal-body")[3].classList.add("sighide");

    houseModal.getElementsByClassName("modal-body")[0].classList.add("sigshow");
    houseModal.getElementsByClassName("modal-body")[0].classList.remove("sighide");

    houseModal.getElementsByClassName("handle-step")[0].children[1].classList.add("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[3].children[1].classList.add("btn-no-drop");

    initData();
}

function pickOption(element) {
    if (element.classList.value.indexOf("option-selected") != -1) {
        element.classList.remove("option-selected");
    }
    else {
        element.classList.add("option-selected");
    }
}

function getOption() {
    var listUtilities = document.getElementById("list-utilities");
    var listRules = document.getElementById("list-rules");
}

function getNameHouse(element) {
    if (element.value.length > 0) {
        data.nameHouse = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.nameHouse = null;
        if (houseModal.getElementsByClassName("handle-step")[0].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.add("btn-no-drop");
        }
    }
}

function getOptionHouse(element, number) {
    if (data.optionHouse == null) {
        if (element.classList.value.indexOf("option-selected") == -1) {
            element.classList.add("option-selected");
            data.optionHouse = number;
            if (passStep(2)) {
                houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
            }
        }
    }
    else {
        if (data.optionHouse != number) {
            if (element.classList.value.indexOf("option-selected") == -1) {
                element.classList.add("option-selected");
                data.optionHouse = number;
                element.parentNode.getElementsByClassName.children[number - 1].classList.remove("option-selected");
                if (passStep(2)) {
                    houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
                }
            }
        }
    }
}

function getDescHouse(element) {
    if (element.value.length > 0) {
        data.descHouse = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.descHouse = null;
        if (houseModal.getElementsByClassName("handle-step")[0].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.add("btn-no-drop");
        }
    }
}


function getPeopleHouse(element) {
    if (element.value.length > 0) {
        data.peopleHouse = element.value;
        if (passStep(3)) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.peopleHouse = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}


function getSleepHouse(element) {
    if (element.value.length > 0) {
        data.sleepHouse = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.sleepHouse = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}


function getBathHouse(element) {
    if (element.value.length > 0) {
        data.bathHouse = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.bathHouse = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}


function getSquareHouse(element) {
    if (element.value.length > 0) {
        data.squareHouse = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.squareHouse = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}

function getLocationHouse(element) {
    if (data.locationHouse == null) {
        $("#btnBingMaps").click();
    }
    else {
        if (element.value.length > 0) {
            data.locationHouse = element.value;
            if (passStep(2)) {
                houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
            }
        }
        else {
            data.locationHouse = null;
            if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
                houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
            }
        }
    }
}

function getPriceHouse(element) {
    if (element.value.length > 0) {
        data.priceHouse = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.priceHouse = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}