const DT_ORDER = [[1, 'asc']];
const DT_PAGE_LENGTH = 5;
const DT_LENGTH_MENU = [5, 10];
const DT_PAGING_TYPE = 'first_last_numbers';

const DT_CONTROL_COLUMN = {
    className: 'dtr-control',
    orderable: false,
    data: null,
    defaultContent: ''
};

const DT_ID_COLUMN = { data: 'id' };

const DT_ACTION_COLUMN = {
    data: null,
    orderable: false,
    className: 'tdAction'
};

function toggleDataTable(clearForm) {
    const table = $('#dataTableWrapper');
    const form = $('#dataFormWrapper');

    if (table.is(':visible')) {
        table.fadeOut(400, function () {
            form.fadeIn(400);
        });
    } else {
        form.fadeOut(400, function () {
            clearForm();
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

function addRowId(row, data) {
    const uId = data['id'];
    $(row).attr('id', `row_${uId}`);
}

function hideTablePagination(oSettings) {
    if (oSettings._iDisplayLength >= oSettings.fnRecordsDisplay()) {
        $(oSettings.nTableWrapper).find('.dt-paging').hide();
    } else {
        $(oSettings.nTableWrapper).find('.dt-paging').show();
    }
}

function dataTableError(settings, helpPage, message) {
    notify(`An error occurred loading table. <br/> <br/> Error: ${message}`, true, 5000);
}