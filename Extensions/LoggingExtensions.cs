// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 04-05-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 04-15-2024
// ***********************************************************************
// <copyright file="LoggingExtensions.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using LandedMVC.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace LandedMVC.Extensions
{
	/// <summary>
	/// Class LoggingExtensions.
	/// </summary>
	public static class LoggingExtensions {

		/// <summary>
		/// The console hub
		/// </summary>
		private static IHubContext<ConsoleHub, IConsoleHub>? _consoleHub;

		/// <summary>
		/// Initializes the hub.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="consoleHub">The console hub.</param>
		/// <returns>ILogger.</returns>
		public static ILogger InitHub(this ILogger logger, IHubContext<ConsoleHub, IConsoleHub> consoleHub)
		{
			_consoleHub = consoleHub;
			return logger;
		}

		/// <summary>
		/// Log debug as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogDebugAsync(this ILogger logger, string message, params object?[] args)
		{
			if (_consoleHub != null) {
				await _consoleHub.Clients.All.SendLogAsync(message, args);
			}
			logger.LogDebug(message, args);
		}

		/// <summary>
		/// Log information as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogInformationAsync(this ILogger logger, string message, params object?[] args)
		{
			if (_consoleHub != null)
			{
				await _consoleHub.Clients.All.SendInfoAsync(message, args);
			}
			logger.LogInformation(message, args);
		}

		/// <summary>
		/// Log warning as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogWarningAsync(this ILogger logger, string message, params object?[] args)
		{
			if (_consoleHub != null)
			{
				await _consoleHub.Clients.All.SendWarnAsync(message, args);
			}
			logger.LogWarning(message, args);
		}

		/// <summary>
		/// Log error as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogErrorAsync(this ILogger logger, string message, params object?[] args)
		{
			if (_consoleHub != null)
			{
				await _consoleHub.Clients.All.SendErrorAsync(message, args);
			}
			logger.LogError(message, args);
		}

		/// <summary>
		/// Log error as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="message">The message.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogErrorAsync(this ILogger logger, Exception exception, string message, params object?[] args)
		{
			if (_consoleHub != null)
			{
				await _consoleHub.Clients.All.SendErrorAsync(message, exception, args);
			}
			logger.LogError(exception, message, args);
		}
	}
}
