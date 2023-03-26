function tabContent(tag, element) {
    $(".nav-link.active")[0].classList.remove("active");
    $(element)[0].classList.add("active");
    if (tag == null) {
        $("div.card.status").show();
    }
    else {
        $("div.card.status").hide();
        $("div.card.status" + tag).show();
    }
}