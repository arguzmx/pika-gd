'use strict'


$(function () {

    
    $("#offsidebar #select-lang").on('change', function () {
        

        var selectedCountry = $(this).children("option:selected").val();
        console.log(selectedCountry);

    });



    $("#offsidebar #set-smalltext").on('click', function () {
        if ($(this).is(':checked')) {
            $('body').addClass('text-sm');
            $('.main-header').addClass('text-sm');
            $('.nav-sidebar').addClass('text-sm');
            $('.main-footer').addClass('text-sm');

        } else {
            $('body').removeClass('text-sm');
            $('.main-header').removeClass('text-sm');
            $('.nav-sidebar').removeClass('text-sm');
            $('.main-footer').removeClass('text-sm');
        }
    });

});

