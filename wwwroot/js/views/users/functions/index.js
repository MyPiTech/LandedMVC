//table
function actionsHandler() {
    const xType = this.getAttribute('x-type');
    const id = this.getAttribute('x-id');

    console.log(`${xType} action selected`);
    if (xType === 'events') {
        handleEvents(id);
    } else if (xType === 'edit') {
        handleEdit(id);
    } else if (xType === 'delete') {
        handleDelete(id);
    }
}

function handleEvents(id) {
    window.location.assign(`Users/${id}/Events`);
};

function handleEdit(id) {
    console.log('Editing:', id);
    const data = table.row(`#row_${id}`).data();

    $('#formHeader').html('Update current user.');
    $('#userId').val(data.id);
    $('#firstName').val(data.firstName);
    $('#lastName').val(data.lastName);
    $('#notes').val(data.notes);

    formState = data;
    toggleDataTable(clearForm);
};

function handleDelete(id) {
    $.ajax({
        url: 'users/delete',
        type: 'DELETE',
        data: `id=${id}`,
        success: function (result) {
            dataTable_connection.send('deleteItem', id);
            table.row(`#row_${id}`).remove().draw('page');
            notify('The User was successfully deleted.');
            console.info(`Item id:${id} was successfully deleted.`, result);
        },
        error: function (xhr, resp, text) {
            notify('An error occured. User was not deleted.', true);
            console.error(`Item id:${id} was not deleted.`, text);
        }
    });
};

function actions(data) {
    let eventsAction = `<span x-type="events" x-id="${data.id}" class="icon solid alt fa-calendar" title="Events"></span>`;
    let editAction = `<span x-type="edit" x-id="${data.id}" class="icon solid alt fa-list-alt" title="Edit"></span>`;
    let deleteAction = `<span x-type="delete" x-id="${data.id}" class="icon solid alt fa-trash" title="Delete"></span>`;

    return `${editAction}&nbsp;&nbsp;${eventsAction}&nbsp;&nbsp;${deleteAction}`;
}

const dateVar = new Date('August 19, 1975 23:15:30');

//form
function createHandler() {
    console.log('Creating.');
    $('#formHeader').html('Create a new user.');
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
    $('label[for="firstName"]').toggle();
    $('label[for="lastName"]').toggle();
    $('label[for="notes"]').toggle();

    validator.resetForm();
    return false;
}

function resetHandler() {
    if (formState != null) {
        console.log('Reset form to last state:', formState);
        $('#userId').val(formState.id);
        $('#firstName').val(formState.firstName);
        $('#lastName').val(formState.lastName);
        $('#notes').val(formState.notes);
    } else {
        console.log('Reset form to last state: (Clear)');
        clearForm();
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
    $('#firstName').val('');
    $('#lastName').val('');
    $('#notes').val('');
    $("#submitBtn").prop("disabled", false);

    validator.resetForm();
    formState = null;
}

function formHandler(form) {
    $("#submitBtn").prop("disabled", true);
    $.ajax({
        url: 'users/upsert',
        method: 'POST',
        data: $(form).serialize(),
        success: function (result) {
            const id = $('#userId').val();
            let action = '';
            if (id > 0) {
                action = 'updated';
                dataTable_connection.send('updateItem', result);
                table.row(`#row_${id}`).data(result);
            } else {
                action = 'created';
                dataTable_connection.send('addItem', result);
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
            notify(`The user was successfully ${action}.`);
            toggleDataTable(clearForm);
        },
        error: function (xhr, resp, text) {
            $("#submitBtn").prop("disabled", false);
            notify('An error occured. The user was not created.', true);
            console.error('An error occured. The user was not created.', xhr, resp, text);
        }
    })
}
