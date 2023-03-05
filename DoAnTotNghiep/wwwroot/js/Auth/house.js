//////var houseModal = document.getElementById("houseModalToggle");
//////var data = {};
//////initData();
//////var indexPicture = undefined;

//////var listHouse = [];

//////function initData() {
//////    data = {
//////        name: null,
//////        option: null,
//////        description: null,
//////        people: null,
//////        bedRoom: null,
//////        bathRoom: null,
//////        square: null,
//////        location: null,
//////        lat: null,
//////        lng: null,
//////        idCity: null,
//////        idDistrict: null,
//////        idWard: null,
//////        price: null,
//////        utilities: [],
//////        rules: [],
//////        images: [null, null, null, null]
//////    };
//////}
//////function passStep(step) {
//////    if (step == 2) {
//////        if (data.name != null && data.option != null && data.description != null) return true;
//////        return false;
//////    }
//////    if (step == 3) {
//////        if (data.name != null && data.option != null && data.description != null && data.people != null && data.bedRoom != null
//////            && data.bathRoom != null && data.square != null && data.location != null && data.price != null) return true;
//////        return false;
//////    }
//////    return true;
//////}
//////function checkStep(step, element) {
//////    if (passStep(step)) {
//////        if (element.classList.value.indexOf("btn-no-drop") != -1) {
//////            element.classList.remove("btn-no-drop");
//////        }
//////    }
//////}
//////function nextStep(step, element) {
//////    if (passStep(step)) {
//////        if (element.classList.value.indexOf("btn-no-drop") != -1) {
//////            element.classList.remove("btn-no-drop");
//////        }
//////        houseModal.getElementsByClassName("progress-bar")[0].style.width = (step * 20) + "%";
//////        houseModal.getElementsByClassName("time-line-step")[step - 1].classList.add("active");
//////        houseModal.getElementsByClassName("modal-body")[step - 1].classList.add("sigshow");
//////        houseModal.getElementsByClassName("modal-body")[step - 2].classList.remove("sigshow");
//////        houseModal.getElementsByClassName("modal-body")[step - 2].classList.add("sighide");
//////        houseModal.getElementsByClassName("modal-body")[step - 1].classList.remove("sighide");
//////    }
//////    else {
//////        if (element.classList.value.indexOf("btn-no-drop") == -1) {
//////            element.classList.add("btn-no-drop");
//////        }
//////    }
//////}
//////function prevStep(step) {
//////    houseModal.getElementsByClassName("progress-bar")[0].style.width = (step * 20) + "%";
//////    if (houseModal.getElementsByClassName("time-line-step")[step].classList.contains("active")) {
//////        houseModal.getElementsByClassName("time-line-step")[step].classList.remove("active");
//////    }
//////    houseModal.getElementsByClassName("modal-body")[step - 1].classList.add("sigshow");
//////    houseModal.getElementsByClassName("modal-body")[step].classList.remove("sigshow");
//////    houseModal.getElementsByClassName("modal-body")[step].classList.add("sighide");
//////    houseModal.getElementsByClassName("modal-body")[step - 1].classList.remove("sighide");
//////}
//////function isDataPass() {
//////    if (data.name != null
//////        && data.option != null
//////        && data.description != null
//////        && data.people != null
//////        && data.bedRoom != null
//////        && data.bathRoom != null
//////        && data.square != null
//////        && data.lat != null
//////        && data.lng != null
//////        && data.location != null
//////        && data.price != null
//////        && data.images.join("").length > 1
//////    ) return true;
//////    return false;
//////}
//////function finish(url) {
//////    if (isDataPass()) {
//////        getOption();
//////        $.ajax({
//////            url: url,
//////            data: JSON.stringify(data),
//////            contentType: "application/json",
//////            type: "POST",
//////            success: function (result) {
//////                //nếu đúng
//////                console.log(result);
//////                houseModal.getElementsByClassName("progress-bar")[0].style.width = "100%";
//////                setTimeout(function () {
//////                    $("#houseModalToggleClose").click();//click => refresh form
//////                }, 500);
//////            },
//////            error: function (error) {
//////                console.log(error);
//////            }
//////        });
//////    }
//////    else {
//////        console.log("ok");
//////    }
//////}
//////function refreshHouseModal() {
//////    houseModal.getElementsByClassName("progress-bar")[0].style.width = "20%";
//////    houseModal.getElementsByClassName("time-line-step")[1].classList.remove("active");
//////    houseModal.getElementsByClassName("time-line-step")[2].classList.remove("active");
//////    houseModal.getElementsByClassName("time-line-step")[3].classList.remove("active");
//////    houseModal.getElementsByClassName("modal-body")[1].classList.remove("sigshow");
//////    houseModal.getElementsByClassName("modal-body")[2].classList.remove("sigshow");
//////    houseModal.getElementsByClassName("modal-body")[3].classList.remove("sigshow");
//////    houseModal.getElementsByClassName("modal-body")[1].classList.add("sighide");
//////    houseModal.getElementsByClassName("modal-body")[2].classList.add("sighide");
//////    houseModal.getElementsByClassName("modal-body")[3].classList.add("sighide");

//////    houseModal.getElementsByClassName("modal-body")[0].classList.add("sigshow");
//////    houseModal.getElementsByClassName("modal-body")[0].classList.remove("sighide");

//////    houseModal.getElementsByClassName("handle-step")[0].children[0].classList.add("btn-no-drop");
//////    houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
//////    houseModal.getElementsByClassName("handle-step")[3].children[1].classList.add("btn-no-drop");

