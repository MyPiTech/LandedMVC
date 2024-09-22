//table
function actionsHandler() {
    const xType = this.getAttribute('x-type');
    const id = this.getAttribute('x-id');

    console.log(`${xType} action selected`);
    if (xType === 'edit') {
        handleEdit(id);
    } else if (xType === 'delete') {
        const msg = "This will permanently delete this event. Are you sure?";
        const title = 'Delete Event?';
        confirmDialog(title, msg).done(function () {
            handleDelete(id);
        });
    }
}

function handleEdit(id) {
    console.log('Editing:', id);
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
    toggleDataTable(clearForm);
};

function handleDelete(id) {
    $.ajax({
        url: 'events/delete',
        type: 'DELETE',
        data: `id=${id}`,
        success: function (result) {
            table.row(`#row_${id}`).remove().draw('page');
            notify('The Event was successfully deleted.');
            console.info(`Item id:${id} was successfully deleted.`, result);
        },
        error: function (xhr, resp, text) {
            notify('An error occurred. Event was not deleted.', true);
            console.error(`Item id:${id} was not deleted.`, text);
        }
    });
};

function actions(data) {
    let editAction = `<span x-type="edit" x-id="${data.id}" class="icon solid alt fa-edit" title="Edit"></span>`;
    let deleteAction = `<span x-type="delete" x-id="${data.id}" class="icon solid alt fa-trash" title="Delete"></span>`;

    return `${editAction}&nbsp;&nbsp;${deleteAction}`;
}


//form
async function fetchUsersAsync() {
    try {
        console.info('Fetching users.');
        const response = await fetch('users/getAll');
        const result = await response.json();
        if (result.length === 0) {
            $('#no-users').dialog('open');
            return;
        }
        users = result.map(u => ({ name: `${u.firstName} ${u.lastName}`, id: u.id }));

        let list = document.getElementById('usersList');
        users.forEach(function (user) {
            var option = document.createElement('option');
            option.value = user.name;
            list.appendChild(option);
        });
    } catch (error) {
        notify(`An error occurred loading users. <br/> <br/> Error: ${error}`, true, 5000);
        console.error('An error occurred loading users:', error);

    }
}

const userInput = document.getElementById('userInput');
const userInputHandler = function (e) {
    const user = users.find(u => u.name === e.target.value);
    const userIdInput = document.getElementById('userId');
    if (user != null) {
        userIdInput.value = user.id;
    }
}
userInput.addEventListener('input', userInputHandler);

function createHandler() {
    console.log('Creating.');
    $('#formHeader').html('Create a new event.');
    toggleDataTable(clearForm);
    return false;
}

function lablesHandler() {
    console.log('Label toggle.');
    const text = $(this).html();
    if (text === 'Show Labels') {
        $(this).html('Hide Labels');
    } else {
        $(this).html('Show Labels');
    }
    $('label[for="userInput"]').toggle();
    $('label[for="title"]').toggle();
    $('label[for="location"]').toggle();

    validator.resetForm();
    return false;
}

function resetHandler() {
    if (formState != null) {
        console.log('Reset form to last state:', formState);
        $('#userId').val(formState.userId);
        $('#eventId').val(formState.id);
        $('#userInput').val(formState.name);
        $('#title').val(formState.title);
        $('#location').val(formState.location);
        $('#start').val(formState.start);
        $('#duration').val(formState.duration);
    } else {
        console.log('Reset form to last state: (Clear)');
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
}

function cancelHandler() {
    console.log('Form cancelled.');
    toggleDataTable(clearForm);
    return false;
}

function clearForm() {
    $('#userId').val(0);
    $('#eventId').val(0);
    $('#userInput').val('');
    $('#title').val('');
    $('#location').val('');
    $('#start').val('');
    $('#duration').val('');
    $("#submitBtn").prop("disabled", false);

    validator.resetForm();
    formState = null;
}

function formHandler(form) {
    $("#submitBtn").prop("disabled", true);
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
                    table.responsive.recalc();
                }, 1000);
            }
            console.info(`Item ${action} successfully.`, result);
            notify(`The event was successfully ${action}.`);
            toggleDataTable(clearForm);
        },
        error: function (xhr, resp, text) {
            $("#submitBtn").prop("disabled", false);
            notify('An error occurred. The event was not created.', true);
            console.error('An error occured. The event was not created.', xhr, resp, text);
        }
    });
}