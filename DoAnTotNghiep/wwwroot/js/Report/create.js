function createReport() {
    let form = $("#form-create");
    $.validator.unobtrusive.parse(form);
    if (form.valid()) {
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


function showReportFile(element) {

}
function checkReportValid() {
    if (!(reportData.content.length > 0)) {
        $("#contentReport-valid").html("Hãy điền nội dung báo cáo");
        return false;
    }
    if (!(reportData.images.length > 0)) {
        $("#filesReport-valid").html("Hãy thêm hình");
        return false;
    }
    return true;
}