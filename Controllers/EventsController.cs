// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 02-27-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 02-27-2024
// ***********************************************************************
// <copyright file="EventsController.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using LandedMVC.Dtos;
using LandedMVC.Hubs;
using LandedMVC.Models;
using LandedMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LandedMVC.Controllers
{
	/// <summary>
	/// Class EventsController.
	/// Implements the <see cref="Controller" />
	/// </summary>
	/// <seealso cref="Controller" />
	public class EventsController: ControllerBase<EventsController>
	{
		/// <summary>
		/// The event API service
		/// </summary>
		private readonly ApiService<EventDto> _apiService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventsController"/> class.
		/// </summary>
		/// <param name="apiService">The API service.</param>
		public EventsController(ApiService<EventDto> apiService,
			IHubContext<ConsoleHub, IConsoleHub> consoleHub,
			ILogger<EventsController> logger,
			IConfiguration configuration
		) : base(configuration, logger, consoleHub)
		{
			_apiService = apiService;
		}

		/// <summary>
		/// Default index view.
		/// </summary>
		/// <returns>IActionResult.</returns>
		public IActionResult Index()
		{
			return View(new ApiModel { ApiBase = _apiBase });
		}

		/// <summary>
		/// Get all events as an asynchronous operation.
		/// </summary>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A collection of model dtos representing the results of the asynchronous operation.</returns>
		[HttpGet]
		public async Task<IActionResult> GetAllAsync(CancellationToken token)
		{
			var results = await _apiService.GetAllAsync(token);
			return Json(results);
		}

		// <summary>
		/// Delete event as an asynchronous operation.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		[HttpDelete]
		public async Task DeleteAsync(int id, CancellationToken token)
		{
			await _apiService.DeleteAsync(() => new EventDto { Id = id }, token);
		}

		/// <summary>
		/// Add or update an event as an asynchronous operation.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A dto representing the results of the asynchronous operation.</returns>
		[HttpPost]
		public async Task<IActionResult> UpsertAsync(EventModel model, CancellationToken token)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (model.Id == 0)
			{
				var dto = await _apiService.AddAsync(model.ToDto(), token);
				if (dto != null)
				{
					model = dto.ToModel();
				}
			}
			else
			{
				await _apiService.EditAsync(model.ToDto(), token);
			}
			return Json(model);
		}

	}
}
