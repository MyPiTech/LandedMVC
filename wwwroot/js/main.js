/*
	Landed by HTML5 UP
	html5up.net | @ajlkn
	Free for personal and commercial use under the CCA 3.0 license (html5up.net/license)
*/

(function($) {

	var	$window = $(window),
		$body = $('body');

	// Breakpoints.
		breakpoints({
			xlarge:   [ '1281px',  '1680px' ],
			large:    [ '981px',   '1280px' ],
			medium:   [ '737px',   '980px'  ],
			small:    [ '481px',   '736px'  ],
			xsmall:   [ null,      '480px'  ]
		});

	// Play initial animations on page load.
		$window.on('load', function() {
			window.setTimeout(function() {
				$body.removeClass('is-preload');
			}, 100);
		});

	// Touch mode.
		if (browser.mobile)
			$body.addClass('is-touch');

	// Scrolly links.
		$('.scrolly').scrolly({
			speed: 2000
		});

	// Dropdowns.
		$('#nav > ul').dropotron({
			alignment: 'right',
			hideDelay: 350
		});

	// Nav.

		// Title Bar.
			$(
				'<div id="titleBar">' +
					'<a href="#navPanel" class="toggle"></a>' +
					'<span class="title">' + $('#logo').html() + '</span>' +
				'</div>'
			)
				.appendTo($body);

		// Panel.
			$(
				'<div id="navPanel">' +
					'<nav>' +
						$('#nav').navList() +
					'</nav>' +
				'</div>'
			)
				.appendTo($body)
				.panel({
					delay: 500,
					hideOnClick: true,
					hideOnSwipe: true,
					resetScroll: true,
					resetForms: true,
					side: 'left',
					target: $body,
					visibleClass: 'navPanel-visible'
				});

	// Parallax.
	// Disabled on IE (choppy scrolling) and mobile platforms (poor performance).
		if (browser.name == 'ie'
		||	browser.mobile) {

			$.fn._parallax = function() {

				return $(this);

			};

		}
		else {

			$.fn._parallax = function() {

				$(this).each(function() {

					var $this = $(this),
						on, off;

					on = function() {

						$this
							.css('background-position', 'center 0px');

						$window
							.on('scroll._parallax', function() {

								var pos = parseInt($window.scrollTop()) - parseInt($this.position().top);

								$this.css('background-position', 'center ' + (pos * -0.15) + 'px');

							});

					};

					off = function() {

						$this
							.css('background-position', '');

						$window
							.off('scroll._parallax');

					};

					breakpoints.on('<=medium', off);
					breakpoints.on('>medium', on);

				});

				return $(this);

			};

			$window
				.on('load resize', function() {
					$window.trigger('scroll');
				});

		}

	// Spotlights.
		var $spotlights = $('.spotlight');

		$spotlights
			._parallax()
			.each(function() {

				var $this = $(this),
					on, off;

				on = function() {

					var top, bottom, mode;

					// Use main <img>'s src as this spotlight's background.
						$this.css('background-image', 'url("' + $this.find('.image.main > img').attr('src') + '")');

					// Side-specific scrollex tweaks.
						if ($this.hasClass('top')) {

							mode = 'top';
							top = '-20%';
							bottom = 0;

						}
						else if ($this.hasClass('bottom')) {

							mode = 'bottom-only';
							top = 0;
							bottom = '20%';

						}
						else {

							mode = 'middle';
							top = 0;
							bottom = 0;

						}

					// Add scrollex.
						$this.scrollex({
							mode:		mode,
							top:		top,
							bottom:		bottom,
							initialize:	function(t) { $this.addClass('inactive'); },
							terminate:	function(t) { $this.removeClass('inactive'); },
							enter:		function(t) { $this.removeClass('inactive'); },

							// Uncomment the line below to "rewind" when this spotlight scrolls out of view.

							//leave:	function(t) { $this.addClass('inactive'); },

						});

				};

				off = function() {

					// Clear spotlight's background.
						$this.css('background-image', '');

					// Remove scrollex.
						$this.unscrollex();

				};

				breakpoints.on('<=medium', off);
				breakpoints.on('>medium', on);

			});

	// Wrappers.
		var $wrappers = $('.wrapper');

		$wrappers
			.each(function() {

				var $this = $(this),
					on, off;

				on = function() {

					$this.scrollex({
						top:		250,
						bottom:		0,
						initialize:	function(t) { $this.addClass('inactive'); },
						terminate:	function(t) { $this.removeClass('inactive'); },
						enter:		function(t) { $this.removeClass('inactive'); },

						// Uncomment the line below to "rewind" when this wrapper scrolls out of view.

						//leave:	function(t) { $this.addClass('inactive'); },

					});

				};

				off = function() {
					$this.unscrollex();
				};

				breakpoints.on('<=medium', off);
				breakpoints.on('>medium', on);

			});

	// Banner.
		var $banner = $('#banner');

		$banner
			._parallax();

})(jQuery);

