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
    $("#numBed").val(temp_data.bed);
    $("#location").val(temp_data.location + ", " + temp_data.wardName + ", " + temp_data.districtName + ", " + temp_data.cityName);
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
    $("#district-select").val(temp_data.idDistrict);
    $("#ward-select").val(temp_data.idWard);
    getDataCityForHouse(temp_data);
    $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>" + temp_data.location);
    loc = new Microsoft.Maps.Location(temp_data.lat, temp_data.lng);
    reloadMap(temp_data.location);
    //thêm thêm cái edit house
    $("#map-location").val(temp_data.location);
    getPredict();
}

function getDataCityForHouse(data_house) {
    if (dataLocation == null) {
        $.ajax({
            url: "/api/Location/City",
            type: "GET",
            dataType: "json",
            success: function (data) {
                let str = "{";
                for (e in data.data) {
                    let omodel = JSON.stringify(data.data[e]);
                    let model = "\"" + data.data[e]["id"] + "\":" + omodel.slice(0, omodel.length - 1) + ", \"district\": null}";
                    str += model + ", ";
                }
                str = str.trim();
                str = str.slice(0, str.length - 1);
                str += "}";
                dataLocation = JSON.parse(str);

                let bingStr = "{";
                for (e in data.data) {
                    let omodel = JSON.stringify(data.data[e]);
                    let model = "\"" + data.data[e]["bingName"] + "\":" + omodel.slice(0, omodel.length - 1) + ", \"district\": null}";
                    bingStr += model + ", ";
                }
                bingStr = bingStr.trim();
                bingStr = bingStr.slice(0, bingStr.length - 1);
                bingStr += "}";
                dataBingLocation = JSON.parse(bingStr);
                changeDataForSelect2("#city-select", getDataFormat(dataLocation, "Tỉnh/Thành", data_house.idCity));
                getDataDistrictForHouse(data_house);
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
    else {
        changeDataForSelect2("#city-select", getDataFormat(dataLocation, "Tỉnh/Thành", data_house.idCity));
        getDataDistrictForHouse(data_house);
    }
}
function getDataDistrictForHouse(data_house) {
    if (dataLocation == null) {
        getDataCityForHouse(data_house);
    }
    else {
        if (dataLocation[data_house.idCity].district == null) {
            $.ajax({
                url: "/api/Location/District?IdCity=" + data_house.idCity,
                type: "GET",
                dataType: "json",
                success: function (data) {
                    var str = "{";
                    for (e in data.data) {
                        let omodel = JSON.stringify(data.data[e]);
                        let model = "\"" + data.data[e]["id"] + "\":" + omodel.slice(0, omodel.length - 1) + ", \"ward\": null}";
                        str += model + ", ";
                    }
                    str = str.trim();
                    str = str.slice(0, str.length - 1);
                    str += "}";
                    dataLocation[data_house.idCity].district = JSON.parse(str);

                    let bingStr = "{";
                    for (e in data.data) {
                        let omodel = JSON.stringify(data.data[e]);
                        let model = "\"" + data.data[e]["bingName"] + "\":" + omodel.slice(0, omodel.length - 1) + ", \"ward\": null}";
                        bingStr += model + ", ";
                    }

                    bingStr = bingStr.trim();
                    bingStr = bingStr.slice(0, bingStr.length - 1);
                    bingStr += "}";
                    console.log(bingStr);
                    dataBingLocation[dataLocation[data_house.idCity].bingName].district = JSON.parse(bingStr);

                    let model = JSON.stringify(data.data).replaceAll("name", "text");

                    changeDataForSelect2("#district-select", getDataFormat(dataLocation[data_house.idCity].district, "Quận / Huyện", data_house.idDistrict));
                    getDataWardForHouse(data_house);
                },
                error: function (e) {
                    console.log(e);
                }
            });
        }
        else {
            changeDataForSelect2("#district-select", getDataFormat(dataLocation[data_house.idCity].district, "Quận / Huyện", data_house.idDistrict));
            getDataWardForHouse(data_house);
        }
    }
}
function getDataWardForHouse(data_house) {
    if (dataLocation == null) {
        getDataCity();
    }
    else {
        if (dataLocation[data_house.idCity].district[data_house.idDistrict].ward == null) {
            $.ajax({
                url: "/api/Location/Ward?IdDistrict=" + data_house.idDistrict,
                type: "GET",
                dataType: "json",
                success: function (data) {
                    let str = "{";
                    for (e in data.data) {
                        let model = "\"" + data.data[e]["id"] + "\":" + JSON.stringify(data.data[e]);
                        str += model + ", ";
                    }
                    str = str.trim();
                    str = str.slice(0, str.length - 1);
                    str += "}";
                    dataLocation[data_house.idCity].district[data_house.idDistrict].ward = JSON.parse(str);

                    let bingStr = "{";
                    for (e in data.data) {
                        let model = "\"" + data.data[e]["id"] + "\":" + JSON.stringify(data.data[e]);
                        bingStr += model + ", ";
                    }
                    bingStr = bingStr.trim();
                    bingStr = bingStr.slice(0, bingStr.length - 1);
                    bingStr += "}";
                    dataBingLocation[dataLocation[data_house.idCity].bingName].district[dataLocation[data_house.idCity].district[data_house.idDistrict].bingName].ward = JSON.parse(bingStr);

                    let model = JSON.stringify(data.data).replaceAll("name", "text");
                    changeDataForSelect2("#ward-select", getDataFormat(dataLocation[data_house.idCity].district[data_house.idDistrict].ward, "Phường / Xã", data_house.idWard));
                },
                error: function (e) {
                    console.log(e);
                }
            });
        }
        else {
            changeDataForSelect2("#ward-select", getDataFormat(dataLocation[data_house.idCity].district[data_house.idDistrict].ward, "Phường / Xã", data_house.idWard));
        }
    }
}