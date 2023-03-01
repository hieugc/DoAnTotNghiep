
function openModal() {
    document.getElementById("myModal").style.display = "block";
}

function closeModal() {
    document.getElementById("myModal").style.display = "none";
}

let menuSection = document.querySelectorAll('.tab-pane');

// for clickable event
menuSection.forEach(v => {
    v.onclick = (() => {
        setTimeout(() => {
            menuSection.forEach(j => j.classList.remove('tab-active'))
            v.classList.add('tab-active')
        }, 300)
    })
})

// for window scrolldown event

window.onscroll = (() => {
    let mainSection = document.querySelectorAll('.content section');

    mainSection.forEach((v, i) => {
        let rect = v.getBoundingClientRect().y
        if (rect < window.innerHeight - 200) {
            menuSection.forEach(v => v.classList.remove('tab-active'))
            menuSection[i].classList.add('tab-active')
        }
    })
})



//var slideIndex = 1;
//showSlides(slideIndex);

//function plusSlides(n) {
//    showSlides(slideIndex += n);
//}

//function currentSlide(n) {
//    showSlides(slideIndex = n);
//}

////function showSlides(n) {
////    var i;
////    var slides = document.getElementsByClassName("mySlides");
////    var dots = document.getElementsByClassName("demo");
////    var captionText = document.getElementById("caption");
////    if (n > slides.length) { slideIndex = 1 }
////    if (n < 1) { slideIndex = slides.length }
////    for (i = 0; i < slides.length; i++) {
////        slides[i].style.display = "none";
////    }
////    for (i = 0; i < dots.length; i++) {
////        dots[i].className = dots[i].className.replace(" active", "");
////    }
////    slides[slideIndex - 1].style.display = "block";
////    dots[slideIndex - 1].className += " active";
////}


var showMore = false;
function changeName(element) {
    if (showMore) {
        element.innerHTML = "Thu gọn";
    }
    else {
        element.innerHTML = "Xem thêm";
    }
}