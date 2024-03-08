$(function () {

    let formState = null;

    //table
    $.fn.dataTable.ext.errMode = function (settings, helpPage, message) {
        notify(`An error occurred loading table. <br/> <br/> Error: ${message}`, true, 5000);
    }

    const table = $('#dataTable').DataTable({
        stateSave: true,
        order: [[1, 'asc']],
        responsive: true,
        pagingType: 'first_last_numbers',
        pageLength: 5,
        lengthMenu: [5, 10],
        ajax: { url: '/users/getAll', dataSrc: '' },
        columns: [
            {
                className: 'dtr-control',
                orderable: false,
                data: null,
                defaultContent: ''
            },
            { data: 'id' },
            {
                data: 'lastName',
                render: DataTable.render.ellipsis(10) },
            {
                data: 'firstName',
                render: DataTable.render.ellipsis(10) },
            {
                data: 'notes',
                orderable: false,
                render: DataTable.render.ellipsis(20)
            },
            {
                data: null,
                orderable: false,
                mRender: function (data, type, row) {
                    let eventsAction = `<span x-type="events" x-id="${data.id}" class="icon solid alt fa-calendar" title="Events"></span>`;
                    let editAction = `<span x-type="edit" x-id="${data.id}" class="icon solid alt fa-list-alt" title="Edit"></span>`;
                    let deleteAction = `<span x-type="delete" x-id="${data.id}" class="icon solid alt fa-trash" title="Delete"></span>`;

                    return `${editAction}&nbsp;&nbsp;${eventsAction}&nbsp;&nbsp;${deleteAction}`;
                }
            }
        ],
        createdRow: function (row, data, dataIndex) {
            const uId = data['id'];
            $(row).attr('id', `row_${uId}`);
        },
        fnDrawCallback: function (oSettings) {
            if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                $(oSettings.nTableWrapper).find('.dt-paging').hide();
            } else {
                $(oSettings.nTableWrapper).find('.dt-paging').show();
            }
        }
    });

    //form
    const validator = $('#dataForm').validate({
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
                url: 'users/upsert',
                method: 'POST',
                data: $(form).serialize(),
                success: function (result) {
                    const id = $('#userId').val();
                    let action = '';
                    if (id > 0) {
                        action = 'updated';
                        table.row(`#row_${id}`).data(result);
                    } else {
                        action = 'created';
                        //when this occurs too quickly the table's pagination breaks
                        //this is a hacky solution
                        //I should probably make all datatables processing server-side
                        setTimeout(function () {
                            table.row.add(result);
                            table.columns.adjust().draw(false);
                        }, 1000);
                    }

                    notify(`The user was successfully ${action}.`);
                    toggleDataTable();
                },
                error: function (xhr, resp, text) {
                    console.log(text);
                    notify('An error occured. The user was not created.', true);
                }
            });
        }
    });

    //event handlers
    $('#dataTable tbody').on('click', 'span', function () {
        const xType = this.getAttribute('x-type');
        const uId = this.getAttribute('x-id');

        if (xType === 'events') {
            handleEvents(uId);
        } else if (xType === 'edit') {
            handleEdit(uId);
        } else if (xType === 'delete') {
            handleDelete(uId);
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

    $('#labelsToggle').on('click', function () {
        const text = $(this).html();
        if (text === 'Show Labels') {
            $(this).html('Hide Labels');
        } else {
            $(this).html('Show Labels');
        }
        $('label[for="firstName"]').toggle();
        $('label[for="lastName"]').toggle();
        $('label[for="notes"]').toggle();

        validator.resetForm();
        return false;
    });

    //functions
    function handleEvents(uId) {
        window.location.assign(
            `users/events?uId=${uId}`
        );
    };

    function handleEdit(uId) {
        const data = table.row(`#row_${uId}`).data();

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

    function notify(message, isError = false, duration = 3000) {
        const notification = $('#notification');

        if (isError == true) {
            notification.removeClass('success').addClass('error');
            notification
                .html(message)
                .show(500)
                .delay(duration)
                .hide(500);
        } else {
            notification.removeClass('error').addClass('success');
            notification
                .html(message)
                .show(500)
                .delay(duration)
                .hide(500);
        }
    }

    function toggleDataTable() {
        const table = $('#dataTableWrapper');
        const form = $('#dataFormWrapper');

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