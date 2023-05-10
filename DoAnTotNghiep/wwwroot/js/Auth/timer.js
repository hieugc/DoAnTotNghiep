function timer(tag, sec) {
    var timer = setInterval(function () {
        $(tag).html(String(timeFormat(Math.floor(sec / 60))) + ":" + String(timeFormat(Math.floor(sec % 60))));
        sec--;
        if (sec < 0) {
            $(".input-timer").html(`<h5 class="text-danger mb-5">Hết thời gian xác thực, vui lòng thực hiện lại</h5>`);
            clearInterval(timer);
        }
    }, 1000);
}
function timeFormat(number) {
    if (number < 10) return "0" + number;
    return number;
}