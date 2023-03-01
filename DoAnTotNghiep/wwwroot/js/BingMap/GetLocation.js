function getDataLocation() {
    $.ajax({
        url: "/Location/Get",
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data.status == 200) {
                dataLocation = data.data;
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function getDataCity() {
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
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function getDataDistrict(id) {
    if (dataLocation != null && dataLocation[id].district == null) {
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
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}
function getDataWard(id_city, id) {
    if (dataLocation != null && dataLocation[id_city].district != null && dataLocation[id_city].district[id].ward == null) {
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
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}
function initCitySelect2(elementTag, data, placeholder) {
    $("#" + elementTag).select2({
        placeholder: placeholder,
        data: data
    });
}
function initCitySelect() {
    initCitySelect2("city-select", )
}
function showAddress(address) {
    $("#mapAddress").html("<strong>Địa chỉ nhà: </strong>" + address);
}
function hideMainForm() {
    $("#houseModalToggle").hide();
}
function showMainForm() {
    $("#BingMapModalClose").click();
    $("#houseModalToggle").show();
}
function GetMap() {
    map = new Microsoft.Maps.Map('#myMap', {
        zoom: 15
    });

    Microsoft.Maps.Events.addHandler(map, 'click', function (e) {
        if (e.target === map) {
            loc = e.location;
            console.log(loc);
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
        title: 'Vị trí nhà của bạn',
        description: address
    });

    pin = new Microsoft.Maps.Pushpin(loc, {
        title: data.nameHouse,
        subTitle: address
    });

    map.entities.push(pin);
    infobox.setMap(map);
    showAddress(address);
}
function getLocation() {
    address = add;

    var url_temp = 'http://dev.virtualearth.net/REST/v1/Locations?query=' + address + '&key=Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk';
    
    $.ajax({
        url: url_temp,
        dataType: "jsonp",
        jsonp: "jsonp",
        success: function (data) {
            if (data.statusCode == 200) {
                var result = data.resourceSets[0].resources[0].geocodePoints[0].coordinates;
                loc = new Microsoft.Maps.Location(result[0], result[1]);
                //loc = {
                //    altitude: 0,
                //    altitudeReference: -1,
                //    latitude: result[0],
                //    longitude: result[1]
                //};
                reloadMap(address);
                console.log(data);
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


var pin = null;
var infobox = null;
var map;
var loc = null;
var address = null;


var dataLocation = null;
getDataCity();