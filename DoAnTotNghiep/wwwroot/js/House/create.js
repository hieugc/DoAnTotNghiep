var houseModal = document.getElementById("houseModalToggle");
var data = {};
initData();
var indexPicture = undefined;
function initData(id) {
    data = {
        name: null,
        option: null,
        description: null,
        people: null,
        bedRoom: null,
        bathRoom: null,
        square: null,
        location: null,
        lat: 0,
        lng: 0,
        idCity: 0,
        idDistrict: 0,
        idWard: 0,
        price: null,
        utilities: [],
        rules: [],
        bed: null,
        images: [null, null, null, null]
    };
    if (id != undefined) data.id = id;
}
function passStep(step) {
    return step <= getStep();
}
function checkStep(step, element) {
    if (passStep(step)) {
        if (element.classList.value.indexOf("btn-no-drop") != -1) {
            element.classList.remove("btn-no-drop");
        }
    }
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
        runBackStep(step);
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
    if (data.name != null
        && data.option != null
        && data.description != null
        && data.people != null
        && data.bedRoom != null
        && data.bathRoom != null
        && data.square != null
        && data.lat != null
        && data.idCity != 0
        && data.lng != null
        && data.location != null
        && data.price != null
        && data.bed != null
        && data.images.join("").length > 1
    ) return true;
    return false;
}
function finish() {
    runBackStep(getStep());
    if (isDataPass()) {
        getOption();
        if (data.id != undefined) {
            $.ajax({
                url: window.location.origin + "/House/Update",
                headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() },
                data: JSON.stringify(data),
                dataType: "json",
                contentType: "application/json",
                type: "POST",
                success: function (result) {
                    //nếu đúng
                    console.log(result);
                    if (result.status == 200) {
                        for (e in listHouse) {
                            if (listHouse[e].id == result.data.id) {
                                listHouse[e] = result.data;
                                break;
                            }
                        }
                        reloadPage();
                        houseModal.getElementsByClassName("progress-bar")[0].style.width = "100%";
                        setTimeout(function () {
                            $("#houseModalToggleClose").click();
                        }, 500);
                    }
                },
                error: function (error) {
                    showNotification("Thao tác thất bại", error.responseJSON.message, 0);
                    runBackStep(getStep());
                }
            });
        }
        else {
            $.ajax({
                url: window.location.origin + "/House/Create",
                headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() },
                data: JSON.stringify(data),
                dataType: "json",
                contentType: "application/json",
                type: "POST",
                success: function (result) {
                    console.log(result);
                    if (result.status == 200) {
                        listHouse.push(result.data);
                        reloadPage();
                        houseModal.getElementsByClassName("progress-bar")[0].style.width = "100%";
                        setTimeout(function () {
                            $("#houseModalToggleClose").click();//click => refresh form
                        }, 500);
                    }
                },
                error: function (error) {
                    console.log(error);
                    showNotification("Thao tác thất bại", error.responseJSON.message, 0);
                    runBackStep(getStep());
                }
            });
        }
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

    houseModal.getElementsByClassName("handle-step")[0].children[0].classList.add("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[3].children[1].classList.add("btn-no-drop");
    document.getElementById("form-create").reset();

    if (map != null) {
        map.entities.pop();
        if (infobox != null) {
            infobox.setMap(null);
        }
    }
    $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>");
    $(".option").removeClass("option-selected");
    initData();
    for (let index in data.images) {
        if (data.images[index] == null) {
            removePiture(index);
        }
    }
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
    var listUtilities = document.getElementById("list-utilities").getElementsByClassName("option");
    data.utilities = [];
    for (let index = 0; index < listUtilities.length; index++) {
        if (listUtilities[index].classList.value.indexOf("option-selected") != -1) {
            data.utilities.push(index + 1);
        }
    }
    var listRules = document.getElementById("list-rules").getElementsByClassName("option");
    data.rules = [];
    for (let index = 0; index < listRules.length; index++) {
        if (listRules[index].classList.value.indexOf("option-selected") != -1) {
            data.rules.push(index + 1);
        }
    }
}
function getNameHouse(element) {
    if (element.value.length > 0) {
        data.name = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
        }
    }
    else {
        data.name = null;
        if (houseModal.getElementsByClassName("handle-step")[0].children[0].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.add("btn-no-drop");
        }
    }
}
function getOptionHouse(element, number) {
    data.option = number;
    if (element.classList.value.indexOf("option-selected") == -1) {
        element.classList.add("option-selected");
        element.parentNode.getElementsByClassName("option")[2 - number].classList.remove("option-selected");
    }
    if (passStep(2)) {
        houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
    }
}
function getDescHouse(element) {
    if (element.value.length > 0) {
        data.description = element.value;
        if (passStep(2)) {
            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
        }
    }
    else {
        data.description = null;
        if (houseModal.getElementsByClassName("handle-step")[0].children[0].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[0].children[0].classList.add("btn-no-drop");
        }
    }
}
function getPeopleHouse(element) {
    if (element.value.length > 0 && element.value > 0) {
        data.people = element.value;
        if (passStep(3)) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.people = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
    getPredict();
}
function getSleepHouse(element) {
    if (element.value.length > 0 && element.value > 0) {
        data.bedRoom = element.value;
        if (passStep(3)) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.bedRoom = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}
function getBathHouse(element) {
    if (element.value.length > 0 && element.value > 0) {
        data.bathRoom = element.value;
        if (passStep(3)) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.bathRoom = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}
function getSquareHouse(element) {
    if (element.value.length > 0 && element.value > 0) {
        data.square = element.value;
        if (passStep(3)) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.square = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
    getPredict();
}
function getLocationHouse(element) {
    if (data.location == null) {
        $("#btnBingMaps").click();
        $("#location-validate").html(null);
    }
    else {
        if (element.value.length > 0) {
            data.location = element.value;
            if (passStep(3)) {
                houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
            }
            $("#location-validate").html(null);
        }
        else {
            data.location = null;
            if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") == -1) {
                houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
            }

            $("#location-validate").html("Hãy điền địa chỉ nhà");
        }
    }
}
function getBedHouse(element) {
    if (element.value.length > 0 && element.value > 0) {
        data.bed = element.value;
        if (passStep(3)) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.bed = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}
function getPriceHouse(element) {
    if (element.value.length > 0 && element.value > 0) {
        data.price = element.value;
        if (passStep(3)) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
        }
    }
    else {
        data.price = null;
        if (houseModal.getElementsByClassName("handle-step")[1].children[1].classList.value.indexOf("btn-no-drop") != -1) {
            houseModal.getElementsByClassName("handle-step")[1].children[1].classList.add("btn-no-drop");
        }
    }
}
function changePicture(index) {
    indexPicture = index;
}
function addPiture() {
    if (data.images.indexOf(null) != -1) {
        getImgData();
    }
}
function getImgData() {
    const files = $("#input-picture")[0].files[0];
    if (files) {
        const fileReader = new FileReader();
        fileReader.readAsDataURL(files);
        fileReader.addEventListener("load", function () {
            if (indexPicture == null) indexPicture = data.images.indexOf(null);
            if (houseModal.getElementsByClassName("handle-step")[3].children[1].classList.value.indexOf("btn-no-drop") != -1) {
                houseModal.getElementsByClassName("handle-step")[3].children[1].classList.remove("btn-no-drop");
            }
            var picture_frame = document.getElementsByClassName("picture-frame")[indexPicture];
            picture_frame.classList.add("picture-frame-added");
            picture_frame.innerHTML = importImage(this.result, indexPicture);
            data.images[indexPicture] = {
                data: this.result,
                name: $("#input-picture")[0].files[0].name
            }
            indexPicture = null;
        });
    }
}
function importImage(src, index) { 
    return `<img src="${src}" alt="Hình ảnh ${index}" />
                            <button type="button" class="btn-close" onclick="removePiture(${index})"></button>`
}
function removePiture(index) {
    if (index != undefined) { 
        var frame_image = document.getElementsByClassName("picture-frame")[index];
        frame_image.classList.remove("picture-frame-added");
        frame_image.innerHTML = "<label for=\"input-picture\" onclick=changePicture(" + index + ")><i class=\"fa-solid fa-circle-plus\"></i></label>";
        data.images[index] = null;

        if (data.images.indexOf(null) != -1) {
            houseModal.getElementsByClassName("handle-step")[3].children[1].classList.add("btn-no-drop");
        }
    }
}
function houseItem(data) {
    let image = window.location.origin + "/Image/logo.png";
    for (e in data.images) {
        if (data.images[e] != null) {
            if (data.images[e].data != null && data.images[e].data.length > 0) image = data.images[e].data;
            else {
                image = window.location.origin + "/" + data.images[e].pathFolder + "/" + data.images[e].fileName;
            }
            break;
        }
    }
    let option = "Căn nhà";
    if (data.option == 2) option = "Căn hộ";
    return `
        <div class="card mb-3 status-${data.status}">
                <div class="row g-0">
                    <div class="col-md-8">
                        <img src="${image}" class="img-fluid rounded-start" alt="hình ảnh nhà">
                    </div>
                    <div class="col-md-4">
                        <div class="card-body">
                            <div class="body-head">
                                <h5 class="card-title">${data.name}</h5>
                                <div>
                                    <div class="rating"><span>0</span><i class="fa-solid fa-star"></i></div>
                                    <div class="point"><strong>${data.price} p/ngày</strong></div>
                                </div>
                            </div>
                            <div class="body-detail">
                                <div class="house-attr">
                                    <div><i class="fa-solid fa-house"></i><span>${option}</span></div>
                                    <div><i class="fa-solid fa-users"></i><span>${data.people}</span></div>
                                    <div><i class="fa-solid fa-bed"></i><span>${data.bedRoom}</span></div>
                                    <div><i class="fa-solid fa-bath"></i><span>${data.bathRoom}</span></div>
                                    <div><i class="fa-solid fa-maximize"></i><span>${data.square} &#13217;</span></div>
                                </div>
                                <div class="alert-swap">
                                    
                                </div>
                            </div>
                            <p class="attribute"><i class="fa-solid fa-map-location-dot px-2"></i><span>${data.location}, ${data.wardName}, ${data.districtName}, ${data.cityName}</span></p>
                            <p class="card-control">
                                <a href="${(window.location.origin + "/House/Details?Id=" + data.id)}" class="btn btn-primary" title="Xem chi tiết">Chi tiết</a>
                                <button type="button" class="btn btn-warning" onclick="editHouse(${data.id})">Chỉnh sửa</button>
                                <button type="button" class="btn btn-danger" onclick="deleteHouse(${data.id})">Xóa</button>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
    `;
}
function reloadPage() {
    $("#list-house").html(null);
    for (let e in listHouse) {
        $("#list-house").append(houseItem(listHouse[e]));
    }
}


//lấy bước hiện tại
//thêm thông tin từng bước
//Làm cái form mới => khi yêu cầu thì show lại
function getStep() {
    if (data.name == null || data.option == null || data.description == null) {
        return 1;
    }
    if (data.people == null || data.bedRoom == null || data.bathRoom == null
        || data.square == null || data.location == null || data.price == null
        || data.bed == null || data.idCity == 0) {
        return 2;
    }

    if (data.images.join("").length > 1) {
        return 5;
    }
    return 4;
}
function runBackStep(step) {
    let isBack = false;
    if (step == 1) {
        if (data.name == null) {
            isBack = true;
            $("#houseName-validate").html("Hãy thêm tên gợi nhớ cho nhà của bạn");
        }
        else {
            $("#houseName-validate").html(null);
        }
        if (data.option == null) {
            isBack = true;
            $("#option-frame-validate").html("Hãy chọn loại nhà");
        }
        else {
            $("#option-frame-validate").html(null);
        }
        if (data.description == null) {
            isBack = true;
            $("#houseDesc-validate").html("Hãy thêm mô tả nhà");
        }
        else {
            $("#houseDesc-validate").html(null);
        }
    }
    else if (step == 2) {
        if (data.people == null) {
            isBack = true;
            $("#numUser-validate").html("Hãy thêm số người có thể ở");
        }
        else {
            $("#numUser-validate").html(null);
        }
        if (data.bedRoom == null) {
            isBack = true;
            $("#numSleep-validate").html("Hãy thêm số phòng ngủ");
        }
        else {
            $("#numSleep-validate").html(null);
        }
        if (data.bathRoom == null) {
            isBack = true;
            $("#numBath-validate").html("Hãy thêm số phòng tắm");
        }
        else {
            $("#numBath-validate").html(null);
        }
        if (data.square == null) {
            isBack = true;
            $("#numSquare-validate").html("Hãy thêm diện tích nhà");
        }
        else {
            $("#numSquare-validate").html(null);
        }
        if (data.location == null) {
            isBack = true;
            $("#location-validate").html("Hãy điền địa chỉ nhà");
        }
        else {
            $("#location-validate").html(null);
        }
        if (data.price == null) {
            isBack = true;
            $("#numPrice-validate").html("Hãy thêm giá mong muốn trao đổi");
        }
        else {
            $("#numPrice-validate").html(null);
        }

        if (data.bed == null) {
            isBack = true;
            $("#numBed-validate").html("Hãy thêm số giường trao đổi");
        }
        else {
            $("#numBed-validate").html(null);
        }
    }
    else if (step == 4) {
        if (data.images.join("").length <= 1) {
            isBack = true;
            $("input-picture-validate").html("Hãy thêm hình ảnh cho căn nhà");
        }
        else {
            $("#input-picture-validate").html(null);
        }
    }
    if (isBack) {
        $(".modal-body.sigshow").removeClass("sigshow");
        $(".modal-body.sighide").removeClass("sighide");
        $(".modal-body").addClass("sighide");
        $(".modal-body")[step - 1].classList.add("sigshow");
    }
}
function distanceTo(lat1, lon1, lat2, lon2) {
    let rlat1 = Math.PI * lat1 / 180;
    let rlat2 = Math.PI * lat2 / 180;
    let theta = lon1 - lon2;
    let rtheta = Math.PI * theta / 180;
    let dist =
        Math.sin(rlat1) * Math.sin(rlat2) + Math.cos(rlat1) *
        Math.cos(rlat2) * Math.cos(rtheta);
    dist = Math.acos(dist);
    dist = dist * 180 / Math.PI;
    dist = dist * 60 * 1.1515;

    return dist;
}
function checkAddress() {
    if ($("#map-location").val().length == 0) {
        $("#map-location-validate").html("Hãy điền địa chỉ nhà");
        return false;
    }
    else {
        $("#map-location-validate").html("");
        return true;
    }
}
function getPredict() {
    if (data.people != null && data.square != null && data.lat != 0 && data.lng != 0 && data.idCity != 0) {
        let objSend = {
            idCity: data.idCity,
            lat: data.lat,
            lng: data.lng,
            capacity: data.people,
            area: data.area,
            rating: 0
        }
        $.ajax({
            url: window.location.origin + "/Predict?" + Object.keys(objSend).map(function (key) { return key + '=' + objSend[key]; }).join('&'),
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                $(".form.suggest").html(`<small>Giá gợi ý từ hệ thống: <strong>${data.data} Point/ngày</strong></small>`);
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}