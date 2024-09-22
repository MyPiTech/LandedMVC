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
    className: 'td-action'
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

function confirmDialog(title, msg) {
	var dialog = $(`<div title="${title}"><p>${msg}</p></div>`);
	var def = $.Deferred();

	$(dialog).dialog({
		resizable: false,
		autoOpen: true,
		draggable: false,
		modal: true,
		width: '45%',
		buttons: {
			'Cancel': function () {
				def.reject();
				$(this).dialog("close");
			},
			'OK': function () {
				def.resolve();
				$(this).dialog("close");
			}
		}
	});
	return def.promise();
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
    console.error(settings, helpPage, message);
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

	$('#no-users').dialog({
		resizable: false,
		draggable: false,
		modal: true,
		autoOpen: false,
		open: function (event, ui) {   // It'll hide Close button
			$(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
		},
		width: '50%',
		closeOnEscape: false,  
		buttons: [
			{
				text: 'Create User',
				click: function () {
					window.location.assign(`Users`);
				}
			},
			{
				text: 'Back',
				click: function () {
					history.back();
				}
			}
		]
	});

	$('#delete-user').dialog({
		resizable: false,
		draggable: false,
		modal: true,
		autoOpen: false,
		width: '50%',
		buttons: [
			{
				text: 'Cancel',
				click: function () {
					window.location.assign(`Users`);
				}
			},
			{
				text: 'Delete User',
				click: function () {
					history.back();
				}
			}
		]
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
	function consoleFormat(obj, parent, indent = false) {
		let parentDiv = document.getElementById(parent);
		let newDiv = document.createElement('div');
		parentDiv.append(newDiv);

		newDiv.id = crypto.randomUUID();

		// strings
		if (typeof obj == 'string') {
			newDiv.classList.add('console-string');
			if (indent) {
				newDiv.classList.add('indent');
			}
			newDiv.innerHTML += obj;
		}
		// booleans, null and undefined
		else if (typeof obj == 'boolean' || obj === null || obj === undefined) {
			newDiv.classList.add('console-bool');
			if (indent) {
				newDiv.classList.add('indent');
			}
			newDiv.innerHTML += obj;
		}
		// numbers
		else if (typeof obj == 'number') {
			newDiv.classList.add('console-number');
			if (indent) {
				newDiv.classList.add('indent');
			}
			newDiv.innerHTML += obj;
		}
		// dates
		else if (Object.prototype.toString.call(obj) == '[object Date]') {
			newDiv.classList.add('console-date');
			if (indent) {
				newDiv.classList.add('indent');
			}
			newDiv.innerHTML += obj;
		}
		// arrays
		else if (Array.isArray(obj)) {
			newDiv.classList.add('console-array');
			newDiv.innerHTML += `Array(${obj.length})`;

			if (indent) {
				newDiv.classList.add('indent');
			}

			let iconSpan = document.createElement('span');
			iconSpan.id = `ic-${newDiv.id}`;
			iconSpan.classList.add('ui-icon', 'ui-icon-triangle-1-e');

			let contentDiv = document.createElement('div');
			contentDiv.id = `ac-${newDiv.id}`;
			contentDiv.classList.add('hide');

			newDiv.append(iconSpan);
			newDiv.append(contentDiv);

			for (const item of obj) {
				consoleFormat(item, contentDiv.id, true);
			}
			newDiv.addEventListener('click', function (e) {
				let contentElem = this.querySelector(`#ac-${this.id}`);
				let iconElem = this.querySelector(`#ic-${this.id}`);
				if (contentElem.checkVisibility()) {
					iconElem.classList.replace('ui-icon-triangle-1-s', 'ui-icon-triangle-1-e');
					contentElem.classList.replace('show', 'hide');
				} else {
					iconElem.classList.replace('ui-icon-triangle-1-e', 'ui-icon-triangle-1-s');
					contentElem.classList.replace('hide', 'show');
				}
				e.stopPropagation();
			});
		}
		// objects
		else if (obj && typeof obj == 'object') {
			let oEntries = Object.entries(obj);
			newDiv.classList.add('console-object');
			newDiv.innerHTML += `Object(${oEntries.length})`;

			if (indent) {
				newDiv.classList.add('indent');
			}

			let iconSpan = document.createElement('span');
			iconSpan.id = `ic-${newDiv.id}`;
			iconSpan.classList.add('ui-icon', 'ui-icon-triangle-1-e');

			let contentDiv = document.createElement('div');
			contentDiv.id = `oc-${newDiv.id}`;
			contentDiv.classList.add('hide');

			newDiv.append(iconSpan);
			newDiv.append(contentDiv);

			oEntries.forEach(([key, value]) => {
				let entryDiv = document.createElement('div');
				entryDiv.classList.add('object-entry');
				contentDiv.append(entryDiv);

				let keySpan = document.createElement('span');
				keySpan.classList.add('object-key');
				keySpan.innerHTML += `${key}: `;
				entryDiv.append(keySpan);

				let valueSpan = document.createElement('span');
				valueSpan.classList.add('object-value');
				valueSpan.id = crypto.randomUUID();
				entryDiv.append(valueSpan);

				consoleFormat(value, valueSpan.id);
			});
			newDiv.addEventListener('click', function (e) {
				let contentElem = this.querySelector(`#oc-${this.id}`);
				let iconElem = this.querySelector(`#ic-${this.id}`);
				if (contentElem.checkVisibility()) {
					iconElem.classList.replace('ui-icon-triangle-1-s', 'ui-icon-triangle-1-e');
					contentElem.classList.replace('show', 'hide');
				} else {
					iconElem.classList.replace('ui-icon-triangle-1-e', 'ui-icon-triangle-1-s');
					contentElem.classList.replace('hide', 'show');
				}
				e.stopPropagation();
			});
		}
		
	}

	const console = (function (defaultConsole) {
		return {
			debug: function (...data) {
				defaultConsole.debug(...data);
			},
			log: function (...data) {
				defaultConsole.log(...data);
				formatData('log', ...data);
			},
			serverLog: function (src, ...data) {
				defaultConsole.log('server', ...data);
				formatData('server-log', src, ...data);
			},
			apiLog: function (src, ...data) {
				defaultConsole.log('api', ...data);
				formatData('api-log', src, ...data);
			},
			info: function (...data) {
				defaultConsole.info(...data);
				formatData('info', ...data);
			},
			serverInfo: function (...data) {
				defaultConsole.info('server', ...data);
				formatData('server-info', ...data);
			},
			apiInfo: function (...data) {
				defaultConsole.info('api', ...data);
				formatData('api-info', ...data);
			},
			warn: function (...data) {
				defaultConsole.warn(...data);
				formatData('warn', ...data);
			},
			serverWarn: function (...data) {
				defaultConsole.warn('server', ...data);
				formatData('server-warn', ...data);
			},
			apiWarn: function (...data) {
				defaultConsole.warn('api', ...data);
				formatData('api-warn', ...data);
			},
			error: function (...data) {
				defaultConsole.error(...data);
				formatData('error', ...data);
			},
			serverError: function (...data) {
				defaultConsole.error('server', ...data);
				formatData('server-error', ...data);
			},
			apiError: function (...data) {
				defaultConsole.error('api', ...data);
				formatData('api-error', ...data);
			}
		};
	}(window.console));
	window.console = console;

	function formatData(type, ...data) {
		let consoleDiv = document.getElementById('console');

		let title;
		let dataClass;

		switch (type) {
			case 'log':
				title = '[CLIENT][LOG]';
				dataClass = 'log';
				break;
			case 'server-log':
				title = '[SERVER][LOG]';
				dataClass = 'log';
				break;
			case 'api-log':
				title = '[API][LOG]';
				dataClass = 'log';
				break;
			case 'info':
				title = '[CLIENT][INFO]';
				dataClass = 'info';
				break;
			case 'server-info':
				title = '[SERVER][INFO]';
				dataClass = 'info';
				break;
			case 'api-info':
				title = '[API][INFO]';
				dataClass = 'info';
				break;
			case 'warn':
				title = '[CLIENT][WARN]';
				dataClass = 'warn';
				break;
			case 'server-warn':
				title = '[SERVER][WARN]';
				dataClass = 'warn';
				break;
			case 'api-warn':
				title = '[API][WARN]';
				dataClass = 'warn';
				break;
			case 'error':
				title = '[CLIENT][ERROR]';
				dataClass = 'error';
				break;
			case 'server-error':
				title = '[SERVER][ERROR]';
				dataClass = 'error';
				break;
			case 'api-error':
				title = '[API][ERROR]';
				dataClass = 'error';
				break;
			default:
				title = '[CLIENT][LOG]';
				dataClass = 'log';
				break;
		}

		let newRow = document.createElement('div');
		newRow.id = crypto.randomUUID();
		newRow.classList.add(dataClass);
		newRow.innerHTML += title;
		consoleDiv.append(newRow);
		data.map((consoleData) => {
			consoleFormat(consoleData, newRow.id);
		});
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



