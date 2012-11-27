/*
* @Author: jcapagcuan
* Custom Javascript file.
*/
if (typeof window.console == 'undefined') {
    window.console = {
        log: function (msg) {
            return;
        }
    };
}

var reservationStatus;
var tabs = ['dates', 'roomRates', 'enhanceStay', 'guestDetails', 'confirmBooking'];
var minDate = 1;
var minDateDeparture = 1;
var reservationData;

jQuery(document).ready(function ($) {
    $('.body').fadeIn(500);

    $.ajaxSetup({
        cache: false
    });

    //Assigning of saved data from cookie
    reservationData = $.cookie('reservationData') ? $.cookie('reservationData') : false;
    reservationStatus = $.cookie('reservationStatus') ? $.cookie('reservationStatus') : false;
    reservationStatus = { 'dates': true };
    reservationData = { 'test': 'test' };

    $('#reservationModal').modal({
        keyboard: false,
        backdrop: 'static',
        show: false
    });

    $('#datepickerArrival').datepicker({
        showOn: 'button',
        minDate: minDate,
        onSelect: function (dateText, inst) {
            minDateDeparture = new Date(dateText);
            $('#datepickerDeparture').datepicker('option', 'minDate', minDateDeparture.addDays(1).asString());
        }
    });

    $('#datepickerDeparture').datepicker({
        showOn: 'button',
        minDate: minDateDeparture
    });

    $('#launchModal').click(function () {

    });

    //Add checking befor proceeding with next step.
    $('a[data-toggle="tab"]').on('show', function (e) {
        var target = $(e.target); // activated tab
        var related = $(e.relatedTarget); // previous tab
        var targetHref = target.attr('href').replace('#', '');
        if (!checkStatus(targetHref)) {
            //Show alert
            $('.alert').show();
            $('.alert').fadeOut(1500);
            return false;
        }
        else {
            return true;
        }
    });

    $('a[data-toggle="tab"]').click(function (e) {
        return false;
    });

    //Add checking befor proceeding to the clicked step.
    /*
    $('a[data-toggle="tab"]').click(function (e) {
    var targetHref = $(e.target).attr('href').replace('#', '');
    console.log(targetHref);
    if(targetHref == getActiveTab()){
    return true;
    }
    if (!checkStatus(targetHref)) {
    //Show alert
    $('.alert').show();
    $('.alert').fadeOut(1500);
    return false;
    }
    else if(getActiveTab() != 'confirmBooking') {
    $.reservation(getActiveTab());
    return true;
    }
    else{
    return true;
    }
    });
    */

    //Logic for clicking the next button
    $('#next').click(function () {
        var activeTabId = getActiveTab();
        var nextTab = tabs.indexOf(activeTabId) + 1;
        $('#reservationModal').modal('show');
        var requestData = $.reservation(activeTabId, nextTab);
    });

    //Logic for clicking the back button
    $('#back').click(function () {
        var activeTabId = getActiveTab();
        var prevTab = tabs.indexOf(activeTabId) - 1;
        if (prevTab < 0) return;
        $('#steps li:eq(' + prevTab + ') a').tab('show');
    });
});


/*
* Checks whether the next step can be performed.
* @params target string
* @return bool
* @author: jcapagcuan
*
*/
function checkStatus(target) {
    currentStatus = false;
    if (reservationStatus[target] != undefined && reservationStatus[target] != false) {
        currentStatus = true;
    }

    return currentStatus;
}

/*
* Gets the active tab id
* @params none
* @return string
* @author: jcapagcuan
*/
function getActiveTab() {
    return $('#steps li.active a').attr('href').replace('#', '');
}


/*
* Handler for the functions/logic/validations per step
* @params options mixed
* 
*/
(function ($) {
    var methods = {
        init: function (options) {
            methods
        },

        hideModal: function(){
            $('#reservationModal').modal('hide');
        },

        dates: function (options) {
            reservationStatus.roomRates = true;
            $.getJSON("/home/getavailablerooms", $('#datesForm').serialize())
                .complete(function(data){
                    methods.hideModal();
                })
                .success(function(data){
                    reservationData.stayDateRange = {
                        'startDate': $('#datepickerArrival').val(),
                        'endDate': $('#datepickerDeparture').val(),
                        'numAdult': $('#adultCount').val(),
                        'numChild': $('#childCount').val(),
                        'groupCode': $('#groupCode').val(),
                        'promoCode': $('#promoCode').val(),
                        'travelId': $('#travelId').val()
                    };
                    if(data.statusCode == 0){
                        dataRoomRates = {};
                        $.each(data.roomRates, function(index, value){
                            ratePlanCode = value.ratePlanCode;
                            rates = value.Total.Value;
                            if(typeof dataRoomRates[value.roomTypeCode] == 'undefined'){
                                dataRoomRates[value.roomTypeCode] = {};
                            }
                            dataRoomRates[value.roomTypeCode][ratePlanCode] = rates;
                        });
                        $('#steps li:eq(' + options + ') a').tab('show');
                    }
                    else{
                        
                    }
                     
                })
                .error(function(data){
                    console.log(data);
                });
        },

        roomRates: function (options) {
            reservationStatus.enhanceStay = true;
            reservationStatus.guestDetails = true;
            reservationStatus.confirmBooking = true;
            methods.hideModal();
            $('#steps li:eq(' + options + ') a').tab('show');
        },

        enhanceStay: function (options) {
            reservationStatus.guestDetails = true;
            methods.hideModal();
            $('#steps li:eq(' + options + ') a').tab('show');
        },

        guestDetails: function (options) {
            reservationStatus.confirmBooking = true;
            methods.hideModal();
            $('#steps li:eq(' + options + ') a').tab('show');
        },

        confirmBooking: function (options) {
            reservationStatus.confirmBooking = true;
            methods.hideModal();
            $('#steps li:eq(' + options + ') a').tab('show');
        }

    };

    $.reservation = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            console.log('Method ' + method + ' does not exist');
            $.error('Method ' + method + ' does not exist');
        }

    };

})(jQuery);