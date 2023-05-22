function getDataCity() {
    if (dataLocation == null) {
        if ($("#district-select").length > 0) {
            changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }]);
            $("#district-select").prop("disabled", true);
        }
        if ($("#ward-select").length > 0) {
            changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
            $("#ward-select").prop("disabled", true);
        }
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

                let model = JSON.stringify(data.data).replaceAll("name", "text");

                if ($("#city-select").length > 0) {
                    changeDataForSelect2("#city-select", [{ "id": -1, "text": "Thành phố / Tỉnh" }].concat(JSON.parse(model)));
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}
function getDataDistrict() {
    var id = $("#city-select").val()
    if (dataLocation == null) {
        getDataCity();
    }
    else {
        if (!(id in dataLocation)) {
            if ($("#district-select").length > 0) {
                changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }]);
                $("#district-select").prop("disabled", true);
            }
            if ($("#ward-select").length > 0) {
                changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
                $("#ward-select").prop("disabled", true);
            }
        }
        else {
            if ($("#ward-select").length > 0) {
                changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
                $("#ward-select").prop("disabled", true);
            }
            if (dataLocation[id].district == null) {
                $.ajax({
                    url: "/api/Location/District?IdCity=" + id,
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
                        dataLocation[id].district = JSON.parse(str);

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
                        dataBingLocation[dataLocation[id].bingName].district = JSON.parse(bingStr);

                        let model = JSON.stringify(data.data).replaceAll("name", "text");
                        
                        if ($("#district-select").length > 0) {
                            changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }].concat(JSON.parse(model)));
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }
            else {
                if ($("#district-select").length > 0) {
                    changeDataForSelect2("#district-select", getDataFormat(dataLocation[id].district, "Quận / Huyện"));
                }
            }
        }
    }
}
function getDataFormat(data, text) {
    let arr = [{"id": -1, "text": text}];
    for (index in data) {
        arr[arr.length] = { "id": data[index]["id"], "text": data[index]["name"] }
    }
    return arr;
}
function getDataWard() {
    if (dataLocation == null) {
        getDataCity();
    }
    else {
        var id_city = $("#city-select").val();
        var id = $("#district-select").val();
        if (!(id_city in dataLocation)) {
            if ($("#district-select").length > 0) {
                changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }]);
                $("#district-select").prop("disabled", true);
            }
            if ($("#ward-select").length > 0) {
                changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
                $("#ward-select").prop("disabled", true);
            }
        }
        else if(dataLocation[id_city].district == null || !(id in dataLocation[id_city].district)) {

            if ($("#ward-select").length > 0) {
                changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
                $("#ward-select").prop("disabled", true);
            }
        }
        else{
            if (dataLocation[id_city].district[id].ward == null) {
                $.ajax({
                    url: "/api/Location/Ward?IdDistrict=" + id,
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
                        dataLocation[id_city].district[id].ward = JSON.parse(str);

                        let bingStr = "{";
                        for (e in data.data) {
                            let model = "\"" + data.data[e]["id"] + "\":" + JSON.stringify(data.data[e]);
                            bingStr += model + ", ";
                        }
                        bingStr = bingStr.trim();
                        bingStr = bingStr.slice(0, bingStr.length - 1);
                        bingStr += "}";
                        dataBingLocation[dataLocation[id_city].bingName].district[dataLocation[id_city].district[id].bingName].ward = JSON.parse(bingStr);

                        let model = JSON.stringify(data.data).replaceAll("name", "text");

                        if ($("#ward-select").length > 0) {
                            changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }].concat(JSON.parse(model)));
                        }
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }
            else {
                if ($("#ward-select").length > 0) {
                    changeDataForSelect2("#ward-select", getDataFormat(dataLocation[id_city].district[id].ward, "Phường / Xã"));
                }
            }
        }
    }
}
function changeDataForSelect2(tag, data) {
    if ($(tag)[0].disabled) {
        $(tag).prop("disabled", false);
    }

    if ($(tag).length > 0) {
        $(tag).select2('destroy');
        $(tag).empty();
        $(tag).select2({
            data: data,
            dropdownParent: $('#BingMapModal')
        });
    }
}
function initCitySelect2(elementTag, data, placeholder) {
    $(elementTag).select2({
        placeholder: placeholder,
        data: data
    });
}
function initSelect() {
    if (window.location.href.indexOf("/Member/History") == -1) {
        if ($("#city-select").length > 0) {
            $("#city-select").select2({
                dropdownParent: $('#BingMapModal')
            });
        }
        if ($("#district-select").length > 0) {
            $("#district-select").select2({
                dropdownParent: $('#BingMapModal')
            });
        }
        if ($("#ward-select").length > 0) {
            $("#ward-select").select2({
                dropdownParent: $('#BingMapModal')
            });
        }
    }
}
function showAddress(address) {
    if ($("#mapAddress").html().length == 0 || $("#map-location").val().length == 0) {
        $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>" + address);
    }
}
function hideMainForm() {
    $("#houseModalToggle").hide();
    if ($("#mapAddress").text().replace("Địa chỉ nhà: ", "").length > 0 && loc != null) {
        getLocation(null);
    }
}
function showMainForm() {
    $("#location").val($("#mapAddress").text().replace("Địa chỉ nhà: ", ""));
    if ($("#location").val().length > 0) {
        data.location = $("#map-location").val();
        if ($("#city-select").val() != -1) {
            data.idCity = $("#city-select").val();
            if ($("#district-select").val() != -1) {
                data.idDistrict = $("#district-select").val();
                if ($("#ward-select").val() != -1) {
                    data.idWard = $("#ward-select").val();
                }
            }
        }        
        if (loc != null) {
            data.lat = loc.latitude;
            data.lng = loc.longitude;
        }
        getPredict();
    }
    $("#location-validate").html(null);
    $("#houseModalToggle").show();
}
function reloadMap(address) {
    if (pin != null) {
        map.entities.clear();
    }
    if (infobox != null) {
        infobox.setMap(null);
    }
    map.setView({
        center: new Microsoft.Maps.Location(loc.latitude, loc.longitude)
    });

    infobox = new Microsoft.Maps.Infobox(loc, {
        title: data.name,
        description: address
    });

    pin = new Microsoft.Maps.Pushpin(loc, {
        title: data.name,
        subTitle: address
    });

    map.entities.push(pin);
    infobox.setMap(map);
    showAddress(address);
}
function getLocation(temp_address) {
    if (checkAddress()) {
        if (temp_address != null) {
            address = temp_address
        }
        else {
            address = "adminDistrict=" + encodeURIComponent(dataLocation[$("#city-select").val()].bingName)
                + "&locality=" + encodeURIComponent(dataLocation[$("#city-select").val()].district[$("#district-select").val()].bingName)
                + "&addressLine=" + encodeURIComponent($("#map-location").val() + ", " + dataLocation[$("#city-select").val()].district[$("#district-select").val()].ward[$("#ward-select").val()].bingName);
        }
        var url_temp = window.location.protocol + '//dev.virtualearth.net/REST/v1/Locations?CountryRegion=VN&' + address + '&key=' + key;

        $.ajax({
            url: url_temp,
            dataType: "jsonp",
            jsonp: "jsonp",
            success: function (data) {
                if (data.statusCode == 200) {
                    var result = data.resourceSets[0].resources[0].geocodePoints[0].coordinates;
                    loc = new Microsoft.Maps.Location(result[0], result[1]);
                    let temp_address = $("#map-location").val()
                        + ", " + dataLocation[$("#city-select").val()].district[$("#district-select").val()].ward[$("#ward-select").val()].name
                        + ", " + dataLocation[$("#city-select").val()].district[$("#district-select").val()].name
                        + ", " + dataLocation[$("#city-select").val()].name;
                    reloadMap(temp_address);
                    $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>" + temp_address);
                    if ($("#city-select").val() != -1) {
                        data.idCity = $("#city-select").val();
                        if ($("#district-select").val() != -1) {
                            data.idDistrict = $("#district-select").val();
                            if ($("#ward-select").val() != -1) {
                                data.idWard = $("#ward-select").val();
                            }
                        }
                    }   
                    if (loc != null) {
                        data.lat = loc.latitude;
                        data.lng = loc.longitude;
                    }
                    getPredict();
                }
                else {
                    alert("Bạn hãy thử lại");
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}
function getIdCity(cityBingName) {
    $.ajax({
        url: window.location.origin + "/Location/GetCityId?BingName=" + cityBingName,
        contentType: "application/json",
        type: "GET",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                data.idCity = result.idCity;
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

var pin = null;
var infobox = null;
var loc = null;
var address = null;
var dataLocation = null; //data chính
var dataBingLocation = null; //data chính
initSelect();
getDataCity();


function StyleMap() {

    ////var myStyle = {
    ////    "version": "1.0",
    ////    "settings": {
    ////        "landColor": "#FF0B334D"
    ////    },
    ////    "elements": {
    ////        "mapElement": {
    ////            "labelColor": "#FFFFFFFF",
    ////            "labelOutlineColor": "#FF000000",
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "area": {
    ////            "fillColor": "#FF115166"
    ////        },
    ////        "neighborhood": {
    ////            "labelVisible": false
    ////        },
    ////        "point": {
    ////            "fillColor": "#FF000000",
    ////            "iconColor": "#FF0C4152",
    ////            "labelVisible": false,
    ////            "strokeColor": "#FF0C4152",
    ////            "visible": false
    ////        },
    ////        "naturalPoint": {
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "pointOfInterest": {
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "populatedPlace": {
    ////            "labelVisible": false,
    ////            "visible": false
    ////        },
    ////        "capital": {
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "adminDistrictCapital": {
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "countryRegionCapital": {
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "roadExit": {
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "transit": {
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "political": {
    ////            "borderOutlineColor": "#00000000",
    ////            "borderStrokeColor": "#FF144B53",
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "adminDistrict": {
    ////            "borderVisible": true,
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "countryRegion": {
    ////            "borderVisible": false,
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "district": {
    ////            "borderVisible": true,
    ////            "labelVisible": true,
    ////            "visible": true
    ////        },
    ////        "structure": {
    ////            "fillColor": "#FF115166",
    ////            "labelVisible": true
    ////        },
    ////        "transportation": {
    ////            "fillColor": "#FF000000",
    ////            "labelVisible": true,
    ////            "overwriteColor": false,
    ////            "strokeColor": "#FF000000",
    ////            "visible": true
    ////        },
    ////        "railway": {
    ////            "fillColor": "#FF000000",
    ////            "strokeColor": "#FF146474"
    ////        },
    ////        "arterialRoad": {
    ////            "fillColor": "#FF000000",
    ////            "strokeColor": "#FF157399"
    ////        },
    ////        "controlledAccessHighway": {
    ////            "fillColor": "#FF000000",
    ////            "strokeColor": "#FF158399"
    ////        },
    ////        "highway": {
    ////            "fillColor": "#FF000000",
    ////            "strokeColor": "#FF158399"
    ////        },
    ////        "majorRoad": {
    ////            "fillColor": "#FF000000",
    ////            "strokeColor": "#FF157399"
    ////        },
    ////        "ramp": {
    ////            "overwriteColor": false,
    ////            "visible": false
    ////        },
    ////        "unpavedStreet": {
    ////            "labelVisible": false,
    ////            "visible": false
    ////        },
    ////        "water": {
    ////            "fillColor": "#FF021019",
    ////            "labelVisible": false
    ////        },
    ////        "userPoint": {
    ////            "labelVisible": true,
    ////            "stemColor": "#FFFFFFFF",
    ////            "visible": true
    ////        }
    ////    }
    ////};

    var myStyle= {
        "version": "1.0",
            "elements": {
            "mapElement": {
                "labelVisible": true,
                    "visible": true
            },
            "neighborhood": {
                "labelVisible": false
            },
            "point": {
                "labelVisible": false,
                    "visible": false
            },
            "naturalPoint": {
                "labelVisible": true,
                    "visible": true
            },
            "pointOfInterest": {
                "labelVisible": true,
                    "visible": true
            },
            "populatedPlace": {
                "labelVisible": false,
                    "visible": false
            },
            "capital": {
                "labelVisible": true,
                    "visible": true
            },
            "adminDistrictCapital": {
                "labelVisible": true,
                    "visible": true
            },
            "countryRegionCapital": {
                "labelVisible": true,
                    "visible": true
            },
            "roadExit": {
                "labelVisible": true,
                    "visible": true
            },
            "transit": {
                "labelVisible": true,
                    "visible": true
            },
            "political": {
                "labelVisible": true,
                    "visible": true
            },
            "adminDistrict": {
                "borderVisible": true,
                    "labelVisible": true,
                        "visible": true
            },
            "countryRegion": {
                "borderVisible": false,
                    "labelVisible": true,
                        "visible": true
            },
            "district": {
                "borderVisible": true,
                    "labelVisible": true,
                        "visible": true
            },
            "structure": {
                "labelVisible": true
            },
            "transportation": {
                "labelVisible": true,
                    "overwriteColor": false,
                        "visible": true
            },
            "ramp": {
                "overwriteColor": false,
                    "visible": false
            },
            "unpavedStreet": {
                "labelVisible": false,
                    "visible": false
            },
            "water": {
                "labelVisible": false
            },
            "userPoint": {
                "labelVisible": true,
                    "stemColor": "#FFFFFFFF",
                        "visible": true
            }
        }
    }


    return myStyle;
}
