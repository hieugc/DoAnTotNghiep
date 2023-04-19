var sec = 0;
function timer(tag) {
    sec = 60*15;
    var timer = setInterval(function () {
        $(tag).html(String(timeFormat(Math.floor(sec / 60))) + ":" + String(timeFormat(Math.floor(sec % 60))));
        sec--;
        if (sec < 0) {
            $("#renderQRCode .data div")[0].innerHTML = `<h5 class="text-danger mb-5">Hết thời gian giao dịch, vui lòng thực hiện lại</h5>`;
            clearInterval(timer);
        }
    }, 1000);
}
function timeFormat(number) {
    if (number < 10) return "0" + number;
    return number;
}
function createQRCode(data) {
    let res = `<div class="data">
                <div style="padding: 12px;box-shadow: 0 0.25rem 0.35rem #00000030;margin-bottom: 24px;border: 1px solid white;border-radius: 4px;"><img src="${data.data}" alt="QR code" /></div>
                <div class="text-success">Đang chờ thanh toán...</div>
                <div>
                    Thời gian quét mã QR để thanh toán <br />
                    còn <span id="qr-timer">15:00</span>
                </div>
            </div>
            <div class="instruction">
                <div class="d-flex justify-content-center">
                    <span>Thanh toán với</span>
                    <span class="logo"><img src="https://simg.zalopay.com.vn/zlp-website/assets/logo1_ff390716a5.svg" alt="logo ZaloPay" /></span>
                    <span>bằng mã QR</span>
                </div>
                <h4>Hướng dẫn thanh toán</h4>
                <ul>
                    <li><u>Bước 1:</u> <strong>Mở</strong> ứng dụng <strong>ZaloPay</strong></li>
                    <li>
                        <u>Bước 2:</u> Chọn <strong>"Thanh toán"</strong>
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" style="fill: rgba(0, 0, 0, 1);transform: ;msFilter:;"><path d="M4 4h4.01V2H2v6h2V4zm0 12H2v6h6.01v-2H4v-4zm16 4h-4v2h6v-6h-2v4zM16 4h4v4h2V2h-6v2z"></path><path d="M5 11h6V5H5zm2-4h2v2H7zM5 19h6v-6H5zm2-4h2v2H7zM19 5h-6v6h6zm-2 4h-2V7h2zm-3.99 4h2v2h-2zm2 2h2v2h-2zm2 2h2v2h-2zm0-4h2v2h-2z"></path></svg>
                        và quét mã QR
                    </li>
                    <li><u>Bước 3:</u> <strong>Mở</strong> ứng dụng <strong>ZaloPay</strong></li>
                </ul>
            </div>`;
    return res;
}
function initQR(data) {
    hideTransaction();
    $("#renderQRCode").html(createQRCode(data));
    timer("#qr-timer");
}
function checkPrice(element) {
    if (Number(element.value) == NaN) {
        $("")
        return false;
    }
    return true;
}
function createTransaction() {
    let form = $("#form-create");
    $.validator.unobtrusive.parse(form);
    if (form.valid() && Number($("#Price").val()) != NaN) {
        let data = {
            price: Number($("#Price").val())
        }
        $.ajax({
            url: window.location.origin + "/Payment/Zalo",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
                if (result.status == 200) {
                    //window.open(result.data.orderurl, "_blank");
                    initQR(result);
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}
function renderTransactionForm() {
    $.get(
        window.location.origin + "/Payment/Form",
        function (data) {
            $("#renderModal").html(data);
        }
    )
}
function hideTransaction() {
    $("#transactionModalToggle #form-create .modal-body").hide();
    $("#transactionModalToggle #form-create .my-modal-title").hide();
    $("#transactionModalToggle #form-create .btn-close")[0].style.marginTop = "-0.5rem";
}
function showTransaction() {
    $("#transactionModalToggle #form-create .modal-body").show();
    $("#transactionModalToggle #form-create .my-modal-title").show();
    $("#transactionModalToggle #form-create .btn-close")[0].style.marginTop = "-32px";

}