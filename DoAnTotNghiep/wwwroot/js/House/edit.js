function editHouse(id) {
    refreshHouseModal();
    let editHouse = null;
    for (e in listHouse) {
        if (listHouse[e].id == id) {
            editHouse = listHouse[e];
            break;
        }
    }
    getHouseModalWithData(editHouse);
}
function getHouseModalWithData(temp_data) {
    if (temp_data != null) {
        let images = [null, null, null, null];
        for (let e in temp_data.images) {
            images[e] = temp_data.images[e];
        }
        temp_data.images = images;
        configData(temp_data);
        //$.ajax({
        //    url: window.location.origin + "/Houses/GetImagesOfHouse?IdHouse=" + data.id,
        //    contentType: "application/json",
        //    method: "GET",
        //    success: function (result) {
        //        if (result.status == 200) {
        //            for (e in listHouse) {
        //                if (listHouse[e].id == result.idHouse) {
        //                    listHouse[e].images = result.data;
                            
        //                    break;
        //                }
        //            }
        //        }
        //    },
        //    error: function (error) {
        //        console.log(error);
        //    }
        //});
    }
}
function configData(temp_data) {
    var picture_frame = document.getElementsByClassName("picture-frame");
    for (e in temp_data.images) {
        if (temp_data.images[e] != null) {
            picture_frame[e].classList.add("picture-frame-added");
            if (temp_data.images[e].data != "") {
                picture_frame[e].innerHTML = importImage(temp_data.images[e].data, e);
            }
            else {
                picture_frame[e].innerHTML = importImage((window.location.origin + "/" + temp_data.images[e].folder + "/" + temp_data.images[e].name), e);
            }
        }
        else {

        }
    }
    $("#houseName").val(temp_data.name);
    document.getElementsByClassName("option-frame")[0].children[temp_data.option - 1].classList.value += " option-selected";
    $("#houseDesc").val(temp_data.description);
    $("#numUser").val(temp_data.people);
    $("#numSleep").val(temp_data.bedRoom);
    $("#numBath").val(temp_data.bathRoom);
    $("#numSquare").val(temp_data.square);
    $("#location").val(temp_data.location);
    $("#numPrice").val(temp_data.price);
    for (e in temp_data.utilities) {
        document.getElementById("list-utilities").getElementsByClassName("option")[temp_data.utilities[e] - 1].classList.add("option-selected");
    }
    for (e in temp_data.rules) {
        document.getElementById("list-rules").getElementsByClassName("option")[temp_data.rules[e] - 1].classList.add("option-selected");
    }
    houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[3].children[1].classList.remove("btn-no-drop");
    data = temp_data;
    $("#houseModalToggleClick").click();
    $("#city-select").val(temp_data.idCity);
    $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>" + temp_data.location);
    loc = new Microsoft.Maps.Location(temp_data.lat, temp_data.lng);
    //reloadMap(temp_data.location);
    //thêm thêm cái edit house
}