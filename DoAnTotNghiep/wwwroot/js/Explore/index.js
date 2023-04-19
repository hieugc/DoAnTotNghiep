window.onload = function () {
    slideOne();
    slideTwo();
}
var infobox = null;
let minGap = 0;
let sliderTrack = document.querySelector(".slider-track");
let sliderMaxValue = document.getElementById("slider-1").max;
function slideOne() {
    if (parseInt($("#slider-2").val()) - parseInt($("#slider-1").val()) <= minGap) {
        $("#slider-1").val(parseInt($("#slider-2").val()) - minGap)
    }
    $("#range1").text($("#slider-1").val());
    fillColor();
}
function slideTwo() {
    if (parseInt($("#slider-2").val()) - parseInt($("#slider-1").val()) <= minGap) {
        $("#slider-2").val(parseInt($("#slider-1").val()) + minGap);
    }
    $("#range2").text($("#slider-2").val());
    fillColor();
}
function fillColor() {
    percent1 = ($("#slider-1").val() / sliderMaxValue) * 100;
    percent2 = ($("#slider-2").val() / sliderMaxValue) * 100;
    sliderTrack.style.background = `linear-gradient(to right, #dadae5 ${percent1}% , #3264fe ${percent1}% , #3264fe ${percent2}%, #dadae5 ${percent2}%)`;
}
function getOptionSort() {
    return "&optionSort=" + $(`input[type=radio][name=filter-sort]:checked`).val();
}
function getPeople() {
    if ($("input[name=people-value]").val().length > 0) {
        return "&people=" + $("input[name=people-value]").val();
    }
    return "";
}
function getRangePrice() {
    return "&priceStart=" + $("#range1").text()
        + "&priceEnd=" + $("#range2").text();
}
function getUtilities() {
    let utilities = [];
    let filter_uti = $(".filter-option input[type=checkbox]:checked");
    for (let index = 0; index < filter_uti.length; index++) {
        utilities[utilities.length] = Number(filter_uti[index].id.split("-").pop());
    }

    if (utilities.length > 0) {
        return "&utilitest=[" + utilities.join(", ") + "]";
    }
    return "";
}
function getRangeDate() {
    let res = "";
    if (picker.options.startDate != null) {
        res += "&dateStart=" + enDate(picker.options.startDate);
    }
    if (picker.options.startDate != null &&
        picker.options.endDate != null
        && picker.options.startDate.getTime() != picker.options.endDate.getTime()) {
        res += "&dateEnd=" + enDate(picker.options.endDate);
    }
    return res;
}
function getPage(page, limit) {
    return "&page=" + page + "&limit=" + limit;
}
function getFilter(page, limit) {
    let idDistrict = null;
    let idCity = 0;
    if (window.location.href.toLowerCase().indexOf("idcity") != -1) {
        let arr = window.location.href.split("?")[1].split("&");
        for (e in arr) {
            if (arr[e].toLowerCase().indexOf("idcity") != -1) {
                idCity = arr[e].split("=").pop();
                break;
            }
        }
    }
    if (suggest != null) idCity = includeIdCity();
    return "?idCity=" + idCity +
        getOptionSort() +
        getPeople() +
        getRangePrice() +
        getUtilities() +
        getRangeDate() +
        getPage(page, limit);
}
function initFilter() {
    $("#filter-sort-1").click();
    $("input[name=people-value]").val(null);
    $("#slider-1").val($("#slider-1")[0].min);
    $("#slider-2").val($("#slider-2")[0].max);
    slideOne();
    slideTwo();
    $(".filter-option input[type=checkbox]:checked").prop("checked", false);
    picker.clearSelection();
}
function search(page, limit) {
    $.get(
        window.location.origin + "/Explore/Search" + getFilter(page, limit),
        function (data) {
            $("#render-result").html(data);
            GetMap();
        }
    )
}
function enDate(date) {
    return date.dateInstance.toISOString().split("T")[0];
}
function initRangeDate() {
    return new Litepicker({
        element: document.getElementById('daterange'),
        singleMode: false,
        tooltipText: {
            one: 'ngày',
            other: 'ngày'
        },
        dropdowns:
        {
            "minYear": new Date().getFullYear(),
            "maxYear": null,
            "months": false,
            "years": false
        },
        lockDays: [[new Date(new Date().getFullYear() + "-01-01").toISOString().split("T")[0]
            , (new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-" + (new Date().getDate() - 1))]],
        tooltipNumber: (totalDays) => {
            return totalDays;
        }   
    });
}
function filter() {
    search(1, 12);
}
function infoboxTemplate(title, index, description) {
    return `<div class="customInfobox">
                <div class="content">
                    <div class="title">${title}</div>
                    <div class="address"><strong>Địa chỉ:</strong> ${description}</div>
                    <div class="price"><strong>Giá:</strong> ${houseResult[index].price}</div>
                    <div class="click"><a href="${window.location.origin + "/Houses/Details/" + houseResult[index].id}" class="btn btn-primary btn-sm"><i class="fa-solid fa-magnifying-glass"></i></a></div>
                </div>
            </div>`;
}
function showItemMap() {
    if (infobox != null) {
        infobox.setMap(null);
        infobox = null;
    }
    for (e in houseResult) {
        let itemloc = new Microsoft.Maps.Location(houseResult[e].lat, houseResult[e].lng);
        var pin = createFontPushpin(itemloc, '\uF3C5', 'FontAwesome', 45, '#2c3e59', houseResult[e], e);
        map.entities.push(pin);
        Microsoft.Maps.Events.addHandler(pin, 'mouseover', displayInfobox);
        Microsoft.Maps.Events.addHandler(pin, 'mouseout', hideInfobox);
    }
}
function createFontPushpin(location, text, fontName, fontSizePx, color, object, index) {
    var c = document.createElement('canvas');
    var ctx = c.getContext('2d');

    //Define font style
    var font = fontSizePx + 'px ' + fontName;
    ctx.font = font

    //Resize canvas based on sie of text.
    var size = ctx.measureText(text);
    c.width = size.width;
    c.height = fontSizePx;

    //Reset font as it will be cleared by the resize.
    ctx.font = font;
    ctx.textBaseline = 'top';
    ctx.fillStyle = color;

    ctx.fillText(text, 0, 0);

    return new Microsoft.Maps.Pushpin(location, {
        icon: c.toDataURL(),
        anchor: new Microsoft.Maps.Point(c.width / 2, c.height / 2),
        title: object.name,
        subTitle: object.location,
        catId: index
    });
}
function displayInfobox(e) {
    if (infobox != null) {
        infobox.setMap(null);
    }
    infobox = new Microsoft.Maps.Infobox(e.target.getLocation(), {
        htmlContent: infoboxTemplate(e.target.entity.title, e.target.catId, e.target.entity.subtitle)
    });
    infobox.setMap(map);
    $(".house-card.selected").removeClass("selected");
    $(".house-card")[e.target.catId].classList.add("selected");
}
function hideInfobox(e) {
    if (infobox != null) {
        infobox.setMap(null);
    }
    $(".house-card.selected").removeClass("selected");
}

function showInfobox(index) {
    if (infobox != null) {
        infobox.setMap(null);
        infobox = null;
    }
    let location = map.entities._primitives[index].entity.subtitle;
    let title = map.entities._primitives[index].entity.title;
    infobox = new Microsoft.Maps.Infobox(map.entities._primitives[index].getLocation(), {
        htmlContent: infoboxTemplate(title, index, location)
    });
    infobox.setMap(map);
}
/*
function pagination(model) {
    let res = `<nav aria-label="Page navigation example">
        <ul class="pagination justify-content-center">`;
    let frame = Math.floor(model.pagination.page / 5);
    let lastframe = Math.floor(model.pagination.total / 5);

    if (model.pagination.total > 5 && frame > 0) {
        res += `<li class="page-item" onclick="search(${model.pagination.page - 1}, ${model.pagination.limit})"><span class="page-link" aria-hidden="true">&laquo;</span></li>`
    }

    for (let page = 1; page <= 5; page++) {
        if ((page + frame * 5) <= model.pagination.total) {
            if (page == (model.pagination.page % 5 + 1)) {
                res += `<li class="page-item active" onclick="search(${page + frame * 5}, ${model.pagination.limit})"><a class="page-link" href="#">${page + frame * 5}</a></li>`;
            }
            else {
                res += `<li class="page-item" onclick="search(${page + frame * 5}, ${model.pagination.limit})"><a class="page-link" href="#">${page + frame * 5}</a></li>`;
            }
        }
    }

    if (model.pagination.total > 5 && frame < lastframe) {
        res += `<li class="page-item" onclick="search(${model.pagination.page + 1}, ${model.pagination.limit})"><span class="page-link" aria-hidden="true">&raquo;</span></li>`;
    }

    res += `
        </ul>
    </nav>`;

    return res;
}

function createModel(page, limit, total) {
    return {
        pagination: {
            page: page,
            limit: limit,
            total: total
        }
    };
}


function renderPagination() {
    for (let index = 0; index < 20; index++) {
        let number = randomNumber(60, 300);
        let limit = 12;
        let total = Math.ceil(number / limit);
        let page = randomNumber(1, total);
        let model = createModel(page, limit, total);
        console.log(model);
        $(".result").append(pagination(model));
    }
}
function randomNumber(min, max) {
    return Math.floor(Math.random() * (max - min) + min);
}*/