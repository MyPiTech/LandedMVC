$(function () {

    $('#contactForm').validate({
        errorClass: "error light",
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
                required: 'Please enter a name.'
            },
            message: {
                required: 'Please enter a message.'
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
});