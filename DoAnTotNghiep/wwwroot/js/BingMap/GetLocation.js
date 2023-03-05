//function getDataLocation() {
//    $.ajax({
//        url: "/Location/Get",
//        type: "GET",
//        dataType: "json",
//        success: function (data) {
//            console.log(data);
//            if (data.status == 200) {
//                dataLocation = data.data;
//            }
//        },
//        error: function (e) {
//            console.log(e);
//        }
//    });
//} //chưa dùng
function getDataCity() {
    if (dataLocation == null) {
        changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }]);
        $("#district-select").prop("disabled", true);
        changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
        $("#ward-select").prop("disabled", true);
        $.ajax({
            url: "/Location/GetCity",
            type: "GET",
            dataType: "json",
            success: function (data) {
                var str = "{";
                for (e in data.city) {
                    let omodel = JSON.stringify(data.city[e]);
                    let model = "\"" + data.city[e]["id"] + "\":" + omodel.slice(0, omodel.length - 1) + ", \"district\": null}";
                    str += model + ", ";
                }
                str = str.trim();
                str = str.slice(0, str.length - 1);
                str += "}";
                dataLocation = JSON.parse(str);

                let model = JSON.stringify(data.city).replaceAll("name", "text");

                changeDataForSelect2("#city-select", [{ "id": -1, "text": "Thành phố / Tỉnh" }].concat(JSON.parse(model)));
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
            changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }]);
            $("#district-select").prop("disabled", true);
            changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
            $("#ward-select").prop("disabled", true);
        }
        else {
            changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
            $("#ward-select").prop("disabled", true);
            if (dataLocation[id].district == null) {
                $.ajax({
                    url: "/Location/GetDistrict?Id=" + id,
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        var str = "{";
                        for (e in data.district) {
                            let omodel = JSON.stringify(data.district[e]);
                            let model = "\"" + data.district[e]["id"] + "\":" + omodel.slice(0, omodel.length - 1) + ", \"ward\": null}";
                            str += model + ", ";
                        }
                        str = str.trim();
                        str = str.slice(0, str.length - 1);
                        str += "}";
                        dataLocation[id].district = JSON.parse(str);

                        let model = JSON.stringify(data.district).replaceAll("name", "text");
                        changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }].concat(JSON.parse(model)));
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }
            else {
                changeDataForSelect2("#district-select", getDataFormat(dataLocation[id].district, "Quận / Huyện"));
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
            changeDataForSelect2("#district-select", [{ "id": -1, "text": "Quận / Huyện" }]);
            $("#district-select").prop("disabled", true);
            changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
            $("#ward-select").prop("disabled", true);
        }
        else if(dataLocation[id_city].district == null || !(id in dataLocation[id_city].district)) {
            changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }]);
            $("#ward-select").prop("disabled", true);
        }
        else{
            if (dataLocation[id_city].district[id].ward == null) {
                $.ajax({
                    url: "/Location/GetWard?Id=" + id,
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        var str = "{";
                        for (e in data.ward) {
                            let model = "\"" + data.ward[e]["id"] + "\":" + JSON.stringify(data.ward[e]);
                            str += model + ", ";
                        }
                        str = str.trim();
                        str = str.slice(0, str.length - 1);
                        str += "}";
                        dataLocation[id_city].district[id].ward = JSON.parse(str);

                        let model = JSON.stringify(data.ward).replaceAll("name", "text");

                        changeDataForSelect2("#ward-select", [{ "id": -1, "text": "Phường / Xã" }].concat(JSON.parse(model)));
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }
            else {
                changeDataForSelect2("#ward-select", getDataFormat(dataLocation[id].district, "Phường / Xã"));
            }
        }
    }
}
function changeDataForSelect2(tag, data) {
    if ($(tag)[0].disabled) {
        $(tag).prop("disabled", false);
    }

    $(tag).select2('destroy');
    $(tag).empty();
    $(tag).select2({
        data: data,
        dropdownParent: $('#BingMapModal')
    });
}
function initCitySelect2(elementTag, data, placeholder) {
    $(elementTag).select2({
        placeholder: placeholder,
        data: data
    });
}
function initSelect() {
    $("#city-select").select2({
        dropdownParent: $('#BingMapModal')
    });
    $("#district-select").select2({
        dropdownParent: $('#BingMapModal')
    });
    $("#ward-select").select2({
        dropdownParent: $('#BingMapModal')
    });
}
function showAddress(address) {
    if ($("#mapAddress").html().length == 0 || $("#map-location").val().length == 0) {
        $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>" + address);
    }
}
function hideMainForm() {
    $("#houseModalToggle").hide();
}
function showMainForm() {
    $("#location").val($("#mapAddress").text().replace("Địa chỉ nhà: ", ""));
    if ($("#location").val().length > 0) {
        data.location = $("#location").val();
        if ($("#city-select").val() != -1) {
            data.idCity = $("#city-select").val();
        }
        if ($("#district-select").val() != -1) {
            data.idDistrict = $("#district-select").val();
        }
        if ($("#ward-select").val() != -1) {
            data.idWard = $("#ward-select").val();
        }
        if (loc != null) {
            data.lat = loc.latitude;
            data.lng = loc.longitude;
        }
    }
    $("#houseModalToggle").show();
}
function GetMap() {
    map = new Microsoft.Maps.Map('#myMap', {
        zoom: 15
    });

    Microsoft.Maps.Events.addHandler(map, 'click', function (e) {
        if (e.target === map) {
            loc = e.location;
            data.lat = loc.latitude;
            data.lng = loc.longitude;

            var url = 'https://dev.virtualearth.net/REST/v1/Locations/'
                + loc.latitude
                + ','
                + loc.longitude
                + '?key=' + key;

            $.ajax({
                url: url,
                dataType: "jsonp",
                jsonp: "jsonp",
                success: function (data) {
                    console.log(data);
                    if (data.statusCode == 200) {
                        var result = data.resourceSets[0].resources[0];
                        address = result.address.formattedAddress;
                        //if (dataLocation != null) {
                        //    if ($("#city-select").val()) {
                        //        let id_city = -1;
                        //        for (e in dataLocation) {
                        //            if (dataLocation[e].name.localeCompare(result.address.adminDistrict, undefined, { sensitivity: 'base' }) == 0) {
                        //                for (f in dataLocation[e].district) {
                        //                    if (dataLocation[e].district[f].name.localeCompare(result.address.adminDistrict2, undefined, { sensitivity: 'base' }) == 0)
                        //                }
                        //                id_city = e;

                        //            }
                        //        }
                        //    }
                        //}
                        reloadMap(address);
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
    });
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
function getLocation() {
    address = $("#map-location").val()
        + ", " + dataLocation[$("#city-select").val()].district[$("#district-select").val()].ward[$("#ward-select").val()].name
        + ", " + dataLocation[$("#city-select").val()].district[$("#district-select").val()].name
        + ", " + dataLocation[$("#city-select").val()].name;

    var url_temp = 'http://dev.virtualearth.net/REST/v1/Locations?query=' + address + '&key=' + key;
    
    $.ajax({
        url: url_temp,
        dataType: "jsonp",
        jsonp: "jsonp",
        success: function (data) {
            if (data.statusCode == 200) {
                var result = data.resourceSets[0].resources[0].geocodePoints[0].coordinates;
                loc = new Microsoft.Maps.Location(result[0], result[1]);
                reloadMap(address);
                $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>" + address);
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
function autoGetLocation() {
    if (dataLocation != null
        && $("#city-select").val() != -1
        && $("#district-select").val() != -1
        && $("#ward-select").val() != -1) {
        getDataLocation();
    }
}

var pin = null;
var infobox = null;
var map;
var loc = null;
var address = null;
var dataLocation = null; //data chính