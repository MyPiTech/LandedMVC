$(function () {

    $('#contactForm').validate({
        rules: {
            name: {
                required: true
            },
            message: {
                required: true
            }
        },
        messages: {
            name: {
                required: '<span style="color: #ffffff">Please enter a name.</span>'
            },
            message: {
                required: '<span style="color: #ffffff">Please enter a message.</span>'
            }
        },
        submitHandler: function (form) {
            notify('Sending email...');
            $.ajax({
                url: 'home/email',
                method: 'POST',
                data: $(form).serialize(),
                success: function (result) {
                    notify('Email sent successfully. Thanks!');
                    $('#name').val('');
                    $('#message').val('');
                },
                error: function (xhr, resp, text) {
                    notify('An error occurred. The email was not sent.', true);
                }
            });

        }

    });

    function notify(message, isError = false) {
        let notification = $('#notification');

        if (isError == true) {
            notification.removeClass('success light').addClass('error light');
            notification
                .html(message)
                .show(500)
                .delay(1500)
                .hide(500);
        } else {
            notification.removeClass('error light').addClass('success light');
            notification
                .html(message)
                .show(500)
                .delay(1500)
                .hide(500);
        }
    }

});