var down = true;

$(document).ready(function() {

    $("#showHelp").click(function() {
        $("#help").toggle();
    });



    $("#showHelp_2").click(function() {
        $("#help_2").toggle();
    })

    $('.nav-button, .joinPop').click(function() {
        $('body').toggleClass('nav-open');
    });
});

function showHelp(val) {
    console.log($("#down-" + val).is(":visible"))
    if ($("#down-" + val).is(":visible") === false) {

        $('#up-' + val).hide();

        $('#down-' + val).show();
        down = false;
    } else {
        $('#down-' + val).hide();

        $('#up-' + val).show();


        down = true;
    }

    $("#help-" + val).toggle();
}

function showMobileHelp(val) {
    console.log($("#down-" + val).is(":visible"))
    if ($("#down-m-" + val).is(":visible") === false) {


        $('#up-m-' + val).hide();
        $('#down-m-' + val).show();
        down = false;
    } else {

        $('#down-m-' + val).hide();
        $('#up-m-' + val).show();
        down = true;
    }

    $("#help-" + val).toggle();
}


