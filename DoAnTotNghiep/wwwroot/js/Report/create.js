function createReport() {
    if (checkReportValid()) {
        $.ajax({
            url: window.location.origin + "/Report/Create",
            data: JSON.stringify(reportData),
            dataType: "json",
            contentType: "application/json",
            type: "POST",
            success: function (result) {
                console.log(result);
                if (result.status == 200) {
                    showNotification("Thông báo", "Báo cáo đã gửi đến hệ thống!", 1);
                    $("#reportModalClick").click();
                }
            },
            error: function (error) {
                console.log(error);
                showNotification("Tác vụ thất bại", "Báo cáo gửi đến hệ thống bị trục trặc!", 0);
            }
        });
    }
}
function showReportFile(element) {
    const files = $("#fileReport")[0].files[0];
    if (files) {
        const fileReader = new FileReader();
        fileReader.readAsDataURL(files);
        fileReader.addEventListener("load", function () {
            reportData.images[reportData.images.length] = {
                data: this.result,
                name: $("#fileReport")[0].files[0].name
            }
            $("#file-result").append(tagImage(this.result));
            checkReportValid();
        });
    }
}
function tagImage(src) {
    let res = `<div class="picture-frame">
                    <img src = "${src}" alt = "Hình ảnh báo cáo" />
                    <button type="button" class="btn-close" onclick="rmReportFile(this)"></button>
                </div>`;
    return res;
}
function rmReportFile(othis) {
    for (e in reportData.images) {
        if (reportData.images[e].data == $(othis)[0].parentNode.getElementsByTagName("img")[0].src) {
            reportData.images.splice(e, 1);
            $($(othis)[0].parentNode).remove();
            checkReportValid();
            break;
        }
    }
}
function checkReportValid() {
    reportData.content = $("#contentReport").val();
    if (!(reportData.content.length > 0)) {
        $("#contentReport-valid").html("Hãy điền nội dung báo cáo");
        return false;
    }
    else {
        $("#contentReport-valid").html("");
    }
    if (!(reportData.images.length > 0)) {
        $("#filesReport-valid").html("Hãy thêm hình");
        return false;
    }
    else {
        $("#filesReport-valid").html("");
    }
    return true;
}