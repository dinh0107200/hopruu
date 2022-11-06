function homeJs() {
    AOS.init();
    $('.number').countUp();

    $('.feedback-list').slick({
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        prevArrow: '<button class="chevron-prev"><i class="fas fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fas fa-chevron-right"></i></button>',
    });
}

function getPrject(action) {
    $('.project-cat .nav-link').click(function () {
        let catId = parseInt($(this).val());
        $.get(action, { catId: catId }, function (data) {
            $('#list-item-sort').empty();
            $('#list-item-sort').html(data);
            slickProject()
        });
    })
}

function slickProject() {
    $('.project-list').slick({
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        centerMode: true,
        variableWidth: true,
        focusOnSelect: true,
        touchMove: true,
        prevArrow: '<button class="chevron-prev"><i class="fas fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fas fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    variableWidth: false,
                }
            },
        ]
    });
    $('.project-list .slick-active.slick-center').prev().addClass('active');
    $('.project-list .slick-arrow').click(function () {
        $('.project-list .slick-slide').removeClass('active');
        $('.project-list .slick-active.slick-center').prev().addClass('active');
    });
    $('.project-list').on('swipe', function () {
        $('.project-list .slick-arrow').click();
    });
}

function projectDetail() {
    var countProject = $('.recent-project-list .project-item').length;

    $('.recent-project-list').slick({
        infinite: true,
        slidesToShow: 2,
        slidesToScroll: 1,
        speed: 1000,
        centerMode: true,
        variableWidth: true,
        focusOnSelect: true,
        touchMove: true,
        dots: true,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-arrow-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-arrow-right"></i></button>',
        responsive: [
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    variableWidth: false,
                }
            },
        ]
    });
    if (countProject > 1) {
        $('.recent-project-list .slick-list').addClass('active');
    }
}

$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('.header-sticky').addClass('active');
        } else {
            $('.header-sticky').removeClass('active');
        }
        if ($(this).scrollTop() > 200) {
            $('.btn-scroll').fadeIn(200);
            $('.header-home').addClass('active');
        } else {
            $('.btn-scroll').fadeOut(200);
            $('.header-home').removeClass('active');
        }
    });

    $('.btn-scroll').click(function (event) {
        event.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, 300);
    });

    $('.back-to-top').click(function (event) {
        event.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, 300);
    });

    $('.btn-search-mb').click(function () {
        $('.search-mb').toggleClass('active');
    });

    $('.hamburger').click(function () {
        $(this).toggleClass('active');
        $('.nav-mb').toggleClass('active');
        $('.overlay').toggleClass('active');
    });

    $('.overlay').click(function () {
        $(this).removeClass('active');
        $('.nav-mb').removeClass('active');
        $('.hamburger').removeClass('active');
    });
});

$(function () {
    $(".contact-form").on("submit", function (e) {
        e.preventDefault();
        $.post("/Home/ContactForm", $(this).serialize(), function (data) {
            if (data.status) {
                $.toast({
                    heading: 'Liên hệ thành công',
                    text: data.msg,
                    icon: 'success'
                })
                $(".contact-form").trigger("reset");
            } else {
                $.toast({
                    heading: 'Liên hệ không thành công',
                    text: data.msg,
                    icon: 'error'
                })
            }
        });
    });
});