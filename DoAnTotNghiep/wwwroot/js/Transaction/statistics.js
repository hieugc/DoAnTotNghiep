var transactionZalo = [];
var selectedTab = 0;
var chart = null;
function getDataFormatNew(data) {
    let arr = [];
    for (index in data) {
        arr[arr.length] = { "id": data[index]["id"], "text": data[index]["name"] }
    }
    return arr;
}

function selectTab(index) {
    $(".nav-link")[selectedTab].classList.remove("active");
    $(".nav-link")[index].classList.add("active");
    $(".head-tab")[selectedTab].classList.add("tab-hide");
    $(".head-tab")[index].classList.remove("tab-hide");
    selectedTab = index;
    hidePredict();
    hideMaps();
    if (index == 0) {
        zaloTransaction();
    }
    else if (index == 1) {
        statisticHouse();
    }
    else {
        showPredict();
        //biểu đồ dự đoán
        
        //selectTab
        showLoader();
        initCitySelect2("#predictCity", getDataFormatNew(dataLocation, "Thành phố/Tỉnh"));
        //showMaps();
        /*
        console.log(dataLocation);
        
        initCitySelect2("#predictDistrict", getDataFormat([], "Quận/Huyện"), "Chọn quận/huyện");
        for (e in dataLocation) {
            getDistrictPoint(e);
            break;
        }*/

    }
}
function setUpData(listData, content, color, bgColor, option) {
    let dataPoints = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    if (option == 1) {
        for (let month = 0; month <= 11; month++) {
            let arr = listData.filter(
                function (e) { return new Date(e.createdDate.split("T")[0]).getMonth() == month; }
            ).map(function (e) { return e.amount; });

            if (arr.length > 0) dataPoints[month] = arr.reduce(function (total, num) { return total + num; });
        }
    }
    else if (option == 2) {
        for (let month = 0; month <= 11; month++) {
            /*let arr = listData.filter(
                function (e) { return (new Date(e.startDate.split("T")[0]).getMonth() == month) || (new Date(e.endDate.split("T")[0]).getMonth() == month); }
            ).map(function (e) { return e.amount; });*/

            let arr = listData.filter(
                function (e) { return (new Date(e.startDate.split("T")[0]).getMonth() == month) || (new Date(e.endDate.split("T")[0]).getMonth() == month); }
            );

            //if (arr.length > 0) dataPoints[month] = arr.reduce(function (total, num) { return total + num; });
            dataPoints[month] = arr.length;
        }
    }
    return {
        label: content,
        data: dataPoints,
        borderColor: color,
        backgroundColor: bgColor,
    };
    //sửa chart nhà
    //Utils.CHART_COLORS.blue
    //Utils.transparentize(Utils.CHART_COLORS.blue, 0.5)
}
function setConfig(data, title, type, func) {

    let labelMonth = [];
    for (let e = 1; e <= 12; e++) {
        labelMonth[labelMonth.length] = "Tháng " + e;
    }
    return config = {
        type: type,
        data: {
            labels: labelMonth,
            datasets: data
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: title
                }
            },
            onClick: func
        },
    };
}
function renderChart(config) {
    if (chart != null) chart.destroy();
    return new Chart(document.getElementById("statistic-render"), config);
}
function getList(index, month) {
    let status = (index == 1);
    let arr = [];
    for (e in transactionZalo) {
        if (transactionZalo[e].status == status) {
            if (transactionZalo[e].createdDate.indexOf(month) != -1) {
                arr[arr.length] = transactionZalo[e];
            }
        }
    }
    return arr;
}
function showInfo(arr, month) {
    let content = "";

    for (let index = 0; index < arr.length; index++) {
        let position = "left";
        if (index % 2 == 0) {
            position = "right";
        }
        content += itemTransaction(arr[index], position);
    }

    return `<div class="modal fade" id="infoStatisticModal" data-bs-backdrop="static" data-bs-keyboard="false" aria-hidden="true" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-xl">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5">Lịch sử giao dịch trong tháng ${month}</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" id="infoStatisticModalClose"></button>
                </div>
                <div class="modal-body">
                    <div class="timeline">
                        ${content}
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button class="d-none" id="infoStatisticModalClick" data-bs-target="#infoStatisticModal" data-bs-toggle="modal"></button>`;
}
function itemTransaction(item, position) {
    let status = "valid";
    let content = "Bạn đã nạp " + item.amount + " vào hệ thống";
    if (item.status == true) { status = "used"; content = "Bạn đã sử dụng " + item.amount; }
    return `<div class="container ${position} ${status}">
    <div class="content ${status}">
        <h5>${item.createdDate}</h5>
        <p>${content}</p>
    </div>
  </div>`;
}
function zaloTransaction() {
    //render loader
    showLoader();
    $.ajax({
        url: window.location.origin + "/Statistics/Payment/All?year=" + $("#transaction-year").val(),
        dataType: "json",
        contentType: "application/json",
        type: "GET",
        success: function (result) {
            if (result.status == 200) {
                let arrValid = [];
                let arrUsed = [];
                transactionZalo = result.data;
                for (e in result.data) {
                    if (result.data[e].status == true) {
                        arrUsed[arrUsed.length] = result.data[e];
                    }
                    else {
                        arrValid[arrValid.length] = result.data[e];
                    }
                }

                let data_1 = setUpData(arrValid, "Điểm nạp vào", 'rgb(255, 99, 132)', 'rgb(255, 99, 132, 0.5)', 1);
                let data_2 = setUpData(arrUsed, "Điểm sử dụng", 'rgb(54, 162, 235)', 'rgb(54, 162, 235, 0.5)', 1);
                let config = setConfig([data_1, data_2], "Lịch sử giao dịch", 'line', zaloDetail);
                chart = renderChart(config);

                //hide loader
                hideLoader();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function zaloDetail() {
    var element = chart.getActiveElements();
    if (element.length > 0) {
        console.log(element[0]);
        let arr = getList(element[0].datasetIndex, (element[0].index + 1));
        $("#renderModal").html(showInfo(arr, (element[0].index + 1)));
        $("#infoStatisticModalClick").click();
    }
}
function statisticHouse() {
    showLoader();
    $.ajax({
        url: window.location.origin + "/Statistics/House?year=" + $("#house-year").val() + "&idHouse=" + $("#statisticHouse").val(),
        dataType: "json",
        contentType: "application/json",
        type: "GET",
        success: function (result) {
            console.log(result);
            if (result.status == 200) {
                arrResult = result.data;
                let data_1 = setUpData(result.data.requests, "Nhận yêu cầu", 'rgb(255, 99, 132)', 'rgb(255, 99, 132, 0.5)', 2);
                let data_2 = setUpData(result.data.useForSwap, "Dùng để trao đổi", 'rgb(54, 162, 235)', 'rgb(54, 162, 235, 0.5)', 2);
                let config = setConfig([data_1, data_2], "Tần suất hoạt động của nhà", 'bar', null);
                console.log(data_1);
                console.log(data_2);
                console.log(config);
                chart = renderChart(config);
                hideLoader();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function showLoader() {
    $(".statistic-img").addClass("loading");
}
function hideLoader() {
    $(".statistic-img.loading").removeClass("loading");
}
function showPredict() {
    $(".statistic-img").addClass("predict");
}
function hidePredict() {
    $(".statistic-img.predict").removeClass("predict");
}
function showMaps() {
    $(".predict-render").addClass("show");
}
function hideMaps() {
    $(".predict-render.show").removeClass("show");
}
function formatAddress(idCity, idDistrict) {
    let district = stringToSlug(dataLocation[idCity].district[idDistrict].name.toLowerCase()).replaceAll(" ", "").trim();
    let city = stringToSlug(dataLocation[idCity].name.toLowerCase()).replaceAll(" ", "").trim();
}
function priceFormat(price) {
    const config = { style: 'currency', currency: 'VND', maximumFractionDigits: 12 }
    let formated = new Intl.NumberFormat('vi-VN', config).format(price);
    return formated;
}
function getPredict() {
    hideMaps();
    let objSend = {
        address: stringToSlug(dataLocation[$("#predictCity").val()].name.toLowerCase()).replaceAll(" ", "").trim(),
        distance: $("#predictDistance").val(),
        capacity: $("#predictPeople").val(),
        area: $("#predictArea").val(),
        rating: $("#predictRating").val()
    }
    $.ajax({
        url: "/PredictHouse",
        data: JSON.stringify(objSend),
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            console.log(data);
            $("#predict-house div.value").html(priceFormat(data.price));
            showMaps();
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function getDistrictPoint(id) {
    $.ajax({
        url: "/api/Location/DistrictWithPoint?IdCity=" + id,
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log(data);
            var str = "{";
            for (e in data.data) {
                let omodel = JSON.stringify(data.data[e]);
                let model = "\"" + data.data[e]["id"] + "\":" + omodel.slice(0, omodel.length - 1) + ", \"ward\": null}";
                str += model + ", ";
            }
            str = str.trim();
            str = str.slice(0, str.length - 1);
            str += "}";
            console.log(str);
            dataLocation[id].district = JSON.parse(str);

            let model = JSON.stringify(data.data).replaceAll("name", "text");
            if ($("#predictDistrict").length > 0) {
                changeDataForSelectPoint("#predictDistrict", JSON.parse(model));
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function infoboxTemplate(title, price) {
    return `<div class="customInfobox">
                <div class="content">
                    <div class="title">${title}</div>
                    <div class="address"><strong>Giá dự đoán:</strong> ${description}</div>
                </div >
            </div >`;
}
function createFontPushpin(location, text, fontName, fontSizePx, color) {
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
        anchor: new Microsoft.Maps.Point(c.width / 2, c.height / 2)
    });
}

function initSelectPoint() {
    if ($("#city-select").length > 0) {
        $("#city-select").select2();
    }
    if ($("#district-select").length > 0) {
        $("#district-select").select2();
    }
}
function setDistrictSelector() {
    let id = $("#predictCity").val();
    if (dataLocation[id].district == null) {
        changeDataForSelectPoint("#predictDistrict", [{ "id": -1, "text": "Quận/Huyện" }]);
        $("#predictDistrict").prop("disabled", true);
        getDistrictPoint(id);
    }
    else {
        changeDataForSelectPoint("#predictDistrict", getDataFormatNew(dataLocation[id].district));
    }
}
function changeDataForSelectPoint(tag, data) {
    if ($(tag)[0].disabled) {
        $(tag).prop("disabled", false);
    }

    if ($(tag).length > 0) {
        $(tag).select2('destroy');
        $(tag).empty();
        $(tag).select2({
            data: data
        });
    }
}
function setMap() {
    lat = dataLocation[$("#predictCity").val()].district[$("#predictDistrict").val()].lat;
    lng = dataLocation[$("#predictCity").val()].district[$("#predictDistrict").val()].lng;
    GetMap();
}


var arrResult = null;