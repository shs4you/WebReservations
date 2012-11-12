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

var reservation_status;
var tabs = ['dates', 'room_rates', 'enhance_stay', 'guest_details', 'confirm_booking'];

jQuery(document).ready(function ($) {
    $('.body').fadeIn(500);

    //Assigning of saved data from cookie
    reservation_data = $.cookie('reservation_data') ? $.cookie('reservation_data') : false;
    reservation_status = $.cookie('reservation_status') ? $.cookie('reservation_status') : false;
    reservation_status = { "dates": true };

    //Add checking befor proceeding with next step.
    $('a[data-toggle="tab"]').on('show', function (e) {
        var target = $(e.target) // activated tab
        var related = $(e.relatedTarget) // previous tab
        var target_href = target.attr('href').replace("#", "");
        if (!check_status(target_href)) {
            //Show alert
            $('.alert').show();
            $('.alert').fadeOut(1500);
            return false;
        }
        else {
            return true;
        }
    })

    //Logic for clicking the next button
    $('#next').click(function () {
        var active_tab_id = get_active_tab();
        var next_tab = tabs.indexOf(active_tab_id) + 1;
        $.reservation(active_tab_id);
        $('#steps li:eq(' + next_tab + ') a').trigger('click');
    });

    //Logic for clicking the back button
    $('#back').click(function () {
        var active_tab_id = get_active_tab();
        var prev_tab = tabs.indexOf(active_tab_id) - 1;
        if (prev_tab < 0) return;
        $('#steps li:eq(' + prev_tab + ') a').trigger('click');
    });
});


/*
* Checks whether the next step can be performed.
* @params target string
* @return bool
* @author: jcapagcuan
*
*/
function check_status(target) {
    current_status = false;
    if (reservation_status[target] != undefined && reservation_status[target] != false) {
        current_status = true;
    }

    return current_status;
}

/*
* Gets the active tab id
* @params none
* @return string
* @author: jcapagcuan
*/
function get_active_tab() {
    return $('#steps li.active a').attr('href').replace('#', '');
}

(function ($) {
    var methods = {
        init: function (options) {

        },

        dates: function (options) {
            reservation_status.room_rates = true;
        },

        room_rates: function (options) {
            reservation_status.enhance_stay = true;
        },

        enhance_stay: function (options) {
            reservation_status.guest_details = true;
        },

        guest_details: function (options) {
            reservation_status.confirm_booking = true;
        },

        confirm_booking: function (options) {
            reservation_status.confirm_booking = true;
        }

    };

    $.reservation = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist');
        }

    };

})(jQuery);