//////    initData();
//////}
//////function pickOption(element) {
//////    if (element.classList.value.indexOf("option-selected") != -1) {
//////        element.classList.remove("option-selected");
//////    }
//////    else {
//////        element.classList.add("option-selected");
//////    }
//////}
//////function getOption() {
//////    var listUtilities = document.getElementById("list-utilities").getElementsByClassName("option");
//////    for (let index = 0; index < listUtilities.length; index++) {
//////        if (listUtilities[index].classList.value.indexOf("option-selected") != -1) {
//////            data.utilities.push(index);
//////        }
//////    }
//////    var listRules = document.getElementById("list-rules").getElementsByClassName("option");
//////    for (let index = 0; index < listRules.length; index++) {
//////        if (listRules[index].classList.value.indexOf("option-selected") != -1) {
//////            data.rules.push(index);
//////        }
//////    }
//////}
//////function getNameHouse(element) {
//////    if (element.value.length > 0) {
//////        data.name = element.value;
//////        if (passStep(2)) {
//////            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
//////        }
//////    }
//////    else {
//////        data.name = null;
//////        if (houseModal.getElementsByClassName("handle-step")[0].children[0].classList.value.indexOf("btn-no-drop") != -1) {
//////            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.add("btn-no-drop");
//////        }
//////    }
//////}
//////function getOptionHouse(element, number) {
//////    if (data.option == null) {
//////        if (element.classList.value.indexOf("option-selected") == -1) {
//////            element.classList.add("option-selected");
//////            data.option = number;
//////            if (passStep(2)) {
//////                houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
//////            }
//////        }
//////    }
//////    else {
//////        if (data.option != number) {
//////            if (element.classList.value.indexOf("option-selected") == -1) {
//////                element.classList.add("option-selected");
//////                data.option = number;
//////                console.log(element.parentNode.getElementsByClassName("option"));
//////                element.parentNode.getElementsByClassName("option")[number - 1].classList.remove("option-selected");
//////                if (passStep(2)) {
//////                    houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
//////                }
//////            }
//////        }
//////    }
//////}
//////function getDescHouse(element) {
//////    if (element.value.length > 0) {
//////        data.description = element.value;
//////        if (passStep(2)) {
//////            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
//////        }
//////    }
//////    else {
//////        data.description = null;
//////        if (houseModal.getElementsByClassName("handle-step")[0].children[0].classList.value.indexOf("btn-no-drop") != -1) {
//////            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.add("btn-no-drop");
//////        }
//////    }
//////}
//////function getPeopleHouse(element) {
//////    if (element.value.length > 0 && element.value > 0) {
//////        data.people = element.value;
//////        if (passStep(3)) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
//////        }
//////    }
//////    else {
//////        data.people = null;
//////        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
//////        }
//////    }
//////}
//////function getSleepHouse(element) {
//////    if (element.value.length > 0 && element.value > 0) {
//////        data.bedRoom = element.value;
//////        if (passStep(2)) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
//////        }
//////    }
//////    else {
//////        data.bedRoom = null;
//////        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
//////        }
//////    }
//////}
//////function getBathHouse(element) {
//////    if (element.value.length > 0 && element.value > 0) {
//////        data.bathRoom = element.value;
//////        if (passStep(2)) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
//////        }
//////    }
//////    else {
//////        data.bathRoom = null;
//////        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
//////        }
//////    }
//////}
//////function getSquareHouse(element) {
//////    if (element.value.length > 0 && element.value > 0) {
//////        data.square = element.value;
//////        if (passStep(2)) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
//////        }
//////    }
//////    else {
//////        data.square = null;
//////        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
//////        }
//////    }
//////}
//////function getLocationHouse(element) {
//////    if (data.location == null) {
//////        $("#btnBingMaps").click();
//////    }
//////    else {
//////        if (element.value.length > 0) {
//////            data.location = element.value;
//////            if (passStep(2)) {
//////                houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
//////            }
//////        }
//////        else {
//////            data.location = null;
//////            if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
//////                houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
//////            }
//////        }
//////    }
//////}
//////function getPriceHouse(element) {
//////    if (element.value.length > 0 && element.value > 0) {
//////        data.price = element.value;
//////        if (passStep(2)) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
//////        }
//////    }
//////    else {
//////        data.price = null;
//////        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
//////            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
//////        }
//////    }
//////}

//////function changePicture(index) {
//////    indexPicture = index;
//////}
//////function addPiture() {
//////    if (data.images.indexOf(null) != -1) {
//////        getImgData();
//////    }
//////}
//////function getImgData() {
//////    const files = $("#input-picture")[0].files[0];
//////    if (files) {
//////        const fileReader = new FileReader();
//////        fileReader.readAsDataURL(files);
//////        fileReader.addEventListener("load", function () {
//////            if (indexPicture == null) indexPicture = data.images.indexOf(null);
//////            var picture_frame = document.getElementsByClassName("picture-frame")[indexPicture];
//////            picture_frame.classList.add("picture-frame-added");
//////            picture_frame.innerHTML = importImage(this.result, indexPicture);
//////            data.images[indexPicture] = {
//////                data: this.result,
//////                name: $("#input-picture")[0].files[0].name
//////            }
//////            indexPicture = null;
//////        });
//////    }
//////}
//////function importImage(src, index) {
//////    return `<img src="${src}" alt="Hình ảnh ${index}" />
//////                            <button type="button" class="btn-close" onclick="removePiture(${index})"></button>`
//////}
//////function removePiture(index) {
//////    if (index != undefined) {
//////        var frame_image = document.getElementsByClassName("picture-frame")[index];
//////        frame_image.classList.remove("picture-frame-added");
//////        frame_image.innerHTML = "<label for=\"input-picture\" onclick=changePicture(" + index + ")><i class=\"fa-solid fa-circle-plus\"></i></label>";
//////        data.images[index] = null;
//////    }
//////}