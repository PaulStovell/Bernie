// Write your Javascript code.
$(function() {
    $('time').each(function (i, e) {
        var time = moment($(e).attr('datetime'));

        var now = moment();
        if (now.diff(time, 'days') <= 1) {
            $(e).html('<span>' + time.from(now) + '</span>');
        }
    });

});

