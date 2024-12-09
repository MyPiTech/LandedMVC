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
        messages: V_MESSAGES,
        submitHandler: function (form) {
            notify(S_SENDING);
            $.ajax({
                url: 'home/email',
                method: 'POST',
                data: $(form).serialize(),
                success: function (result) {
                    notify(S_SUCCESS);
                    $('#name').val('');
                    $('#message').val('');
                },
                error: function (xhr, resp, text) {
                    notify(S_ERROR, true);
            }});
        }
    });
});