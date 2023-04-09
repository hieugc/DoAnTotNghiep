function getRating(id) {
    $.get(
        window.location.origin + "/Rating/GetByHouse?id=" + id,
        function (data) {
            $("#tab-6").html(data);
        }
    );
}