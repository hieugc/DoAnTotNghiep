function showListHouseModal(index) {
    $("#messageRequestModal .list-house")[index].style.display = "flex";
    $("#messageRequestModal .modal-content")[0].style.display = "none";
}
function hideListHouseModal(index) {
    $("#messageRequestModal .list-house")[index].style.display = "none";
    $("#messageRequestModal .modal-content")[0].style.display = "flex";
}
function selectItem(tagList, tag) {
    if ($(tagList + " .house-card.selected").length > 0) {
        $(tagList + " .house-card.selected")[0].classList.remove("selected");
    }
    $(tagList + " " + tag)[0].classList.add("selected");
}
function price(model1, model2) {
    let price_1 = 0;
    if (model1 != null) {
        price_1 = model1.price;
    }
    let price_2 = 0;
    if (model2 != null) {
        price_2 = model2.price;
    }

    if (picker.options.endDate == null || picker.options.startDate == null) {
        return 0;
    }
    let total = (price_1 - price_2) * ((picker.options.endDate.getTime() - picker.options.startDate.getTime()) / (3600 * 24 * 1000) + 1);
    return total;
}
function calPrice(model1, model2) {
    let total = price(model1, model2);

    if (total < 0) {
        return `Bạn sẽ trả <strong>${(-total)} (point/ngày)</strong>`;
    }
    return `Bạn nhận được <strong>${total} (point/ngày)</strong>`;
}
function houseItem(index, model) {
    if (model == null) return ``;

    return `<div class="house-card house-card-${model.id}">
                            <img src="${model.images[0].data}" alt="Mike" style="width:100%">
                            <div class="house-img-container">
                                <h2>${model.name}</h2>
                                <p class="attribute"><i class="fa-solid fa-map-location-dot"></i><span>${model.location}</span></p>
                                <div class="list-attribute">
                                    <p class="attribute"><span>${model.rating} (${model.numberRating})</span><i class="fa-solid fa-star"></i></p>
                                    <p class="attribute"><i class="fa-solid fa-users"></i><span>${model.people}</span></p>
                                    <p class="attribute"><i class="fa-solid fa-bed"></i><span>${model.bedRoom}</span></p>
                                </div>
                            <p>
                                <button class="item-button" onclick="showListHouseModal(${index})">
                                    <span>${model.price} (point/ngày)</span>
                                </button>
                            </p>
                            </div>
                            <a target="_blank" href="${window.location.origin + "/Houses/Details/" + model.id}" class="finding"><i class="fa-sharp fa-solid fa-magnifying-glass"></i></a>
                        </div>`;
}
function confirmSelection(index) {
    $(".frame-empty")[index].classList.add("d-none");
    if (index == 0) {
        model_1 = getModel(".list-house-1", listModel_1);
        $(".house-result")[index].innerHTML = houseItem(index, model_1);
    }
    else {
        model_2 = getModel(".list-house-2", listModel_2);
        $(".house-result")[index].innerHTML = houseItem(index, model_2);
    }
    hideListHouseModal(index);
    $(".contain-price").html(calPrice(model_1, model_2));
}
function getModel(tagList, listModel) {
    if (listModel.length > 0) {
        if ($(tagList + " .house-card.selected").length == 0) {
            return listModel[0];
        }
        else {
            let selected = $(tagList + " .house-card.selected");
            let index = 0;
            if (selected.length > 0) {
                for (e in selected) {
                    if (selected[e].classList != undefined) {
                        index++;
                        if (selected[e].classList.value.indexOf("selected") != -1) {
                            break;
                        }
                    }
                }
            }
            return listModel[index];
        }
    }
    return null;
}
function getRequestFormByUserAccess(userAccess) {
    $.get(
        window.location.origin + "/Request/FormWithUserAccess?userAccess=M9z0Tfhfwj1i7om2qf3XVA==",
        function (data) {
            $("#renderModal").html(data);
        }
    )
}
function showOption() {
    let option = $("input[name='swap-option']:checked").val();
    if (option == "swap-option-1") {
        $(".houseSwap")[0].classList.remove("houseSwap-show");
        $(".contain-price").html(calPrice(model_1, null));
    }
    else {
        $(".houseSwap")[0].classList.add("houseSwap-show");
        model_2 = getModel(".list-house-2", listModel_2);
        $(".contain-price").html(calPrice(model_1, model_2));
    }
}
function getDateSwap() {
    let option = $("input[name='swap-option']:checked").val();
    if (option == "swap-option-1") {
        //tiền
        $(".contain-price").html(calPrice(model_1, null));
    }
    else {
        //nhà
        $(".contain-price").html(calPrice(model_1, model_2));
    }
}
function createRequest() {
    let rangeDate = (picker.options.endDate.getTime() - picker.options.startDate.getTime()) / (3600 * 24 * 1000);
    if (rangeDate > 0 && model_1 != null) {
        let option = $("input[name='swap-option']:checked").val();
        if (option == "swap-option-1") {
            //tiền
            let data = {
                idHouse: model_1.id,
                idSwapHouse: null,
                startDate: enDate(picker.options.startDate),
                endDate: enDate(picker.options.endDate),
                type: 1,
                price: price(model_1, null)
            }
            create(data);
        }
        else if (option == "swap-option-2" && model_2 != null) {
            let data = {
                idHouse: model_1.id,
                idSwapHouse: model_2.id,
                startDate: enDate(picker.options.startDate),
                endDate: enDate(picker.options.endDate),
                type: 2,
                price: price(model_1, model_2)
            }
            create(data);
        }
    }
}
function create(data) {
    $.ajax({
        url: window.location.origin + "/Request/Create",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        type: "POST",
        success: function (result) {
            console.log(result);
            $("#requestFormClick").click();
        },
        error: function (error) {
            console.log(error);
        }
    });
}


function enDate(date) {
    return date.dateInstance.toISOString().split("T")[0];
}