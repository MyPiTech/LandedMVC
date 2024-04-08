const DT_ORDER = [[1, 'asc']];
const DT_PAGE_LENGTH = 5;
const DT_LENGTH_MENU = [5, 10];
const DT_PAGING_TYPE = 'first_last_numbers';

const RAW_GITHUB_BASE = 'https://raw.githubusercontent.com/MyPiTech/';
const RAW_GITHUB_BRANCH = '/master/';
const GITHUB_BASE = 'https://github.com/MyPiTech/';
const GITHUB_BRANCH = '/blob/master/';


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
    console.error(message);
    notify(`An error occurred loading table. <br/> <br/> Error: ${message}`, true, 5000);
}

$(function () {
    $('#ctabs').tabs({
        activate: function (event, ui) {
            $('.scroll-div').scrollLeft(0).scrollTop(0);
        }
    });
    $('#stabs').tabs({
        activate: function (event, ui) {
            $('.scroll-div').scrollLeft(0).scrollTop(0);
        }
    });
    $('#atabs').tabs({
        activate: function (event, ui) {
            $('.scroll-div').scrollLeft(0).scrollTop(0);
        }
    });
    $('#accordion').accordion({
        collapsible: true,
        active: false,
        heightStyle: 'auto'
    });

    $('pre[data-src]').each(function () {
        const elem = $(this);
        const src = elem.attr('data-src');
        let repo = elem.attr('data-repo');

        repo += RAW_GITHUB_BRANCH;

        $.ajax({
            url: `${RAW_GITHUB_BASE}${repo}${src}`,
            dataType: 'text',
            type: 'GET',
            async: true,
            success: function (result) {
                let rText = result.replaceAll('&', '&amp;').replaceAll('<', '&lt;');
                elem.html(`<code>${rText}</code>`);
                Prism.highlightAll();
            },
            error: function (jqXHR, status, error) {
                console.info(jqXHR);
                console.warn(status);
                console.error(error);
            }
        });
    });

    let cursordown = false;
    let cursorypos = 0;
    let cursorxpos = 0;
    $('.code-tabs').on('mouseenter', 'div', function (e) {
        $(this).css('user-select', 'none').css('cursor', 'grab');
    }).on('mouseleave', 'div', function (e) {
        cursordown = false;
        $(this).css('user-select', 'auto').css('cursor', 'auto');
    }).on('mousedown', 'div', function (e) {
        $(this).css('cursor', 'grabbing');
        cursordown = true;
        cursorxpos = $(this).scrollLeft() + e.clientX;
        cursorypos = $(this).scrollTop() + e.clientY;
    }).on('mousemove', 'div', function (e) {
        if (!cursordown) return;
        try { $(this).scrollLeft(cursorxpos - e.clientX); } catch (e) { }
        try { $(this).scrollTop(cursorypos - e.clientY); } catch (e) { }
    }).on('mouseup', 'div', end = function (e) {
        $(this).css('cursor', 'grab');
        cursordown = false;
    });

    $('.console-view').on('mouseenter mouseleave', 'div.object', function () {
        $(this).children('.object-content').toggle('fast');
    });
    $('.console-view').on('mouseenter mouseleave', 'div.console-array', function () {
        $(this).children('.array-data').toggle('fast');
    });

    //event handlers
    $('#dataTable tbody').on('click', 'span', actionsHandler);

    $('#createBtn').on('click', createHandler);

    $('#resetBtn').on('click', resetHandler);

    $('#cancelBtn').on('click', cancelHandler);

    $('#labelsToggle').on('click', lablesHandler);

    $('#ctabs-copy, #stabs-copy, #atabs-copy').on('click', copyHandler);

    $('#ctabs-repo, #stabs-repo, #atabs-repo').on('click', repoHandler);

    //code viewer
    function repoHandler() {
        const pId = $(this).closest('div').attr('id');
        const selected = $(`#${pId}`).tabs('option', 'active') + 1;
        const elem = $(`#${pId}-${selected} pre`);
        const src = elem.attr('data-src');
        let repo = elem.attr('data-repo');

        repo += GITHUB_BRANCH;
        console.log('repo:', `${GITHUB_BASE}${repo}${src}`);
        window.open(`${GITHUB_BASE}${repo}${src}`, '_blank').focus();
    }

    function copyHandler() {
        var pId = $(this).closest('div').attr('id');
        var selected = $(`#${pId}`).tabs('option', 'active') + 1;
        const elem = $(`#${pId}-${selected} pre`);
        const src = elem.attr('data-src');
        let repo = elem.attr('data-repo');

        repo += RAW_GITHUB_BRANCH;
        console.log('copy:', `${RAW_GITHUB_BASE}${repo}${src}`);
        $.ajax({
            url: `${RAW_GITHUB_BASE}${repo}${src}`,
            dataType: 'text',
            type: 'GET',
            async: true,
            success: function (result) {
                navigator.clipboard.writeText(result).then(function () {
                    notify('Repository code copied to clipboard.');
                }, function () {
                    notify('Failed to copy code to clipboard. Check permissions for clipboard.', true);
                });
            },
            error: function (jqXHR, status, error) {
                console.info(jqXHR);
                console.warn(status);
                console.error(error);
            }
        });

        return false;
    }
});

Prism.plugins.NormalizeWhitespace.setDefaults({
    'break-lines': 500,
    'indent': 0,
    'remove-initial-line-feed': true
});

/*Prism.plugins.NormalizeWhitespace.setDefaults({
    'remove-trailing': true,
    'remove-indent': true,
    'left-trim': true,
    'right-trim': true,
    'break-lines': 500,
    'indent': 0,
    'remove-initial-line-feed': true,
    'tabs-to-spaces': 2,
    'spaces-to-tabs': 2
});*/



