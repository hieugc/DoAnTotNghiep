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
function getHouseModalWithData(data) {
    if (data != null) {
        $.ajax({
            url: window.location.origin + "/Houses/GetImagesOfHouse?IdHouse=" + data.id,
            contentType: "application/json",
            method: "GET",
            success: function (result) {
                if (result.status == 200) {
                    for (e in listHouse) {
                        if (listHouse[e].id == result.idHouse) {
                            listHouse[e].images = result.data;
                            configData(listHouse[e]);
                            break;
                        }
                    }
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
        
    }
}
function configData(temp_data) {
    var picture_frame = document.getElementsByClassName("picture-frame");
    for (e in temp_data.images) {
        picture_frame[e].classList.add("picture-frame-added");
        if (temp_data.images[e].data != "") {
            picture_frame[e].innerHTML = importImage(temp_data.images[e].data, e);
        }
        else {
            picture_frame[e].innerHTML = importImage((window.location.origin + "/" + temp_data.images[e].folder + "/" + temp_data.images[e].name), e);
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
        document.getElementById("list-utilities").getElementsByClassName("option")[e].classList.add("option-selected");
    }
    for (e in temp_data.rules) {
        document.getElementById("list-rules").getElementsByClassName("option")[e].classList.add("option-selected");
    }
    houseModal.getElementsByClassName("handle-step")[0].children[0].classList.remove("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[1].children[1].classList.remove("btn-no-drop");
    houseModal.getElementsByClassName("handle-step")[3].children[1].classList.remove("btn-no-drop");
    data = temp_data;
    $("#houseModalToggleClick").click();
}