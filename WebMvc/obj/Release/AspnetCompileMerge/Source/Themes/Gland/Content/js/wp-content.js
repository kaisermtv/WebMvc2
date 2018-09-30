jQuery(document).ready(function ($) {
    function mtsDropdownMenu() { var wWidth = $(window).width(); if (wWidth > 865) { $('#navigation ul.sub-menu, #navigation ul.children').hide(); var timer; var delay = 100; $('#navigation li').hover(function () { var $this = $(this); timer = setTimeout(function () { $this.children('ul.sub-menu, ul.children').slideDown('fast'); }, delay); }, function () { $(this).children('ul.sub-menu, ul.children').hide(); clearTimeout(timer); }); } else { $('#navigation li').unbind('hover'); $('#navigation li.active > ul.sub-menu, #navigation li.active > ul.children').show(); } }
    mtsDropdownMenu(); $(window).resize(function () { mtsDropdownMenu(); });
});
jQuery(document).ready(function ($) { $('.widget_nav_menu, #navigation .menu').addClass('toggle-menu'); $('.toggle-menu ul.sub-menu, .toggle-menu ul.children').addClass('toggle-submenu'); $('.toggle-menu ul.sub-menu').parent().addClass('toggle-menu-item-parent'); $('.toggle-menu .toggle-menu-item-parent').append('<span class="toggle-caret"><i class="fa fa-plus"></i></span>'); $('.toggle-caret').click(function (e) { e.preventDefault(); $(this).parent().toggleClass('active').children('.toggle-submenu').slideToggle('fast'); }); }); jQuery(document).ready(function () {
    jQuery.fn.exists = function (callback) {
        var args = [].slice.call(arguments, 1); if (this.length) { callback.call(this, args); }
        return this;
    };
    (function (d, s) {
        var js, fjs = d.getElementsByTagName(s)[0], load = function (url, id) {
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.src = url; js.id = id; fjs.parentNode.insertBefore(js, fjs);
        };
        //jQuery('span.facebookbtn, span.facebooksharebtn, .facebook_like').exists(function () { load('//connect.facebook.net/en_US/all.js#xfbml=1', 'fbjssdk'); });
        jQuery('span.gplusbtn').exists(function () { load('https://apis.google.com/js/plusone.js', 'gplus1js'); });
        jQuery('span.twitterbtn').exists(function () { load('//platform.twitter.com/widgets.js', 'tweetjs'); });
        jQuery('span.linkedinbtn').exists(function () { load('//platform.linkedin.com/in.js', 'linkedinjs'); });
        jQuery('span.pinbtn').exists(function () { load('//assets.pinterest.com/js/pinit.js', 'pinterestjs'); });
        jQuery('span.stumblebtn').exists(function () { load('//platform.stumbleupon.com/1/widgets.js', 'stumbleuponjs'); });
    }(document, 'script'));
});