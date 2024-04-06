// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 04-05-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 04-05-2024
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
	public static class LoggingExtensions
	{
		/// <summary>
		/// Log debug as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="consoleHub">The console hub.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogDebugAsync(this ILogger logger, string? message, IHubContext<ConsoleHub, IConsoleHub> consoleHub, params object?[] args)
		{
			await consoleHub.Clients.All.SendLogAsync(message ?? string.Empty, args);
			logger.LogDebug(message, args);
		}
		/// <summary>
		/// Log information as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="consoleHub">The console hub.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogInformationAsync(this ILogger logger, string? message, IHubContext<ConsoleHub, IConsoleHub> consoleHub, params object?[] args)
		{
			await consoleHub.Clients.All.SendInfoAsync(message ?? string.Empty, args);
			logger.LogInformation(message, args);
		}
		/// <summary>
		/// Log warning as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="consoleHub">The console hub.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogWarningAsync(this ILogger logger, string? message, IHubContext<ConsoleHub, IConsoleHub> consoleHub, params object?[] args)
		{
			await consoleHub.Clients.All.SendWarnAsync(message ?? string.Empty, args);
			logger.LogWarning(message, args);
		}
		/// <summary>
		/// Log error as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="message">The message.</param>
		/// <param name="consoleHub">The console hub.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogErrorAsync(this ILogger logger, string? message, IHubContext<ConsoleHub, IConsoleHub> consoleHub, params object?[] args)
		{
			await consoleHub.Clients.All.SendErrorAsync(message ?? string.Empty, args);
			logger.LogError(message, args);
		}
		/// <summary>
		/// Log error as an asynchronous operation.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="message">The message.</param>
		/// <param name="consoleHub">The console hub.</param>
		/// <param name="args">The arguments.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public static async Task LogErrorAsync(this ILogger logger, Exception exception, string? message, IHubContext<ConsoleHub, IConsoleHub> consoleHub, params object?[] args)
		{
			await consoleHub.Clients.All.SendErrorAsync(message ?? string.Empty, exception, args);
			logger.LogError(exception, message, args);
		}
	}
}
