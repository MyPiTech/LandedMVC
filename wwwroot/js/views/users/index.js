$(function () {

    let formState = null;

    let table = $('#usersTbl').DataTable({

        pagingType: 'first_last_numbers',
        pageLength: 5,
        lengthMenu: [5, 10],
        ajax: { url: '/users/users', dataSrc: '' },
        columns: [
            { data: 'id' },
            { data: 'lastName' },
            { data: 'firstName' },
            {
                data: 'notes',
                orderable: false,
                render: DataTable.render.ellipsis(5)
            },
            {
                data: null,
                orderable: false,
                mRender: function (data, type, row) {
                    let eventsAction = `<span x-type="events" x-id= ${data.id} class="icon solid alt fa-calendar" title="Events"></span>`;
                    let editAction = `<span x-type="edit" x-id= ${data.id} class="icon solid alt fa-list-alt" title="Edit"></span>`;
                    let deleteAction = `<span x-type="delete" x-id= ${data.id} class="icon solid alt fa-trash" title="Delete"></span>`;

                    return `${editAction}&nbsp;&nbsp;${deleteAction}&nbsp;&nbsp;${eventsAction}`;
                }
            }
        ],
        createdRow: function (row, data, dataIndex) {
            let uId = data['id'];
            $(row).attr('id', `row_${uId}`);
        },
        fnDrawCallback: function (oSettings) {
            if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
            } else {
                $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
            }
        }

    });

    $('#usersTbl tbody').on('click', 'span', function () {

        let xType = this.getAttribute('x-type');
        let uId = this.getAttribute('x-id');

        if (xType === 'events') {
            handleEvents(uId);
        } else if (xType === 'edit') {
            handleEdit(uId);
        } else {
            handleDelete(uId);
        }

    });

    let validator = $('#userForm').validate({
        rules: {
            firstName: {
                required: true
            },
            lastName: {
                required: true
            }
        },
        messages: {
            firstName: {
                required: '<span style="color: #e44c65">Please enter a first name.</span>'
            },
            lastName: {
                required: '<span style="color: #e44c65">Please enter a last name.</span>'
            }
        },
        submitHandler: function (form) {

            $.ajax({
                url: 'users/user',
                method: 'POST',
                data: $(form).serialize(),
                success: function (result) {
                    console.log(result);

                    let id = $('#userId').val();
                    let action = '';
                    if (id > 0) {
                        action = 'updated';
                        table.row(`#row_${id}`).data(result).draw();
                    } else {
                        action = 'created';
                        table.row.add(result).draw();
                    }

                    notify(`The User was successfully ${action}.`);
                    toggleDataTable();
                },
                error: function (xhr, resp, text) {
                    console.log(text);
                    notify('An error occured. User was not created.', true);
                }
            });

        }

    });

    $('#createBtn').on('click', function () {

        $('#formHeader').html('Create a new user.');
        toggleDataTable();
        return false;

    });

    $('#resetBtn').on('click', function () {

        if (formState != null) {
            $('#userId').val(formState['id']);
            $('#firstName').val(formState['firstName']);
            $('#lastName').val(formState['lastName']);
            $('#notes').val(formState['notes']);
        } else {
            $('#userId').val(0);
            $('#firstName').val('');
            $('#lastName').val('');
            $('#notes').val('');
        }

        validator.resetForm();
        return false;
    });

    $('#cancelBtn').on('click', function () {

        toggleDataTable();
        return false;

    });
    function handleEvents(uId) {
        console.log(`events ${uId}`);
    };

    function handleEdit(uId) {
        var data = table.row(`#row_${uId}`).data();

        $('#formHeader').html('Update current user.');

        $('#userId').val(data['id']);
        $('#firstName').val(data['firstName']);
        $('#lastName').val(data['lastName']);
        $('#notes').val(data['notes']);

        formState = data;
        toggleDataTable();
    };

    function handleDelete(uId) {
        $.ajax({
            url: 'users/delete',
            type: 'DELETE',
            data: `id=${uId}`,
            success: function (result) {
                table.row(`#row_${uId}`).remove().draw('page');
                notify('The User was successfully deleted.');
            },
            error: function (xhr, resp, text) {
                notify('An error occured. User was not deleted.', true);
            }
        });
    };

    function notify(message, isError = false) {
        let notification = $('#notification');

        if (isError == true) {
            notification.removeClass('success').addClass('error');
            notification
                .html(message)
                .show(500)
                .delay(1500)
                .hide(500);
        } else {
            notification.removeClass('error').addClass('success');
            notification
                .html(message)
                .show(500)
                .delay(1500)
                .hide(500);
        }
    }

    function toggleDataTable() {
        let table = $('#dTableWrapper');
        let form = $('#dFormWrapper');

        if (table.is(':visible')) {
            table.fadeOut(400, function () {
                form.fadeIn(400);
            });
        } else {
            form.fadeOut(400, function () {

                $('#userId').val(0);
                $('#firstName').val('');
                $('#lastName').val('');
                $('#notes').val('');

                validator.resetForm();
                formState = null;

                table.fadeIn(400);
            });
        }

    }

});