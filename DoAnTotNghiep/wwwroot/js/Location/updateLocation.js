var dataLocation = null;
var left = null;
var right = null;


//lấy danh sách parse ngược lại => 

function getDataCity(_left, _right) {
    left = _left;
    right = _right
    if (dataLocation == null) {
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
                for (e in dataLocation) {
                    getDataDistrict(dataLocation[e].id);
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}
function getDataDistrict(id) {
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

                for (e in dataLocation[id].district) {
                    getDataWard(dataLocation[id].id, dataLocation[id].district[e].id);
                }

            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}
function getDataWard(id_city, id) {
    if (dataLocation[id_city].district[id].ward == null) {
        $.ajax({
            url: "/Location/GetWard?Id=" + id,
            type: "GET",
            dataType: "json",
            success: function (data) {
                if (data.ward.length > 0) {
                    var str = "{";
                    for (e in data.ward) {
                        let model = "\"" + data.ward[e]["id"] + "\":" + JSON.stringify(data.ward[e]);
                        str += model + ", ";
                    }
                    str = str.trim();
                    str = str.slice(0, str.length - 1);
                    str += "}";
                    dataLocation[id_city].district[id].ward = JSON.parse(str);
                    for (e in dataLocation[id_city].district[id].ward) {
                        if (left <= dataLocation[id_city].district[id].ward[e].id && dataLocation[id_city].district[id].ward[e].id <= right && dataLocation[id_city].district[id].ward[e].isUpdated == false) {
                            getMap(dataLocation[id_city].district[id].ward[e].name + ", " + dataLocation[id_city].district[id].name + ", " + dataLocation[id_city].name, "locality"
                                , (window.location.origin + "/Ward/Update"), dataLocation[id_city].district[id].ward[e].id);
                        }
                    }
                }
                else {
                    dataLocation[id_city].district[id].ward = null;
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
}
function updateLocation(url, data) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        contentType: "application/json",
        type: "PUT",
        dataType: "json",
        success: function (data) {
            console.log(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
}

//locality
//adminDistrict
//locality
//adminDistrict2
// quận + huyện lấy locality
//    => lúc parse ngược lại có thể không có adminDistrict2
//      => locality có thể là ward hoặc district => mặc định là district
//          => rồi cảnh báo cho người dùng chọn lại

function getMap(address, column, urlsend, _id) {
    var url_temp = 'http://dev.virtualearth.net/REST/v1/Locations?query=' + address + '&key=Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk';

    $.ajax({
        url: url_temp,
        dataType: "jsonp",
        jsonp: "jsonp",
        success: function (data) {
            if (data.statusCode == 200) {
                var location = data.resourceSets[0].resources[0];
                let dataSend = {
                    id: _id,
                    "bingName": location["address"][column],
                    "lat": location.geocodePoints[0].coordinates[0],
                    "lng": location.geocodePoints[0].coordinates[1]
                }
                updateLocation(urlsend, dataSend);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

