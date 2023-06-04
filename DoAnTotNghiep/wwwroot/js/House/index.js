
function ScrollTo(index) {
    window.scrollTo(0, ($(`#tab${index}`).offset().top - 150));
    $(`.tab-pane.tab-active`).removeClass("tab-active");
    $(`.tab-pane`)[index - 1].classList.add("tab-active");
}
window.addEventListener("scroll", function () {
    console.log(this.window.scrollY.toFixed());
    if (this.window.scrollY.toFixed() <= ($(`#tab1`).offset().top - 150)) {
        $(`.tab-pane.tab-active`).removeClass("tab-active");
        $(`.tab-pane`)[0].classList.add("tab-active");
        console.log("scroll0");
    }
    else if (this.window.scrollY.toFixed() <= ($(`#tab2`).offset().top - 150)) {
        $(`.tab-pane.tab-active`).removeClass("tab-active");
        $(`.tab-pane`)[1].classList.add("tab-active");
        console.log("scroll1");
    }
    else if (this.window.scrollY.toFixed() <= ($(`#tab3`).offset().top - 150)) {
        $(`.tab-pane.tab-active`).removeClass("tab-active");
        $(`.tab-pane`)[2].classList.add("tab-active");
        console.log("scroll2");
    }
    else if (this.window.scrollY.toFixed() <= ($(`#tab4`).offset().top - 150)) {
        $(`.tab-pane.tab-active`).removeClass("tab-active");
        $(`.tab-pane`)[3].classList.add("tab-active");
        console.log("scroll3");
    }
    else if (this.window.scrollY.toFixed() <= ($(`#tab5`).offset().top - 150)) {
        $(`.tab-pane.tab-active`).removeClass("tab-active");
        $(`.tab-pane`)[4].classList.add("tab-active");
        console.log("scroll4");
    }
    else if (this.window.scrollY.toFixed() <= ($(`#tab6`).offset().top - 150)) {
        $(`.tab-pane.tab-active`).removeClass("tab-active");
        $(`.tab-pane`)[5].classList.add("tab-active");
        console.log("scroll5");
    }
});