function consoleFormat(obj, level, multi = false) {

	level = level ?? 0;
	let tag = (level) ? 'span' : 'div';
	let nextLevel = (level || 0) + 1;
	let display = (multi) ? 'inline' : 'block';

	// strings
	if (typeof obj == 'string') {
		return `<${tag} class="console-string" style="display: ${display};">${obj}</${tag}>`;
	}
	// booleans, null and undefined
	else if (typeof obj == 'boolean' || obj === null || obj === undefined) {
		return `<${tag} style="display: ${display};"><em class="console-bool">${obj}</em></${tag}>`;
	}
	// numbers
	else if (typeof obj == 'number') {
		return `<${tag} class="console-number" style="display: ${display};">${obj}</${tag}>`;
	}
	// dates
	else if (Object.prototype.toString.call(obj) == '[object Date]') {
		//console.info(obj);
		return `<${tag} class="console-date" style="display: ${display};">${obj.toJSON()}</${tag}>`;
	}
	// arrays
	else if (Array.isArray(obj)) {

		let result = obj.map((oData, i, data) => {
			return consoleFormat(oData, null, true);
		});
		let rtn = `<div class="console-array" style="display: ${display};">Array: [`;

		if (!obj.length) {
			return `${rtn}]</div>`;
		}

		rtn += `...<div class="array-data">${result.join(',</br>')}</div>`;

		return `${rtn}]</${tag}>`;
	}
	// objects
	else if (obj && typeof obj == 'object') {

		let rtn = '',
			len = Object.keys(obj).length;

		if (level && !len) {
			return `<${tag} class="object" style="display: ${display}-grid;">Object: {}</${tag}>`;
		}

		rtn += `<div class="object" style="display: ${display}-grid;">Object: {...}<ul class="object-content">`;
		for (var key in obj) {
			if (typeof obj[key] != 'function') {
				rtn += `<li><div class="object-data"><span class="object-key" >${key}:</span> ${consoleFormat(obj[key], nextLevel, true)}</div></li>`;
			}
		}
		rtn += `</ul></div>`;

		return rtn;
	}

	return '';
}

const isDev = true;
const console = (function (defaultConsole) {
	const consoleView = document.getElementById('console');
	return {
		debug: function (...data) {
			defaultConsole.debug(...data);
		},
		log: function (...data) {
			defaultConsole.log(...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('log', ...data);;
			}
		},
		serverLog: function (src, ...data) {
			defaultConsole.log('server', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('server-log', src, ...data);;
			}
		},
		apiLog: function (src, ...data) {
			defaultConsole.log('api', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('api-log', src, ...data);;
			}
		},
		info: function (...data) {
			defaultConsole.info(...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('info', ...data);
			}
		},
		serverInfo: function (...data) {
			defaultConsole.info('server', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('server-info', ...data);
			}
		},
		apiInfo: function (...data) {
			defaultConsole.info('api', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('api-info', ...data);
			}
		},
		warn: function (...data) {
			defaultConsole.warn(...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('warn', ...data);
			}
		},
		serverWarn: function (...data) {
			defaultConsole.warn('server', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('server-warn', ...data);
			}
		},
		apiWarn: function (...data) {
			defaultConsole.warn('api', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('api-warn', ...data);
			}
		},
		error: function (...data) {
			defaultConsole.error(...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('error', ...data);
			}
		},
		serverError: function (...data) {
			defaultConsole.error('server', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('server-error', ...data);
			}
		},
		apiError: function (...data) {
			defaultConsole.error('api', ...data);
			if (consoleView != null) {
				consoleView.innerHTML += formatData('api-error', ...data);
			}
		}
	};
}(window.console));

window.console = console;

function formatData(type,...data) {
	let title;
	let dataClass;
	let consoleDate = new Date().toJSON()

	switch (type) {
		case 'log':
			title = '[LOG]';
			dataClass = 'log';
			break;
		case 'server-log':
			title = '[LOG][SERVER]';
			dataClass = 'log';
			break;
		case 'api-log':
			title = '[LOG][API]';
			dataClass = 'log';
			break;
		case 'info':
			title = '[INFO]';
			dataClass = 'info';
			break;
		case 'server-info':
			title = '[INFO][SERVER]';
			dataClass = 'info';
			break;
		case 'api-info':
			title = '[INFO][API]';
			dataClass = 'info';
			break;
		case 'warn':
			title = '[WARN]';
			dataClass = 'warn';
			break;
		case 'server-warn':
			title = '[WARN][SERVER]';
			dataClass = 'warn';
			break;
		case 'api-warn':
			title = '[WARN][API]';
			dataClass = 'warn';
			break;
		case 'error':
			title = '[ERROR]';
			dataClass = 'error';
			break;
		case 'server-error':
			title = '[ERROR][SERVER]';
			dataClass = 'error';
			break;
		case 'api-error':
			title = '[ERROR][API]';
			dataClass = 'error';
			break;
		default:
			title = '[LOG]';
			dataClass = 'log';
			break;
	}

	/*title += `[${consoleDate}]`;*/

	let result = data.map((consoleData) => {
		return consoleFormat(consoleData, null, true);
	});

	return `<div class="${dataClass}">${title} ${result.join(' ')}</div>`;
}

