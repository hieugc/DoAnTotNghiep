
function createRating() {
    if (finalCheck()) {
        $.ajax({
            url: window.location.origin + "/CircleRating/Create",
            data: JSON.stringify(ratingData),
            dataType: "json",
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
                if (result.status == 200) {
                    $.get(
                        window.location.origin + "/CircleRequest/Detail?Id=" + ratingData.idRequest,
                        function (data) {
                            $(`.card-${ratingData.idRequest}`).replaceWith(data);
                            showNotification("Thông báo", "Đánh giá đã được gửi!", 1);
                            $("#ratingModalClick").click();
                        }
                    )
                }
            },
            error: function (error) {
                console.log(error);
                finalCheck();
                showNotification("Tác vụ thất bại", "Đánh giá không thể khởi tạo!", 0);
            }
        });
    }
}

function checkRatingValid() {
    ratingData.content = $("#contentRating").val();
    if (!(ratingData.content.length > 0)) {
        $("#contentRating-valid").html("");
        return false;
    }
    else {
        $("#contentRating-valid").html("");
    }
    return true;
}

function finalCheck() {
    let res = checkRatingValid();
    ratingData.ratingHouse = $("#ratingModal .mb-3.item-house .star-select i.selected").length;
    ratingData.ratingUser = $("#ratingModal .mb-3.user .star-select i.selected").length;
    if (ratingData.ratingUser == 0) {
        ratingData.ratingUser = null;
    }
    if (ratingData.ratingHouse == 0) {
        ratingData.ratingHouse = null;
    }
    if (ratingData.ratingUser == null) {
        res = false;
        $("#user-valid").html("Hãy đánh giá người dùng");
    }
    else {
        $("#user-valid").html("");
    }
    return res;
}
function addStar(tag, number) {
    //1 -> nhà
    //2 -> user
    let star_selected = $("#ratingModal .mb-3." + tag + " .star-select i.selected");
    for (let index = 0; index < star_selected.length; index++) {
        console.log(star_selected[index]);
        star_selected[index].classList.remove("selected");
    }

    for (let index = 0; index < number; index++) {
        $("#ratingModal .mb-3." + tag + " .star-select i")[index].classList.add("selected");
    }

    if (tag == "item-house") {
        ratingData.ratingHouse = number;
    }
    else {
        ratingData.ratingUser = number;
    }
}