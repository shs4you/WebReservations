/*
* @Author: jcapagcuan
* Custom Javascript file.
*/
if (typeof window.console == "undefined") {
    window.console = {
        log: function (msg) {
            return;
        }
    };
}

jQuery(document).ready(function ($) {
    $('.body').fadeIn(500);
});