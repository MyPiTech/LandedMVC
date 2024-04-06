// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 04-05-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 04-06-2024
// ***********************************************************************
// <copyright file="ControllerBase.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using LandedMVC.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LandedMVC.Controllers
{
	/// <summary>
	/// Class ControllerBase.
	/// Implements the <see cref="Controller" />
	/// </summary>
	/// <typeparam name="C"></typeparam>
	/// <seealso cref="Controller" />
	public class ControllerBase<C> : Controller where C : class
	{
		/// <summary>
		/// The console hub
		/// </summary>
		protected readonly IHubContext<ConsoleHub, IConsoleHub> _consoleHub;

		/// <summary>
		/// The logger
		/// </summary>
		protected readonly ILogger<C> _logger;

		/// <summary>
		/// The API base
		/// </summary>
		protected readonly string _apiBase;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerBase{C}"/> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="consoleHub">The console hub.</param>
		public ControllerBase(
			IConfiguration configuration,
			ILogger<C> logger,
			IHubContext<ConsoleHub, IConsoleHub> consoleHub
		)
		{
			_apiBase = configuration.GetValue<string>("ApiBase") ?? string.Empty;
			_consoleHub = consoleHub;
			_logger = logger;
		}
	}
}
