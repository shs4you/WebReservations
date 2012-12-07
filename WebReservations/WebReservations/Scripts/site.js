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
        altField: '#datepickerArrivalText',
        altFormat: 'DD, MM dd, yy',
        onSelect: function (dateText, inst) {
            minDateDeparture = new Date(dateText);
            $('#datepickerDeparture').datepicker('option', 'minDate', minDateDeparture.addDays(1).asString());
        }
    });

    $('#datepickerDeparture').datepicker({
        showOn: 'button',
        altField: '#datepickerDepartureText',
        altFormat: 'DD, MM dd, yy',
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

    $('.roomRow button.active').live('click', function (e) {
        $(this).removeClass('active');
        e.stopImmediatePropagation();
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
                    var numNights = (Date.fromString($('#datepickerDeparture').val()) - Date.fromString($('#datepickerArrival').val()))/86400000;
                    reservationData.stayDateRange = {
                        'startDate': $('#datepickerArrival').val(),
                        'startDateText': $('#datepickerArrivalText').val(),
                        'endDate': $('#datepickerDeparture').val(),
                        'endDateText': $('#datepickerDepartureText').val(),
                        'numNights': numNights,
                        'numAdult': $('#adultCount').val(),
                        'numChild': $('#childCount').val(),
                        'groupCode': $('#groupCode').val(),
                        'promoCode': $('#promoCode').val(),
                        'travelId': $('#travelId').val()
                    };
                    if(data.statusCode == 0){
                        dataRoomRates = {};
                        dataRoomtypes = data.roomTypes;
                        $.each(data.roomRates, function(index, value){
                            ratePlanCode = value.ratePlanCode;
                            rates = value.Total.Value;
                            if(typeof dataRoomRates[value.roomTypeCode] == 'undefined'){
                                dataRoomRates[value.roomTypeCode] = {};
                            }
                            if(typeof dataRoomRates[value.roomTypeCode]['rates'] == 'undefined'){
                                dataRoomRates[value.roomTypeCode]['rates'] = {}
                            }
                            dataRoomRates[value.roomTypeCode]['rates'][value.ratePlanCode] = rates;
                            $.each(dataRoomtypes, function(roomTypeIndex, roomTypeValue){
                                if(value.roomTypeCode == roomTypeValue.roomTypeCode){
                                    dataRoomRates[value.roomTypeCode]['description'] = roomTypeValue.RoomTypeDescription.Items[0].Value;
                                    dataRoomRates[value.roomTypeCode]['numberOfUnits'] = roomTypeValue.numberOfUnits;
                                }
                            });
                            
                            reservationData['dataRoomRates'] = dataRoomRates;
                        });

                        var roomTemp = $('#rowTemplate');
                        //roomTemp = $(roomTemp);
                        finalRoomTemplate = $('<ul class="media-list" />');
                        $.each(dataRoomRates, function(index, value){
                            //console.log(value.rates);
                            $.each(value.rates, function(ratesIndex, ratesValue){
                                $('img.media-object', roomTemp).attr('src', '/content/themes/base/images/' + index + '.png');
                                //console.log($('img.media-object', roomTemp).attr('src'));
                                //console.log($('div.media-body.media-heading', roomTemp).html());
                                $('div.media-body h4.media-heading', roomTemp).html(value.description);
                                //console.log($('div.roomDesc p', roomTemp).html());
                                $('div.roomDesc p', roomTemp).html(ratesValue);
                                $('div.roomButton button', roomTemp).attr('data-room', index);
                                var liTemplate = $('<li class="media" />');
                                var roomClone = roomTemp.clone();
                                console.log(roomClone.html());
                                $(liTemplate).append(roomClone.html());
                                $(finalRoomTemplate).append(liTemplate);
                                //console.log(finalRoomTemplate.html());
                            });
                        });
                        $('.roomRow').append($(finalRoomTemplate).clone());
                        methods.buildStaySummary();
                        console.log(dataRoomRates);
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
            $.getJSON("/home/getavailablepackages", $('#datesForm').serialize())
                .complete(function(data){
                    methods.hideModal();
                })
                .success(function(data){
                    if(data.statusCode == 0){
                        methods.buildStaySummary();
                        console.log(data);
                        $('#steps li:eq(' + options + ') a').tab('show');
                    }
                    else{
                        
                    }
                     
                })
                .error(function(data){
                    console.log(data);
                });
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
        },

        buildStaySummary: function(data){
            if(typeof reservationData.stayDateRange != 'undefined'){
                $('.arrivalDate [class*="span"]:last-child').html(reservationData.stayDateRange.startDateText).parent('div').show();
                $('.departureDate [class*="span"]:last-child').html(reservationData.stayDateRange.endDateText).parent('div').show();
                $('.numNights [class*="span"]:last-child').html(reservationData.stayDateRange.numNights).parent('div').show();
                $('.numRooms [class*="span"]:last-child').html('1').parent('div').show();
                $('.numAdultsChildren [class*="span"]:last-child').html(reservationData.stayDateRange.numAdult + '/' + reservationData.stayDateRange.numChild).parent('div').show();
            }
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