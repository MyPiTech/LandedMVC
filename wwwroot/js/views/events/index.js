$(function () {

    let formState = null;
    let users = null;

    //table
    $.fn.dataTable.ext.errMode = function (settings, helpPage, message) {
        notify(`An error occurred loading table. <br/> <br/> Error: ${message}`, true, 5000);
    }

    const table = $('#dataTable').DataTable({
        order: [[1, 'asc']],
        responsive: true,
        pagingType: 'first_last_numbers',
        pageLength: 5,
        lengthMenu: [5, 10],
        ajax: { url: '/events/getAll', dataSrc: '' },
        columns: [
            {
                className: 'dtr-control',
                orderable: false,
                data: null,
                defaultContent: ''
            },
            { data: 'id' },
            { data: 'userId' },
            { data: 'title',
                render: DataTable.render.ellipsis(10)
            },
            { data: 'location',
                render: DataTable.render.ellipsis(10)
            },
            {
                data: 'start',
                render: DataTable.render.datetime()
            },
            { data: 'duration' },
            {
                data: null,
                orderable: false,
                mRender: function (data, type, row) {
                    let editAction = `<span x-type="edit" x-id="${data.id}" class="icon solid alt fa-list-alt" title="Edit"></span>`;
                    let deleteAction = `<span x-type="delete" x-id="${data.id}" class="icon solid alt fa-trash" title="Delete"></span>`;

                    return `${editAction}&nbsp;&nbsp;${deleteAction}`;
                }
            }
        ],
        createdRow: function (row, data, dataIndex) {
            const id = data['id'];
            $(row).attr('id', `row_${id}`);
        },
        fnDrawCallback: function (oSettings) {
            if (oSettings._iDisplayLength >= oSettings.fnRecordsDisplay()) {
                $(oSettings.nTableWrapper).find('.dt-paging').hide();
            } else {
                $(oSettings.nTableWrapper).find('.dt-paging').show();
            }
        }

    });

    //form
    fetchUsersAsync();

    const validator = $('#dataForm').validate({
        rules: {
            userInput: {
                required: true,
                inUserArray: true
            },
            title: {
                required: true
            },
            start: {
                required: true
            },
            duration: {
                required: true
            }
        },
        messages: {
            userInput: {
                required: '<span style="color: #e44c65">You must enter/select the name of an exisitng user.</span>'
            },
            title: {
                required: '<span style="color: #e44c65">Please enter an event title.</span>'
            },
            start: {
                required: '<span style="color: #e44c65">Please enter a valid start date/time.</span>'
            },
            duration: {
                required: '<span style="color: #e44c65">Please enter a duration.</span>'
            }
        },
        submitHandler: function (form) {

            $.ajax({
                url: 'events/upsert',
                method: 'POST',
                data: $(form).serialize(),
                success: function (result) {
                    let id = $('#eventId').val();
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

                    notify(`The event was successfully ${action}.`);
                    toggleDataTable();
                },
                error: function (xhr, resp, text) {
                    console.log(text);
                    notify('An error occurred. The event was not created.', true);
                }
            });

        }

    });

    $.validator.addMethod('inUserArray', function (value, element) {
        const valid = users.some(u => u.name === value);
        return this.optional(element) || valid;
    }, '<span style="color: #e44c65">You must enter/select the name of an exisitng user.</span>');

    //event handlers
    const userInput = document.getElementById('userInput');
    const userInputHandler = function (e) {
        const user = users.find(u => u.name === e.target.value);
        const userIdInput = document.getElementById('userId');
        if (user != null) {
            userIdInput.value = user.id;
        }
    }
    userInput.addEventListener('input', userInputHandler);


    $('#dataTable tbody').on('click', 'span', function () {
        const xType = this.getAttribute('x-type');
        const id = this.getAttribute('x-id');

        if (xType === 'edit') {
            handleEdit(id);
        } else if (xType === 'delete') {
            handleDelete(id);
        }
    });

    $('#createBtn').on('click', function () {
        $('#formHeader').html('Create a new event.');
        toggleDataTable();
        return false;
    });

    $('#resetBtn').on('click', function () {
        if (formState != null) {
            $('#userId').val(formState.userId);
            $('#eventId').val(formState.id);
            $('#userInput').val(formState.name);
            $('#title').val(formState.title);
            $('#location').val(formState.location);
            $('#start').val(formState.start);
            $('#duration').val(formState.duration);
        } else {
            $('#userId').val(0);
            $('#eventId').val(0);
            $('#userInput').val('');
            $('#title').val('');
            $('#location').val('');
            $('#start').val('');
            $('#duration').val('');
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
        $('label[for="title"]').toggle();
        $('label[for="location"]').toggle();

        validator.resetForm();
        return false;
    });

    //functions
    function handleEdit(id) {
        const data = table.row(`#row_${id}`).data();
        const user = users.find(u => u.id === data.userId);

        formState = data;
        formState.name = user.name;

        $('#formHeader').html('Update current event.');

        $('#userId').val(formState.userId);
        $('#eventId').val(formState.id);
        $('#userInput').val(formState.name);
        $('#title').val(formState.title);
        $('#location').val(formState.location);
        $('#start').val(formState.start);
        $('#duration').val(formState.duration);

        toggleDataTable();
    };

    function handleDelete(id) {
        $.ajax({
            url: 'events/delete',
            type: 'DELETE',
            data: `id=${id}`,
            success: function (result) {
                table.row(`#row_${id}`).remove().draw('page');
                notify('The Event was successfully deleted.');
            },
            error: function (xhr, resp, text) {
                notify('An error occurred. Event was not deleted.', true);
            }
        });
    };

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
                $('#eventId').val(0);
                $('#userInput').val('');
                $('#title').val('');
                $('#location').val('');
                $('#start').val('');
                $('#duration').val('');

                validator.resetForm();
                formState = null;

                table.fadeIn(400);
            });
        }
    }

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

    async function fetchUsersAsync() {
        try {
            const response = await fetch("users/getAll");
            const result = await response.json();

            users = result.map(u => ({ name: `${u.firstName} ${u.lastName}`, id: u.id }));

            let list = document.getElementById('usersList');
            users.forEach(function (user) {
                var option = document.createElement('option');
                option.value = user.name;
                list.appendChild(option);
            });
        } catch (error) {
            console.log(error);
            notify(`An error occurred loading users. <br/> <br/> Error: ${error}`, true, 5000);
        }  
    }

});