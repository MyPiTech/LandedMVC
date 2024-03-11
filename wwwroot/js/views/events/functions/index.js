//table
function actionsHandler() {
    const xType = this.getAttribute('x-type');
    const id = this.getAttribute('x-id');

    if (xType === 'edit') {
        handleEdit(id);
    } else if (xType === 'delete') {
        handleDelete(id);
    }
}

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
        },
        error: function (xhr, resp, text) {
            notify('An error occurred. Event was not deleted.', true);
        }
    });
};

function actions(data) {
    let editAction = `<span x-type="edit" x-id="${data.id}" class="icon solid alt fa-list-alt" title="Edit"></span>`;
    let deleteAction = `<span x-type="delete" x-id="${data.id}" class="icon solid alt fa-trash" title="Delete"></span>`;

    return `${editAction}&nbsp;&nbsp;${deleteAction}`;
}


//form
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
    $('#formHeader').html('Create a new event.');
    toggleDataTable(clearForm);
    return false;
}

function lablesHandler() {
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
}

function resetHandler() {
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
}

function cancelHandler() {
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

    validator.resetForm();
    formState = null;
}

function formHandler(form) {
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

            notify(`The event was successfully ${action}.`);
            toggleDataTable(clearForm);
        },
        error: function (xhr, resp, text) {
            console.log(text);
            notify('An error occurred. The event was not created.', true);
        }
    });
}