$(document).ready(function () {

    $.toctoc({
        target: '.post-context',
        headText: "Mục lục bài viết",
        headLinkText: ['', ''],
        headBackgroundColor: '#ddd',
        borderColor: '#edf6ff',
        bodyBackgroundColor: '#edf6ff',
        headTextColor: '#000',
        opened: true,
    });

    //Check to see if the window is top if not then display button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn();
        } else {
            $('.scrollToTopback-to-top').fadeOut();
        }
    });

    //Click event to scroll to top
    $('.back-to-top').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
        return false;
    });

    $(".order_btn > a").click(function(event) {
        event.preventDefault();
    });

    var zoom = function (elm) {
        elm
            .on('mouseover',
                function () {
                    $(this).children('.img').css({ 'transform': 'scale(2)' });
                })
            .on('mouseout',
                function () {
                    $(this).children('.img').css({ 'transform': 'scale(1)' });
                })
            .on('mousemove',
                function (e) {
                    $(this).children('.img').css({
                        'transform-origin': ((e.pageX - $(this).offset().left) / $(this).width()) * 100 +
                            '% ' +
                            ((e.pageY - $(this).offset().top) / $(this).height()) * 100 +
                            '%'
                    });
                });
    }

    $('.item').each(function () {
        $(this)
            .append('<div class="img"></div>')
            .children('.img').css({ 'background-image': 'url(' + $(this).data('image') + ')' });
        zoom($(this));
    });
});

function homeJs() {
    $('.banner').slick({
        arrows: true,
        autoplay: true,
        speed: 1500,
        autoplaySpeed: 2000,
        prevArrow: '<ion-icon name="chevron-back-outline"></ion-icon>',
        nextArrow: '<ion-icon class="nextbtn" name="chevron-forward-outline"></ion-icon>'
    });

    $('.category-slides').slick({
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: true,
        arrows: true,
        autoplaySpeed: 2000,
        prevArrow: '<i class="fa fa-chevron-left"></i>',
        nextArrow: '<i class="fa fa-chevron-right icon-next"></i>',
        responsive: [
            {
                breakpoint: 800,
                settings: {
                    slidesToShow: 3

                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 2
                }
            }]
    });

}

function productJs() {
    $('.product-detail-list').slick({
        arrows: false
    });
    $('.product-slide').slick({
        arrows: false,
        asNavFor: '.product-detail-list',
        focusOnSelect: true,
        //vertical: true,
        slidesToShow: 4,
        responsive: [
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 3,
                    vertical: false

                }
            }
        ]
    });

}

const listMenu = document.querySelector('.menu-nav');
const menu = document.querySelectorAll('.icon-item');
const itemMenu = document.querySelector('.menu-item');

function changeMenu() {
    for (i = 0; i < menu.length; i++) {
        menu[i].classList.toggle('active');
    }
}

function showMenu() {
    itemMenu.classList.toggle('active');
}

listMenu.addEventListener('click', showMenu);
listMenu.addEventListener('click', changeMenu);