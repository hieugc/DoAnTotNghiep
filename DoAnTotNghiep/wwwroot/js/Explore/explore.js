var suggest = null;
var moduleSuggest = null;
var manager = null;
var isGetSuggest = false;
var idCitySuggest = null;
function onLoad() {
    var options = { maxResults: 5 };
    manager = new Microsoft.Maps.AutosuggestManager(options);
    manager.attachAutosuggest('#explore-input', '#container-explore', selectedSuggestion);
}
function onError(message) {
    suggest = null;
}
function selectedSuggestion(suggestionResult) {
    suggest = suggestionResult;
}
function stringToSlug(str) {
    // remove accents
    var from = "àáãảạăằắẳẵặâầấẩẫậèéẻẽẹêềếểễệđùúủũụưừứửữựòóỏõọôồốổỗộơờớởỡợìíỉĩịäëïîöüûñçýỳỹỵỷ",
        to = "aaaaaaaaaaaaaaaaaeeeeeeeeeeeduuuuuuuuuuuoooooooooooooooooiiiiiaeiiouuncyyyyy";
    for (var i = 0, l = from.length; i < l; i++) {
        str = str.replace(RegExp(from[i], "gi"), to[i]);
    }

    str = str.toLowerCase()
        .trim()
        .replace(/[^a-z0-9\-]/g, '-')
        .replace(/-+/g, '-');

    return str;
}
function includeIdCity() {
    if (suggest != null) {
        for (e in dataLocation) {
            if (stringToSlug(suggest.address.adminDistrict).indexOf(stringToSlug(dataLocation[e].name)) != -1) {
                return e;
            }
        }
    }
}
function getSuggest() {
    /*
    moduleSuggest = Microsoft.Maps.loadModule('Microsoft.Maps.AutoSuggest', {
        callback: onLoad,
        errorCallback: onError
    });
    */
    document.getElementById("explore-input").addEventListener("input", function (event) {
        var node = this;
        setTimeout(function () {
            if (!isGetSuggest && node.value.length > 0) {
                isGetSuggest = true;
                $.ajax({
                    url: window.location.origin + `/api/Suggest?location=${node.value}`,
                    dataType: "json",
                    contentType: "application/json",
                    type: "GET",
                    success: function (result) {
                        console.log(result);
                        if (result.status == 200) {
                            $(".explore-location-result").html(null);
                            for (e in result.data) {
                                $(".explore-location-result").append(suggestItem(result.data[e]));
                            }
                            $("#explore-location-result div.dropdown-menu")[0].classList.add("show");
                        }
                        isGetSuggest = false;
                    },
                    error: function (error) {
                        console.log(error);
                        isGetSuggest = false;
                    }
                });
            }
        }, 1000);
    });
    document.getElementById("explore-input").addEventListener("keypress", function (e) {
        if (e.keyCode == 13) {
            selectSuggest(null);
        }
    });
    document.getElementById("explore-input").addEventListener("focusout", function (e) {
        setTimeout(function () {
            $("#explore-location-result div.dropdown-menu.show")[0].classList.remove("show");
        }, 200);
    });

    if ($("#myMap").length > 0 || $("#predict-render").length > 0) {
        GetMap();
    }
}

function explore(idCity) {
    if (window.location.href.indexOf("/Explore") == -1) {
        if (suggest == null) {
            window.location.href = window.location.origin + "/Explore/Index?location=" + $("#explore-input").val();
        }
        else {
            window.location.href = window.location.origin + "/Explore/Index?idCity=" + includeIdCity();
        }
    }
    else {
        search(1, 12);
    }
}
function suggestItem(data) {
    let res = `<img src="${window.location.origin}/Image/location.svg" alt="icon suggest">`
    if (data.districtName == null) {
        res += `<span>${data.cityName}</span>`;
    }
    else {
        res += `<span>${data.districtName}, ${data.cityName}</span>`;
    }
    return `<div onclick="selectSuggest(${data.idCity})" class="item-suggest">${res}</div>`;
}
function selectSuggest(idCity) {
    if (window.location.href.indexOf("/Explore") == -1) {
        if (idCity == null && $("#explore-input").val().length > 0) {
            window.location.href = window.location.origin + "/Explore/Index?location=" + $("#explore-input").val();
        }
        else if (idCity != null){
            window.location.href = window.location.origin + `/Explore/Index?idCity=${idCity}`;
        }
    }
    else {
        search(idCity, 1, 12);
        idCitySuggest = idCity;
    }
}